﻿using System;
using System.Windows;
using System.Windows.Interop;
using Outlines.Inspection;
namespace Outlines.App
{
    public partial class OverlayWindow : Window
    {
        public IntPtr Hwnd { get; private set; }

        public OverlayWindow()
        {
            InitializeComponent();

            Hwnd = new WindowInteropHelper(this).EnsureHandle();

            var focusHelper = new FocusHelper();
            focusHelper.DisableTakingFocus(Hwnd);

            var taskViewHelper = new TaskViewHelper();
            taskViewHelper.HideWindowFromTaskView(Hwnd);

            var windowZOrderHelper = new WindowZOrderHelper();
            windowZOrderHelper.MoveWindowToUIAccessZBand(Hwnd);
        }
    }
}
