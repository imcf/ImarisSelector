using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ImarisSelectorLib;
using System.Collections.Generic;

namespace ImarisSelectorAdmin
{
    /// <summary>
    /// ImarisSelectorAdmin main window.
    /// </summary>
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Full path of the Imaris executable.
        /// </summary>
        private String m_ImarisPath;

        /// <summary>
        /// Imaris version in the form "Imaris x64 7.6" (no patch version).
        /// </summary>
        private String m_ImarisVersion;

        /// <summary>
        /// Dictionary of (name, description) for all known products.
        /// </summary>
        private Dictionary<String, String> m_Products;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            // Intialize properties
            this.m_ImarisVersion = "";
            this.m_ImarisPath = "";

            // Initialize the Module Manager
            this.m_Products = new ModuleCatalog().GetAllKnownProducts();

            // Initialize the UI
            InitializeComponent();

            // Make window unresizable
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Get the application settings from the settings file.
            String ImarisVersionFromSettingsFile = "";
            String ImarisPathFromSettingsFile = "";
            Dictionary<String, bool> ImarisProductStates = new Dictionary<String, bool>();
            if (ApplicationSettings.read(out ImarisVersionFromSettingsFile,
                out ImarisPathFromSettingsFile, out ImarisProductStates))
            {
                this.m_ImarisPath = ImarisPathFromSettingsFile;
                this.m_ImarisVersion = ImarisVersionFromSettingsFile;
                buttonImarisPath.Text = this.m_ImarisPath;
            }

            // Fill in the list of products
            foreach (String productName in this.m_Products.Keys)
            {
                bool state;
                if (ImarisProductStates.ContainsKey(productName))
                {
                    if (!ImarisProductStates.TryGetValue(productName, out state))
                    {
                        state = true;
                    }
                }
                else
                {
                    state = true;
                }

                // Add the product with the state read from the application settings
                checkedListBoxProducts.Items.Add(productName, state);
            }

            // Enable save button?
            if (!this.m_ImarisPath.Equals(""))
            {
                this.buttonSave.Enabled = true;
            }

        }

        /// <summary>
        /// Ask the admin to pick the Imaris executable to be managed by ImarisSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonImarisPath_Click(object sender, EventArgs e)
        {
            // Open a file dialog to pick the Imaris executable
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            dialog.Title = "Please select the Imaris executable to manage";
            DialogResult result = dialog.ShowDialog();
	        if (result == DialogResult.OK)
	        {
                // Set the Imaris path
                this.m_ImarisPath = dialog.FileName;
                
                // Display it also on the button
                buttonImarisPath.Text = dialog.FileName;

                // Extract version information from the Imaris path
                if (processImarisExecutablePath() == true)
                {
                    this.buttonSave.Enabled = true;
                }
	        }
        }

        /// <summary>
        /// Processes the Imaris executable path to extract version information.
        /// </summary>
        private bool processImarisExecutablePath()
        {
            // Get the Imaris version from the path
            // We need to extract the optional 'x64 string' and major plus minor
            // version to look up the correct licenses in the registry. The patch
            // version is ignored.
            Match match = Regex.Match(this.m_ImarisPath, 
                @"(Imaris)\s*(x64)*\s*(\d{1,2}).(\d{1,2})(.\d{1,2})*\\Imaris\.exe",
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                if (!match.Groups[2].Value.Equals("x64"))
                {
                    // 32-bit version of Imaris
                    this.m_ImarisVersion = match.Groups[1].Value + " " +
                        match.Groups[3].Value + "." +
                        match.Groups[4].Value;
                }
                else
                {
                    // 64-bit version of Imaris
                    this.m_ImarisVersion = match.Groups[1].Value + " " +
                        match.Groups[2].Value + " " +
                        match.Groups[3].Value + "." +
                        match.Groups[4].Value;
                }
            }
            else
            {
                // Inform the user
                MessageBox.Show("Invalid Imaris executable selected!\n" +
                    "Please try again.",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Return success
            return true;

        }

        /// <summary>
        /// Display application help.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            // Display short usage help
            MessageBox.Show("The administrator backend of ImarisSelector lets you choose\n" +
                "the Imaris executable to manage.\n\n" +
                "Click on the button to browse for the Imaris.exe file of choice.",
                "ImarisSelector :: Admin -- Help",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Return the application version.
        /// </summary>
        /// <returns></returns>
        private String GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductVersion;
        }

        /// <summary>
        /// Display application About dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAbout_Click(object sender, EventArgs e)
        {
            // Display version and copyright information
            MessageBox.Show("ImarisSelector :: Admin v" + GetVersion() + " (preview release)\n\n" +
                "Aaron Ponti\n" +
                "Single-Cell Facility\n" +
                "Department of Biosystems Science and Engineering\n" +
                "ETHZ (Basel)\n" +
                "Copyright (c) 2012.",
                "ImarisSelector :: Admin -- About",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        /// <summary>
        /// Update product description in the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBoxProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected product name
            String productName = (String)checkedListBoxProducts.SelectedItem;

            String descr;
            if (!this.m_Products.TryGetValue(productName, out descr))
            {
                descr = "Unknown module.";
            }
            labelProductDescription.Text = descr;
        }

        /// <summary>
        /// Save settings to disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Make sure everything is set
            if (this.m_ImarisPath.Equals("") || this.m_ImarisVersion.Equals(""))
            {
                // Inform the user
                MessageBox.Show("Please choose a valid Imaris executable first!",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Collect all products and states
            Dictionary<String, bool> productsWithStates = new Dictionary<String, bool>();

            // Store the items with their checked state
            for (int i = 0; i < checkedListBoxProducts.Items.Count; i++)
            {
                bool state = false;
                if (checkedListBoxProducts.GetItemChecked(i))
                {
                    state = true;
                }
                productsWithStates.Add(checkedListBoxProducts.Items[i].ToString(), state);
            }

            // Store ImarisPath and ImarisVersion to the settings file
            ApplicationSettings.write(this.m_ImarisVersion, this.m_ImarisPath, productsWithStates);
        }
    }
}
