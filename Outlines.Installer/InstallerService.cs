using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using IWshRuntimeLibrary;

namespace Outlines.Installer
{
    class InstallerService
    {
        private InstallerConfig Config { get; set; }

        public void Install()
        {
            CreateInstallFolder();
            CopyPackage();
            ExtractPackage();
            RegisterCertifcate();
            if (Config.ShouldCreateDesktopShortcut)
            {
                CreateDesktopShortcut();
            }
            CleanupInstallationArtifacts();
        }

        private void CreateInstallFolder()
        {
            if (!Directory.Exists(Config.InstallLocation))
            {
                Directory.CreateDirectory(Config.InstallLocation);
            }
        }

        private void CopyPackage()
        {
            System.IO.File.Copy(Config.PackageArchiveSourcePath, Config.PackageArchiveDestinationPath);
        }

        private void ExtractPackage()
        {
            ZipFile.ExtractToDirectory(Config.PackageArchiveDestinationPath, Config.PackageExtractionPath);
        }

        private void RegisterCertifcate()
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(Config.CertificatePath)));
            store.Close();
        }

        private void CreateDesktopShortcut()
        {
            WshShell shell = new WshShell();
            object shDesktop = (object)"Desktop";
            string shortcutPath = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Outlines.lnk";

            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.Description = "Outlines App";
            shortcut.TargetPath = Config.InstalledApplicationPath;

            if (Config.ShouldRegisterHotkey)
            {
                shortcut.Hotkey = Config.Hotkey;
            }

            shortcut.Save();
        }

        private void CleanupInstallationArtifacts()
        {
            System.IO.File.Delete(Config.PackageArchiveDestinationPath);
        }
    }
}
