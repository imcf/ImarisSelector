using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ImarisSelectorLib;

namespace ImarisSelector
{
    /// <summary>
    /// ImarisSelector main window.
    /// </summary>
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Settings.
        /// </summary>
        private Settings m_Settings;

        /// <summary>
        /// Protected RegistryManager instance.
        /// </summary>
        ///protected RegistryManager m_Manager;

        protected ModuleManager m_ModuleManager;

        /// <summary>
        /// Dictionary of Imaris product names and state.
        /// </summary>
        protected Dictionary<String, bool> m_ImarisProducts;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            // Get the application settings
            this.m_Settings = SettingsManager.read();
            if (!this.m_Settings.isValid)
            {
                // Inform the user
                MessageBox.Show("ImarisSelector was not configured on this machine!\n" +
                    "Please contact your administrator.",
                    "Process Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Exit
                Environment.Exit(1);
            }

            // Instantiate the registry manager
            ///this.m_Manager = new RegistryManager(this.m_Settings.ImarisVersion);
            this.m_ModuleManager = new ModuleManager(this.m_Settings);

            // Initialize the window components
            InitializeComponent();

            // Make window unresizable
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

        }

        /// <summary>
        /// Initialize the MainWindow element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Load(object sender, EventArgs e)
        {
            // If no license information is found in the registry, it most likely
            // means that Imaris has never been started by this user. So, we just
            // go ahead and launch it.
            if (!this.m_ModuleManager.LicenseInformationFound())
            {
                // This call will start Imaris and close ImarisSelector
                StartImaris();
            }

            // Initially disable all modules
            this.m_ModuleManager.DisableAllModules();

            // But enable "Imaris" and "File Reader"
            this.m_ModuleManager.EnableProducts(new List<String> {"Imaris", "File Reader"});

            // Fill the checkedListBox
            FillProductOrModuleList();
        }

        /// <summary>
        /// Update license name and description in the UI in response to module selection
        /// in the checkedListBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBoxLicenses_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected module name
            String itemName = (String)checkedListBoxLicenses.SelectedItem;

