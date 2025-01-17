using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Outlines.Inspection
{
    public class GlobalInputListener : IGlobalInputListener
    {
        private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowsHookEx", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int vkCode);

        // Based on https://docs.microsoft.com/en-us/previous-versions/dd162805(v=vs.85)
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        // Based on https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mousehookstruct
        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            public POINT pt;
            public IntPtr hwnd;
            public uint wHitTestCode;
            public IntPtr dwExtraInfo;
        }

        // From https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        private const int WM_LBUTTONDOWN = 0x0201; //https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-lbuttondown
        private const int WM_KEYDOWN = 0x0100; // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-keydown
        private const int WM_KEYUP = 0x0101; // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-keyup

        private IInputMaskingService InputMaskingService { get; set; }

        private HookProc KeyboardHookProc { get; set; }
        private HookProc MouseHookProc { get; set; }
        private IntPtr KeyboardHookPtr { get; set; }
        private IntPtr MouseHookPtr { get; set; }

        public event KeyDownEventHandler KeyDown;
        public event KeyUpEventHandler KeyUp;
        public event MouseDownEventHandler MouseDown;

        public GlobalInputListener(IInputMaskingService inputMaskingService = null)
        {
            InputMaskingService = inputMaskingService;
        }

        public void RegisterToInputEvents()
        {
            KeyboardHookProc = new HookProc(KeyboardProc);
            MouseHookProc = new HookProc(MouseProc);

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                KeyboardHookPtr = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProc, GetModuleHandle(curModule.ModuleName), 0);
                MouseHookPtr = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void UnregisterFromInputEvents()
        {
            if (MouseHookPtr != IntPtr.Zero)
            {
                UnhookWindowsHookEx(MouseHookPtr);
                MouseHookPtr = IntPtr.Zero;
                MouseHookProc = null;
            }
            if (KeyboardHookPtr != IntPtr.Zero)
            {
                UnhookWindowsHookEx(KeyboardHookPtr);
                KeyboardHookPtr = IntPtr.Zero;
                KeyboardHookProc = null;
            }
        }

        private int KeyboardProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
            {
                return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            int message = wParam.ToInt32();
            int vkCode = Marshal.ReadInt32(lParam); ;

            switch (message)
            {
                case WM_KEYDOWN:
                    KeyDown?.Invoke(vkCode);
                    break;
                case WM_KEYUP:
                    KeyUp?.Invoke(vkCode);
                    break;
            }

            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        private int MouseProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
            {
                return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            int message = wParam.ToInt32();
            switch (message)
            {
                case WM_LBUTTONDOWN:
                    var mouseHookStruct = Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)) as MouseHookStruct;
                    var cursorPos = new Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y);
                    if (InputMaskingService == null || !InputMaskingService.IsInInputMask(cursorPos))
                    {
                        MouseDown?.Invoke(cursorPos);
                    }
                    break;
            }

            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        public Point GetCursorPosition()
        {
            return GetCursorPos(out POINT cursorPos) ? new Point(cursorPos.x, cursorPos.y) : Point.Empty;
        }

        public bool IsKeyDown(int vkCode)
        {
            // Based on https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeystate#return-value
            return (GetKeyState(vkCode) & 0x8000) != 0;
        }
    }
}
