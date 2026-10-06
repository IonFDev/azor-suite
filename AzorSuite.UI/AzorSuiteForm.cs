using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace AzorSuite.UI
{
    public partial class AzorSuiteForm : Form
    {
        private WebView2 _webView;

        public AzorSuiteForm()
        {
            Text = "Azor Suite";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1200;
            Height = 800;
            MinimumSize = new Size(900, 600);

            _webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(_webView);

            Load += AzorSuiteForm_Load;
        }

        private async void AzorSuiteForm_Load(object sender, EventArgs e)
        {
            try
            {
                string userDataFolder = System.IO.Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "AzorSuite",
                    "WebView2",
                    "Inventor2017"
                );

                System.IO.Directory.CreateDirectory(userDataFolder);

                var environment =
                    await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(
                        null,
                        userDataFolder
                    );

                await _webView.EnsureCoreWebView2Async(environment);

                string uiDirectory = Path.Combine(
                    Path.GetDirectoryName(
                        typeof(AzorSuiteForm).Assembly.Location
                    ),
                    "UI"
                );

                _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "azor-suite",
                    uiDirectory,
                    Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow
                );

                _webView.CoreWebView2.Navigate(
                    "https://azor-suite/index.html"
                );

            } 
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Error iniciando Azor Suite",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static Thread _uiThread;
        private static AzorSuiteForm _instance;
        private static readonly object _lock = new object();

        public static void ShowForm()
        {
            lock (_lock)
            {
                if (_instance != null && !_instance.IsDisposed)
                {
                    _instance.BeginInvoke(
                        new Action(() =>
                        {
                            _instance.BringToFront();
                            _instance.Activate();
                        })
                    );

                    return;
                }

                _uiThread = new Thread(() =>
                {
                    _instance = new AzorSuiteForm();

                    _instance.FormClosed += (sender, e) =>
                    {
                        _instance = null;
                        Application.ExitThread();
                    };

                    Application.Run(_instance);
                });

                _uiThread.SetApartmentState(ApartmentState.STA);
                _uiThread.IsBackground = true;
                _uiThread.Start();
            }
        }
    }
}