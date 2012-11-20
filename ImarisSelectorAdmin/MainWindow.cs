using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ImarisSelectorLib;

namespace ImarisSelectorAdmin
{
    public partial class MainWindow : Form
    {
        private String m_ImarisPath;
        private String m_ImarisVersion;
        private String m_ImarisSelectorAdminVersion = "0.0.0";

        public MainWindow()
        {
            InitializeComponent();

            // Get the application settings from the registry
            String ImarisVersionFromSettingsFile;
            String ImarisPathFromSettingsFile;
            if (ApplicationSettings.read(out ImarisVersionFromSettingsFile,
                out ImarisPathFromSettingsFile))
            {
                this.m_ImarisPath = ImarisPathFromSettingsFile;
                this.m_ImarisVersion = ImarisVersionFromSettingsFile;
                buttonImarisPath.Text = this.m_ImarisPath;
            }

        }

        /// <summary>
        /// Ask the admin to pick the Imaris executable to be managed by ImarisSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonImarisPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
	        if (result == DialogResult.OK) // Test result.
	        {
                // Set the Imaris path
                this.m_ImarisPath = dialog.FileName;
                
                // Display it also on the button
                buttonImarisPath.Text = dialog.FileName;

                // Set Imaris version
                processImarisExecutablePath();

                // Store ImarisPath and ImarisVersion to the registry
                ApplicationSettings.write(this.m_ImarisVersion, this.m_ImarisPath);
                
	        }
        }

        /// <summary>
        /// Processes the Imaris executable path to extract the needed information
        /// </summary>
        private void processImarisExecutablePath()
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
                    "Process Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        /// <summary>
        /// Display application help.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            // Display version and copyright information
            MessageBox.Show("The administrator backend of ImarisSelector lets you choose\n" +
                "the Imaris executable to manage.\n\n" +
                "Click on the button to browse for the Imaris.exe file of choice.",
                "ImarisSelector :: Admin -- Help",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Display application About dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAbout_Click(object sender, EventArgs e)
        {
            // Display version and copyright information
            MessageBox.Show("ImarisSelector :: Admin v" + m_ImarisSelectorAdminVersion + "\n\n" +
                "Copyright (c) Aaron Ponti, 2012.",
                "ImarisSelector :: Admin -- About",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

    }
}
