using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.IO;
using System.Windows.Forms;

namespace AzorSuite.WebView2Test
{
    public partial class Form1 : Form
    {
        private WebView2 webView;

        public Form1()
        {
            InitializeComponent();

            MessageBox.Show(
            "AzorSuite.WebView2Test se ha iniciado.",
            "PRUEBA");

            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(webView);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "AzorSuiteTest",
                    "WebView2_FromInventor");

                Directory.CreateDirectory(userDataFolder);

                string log = "";

                log += "User: " + Environment.UserName + Environment.NewLine;
                log += "UserDomain: " + Environment.UserDomainName + Environment.NewLine;
                log += "64bit OS: " + Environment.Is64BitOperatingSystem + Environment.NewLine;
                log += "64bit Process: " + Environment.Is64BitProcess + Environment.NewLine;
                log += "CurrentDirectory: " + Environment.CurrentDirectory + Environment.NewLine;
                log += "UDF: " + userDataFolder + Environment.NewLine;
                log += "WEBVIEW2_USER_DATA_FOLDER: " +
                       Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER") +
                       Environment.NewLine;
                log += "WEBVIEW2_BROWSER_EXECUTABLE_FOLDER: " +
                       Environment.GetEnvironmentVariable("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER") +
                       Environment.NewLine;
                log += "WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS: " +
                       Environment.GetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS") +
                       Environment.NewLine;

                string logPath = Path.Combine(
                    Path.GetTempPath(),
                    "AzorSuite_WebView2_Test.txt");

                File.WriteAllText(logPath, log);

                Environment.SetEnvironmentVariable(
                    "WEBVIEW2_USER_DATA_FOLDER",
                    null,
                    EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable(
                    "WEBVIEW2_BROWSER_EXECUTABLE_FOLDER",
                    null,
                    EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable(
                    "WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS",
                    null,
                    EnvironmentVariableTarget.Process);

                var environment =
                    await CoreWebView2Environment.CreateAsync(
                        null,
                        userDataFolder);

                await webView.EnsureCoreWebView2Async(environment);

                webView.CoreWebView2.Navigate(
                    "https://www.microsoft.com");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "WebView2");
            }
        }
    }
}