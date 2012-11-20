using System;
using Microsoft.Win32;
using System.IO;

namespace ImarisSelectorLib
{

    public static class ApplicationSettings
    {
        /// <summary>
        /// Get the ImarisSelector settings from the registry
        /// </summary>
        public static bool read(out String ImarisVersion, out String ImarisPath)
        {
            // Initialize output parameters
            ImarisVersion = "";
            ImarisPath = "";

            // Does the settings file exist?
            String ImarisVersionFromFile = "";
            String ImarisPathFromFile = "";
            if (File.Exists(settingsFullFileName()))
            {
                // Read the settings file
                // First line is the header, the second is ImarisVersion, the third is ImarisPath
                StreamReader file = new System.IO.StreamReader(settingsFullFileName());
                if (file != null)
                {
                    int num = 0;
                    String line;
                    while ((line = file.ReadLine()) != null)
                    {
                        num++;
                        if (num == 1)
                        {
                            // TODO Check the version
                        }
                        else if (num == 2)
                        {
                            ImarisVersionFromFile = line;
                        }
                        else if (num == 3)
                        {
                            ImarisPathFromFile = line;
                        }
                        else
                        {
                            // TODO Inform that the file is corrupted
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

            // Ready to return the values
            ImarisVersion = ImarisVersionFromFile;
            ImarisPath = ImarisPathFromFile;

            // Success
            return true;
        }

        /// <summary>
        /// Set the application settings to the registry
        /// </summary>
        /// <returns></returns>
        public static bool write(String ImarisVersion, String ImarisPath)
        {
            // Make sure the settings directory exists
            CreateSettingsDirIfNeeded(SettingsDirectoryName());

            // Write the settings to file
            StreamWriter file = new System.IO.StreamWriter(settingsFullFileName());
            if (file != null)
            {
                file.WriteLine("ImarisSelector Settings File version 1");
                file.WriteLine(ImarisVersion);
                file.WriteLine(ImarisPath);
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
        /// Returns the setting file name with full path
        /// </summary>
        /// <returns>Setting file name with full path.</returns>
        private static String settingsFullFileName()
        {
            // Append the ImarisSelector path
            return SettingsDirectoryName() + @"settings.conf";
        }

        /// <summary>
        /// Create the ImarisSelector directory in commonAppData if it does not exist
        /// </summary>
        /// <param name="dir">Full directory name to be created</param>
        private static void CreateSettingsDirIfNeeded(String dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

    }
}
