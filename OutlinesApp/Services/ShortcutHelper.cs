using System;
using System.IO;
using IWshRuntimeLibrary;

namespace OutlinesApp.Services
{
    public class ShortcutHelper
    {
        private string InstallDirectory { get; set; } = "";
        private string BinaryName { get; set; } = "OutlinesApp.exe";
        private string AppName { get; set; } = "Outlines";

        public void TryEnsureStartShortcut()
        {
            try
            {
                if (!StartShortcutExists())
                {
                    AddStartShortcut();
                }
            }
            catch (Exception) { }
        }

        public void TryRemoveStartShortcut()
        {
            try
            {
                if (StartShortcutExists())
                {
                    string shortcutPath = GetShortcutPath();
                    System.IO.File.Delete(shortcutPath);
                }

            }
            catch (Exception) { }
        }

        private string GetShortcutFolderPath()
        {
            string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            return Path.Combine(commonStartMenuPath, "Programs");
        }

        private string GetShortcutPath()
        {
            return Path.Combine(GetShortcutFolderPath(), AppName + ".lnk");
        }

        private bool StartShortcutExists()
        {
            return System.IO.File.Exists(GetShortcutPath());
        }

        private void AddStartShortcut()
        {
            string shortcutFolderPath = GetShortcutFolderPath();

            if (!Directory.Exists(shortcutFolderPath))
            {
                Directory.CreateDirectory(shortcutFolderPath);
            }

            string shortcutPath = GetShortcutPath();
            string binaryPath = Path.Combine(InstallDirectory, BinaryName);

            var wshShell = new WshShell();
            IWshShortcut shortcut = wshShell.CreateShortcut(shortcutPath);
            shortcut.Description = "Outlines - An app to help you validate your UI implementation.";
            shortcut.TargetPath = binaryPath;
            shortcut.Save();
        }
    }
}
