using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace AzorSuite.Publisher
{
    internal class Program
    {
        private const string Version = "1.0.1";

        private const string ServerRoot =
            @"\\10.19.20.240\tecnico\Proyectos\MODELOS\_MODELO HOTEL\XX AZOR SUITE (NO MODIFICAR)";

        private const string ProductName = "Inventor2017";

        static int Main(string[] args)
        {
            try
            {
                Publish();

                Console.WriteLine("Publicación completada correctamente.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        private static void Publish()
        {
            string projectRoot = GetProjectRoot();

            string inventorProject =
                Path.Combine(projectRoot, "AzorSuite.Inventor2017");

            string uiProject =
                Path.Combine(projectRoot, "AzorSuite.UI");

            string updaterProject =
                Path.Combine(projectRoot, "AzorSuite.Updater");

            string inventorOutput =
                Path.Combine(
                    inventorProject,
                    "bin",
                    "x64",
                    "Release"
                );

            string uiOutput =
                Path.Combine(
                    uiProject,
                    "bin",
                    "x64",
                    "Release"
                );

            string updaterOutput =
                Path.Combine(
                    updaterProject,
                    "bin",
                    "x64",
                    "Release"
                );

            ValidateFile(
                Path.Combine(
                    inventorOutput,
                    "AzorSuite.Inventor2017.dll"
                )
            );

            ValidateFile(
                Path.Combine(
                    uiOutput,
                    "AzorSuite.UI.dll"
                )
            );

            ValidateFile(
                Path.Combine(
                    uiOutput,
                    "Microsoft.Web.WebView2.Core.dll"
                )
            );

            ValidateFile(
                Path.Combine(
                    uiOutput,
                    "Microsoft.Web.WebView2.WinForms.dll"
                )
            );

            ValidateFile(
                Path.Combine(
                    uiOutput,
                    "runtimes",
                    "win-x64",
                    "native",
                    "WebView2Loader.dll"
                )
            );

            ValidateFile(
                Path.Combine(
                    updaterOutput,
                    "AzorSuite.Updater.exe"
                )
            );

            string serverVersionDirectory =
                Path.Combine(
                    ServerRoot,
                    ProductName,
                    Version
                );

            string packageDirectory =
                Path.Combine(
                    serverVersionDirectory,
                    "Package"
                );

            Directory.CreateDirectory(packageDirectory);

            // --------------------------------------------------------
            // Copiar Add-in
            // --------------------------------------------------------

            CopyFile(
                Path.Combine(
                    inventorOutput,
                    "AzorSuite.Inventor2017.dll"
                ),
                packageDirectory
            );

            // --------------------------------------------------------
            // Copiar UI
            // --------------------------------------------------------

            CopyFile(
                Path.Combine(
                    uiOutput,
                    "AzorSuite.UI.dll"
                ),
                packageDirectory
            );

            CopyFile(
                Path.Combine(
                    uiOutput,
                    "Microsoft.Web.WebView2.Core.dll"
                ),
                packageDirectory
            );

            CopyFile(
                Path.Combine(
                    uiOutput,
                    "Microsoft.Web.WebView2.WinForms.dll"
                ),
                packageDirectory
            );

            CopyFile(
                Path.Combine(
                    uiOutput,
                    "runtimes",
                    "win-x64",
                    "native",
                    "WebView2Loader.dll"
                ),
                packageDirectory
            );

            // --------------------------------------------------------
            // Copiar Updater
            // --------------------------------------------------------

            string updaterDirectory =
                Path.Combine(
                    packageDirectory,
                    "Updater"
                );

            Directory.CreateDirectory(
                updaterDirectory
            );

            CopyFile(
                Path.Combine(
                    updaterOutput,
                    "AzorSuite.Updater.exe"
                ),
                updaterDirectory
            );

            // --------------------------------------------------------
            // Crear ZIP
            // --------------------------------------------------------

            string zipPath =
                Path.Combine(
                    ServerRoot,
                    ProductName,
                    "AzorSuite-Inventor2017-" +
                    Version +
                    ".zip"
                );

            if (File.Exists(zipPath))
                File.Delete(zipPath);

            ZipFile.CreateFromDirectory(
                packageDirectory,
                zipPath,
                CompressionLevel.Optimal,
                false
            );

            // --------------------------------------------------------
            // Actualizar manifest
            // --------------------------------------------------------

            UpdateManifest(
                zipPath
            );

            Console.WriteLine(
                "Versión publicada: " + Version
            );

            Console.WriteLine(
                "Servidor: " + ServerRoot
            );
        }

        private static void UpdateManifest(
            string zipPath)
        {
            string manifestPath =
                Path.Combine(
                    ServerRoot,
                    "manifest.xml"
                );

            XDocument document;

            if (File.Exists(manifestPath))
            {
                document = XDocument.Load(
                    manifestPath
                );
            }
            else
            {
                document = new XDocument(
                    new XElement(
                        "AzorSuite"
                    )
                );
            }

            XElement root = document.Root;

            XElement product =
                root.Elements("Product")
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                (string)x.Attribute("Name"),
                                ProductName,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (product == null)
            {
                product = new XElement(
                    "Product"
                );

                root.Add(product);
            }

            product.SetAttributeValue(
                "Name",
                ProductName
            );

            product.SetAttributeValue(
                "Version",
                Version
            );

            string relativePackage =
                Path.Combine(
                    ProductName,
                    Path.GetFileName(zipPath)
                ).Replace(
                    Path.DirectorySeparatorChar,
                    '\\'
                );

            product.SetAttributeValue(
                "Package",
                relativePackage
            );

            document.Save(
                manifestPath
            );
        }

        private static void ValidateFile(
            string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "No se encontró el archivo requerido:",
                    path
                );
            }
        }

        private static void CopyFile(
            string source,
            string destinationDirectory)
        {
            Directory.CreateDirectory(
                destinationDirectory
            );

            File.Copy(
                source,
                Path.Combine(
                    destinationDirectory,
                    Path.GetFileName(source)
                ),
                true
            );
        }

        private static string GetProjectRoot()
        {
            DirectoryInfo directory =
                new DirectoryInfo(
                    AppDomain.CurrentDomain.BaseDirectory
                );

            // Release
            directory = directory.Parent;

            // x64
            directory = directory.Parent;

            // bin
            directory = directory.Parent;

            // AzorSuite.Publisher
            directory = directory.Parent;

            // Azor Suite
            return directory.FullName;
        }
    }
}