using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace AzorSuite.Updater
{
    internal class Program
    {
        // ============================================================
        // CONFIGURACIÓN
        // ============================================================

        private const string UpdateRoot = @"\\10.19.20.240\tecnico\Proyectos\MODELOS\_MODELO HOTEL\XX AZOR SUITE (NO MODIFICAR)";

        private const string ProductName = "Inventor2017";

        // ============================================================
        // MAIN
        // ============================================================

        static int Main(string[] args)
        {
            try
            {
                string addinPath = GetArgument(args, "addin");
                string inventorPidText = GetArgument(args, "pid");

                if (string.IsNullOrWhiteSpace(addinPath))
                    return 1;

                int inventorPid = 0;

                if (!string.IsNullOrWhiteSpace(inventorPidText))
                {
                    int.TryParse(inventorPidText, out inventorPid);
                }

                RunUpdate(addinPath, inventorPid);

                return 0;
            }
            catch
            {
                // El updater debe fallar silenciosamente.
                return 1;
            }
        }

        // ============================================================
        // ACTUALIZACIÓN
        // ============================================================

        private static void RunUpdate(string addinPath, int inventorPid)
        {
            string localRoot = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "AzorSuite",
                ProductName
            );

            string versionFile = Path.Combine(localRoot, "version.txt");

            Directory.CreateDirectory(localRoot);

            Version localVersion = ReadLocalVersion(versionFile);

            // --------------------------------------------------------
            // Leer manifest remoto
            // --------------------------------------------------------

            string manifestPath = Path.Combine(
                UpdateRoot,
                "manifest.xml"
            );

            if (!File.Exists(manifestPath))
                return;

            XDocument manifest = XDocument.Load(manifestPath);

            XElement product = manifest
                .Root
                .Elements("Product")
                .FirstOrDefault(x =>
                    string.Equals(
                        (string)x.Attribute("Name"),
                        ProductName,
                        StringComparison.OrdinalIgnoreCase));

            if (product == null)
                return;

            Version serverVersion = new Version(
                (string)product.Attribute("Version")
            );

            string packageRelativePath =
                (string)product.Attribute("Package");

            // --------------------------------------------------------
            // No hay actualización
            // --------------------------------------------------------

            if (serverVersion <= localVersion)
                return;

            string packagePath = Path.Combine(
                UpdateRoot,
                packageRelativePath
            );

            if (!File.Exists(packagePath))
                return;

            // --------------------------------------------------------
            // Copiar paquete a local
            // --------------------------------------------------------

            string tempRoot = Path.Combine(
                Path.GetTempPath(),
                "AzorSuite",
                ProductName,
                Guid.NewGuid().ToString("N")
            );

            Directory.CreateDirectory(tempRoot);

            string localPackage = Path.Combine(
                tempRoot,
                Path.GetFileName(packagePath)
            );

            File.Copy(packagePath, localPackage, true);

            // --------------------------------------------------------
            // Esperar a que Inventor se cierre
            // --------------------------------------------------------

            WaitForInventor(inventorPid);

            // --------------------------------------------------------
            // Preparar nueva versión
            // --------------------------------------------------------

            string versionsRoot = Path.Combine(
                localRoot,
                "Versions"
            );

            string newVersionFolder = Path.Combine(
                versionsRoot,
                serverVersion.ToString()
            );

            if (Directory.Exists(newVersionFolder))
            {
                Directory.Delete(
                    newVersionFolder,
                    true
                );
            }

            Directory.CreateDirectory(newVersionFolder);

            // --------------------------------------------------------
            // Extraer ZIP
            // --------------------------------------------------------

            ZipFile.ExtractToDirectory(
                localPackage,
                newVersionFolder
            );

            // --------------------------------------------------------
            // Encontrar DLL principal
            // --------------------------------------------------------

            string addinDll = Directory
                .GetFiles(
                    newVersionFolder,
                    "AzorSuite.Inventor2017.dll",
                    SearchOption.AllDirectories)
                .FirstOrDefault();

            if (addinDll == null)
                throw new FileNotFoundException(
                    "No se encontró AzorSuite.Inventor2017.dll."
                );

            // --------------------------------------------------------
            // Actualizar .addin
            // --------------------------------------------------------

            UpdateAddinFile(
                addinPath,
                addinDll
            );

            // --------------------------------------------------------
            // Guardar versión instalada
            // --------------------------------------------------------

            File.WriteAllText(
                versionFile,
                serverVersion.ToString()
            );

            // --------------------------------------------------------
            // Limpieza
            // --------------------------------------------------------

            try
            {
                Directory.Delete(
                    tempRoot,
                    true
                );
            }
            catch
            {
                // No pasa nada si la limpieza falla.
            }
        }

        // ============================================================
        // ESPERAR A INVENTOR
        // ============================================================

        private static void WaitForInventor(int processId)
        {
            if (processId <= 0)
                return;

            try
            {
                Process process = Process.GetProcessById(processId);

                if (!process.HasExited)
                {
                    process.WaitForExit();
                }
            }
            catch
            {
                // Inventor ya no existe.
            }
        }

        // ============================================================
        // VERSION LOCAL
        // ============================================================

        private static Version ReadLocalVersion(string versionFile)
        {
            if (!File.Exists(versionFile))
                return new Version(0, 0, 0);

            string text = File.ReadAllText(
                versionFile
            ).Trim();

            if (Version.TryParse(text, out Version version))
                return version;

            return new Version(0, 0, 0);
        }

        // ============================================================
        // ACTUALIZAR ADDIN
        // ============================================================

        private static void UpdateAddinFile(
            string addinPath,
            string assemblyPath)
        {
            XDocument document = XDocument.Load(
                addinPath
            );

            XElement assemblyElement =
                document.Root.Element("Assembly");

            if (assemblyElement == null)
                throw new InvalidOperationException(
                    "El archivo .addin no contiene <Assembly>."
                );

            assemblyElement.Value = assemblyPath;

            string tempFile = addinPath + ".tmp";

            document.Save(tempFile);

            File.Copy(
                tempFile,
                addinPath,
                true
            );

            File.Delete(tempFile);
        }

        // ============================================================
        // ARGUMENTOS
        // ============================================================

        private static string GetArgument(
            string[] args,
            string name)
        {
            string prefix = "/" + name + ":";

            foreach (string arg in args)
            {
                if (arg.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return arg.Substring(
                        prefix.Length
                    ).Trim('"');
                }
            }

            return null;
        }
    }
}