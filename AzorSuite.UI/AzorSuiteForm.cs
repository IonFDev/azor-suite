using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

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

                _webView.NavigateToString(@"
                    <!DOCTYPE html>
                    <html lang='es'>
                    <head>
                        <meta charset='UTF-8'>
                        <title>Azor Suite</title>

                        <style>
                            body {
                                margin: 0;
                                font-family: Segoe UI, sans-serif;
                                background: #f3f4f6;
                                display: flex;
                                align-items: center;
                                justify-content: center;
                                height: 100vh;
                            }

                            .box {
                                background: white;
                                padding: 40px;
                                border-radius: 12px;
                                text-align: center;
                                box-shadow: 0 4px 20px rgba(0,0,0,.12);
                            }

                            h1 {
                                margin: 0 0 10px;
                            }

                            p {
                                color: #666;
                            }

                            button {
                                margin-top: 20px;
                                padding: 12px 24px;
                                border: none;
                                border-radius: 6px;
                                background: #e67e22;
                                color: white;
                                font-size: 16px;
                                cursor: pointer;
                            }
                        </style>
                    </head>

                    <body>

                        <div class='box'>
                            <h1>Azor Suite</h1>
                            <p>WebView2 funciona correctamente.</p>

                            <button onclick='alert(""Azor Suite funciona!"")'>
                                Probar
                            </button>
                        </div>

                    </body>
                    </html>");

            } catch (Exception ex)
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