            if (isProductView())
            {
                // Update the product name and description fields
                labelLicenseName.Text = itemName;
                labelLicenseDescription.Text = this.m_ModuleManager.GetProductDescription(itemName);
                labelLicenseMoreDescription.Text = "";
            }
            else
            {
                // Update the module name and description fields
                labelLicenseName.Text = this.m_ModuleManager.GetModuleName(itemName);
                labelLicenseDescription.Text = 
                    this.m_ModuleManager.GetProductDescription(this.m_ModuleManager.GetProductForModule(itemName));
                labelLicenseMoreDescription.Text = 
                    this.m_ModuleManager.GetModuleDescription(itemName);
            }
        }

        /// <summary>
        /// Update registry in response to licenses check status change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBoxLicenses_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Get the module name
            String itemName = checkedListBoxLicenses.Items[e.Index].ToString();

            // Set the new state
            if (e.NewValue == CheckState.Checked)
            {
                if (isProductView())
                {
                    // Enable the module
                    this.m_ModuleManager.EnableModules(this.m_ModuleManager.GetModulesForProduct(itemName));
                }
                else
                {
                    // Enable the module
                    this.m_ModuleManager.EnableModule(itemName);
                }
            }
            else
            {
                if (radioSelByProduct.Checked)
                {
                    // Enable the module
                    this.m_ModuleManager.DisableModules(this.m_ModuleManager.GetModulesForProduct(itemName));
                }
                else
                {
                    // Enable the module
                    this.m_ModuleManager.DisableModule(itemName);
                }
            }
        }

        /// <summary>
        /// Callback for the change of check state of the radioSelSimple radio button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioSelSimple_CheckedChanged(object sender, EventArgs e)
        {
            // Fill the checkedListBox
            FillProductOrModuleList();
        }

        /// <summary>
        /// Callback for the change of check state of the radioSelAdvanced radio button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioSelAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            // Fill the checkedListBox
            FillProductOrModuleList();
        }

        /// <summary>
        /// Callback for the click event of the buttonStartImaris button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStartImaris_Click(object sender, EventArgs e)
        {
            StartImaris();
        }

        /// <summary>
        /// Launches Imaris.
        /// </summary>
        private void StartImaris()
        {
            // Get process start info for Imaris
            ProcessStartInfo startInfo = new ProcessStartInfo(this.m_Settings.ImarisPath);

            // Try launching the application
            try
            {
                // Start the application
                Process p = Process.Start(startInfo);
            }
            catch (Exception)
            {
                // Inform the user
                MessageBox.Show("Imaris could not be started!\n" +
                    "Please contact your administrator.",
                    "Process Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Imaris started correctly, we can close ImarisSelector
            Application.Exit();
        }

        /// <summary>
        /// Fill the checkedListBox with module names and associated license state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillProductOrModuleList()
        {
            // Unbind the event handler to prevent the ItemCheck event t
            this.checkedListBoxLicenses.ItemCheck -=
                new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxLicenses_ItemCheck);

            List<String> moduleNames;

            // Fill the checkedListBox with either the selected or the 
            // complete list, depending on the ratio button values
            if (isProductView())
            {
                moduleNames = this.m_ModuleManager.GetInstalledAndFilteredProductList();
            }
            else
            {
                moduleNames = this.m_ModuleManager.GetFilteredModuleNames();
            }

            // Remove current items
            checkedListBoxLicenses.Items.Clear();

            // Fill the checkedListBox
            bool isEnabled;
            foreach (String moduleName in moduleNames)
            {
                if (isProductView())
                {
                    isEnabled = this.m_ModuleManager.IsProductEnabled(moduleName);
                }
                else
                {
                    isEnabled = this.m_ModuleManager.IsModuleEnabled(moduleName);
                }
                checkedListBoxLicenses.Items.Add(moduleName, isEnabled);
            }

            // Rebind the event handler
            this.checkedListBoxLicenses.ItemCheck += 
                new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxLicenses_ItemCheck);

            // In the pathological case that no products exist or have been enabled by the administrator,
            // inform the user
            if (checkedListBoxLicenses.Items.Count == 0)
            {
                labelLicenseName.Text = "No products installed or enabled.";
            }
        }

        /// <summary>
        /// Display the application Help dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            // Display short usage help
            MessageBox.Show(
                "Enable and/or disable the set of products you want to use in current\n" +
                "Imaris session. You can switch to the \"By module\" view for more\n" +
                "granularity in your selection.",
                "ImarisSelector -- Help",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Display the application About dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAbout_Click(object sender, EventArgs e)
        {
            // Display version and copyright information
            MessageBox.Show("ImarisSelector v" + GetVersion() + " (preview release)\n\n" +
                "Aaron Ponti\n" +
                "Single-Cell Facility\n" +
                "Department of Biosystems Science and Engineering\n" +
                "ETHZ (Basel)\n" +
                "Copyright (c) 2012.",
                "ImarisSelector -- About",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Return the application version.
        /// </summary>
        /// <returns></returns>
        private String GetVersion()
        {
            // Get version info from the assembly
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductVersion;
        }

        /// <summary>
        /// Exports the list of module names to a user-defined (text) file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExport_Click(object sender, EventArgs e)
        {
            // Export the licenses to file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file|*.txt";
            saveFileDialog.Title = "Export module list to file...";
            saveFileDialog.ShowDialog();

            // If the file name is not an empty we save the module names to the selected file
            if (saveFileDialog.FileName != "")
            {
                StreamWriter file = new StreamWriter(saveFileDialog.FileName);
                if (file != null)
                {
                    foreach (String moduleName in this.m_ModuleManager.GetAllModuleNames())
                    {
                        file.WriteLine(moduleName + "\n");
                    }
                    file.WriteLine();
                    file.WriteLine("{0} modules exported.", 
                        this.m_ModuleManager.GetAllModuleNames().Count);
                    file.Close();
                }
            }
        }

        /// <summary>
        /// Checks whether the product view is enabled.
        /// </summary>
        /// <returns>true if the product view is enable, false if the module view is enabled.</returns>
        private bool isProductView()
        {
            return radioSelByProduct.Checked;
        }

    }
}