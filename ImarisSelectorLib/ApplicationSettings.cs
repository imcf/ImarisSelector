using System;
using System.IO;
using System.Collections.Generic;

namespace ImarisSelectorLib
{
    /// <summary>
    /// Static class to manage the ImarisSelector settings.
    /// </summary>
    public static class ApplicationSettings
    {
        /// <summary>
        /// Get the ImarisSelector settings from the settings file.
        /// </summary>
        public static bool read(out String ImarisVersion, out String ImarisPath,
            out Dictionary<String, bool> ImarisProducts)
        {
            // Initialize output parameters
            ImarisVersion = "";
            ImarisPath = "";
            ImarisProducts = new Dictionary<String, bool>();

            // Does the settings file exist?
            String ImarisVersionFromFile = "";
            String ImarisPathFromFile = "";
            Dictionary<String, bool> ImarisProductStates = new Dictionary<String, bool>();
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
                            ImarisVersionFromFile = components[1];
                        }
                        else if (components[0].Equals("ImarisPath"))
                        {
                            ImarisPathFromFile = components[1];
                        }
                        else
                        {
                            if (components[1].Equals("true"))
                            {
                                ImarisProductStates.Add(components[0], true);
                            }
                            else
                            {
                                ImarisProductStates.Add(components[0], false);
                            }
                        }
                    }
                    file.Close();
                }
            }
            
            // Now check
            if (ImarisVersionFromFile.Equals(""))
            {
                return false;
            }
            if (ImarisPathFromFile.Equals(""))
            {
                return false;
            }
            if (ImarisProductStates.Count == 0)
            {
                return false;
            }

            // Ready to return the values
            ImarisVersion = ImarisVersionFromFile;
            ImarisPath = ImarisPathFromFile;
            ImarisProducts = new Dictionary<String, bool>(ImarisProductStates);


            // Success
            return true;
        }

        /// <summary>
        /// Set the application settings to the settings file.
        /// </summary>
        /// <param name="ImarisVersion">Imaris version, in the form "Imaris x64 7.6"</param>
        /// <param name="ImarisPath">Full path of the Imaris executable.</param>
        /// <param name="ProductsWithStates">Dictionary of product names with their states.</param>

        /// <returns></returns>
        public static bool write(String ImarisVersion, String ImarisPath, Dictionary<String, bool> ProductsWithStates)
        {
            // Make sure the settings directory exists
            CreateSettingsDirIfNeeded(SettingsDirectoryName());

            // Write the settings to file
            StreamWriter file = new StreamWriter(settingsFullFileName());
            if (file != null)
            {
                file.WriteLine("FileVersion=ImarisSelector Settings File version 1");
                file.WriteLine("ImarisVersion=" + ImarisVersion);
                file.WriteLine("ImarisPath=" + ImarisPath);
                foreach (KeyValuePair<String, bool> entry in ProductsWithStates)
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
