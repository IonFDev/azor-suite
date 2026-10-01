using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

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
                await _webView.EnsureCoreWebView2Async();

                _webView.NavigateToString(@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Azor Suite</title>

    <style>
        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            font-family: Segoe UI, Arial, sans-serif;
            background: #f4f6f8;
            color: #1f2933;
        }

        .header {
            padding: 28px 36px;
            background: white;
            border-bottom: 1px solid #d9dee3;
        }

        .header h1 {
            margin: 0 0 6px 0;
            font-size: 30px;
        }

        .header p {
            margin: 0;
            font-size: 16px;
            color: #697586;
        }

        .content {
            padding: 36px;
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 20px;
        }

        .card {
            background: white;
            border: 1px solid #d9dee3;
            border-radius: 12px;
            padding: 24px;
            min-height: 150px;
        }

        .card h2 {
            margin-top: 0;
            font-size: 20px;
        }

        .card p {
            color: #697586;
            line-height: 1.5;
        }
    </style>
</head>

<body>

    <header class='header'>
        <h1>Azor Suite</h1>
        <p>Plataforma de automatización para Autodesk Inventor</p>
    </header>

    <main class='content'>

        <div class='grid'>

            <div class='card'>
                <h2>Planos</h2>
                <p>
                    Herramientas para trabajar con planos,
                    dimensiones y documentación.
                </p>
            </div>

            <div class='card'>
                <h2>Piezas</h2>
                <p>
                    Automatización y consulta de información
                    de piezas.
                </p>
            </div>

            <div class='card'>
                <h2>Ensamblajes</h2>
                <p>
                    Consulta y automatización de ensamblajes
                    de Inventor.
                </p>
            </div>

            <div class='card'>
                <h2>Listados</h2>
                <p>
                    Generación de listados y extracción
                    de información.
                </p>
            </div>

            <div class='card'>
                <h2>Herramientas</h2>
                <p>
                    Automatizaciones y utilidades para
                    el trabajo diario.
                </p>
            </div>

            <div class='card'>
                <h2>IA</h2>
                <p>
                    Consulta de información mediante
                    lenguaje natural.
                </p>
            </div>

        </div>

    </main>

</body>
</html>");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error iniciando Azor Suite",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static AzorSuiteForm _instance;

        public static void ShowForm()
        {
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new AzorSuiteForm();
                _instance.FormClosed += (s, e) =>
                {
                    _instance.Dispose();
                    _instance = null;
                };

                _instance.Show();
            }
            else
            {
                _instance.BringToFront();
                _instance.Activate();
            }
        }
    }
}