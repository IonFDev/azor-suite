using Inventor;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
            string uiPath =
                @"C:\Users\Usuario\Desktop\Programas\Azor Suite\AzorSuite.WebView2Test\bin\Release\AzorSuite.WebView2Test.exe";

            if (!System.IO.File.Exists(uiPath))
            {
                System.Windows.Forms.MessageBox.Show(
                    "No se encuentra:\n" + uiPath,
                    "Azor Suite"
                );

                return;
            }

            System.Diagnostics.Process.Start(uiPath);
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
    }
}