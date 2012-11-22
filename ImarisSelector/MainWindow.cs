using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using ImarisSelectorLib;
using System.IO;

namespace ImarisSelector
{
    public partial class MainWindow : Form
    {
        protected String m_ImarisSelectorVersion = "0.0.0";
        protected RegistryManager m_Manager;

        // ImarisVersion and ImarisFullPath are read from the ImarisSelector settings file
        protected String m_ImarisVersion;
        protected String m_ImarisPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            // Get the application settings
            String ImarisVersionFromSettings = "";
            String ImarisPathFromSettings = "";

            if (!ApplicationSettings.read(out ImarisVersionFromSettings, out ImarisPathFromSettings))
            {
                // Inform the user
                MessageBox.Show("ImarisSelector was not configured on this machine!\n" +
                    "Please contact your administrator.",
                    "Process Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Exit
                Environment.Exit(1);
            }

            // Store the loaded entries
            this.m_ImarisVersion = ImarisVersionFromSettings;
            this.m_ImarisPath = ImarisPathFromSettings;

            // Instantiate the registry manager
            this.m_Manager = new RegistryManager(this.m_ImarisVersion);

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
            if (!m_Manager.LicenseInformationFound())
            {
                // This call will start Imaris and close ImarisSelector
                StartImaris();
            }

            // Initially enable all licenses...
            this.m_Manager.EnableAllModules();

            // ... and disable the selected ones (in the registry)
            this.m_Manager.DisableModules(m_Manager.GetSelectedModuleNames());

            // Fill the checkedListBox
            FillLicenseList();
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
            String moduleName = (String)checkedListBoxLicenses.SelectedItem;
            
            // Update the module name field
            labelLicenseName.Text = moduleName;

            // Update the description field
            labelLicenseDescription.Text = m_Manager.GetModuleDescription(moduleName);
        }

        /// <summary>
        /// Update registry in response to licenses check status change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBoxLicenses_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Get the module name
            String module = checkedListBoxLicenses.Items[e.Index].ToString();

            // Set the new state
            if (e.NewValue == CheckState.Checked)
            {
                // Enable the module
                m_Manager.EnableModule(module);
            }
            else
            {
                // Disable the module
                m_Manager.DisableModule(module);
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
            FillLicenseList();
        }

        /// <summary>
        /// Callback for the change of check state of the radioSelAdvanced radio button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioSelAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            // Fill the checkedListBox
            FillLicenseList();
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
            ProcessStartInfo startInfo = new ProcessStartInfo(this.m_ImarisPath);

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
        private void FillLicenseList()
        {
            List<String> moduleNames;

            // Fill the checkedListBox with either the selected or the 
            // complete list, depending on the datio button values
            if (radioSelSimple.Checked)
            {
                moduleNames = m_Manager.GetSelectedModuleNames();
            }
            else
            {
                moduleNames = m_Manager.GetAllModuleNames();
            }

            // Remove current items
            checkedListBoxLicenses.Items.Clear();

            // Fill the checkedListBox
            foreach (String moduleName in moduleNames)
            {
                bool isChecked = m_Manager.GetLicenseState(moduleName).Equals("true");
                checkedListBoxLicenses.Items.Add(moduleName, isChecked);
            }
        }

        /// <summary>
        /// Display the application Help dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            // Display version and copyright information
            MessageBox.Show("Will follow.",
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
            MessageBox.Show("ImarisSelector v" + m_ImarisSelectorVersion + "\n\n" +
                "Aaron Ponti\n" +
                "Single-Cell Facility\n" +
                "Department of Biosystems Science and Engineering\n" +
                "ETHZ (Basel)\n" +
                "Copyright (c) 2012.",
                "ImarisSelector -- About",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

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
                    foreach (String moduleName in this.m_Manager.GetAllModuleNames())
                    {
                        file.WriteLine(moduleName + "\n");
                    }
                    file.WriteLine();
                    file.WriteLine("{0} modules exported.", 
                        m_Manager.GetAllModuleNames().Count);
                    file.Close();
                }
            }
        }
    }
}