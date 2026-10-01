using Inventor;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace AzorSuite.Inventor2017
{
    [ProgId("AzorSuite.Inventor2017.StandardAddInServer")]
    [Guid("8F5E4D7A-2B61-4C9E-9A37-6D2F8B14C501")]
    [ComVisible(true)]
    public class StandardAddInServer : ApplicationAddInServer
    {
        private Inventor.Application _inventorApplication;
        private ButtonDefinition _azorSuiteButton;
        private ButtonDefinitionSink_OnExecuteEventHandler _azorSuiteButtonHandler;

        public void Activate(ApplicationAddInSite AddInSiteObject, bool FirstTime)
        {
            _inventorApplication = AddInSiteObject.Application;

            StartUpdater();

            string clientId = "{8F5E4D7A-2B61-4C9E-9A37-6D2F8B14C501}";

            ControlDefinitions controlDefinitions =
                _inventorApplication.CommandManager.ControlDefinitions;

            _azorSuiteButton = controlDefinitions.AddButtonDefinition(
                "Azor Suite",
                "AzorSuite_MainButton",
                CommandTypesEnum.kNonShapeEditCmdType,
                clientId,
                "Abrir Azor Suite",
                "Abrir Azor Suite"
            );

            _azorSuiteButtonHandler =
                new ButtonDefinitionSink_OnExecuteEventHandler(
                    AzorSuiteButton_OnExecute
                );

            _azorSuiteButton.OnExecute += _azorSuiteButtonHandler;

            if (FirstTime)
            {
                CreateUserInterface();
            }
        }

        private void CreateUserInterface()
        {
            UserInterfaceManager uiManager =
                _inventorApplication.UserInterfaceManager;

            string[] ribbonNames =
            {
                "ZeroDoc",
                "Part",
                "Assembly",
                "Drawing"
            };

            foreach (string ribbonName in ribbonNames)
            {
                Ribbon ribbon = uiManager.Ribbons[ribbonName];

                RibbonTab azorSuiteTab;

                try
                {
                    azorSuiteTab = ribbon.RibbonTabs["AzorSuite_Tab"];
                }
                catch
                {
                    azorSuiteTab = ribbon.RibbonTabs.Add(
                        "Azor Suite",
                        "AzorSuite_Tab",
                        "{8F5E4D7A-2B61-4C9E-9A37-6D2F8B14C501}"
                    );
                }

                RibbonPanel mainPanel;

                try
                {
                    mainPanel = azorSuiteTab.RibbonPanels["AzorSuite_MainPanel"];
                }
                catch
                {
                    mainPanel = azorSuiteTab.RibbonPanels.Add(
                        "General",
                        "AzorSuite_MainPanel",
                        "{8F5E4D7A-2B61-4C9E-9A37-6D2F8B14C501}"
                    );
                }

                try
                {
                    mainPanel.CommandControls.AddButton(
                        _azorSuiteButton,
                        true
                    );
                }
                catch
                {
                    // El botón ya existe en este panel.
                }
            }
        }

        private void AzorSuiteButton_OnExecute(NameValueMap Context)
        {
            AzorSuite.UI.AzorSuiteForm.ShowForm();
        }

        public void Deactivate()
        {
            if (_azorSuiteButton != null &&
                _azorSuiteButtonHandler != null)
            {
                _azorSuiteButton.OnExecute -= _azorSuiteButtonHandler;
            }

            _azorSuiteButton = null;
            _azorSuiteButtonHandler = null;
            _inventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int CommandID)
        {
        }

        public object Automation
        {
            get { return null; }
        }

        private void StartUpdater()
        {
            try
            {
                string updaterPath = System.IO.Path.Combine(
                    System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.LocalApplicationData),
                    "AzorSuite",
                    "Updater",
                    "AzorSuite.Updater.exe"
                );

                if (!System.IO.File.Exists(updaterPath))
                    return;

                string addinPath = System.IO.Path.Combine(
                    System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.ApplicationData),
                    "Autodesk",
                    "Inventor 2017",
                    "Addins",
                    "AzorSuite.Inventor2017.addin"
                );

                string arguments =
                    "/addin:\"" + addinPath + "\" " +
                    "/pid:" + Process.GetCurrentProcess().Id;

                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterPath,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false
                });
            }
            catch
            {
                // El updater nunca debe impedir que Inventor cargue Azor Suite.
            }
        }
    }
}