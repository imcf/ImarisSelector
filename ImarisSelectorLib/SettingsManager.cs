﻿using System;
using System.IO;
using System.Collections.Generic;

namespace ImarisSelectorLib
{
    /// <summary>
    /// Simple class to hold the settings.
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
        /// Constructor.
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
        /// <returns>A Settings object with the loaded settings.</returns>
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
                        String[] parts = line.Split('=');

                        if (parts.Length != 2)
                        {
                            continue;
                        }

                        if (parts[0].Equals("FileVersion"))
                        {
                            if (!parts[1].Equals("ImarisSelector Settings File version 1.0.0"))
                            {
                                // Invalid settings file version - ignore the file
                                return new Settings();
                            }
                        }
                        else if (parts[0].Equals("ImarisVersion"))
                        {
                            settings.ImarisVersion = parts[1];
                        }
                        else if (parts[0].Equals("ImarisPath"))
                        {
                            settings.ImarisPath = parts[1];
                        }
                        else
                        {
                            settings.ProductsWithEnabledState.Add(parts[0], parts[1].Equals("true"));
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
        /// <param name="settings">A Settings object with the settings to write to disk.</param>
        /// <returns>True if the settings could be saved to disk, false otherwise.</returns>
        public static bool write(Settings settings)
        {
            // We put the whole writing block in a try...catch block
            try
            {

                // Make sure the settings directory exists
                CreateSettingsDirIfNeeded(SettingsDirectoryName());

                // Write the settings to file
                StreamWriter file = new StreamWriter(settingsFullFileName());
                if (file != null)
                {
                    file.WriteLine("FileVersion=ImarisSelector Settings File version 1.0.0");
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
            catch
            {
                // Could not write to disk!
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
