using System;
using System.IO;
using System.Collections.Generic;

namespace ImarisSelectorLib
{
    /// <summary>
    /// Simple class to hold the settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Imaris version in the form "Imaris x64 7.6" (no patch version).
        /// </summary>
        public String ImarisVersion { get; set; }

        /// <summary>
        /// Full path of the Imaris executable.
        /// </summary>
        public String ImarisPath { get; set; }

        /// <summary>
        /// List of product names with their enabled state.
        /// </summary>
        public Dictionary<String, bool> ProductsWithEnabledState { get; set; }

        /// <summary>
        /// True if the Settings is valid (i.e. all fields are defined and valid).
        /// </summary>
        public bool isValid { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Settings()
        {
            // Set defaults
            ImarisVersion = "";
            ImarisPath = "";
            ProductsWithEnabledState = new Dictionary<String, bool>();
            isValid = false;
        }
    }

    /// <summary>
    /// Static class to manage the ImarisSelector settings.
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>
        /// Get the ImarisSelector settings from the settings file.
        /// </summary>
        public static Settings read()
        {
            // Initialize output
            Settings settings = new Settings();

            // Does the settings file exist?
            if (File.Exists(settingsFullFileName()))
            {
                // Read the settings file
                // First line is the header, the second is ImarisVersion, the third is ImarisPath
                StreamReader file = new System.IO.StreamReader(settingsFullFileName());
                if (file != null)
                {
                    String line;
                    while ((line = file.ReadLine()) != null)
                    {
                        // Get the line key and value
                        String[] components = line.Split('=');

                        if (components.Length != 2)
                        {
                            continue;
                        }

                        if (components[0].Equals("FileVersion"))
                        {
                            // TODO Check the version
                        }
                        else if (components[0].Equals("ImarisVersion"))
                        {
                            settings.ImarisVersion = components[1];
                        }
                        else if (components[0].Equals("ImarisPath"))
                        {
                            settings.ImarisPath = components[1];
                        }
                        else
                        {
                            settings.ProductsWithEnabledState.Add(components[0], components[1].Equals("true"));
                        }
                    }
                    file.Close();
                }
            }

            // Now check
            if (!settings.ImarisVersion.Equals("") && !settings.ImarisPath.Equals("") &&
                settings.ProductsWithEnabledState.Count > 0)
            {
                settings.isValid = true;
            }
            else
            {
                settings.isValid = false;
            }

            // Return the settings
            return settings;
        }

        /// <summary>
        /// Set the application settings to the settings file.
        /// </summary>
        /// <param name="ImarisVersion">Imaris version, in the form "Imaris x64 7.6"</param>
        /// <param name="ImarisPath">Full path of the Imaris executable.</param>
        /// <param name="ProductsWithStates">Dictionary of product names with their states.</param>

        /// <returns></returns>
        public static bool write(Settings settings)
        {
            // Make sure the settings directory exists
            CreateSettingsDirIfNeeded(SettingsDirectoryName());

            // Write the settings to file
            StreamWriter file = new StreamWriter(settingsFullFileName());
            if (file != null)
            {
                file.WriteLine("FileVersion=ImarisSelector Settings File version 1");
                file.WriteLine("ImarisVersion=" + settings.ImarisVersion);
                file.WriteLine("ImarisPath=" + settings.ImarisPath);
                foreach (KeyValuePair<String, bool> entry in settings.ProductsWithEnabledState)
                {
                    String state = "false";
                    if (entry.Value == true)
                    {
                        state = "true";
                    }
                    file.WriteLine(entry.Key + "=" + state);
                }
                file.Close();

                // Success
                return true;
            }
            else
            {
                // Failure
                return false;
            }

        }

        /// <summary>
        /// Returns the full path fo the settings directory.
        /// </summary>
        /// <returns>Full path of the settings directory.</returns>
        private static String SettingsDirectoryName()
        {
            // Get the commonAppData path
            String commonAppData = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData);

            // Append the ImarisSelector path
            return commonAppData + @"\ImarisSelector\";
        }


        /// <summary>
        /// Returns the setting file name with full path.
        /// </summary>
        /// <returns>Setting file name with full path.</returns>
        private static String settingsFullFileName()
        {
            // Append the ImarisSelector path
            return SettingsDirectoryName() + @"settings.conf";
        }

        /// <summary>
        /// Create the specified directory if it does not exist.
        /// </summary>
        /// <param name="dir">Full directory name to be created.</param>
        private static void CreateSettingsDirIfNeeded(String dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

    }
}
