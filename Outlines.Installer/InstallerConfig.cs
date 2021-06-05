using System.IO;

namespace Outlines.Installer
{
    public class InstallerConfig
    {
        public string PackageArchiveName { get; set; } = "outlines.zip";
        public string SourceLocation { get; set; }
        public string InstallLocation { get; set; } = "C:\\Program Files (x86)\\outlines\\";
        public string PackageArchiveSourcePath => Path.Combine(SourceLocation, PackageArchiveName);
        public string PackageArchiveDestinationPath => Path.Combine(InstallLocation, PackageArchiveName);
        public string PackageExtractionPath => InstallLocation;
        public string CertificateName { get; set; }
        public string CertificatePath => Path.Combine(PackageExtractionPath, CertificateName);
        public string ApplicationName { get; set; } = "OutlinesApp.exe";
        public string InstalledApplicationPath => Path.Combine(InstallLocation, ApplicationName);
        public string Hotkey { get; set; } = "Ctrl+Shift+O";
        public bool ShouldCreateDesktopShortcut { get; set; } = true;
        public bool ShouldRegisterHotkey { get; set; } = false;
        public bool ShouldInstallSystemApp { get; set; } = false;
    }
}
