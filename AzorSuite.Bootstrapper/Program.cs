using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace AzorSuite.Bootstrapper
{
    internal class Program
    {
        private const string ServerRoot =
            @"\\10.19.20.240\tecnico\Proyectos\MODELOS\_MODELO HOTEL\XX AZOR SUITE (NO MODIFICAR)";

        private const string ProductName = "Inventor2017";

        private static string LocalRoot =>
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "AzorSuite");

        private static string LocalInventorDirectory =>
            Path.Combine(
                LocalRoot,
                ProductName);

        private static string LocalVersionsDirectory =>
            Path.Combine(
                LocalInventorDirectory,
                "Versions");

        private static string LocalVersionFile =>
            Path.Combine(
                LocalInventorDirectory,
                "version.txt");

        private static string AddinDirectory =>
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                "Autodesk",
                "Inventor 2017",
                "Addins");

        private static string AddinPath =>
            Path.Combine(
                AddinDirectory,
                "AzorSuite.Inventor2017.addin");

        static int Main(string[] args)
        {
            bool createdNew;

            using (Mutex mutex = new Mutex(
                true,
                @"Local\AzorSuite.Bootstrapper",
                out createdNew))
            {
                if (!createdNew)
                {
                    return 0;
                }

                try
                {
                    InstallOrUpdate();
                    return 0;
                }
                catch (Exception ex)
                {
                    WriteLog(ex);
                    return 1;
                }
            }
        }

        private static void InstallOrUpdate()
        {
            Directory.CreateDirectory(LocalRoot);
            Directory.CreateDirectory(LocalInventorDirectory);
            Directory.CreateDirectory(LocalVersionsDirectory);
            Directory.CreateDirectory(AddinDirectory);

            string manifestPath =
                Path.Combine(
                    ServerRoot,
                    "manifest.xml");

            if (!File.Exists(manifestPath))
                return;

            XDocument manifest =
                XDocument.Load(manifestPath);

            XElement product =
                manifest.Root
                    .Elements("Product")
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                (string)x.Attribute("Name"),
                                ProductName,
                                StringComparison.OrdinalIgnoreCase));

            if (product == null)
                return;

            string serverVersionText =
                (string)product.Attribute("Version");

            string packageRelativePath =
                (string)product.Attribute("Package");

            if (!Version.TryParse(
                    serverVersionText,
                    out Version serverVersion))
            {
                return;
            }

            string packagePath =
                Path.Combine(
                    ServerRoot,
                    packageRelativePath);

            if (!File.Exists(packagePath))
                return;

            Version localVersion =
                ReadLocalVersion();

            if (serverVersion <= localVersion)
                return;

            InstallVersion(
                serverVersion,
                packagePath);
        }

        private static void InstallVersion(
            Version version,
            string packagePath)
        {
            string versionDirectory =
                Path.Combine(
                    LocalVersionsDirectory,
                    version.ToString());

            string tempDirectory =
                Path.Combine(
                    Path.GetTempPath(),
                    "AzorSuite",
                    Guid.NewGuid().ToString("N"));

            Directory.CreateDirectory(
                tempDirectory);

            try
            {
                string localZip =
                    Path.Combine(
                        tempDirectory,
                        Path.GetFileName(packagePath));

                File.Copy(
                    packagePath,
                    localZip,
                    true);

                if (Directory.Exists(versionDirectory))
                {
                    Directory.Delete(
                        versionDirectory,
                        true);
                }

                Directory.CreateDirectory(
                    versionDirectory);

                ZipFile.ExtractToDirectory(
                    localZip,
                    versionDirectory);

                string addinDll =
                    Path.Combine(
                        versionDirectory,
                        "AzorSuite.Inventor2017.dll");

                string uiDll =
                    Path.Combine(
                        versionDirectory,
                        "AzorSuite.UI.dll");

                string webViewCoreDll =
                    Path.Combine(
                        versionDirectory,
                        "Microsoft.Web.WebView2.Core.dll");

                string webViewWinFormsDll =
                    Path.Combine(
                        versionDirectory,
                        "Microsoft.Web.WebView2.WinForms.dll");

                string webViewLoaderDll =
                    Path.Combine(
                        versionDirectory,
                        "WebView2Loader.dll");

                ValidateFile(addinDll);
                ValidateFile(uiDll);
                ValidateFile(webViewCoreDll);
                ValidateFile(webViewWinFormsDll);
                ValidateFile(webViewLoaderDll);

                CreateAddinFile(
                    addinDll);

                File.WriteAllText(
                    LocalVersionFile,
                    version.ToString());
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempDirectory))
                    {
                        Directory.Delete(
                            tempDirectory,
                            true);
                    }
                }
                catch
                {
                }
            }
        }

        private static void CreateAddinFile(
            string assemblyPath)
        {
            string xml =
$@"<?xml version=""1.0"" encoding=""utf-8""?>

<Addin Type=""Standard"">

  <ClassId>{{8F5E4D7A-2B61-4C9E-9A37-6D2F8B14C501}}</ClassId>
  <ClientId>{{8F5E4D7A-2B61-4C9E-9A37-6D2F8B14C501}}</ClientId>

  <DisplayName>Azor Suite</DisplayName>
  <Description>Azor Suite para Autodesk Inventor 2017</Description>

  <Assembly>{assemblyPath}</Assembly>

  <LoadOnStartUp>1</LoadOnStartUp>
  <UserUnloadable>1</UserUnloadable>
  <Hidden>0</Hidden>

  <SupportedSoftwareVersionGreaterThan>20..</SupportedSoftwareVersionGreaterThan>

  <DataVersion>1</DataVersion>
  <UserInterfaceVersion>1</UserInterfaceVersion>

</Addin>";

            string tempPath =
                AddinPath + ".tmp";

            File.WriteAllText(
                tempPath,
                xml);

            File.Copy(
                tempPath,
                AddinPath,
                true);

            File.Delete(tempPath);
        }

        private static Version ReadLocalVersion()
        {
            if (!File.Exists(
                    LocalVersionFile))
            {
                return new Version(
                    0, 0, 0);
            }

            string text =
                File.ReadAllText(
                    LocalVersionFile).Trim();

            if (Version.TryParse(
                    text,
                    out Version version))
            {
                return version;
            }

            return new Version(
                0, 0, 0);
        }

        private static void ValidateFile(
            string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "Archivo requerido no encontrado.",
                    path);
            }
        }

        private static void WriteLog(
            Exception ex)
        {
            try
            {
                string logDirectory =
                    Path.Combine(
                        LocalRoot,
                        "Logs");

                Directory.CreateDirectory(
                    logDirectory);

                string logPath =
                    Path.Combine(
                        logDirectory,
                        "Bootstrapper.log");

                File.AppendAllText(
                    logPath,
                    Environment.NewLine +
                    DateTime.Now.ToString(
                        "yyyy-MM-dd HH:mm:ss") +
                    Environment.NewLine +
                    ex +
                    Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}