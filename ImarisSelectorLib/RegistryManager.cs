using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using Microsoft.Win32;

/*
 * Static class for registry management
 */
namespace ImarisSelector
{
    public class RegistryManager
    {
        // Private backing stores
        private String m_UserSID;
        private String m_ImarisVersionString;
        private List<String> m_CompleteModuleList;
        private List<String> m_SelectedModuleList;
        private Dictionary<String, String> m_ModuleDescriptions;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ver">Imaris version in the form "Imaris x64 7.6" (no patch version).</param>
        public RegistryManager(String ver)
        {
            // Fill in the database of module descriptions
            fillModuleDescriptions();

            // Set user SID and Imaris version for use by other methods
            this.m_UserSID = WindowsIdentity.GetCurrent().User.ToString();
            this.m_ImarisVersionString = ver;

            // Get and store the licenses from the registry
            BuildAndStoreAllModuleNameList();
            BuildAndStoreSimpleModuleNameList();
        }

        /// <summary>
        /// Returns true if license information could be found in the registry.
        /// </summary>
        /// <returns>True if license information could be found, false otherwise.</returns>
        public bool LicenseInformationFound()
        {
            return (this.m_CompleteModuleList.Count() > 0);
        }

        /// <summary>
        /// Return the state of the license for the selected module.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>"ture" if the license is enabled, "false" if it is disabled, 
        /// and "" if the license could not be found.</returns>
        public String GetLicenseState(String moduleName)
        {
            // Get the licenses key
            RegistryKey licensesKey = GetLicensesKey();
            if (licensesKey == null)
            {
                // TODO Handle this case properly!
                return "";
            }

            // Get the value for the module
            String value = (String)licensesKey.GetValue(moduleName);

            // Close key
            licensesKey.Close();

            if (value == null)
            {
                return "";
            }
            return value;
        }
        
        /// <summary>
        /// Toggles the state of the license for the selected module.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>True if the state could be set, false otherwise.</returns>
        public bool ToggleLicenseState(String moduleName)
        {
            // Get current state
            String state = GetLicenseState(moduleName);
            if (state.Equals(""))
            {
                return false;
            }

            if (state.Equals("true")) 
            {
                DisableModule(moduleName);
            } 
            else
            {
                EnableModule(moduleName);
            }

            return true;
        }

        /// <summary>
        /// Disable module of given name.
        /// </summary>
        /// <param name="moduleName">Name of the module to be disabled.</param>
        public void DisableModule(String moduleName)
        {
            SetLicenseState(moduleName, false);
        }

        /// <summary>
        /// Disable all licenses in the registry (with the exception of Imaris base).
        /// </summary>
        public void DisableModules(List<String> moduleNames)
        {
            // Iterate over modules
            foreach (String moduleName in moduleNames)
            {
                // Disable the module
                DisableModule(moduleName);

            }
        }

        /// <summary>
        /// Disable all licenses in the registry (with the exception of Imaris base).
        /// </summary>
        public void DisableAllModules()
        {
            // Get all licenses
            List<String> allModuleNames = GetAllModuleNames();

            // Diable all modules
            DisableModules(allModuleNames);
        }

        /// <summary>
        /// Enable module of given name.
        /// </summary>
        /// <param name="moduleName">Name of the module to be enabled.</param>
        public void EnableModule(String moduleName)
        {
            SetLicenseState(moduleName, true);
        }

        /// <summary>
        /// Enable all licenses in the registry
        /// </summary>
        public void EnableAllModules()
        {
            // Get all licenses
            List<String> allModuleNames = GetAllModuleNames();

            // Enable all
            EnableModules(allModuleNames);
        
        }

        /// <summary>
        /// Enable all licenses in the registry
        /// </summary>
        public void EnableModules(List<String> moduleNames)
        {
            // Iterate over licenses
            foreach (String moduleName in moduleNames)
            {
                // Check it the license is disabled
                if (GetLicenseState(moduleName).Equals("false"))
                {
                    // Enable the license
                    EnableModule(moduleName);
                }
            }

        }
        
        /// <summary>
        /// Get all module names (with the exception of ImarisBase)
        /// </summary>
        /// <returns>A List of all module names </returns>
        public List<String> GetAllModuleNames()
        {
            return this.m_CompleteModuleList;
        }

        /// <summary>
        /// Get selected module names for simple view
        /// </summary>
        /// <returns></returns>
        public List<String> GetSelectedModuleNames()
        {
            return this.m_SelectedModuleList;
        }

        /// <summary>
        /// Returns the description for a given module.
        /// </summary>
        /// <returns>Module description.</returns>
        public String GetModuleDescription(String moduleName)
        {
            String descr;
            if (m_ModuleDescriptions.TryGetValue(moduleName, out descr))
            {
                return descr;
            }
            else
            {
                return "Unknown module.";
            }
        }


        //// PRIVATE METHODS 

        /// <summary>
        /// Gets and stores all licenses from the registry (with the exception of ImarisBase).
        /// </summary>
        private void BuildAndStoreAllModuleNameList()
        {
            // Initialize List
            this.m_CompleteModuleList = new List<String>();

            // Get the licenses registry key (read-only)
            RegistryKey licensesKey = GetLicensesKey(false);
            if (licensesKey == null)
            {
                return;
            }

            // Iterate over the modules and store them with their state
            String[] moduleNames = licensesKey.GetValueNames();
            foreach (String module in moduleNames)
            {
                // Get the license state
                String value = GetLicenseState(module);
                if (!module.Equals("ImarisBase") && !value.Equals(""))
                {
                    this.m_CompleteModuleList.Add(module);
                }
                else
                {
                    System.Console.WriteLine("Could not retrieve state for module" + module + ".");
                }

            }
        }

        /// <summary>
        /// Stores a subset of all module names to be displayed in the simple view
        /// </summary>
        private void BuildAndStoreSimpleModuleNameList()
        {
            // Initialize List
            this.m_SelectedModuleList = new List<String>();

            // Iterate over all known licenses and store a subset for the simple view
            foreach (String moduleName in this.m_CompleteModuleList)
            {
                switch (moduleName)
                {
                    case "ImarisAnalyzer":
                    case "ImarisColoc":
                    case "ImarisFilament":
                    case "ImarisIPSS":
                    case "ImarisInPress":
                    case "ImarisManualSurface":
                    case "ImarisStatistics":
                    case "ImarisTrack":
                    case "ImarisVantage":
                        this.m_SelectedModuleList.Add(moduleName);
                        break;
                    default:
                        break;
                }


            }
        }

        /// <summary>
        /// Sets the license state for a given module.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="enabled">True to enable, false to disable.</param>
        /// <returns>True if the state could be changed, false otherwise.</returns>
        private bool SetLicenseState(String moduleName, bool enabled)
        {

            // Get the licenses key in read-write mode
            RegistryKey licensesKey = GetLicensesKey(true);
            if (licensesKey == null)
            {
                return false;
            }

            // Set the value for the module
            if (enabled == true)
            {
                licensesKey.SetValue(moduleName, "true", RegistryValueKind.String);
            }
            else
            {
                licensesKey.SetValue(moduleName, "false", RegistryValueKind.String);
            }
            return true;
        }

        /// <summary>
        /// Return the Licenses registry key or null if not found.
        /// </summary>
        /// <param name="writable">True for a read-write registry key, false for a read-only one.</param>
        /// <returns>A read-only or read-write registry key.</returns>
        private RegistryKey GetLicensesKey(bool writable = false)
        {
            // Get the HKEY_USERS tree
            RegistryKey reg = Registry.Users;

            // Get the Software key
            String licensePath = GetImarisLicenseStatePath();
            RegistryKey licenseKey = reg.OpenSubKey(licensePath, writable);

            return licenseKey;
        }

        /// <summary>
        /// Return full path (under HKEY_Users) to the Licenses key
        /// </summary>
        /// <returns>Full path of the Licenses key</returns>
        private String GetImarisLicenseStatePath()
        {
            return m_UserSID + "\\Software\\Bitplane\\" + 
                m_ImarisVersionString + "\\Licenses\\";
        }

        /// <summary>
        /// Fills the dictionary of module name - descriptions to be used in the UI.
        /// </summary>
        private void fillModuleDescriptions()
        {
            // Initialize the dictionary
            this.m_ModuleDescriptions = new Dictionary<String, String>();

            // Add all known modules with a short description
            m_ModuleDescriptions.Add(
                "ImarisAnalyzer", 
                "");
            m_ModuleDescriptions.Add(
                "ImarisBase", 
                "Imaris");
            m_ModuleDescriptions.Add(
                "ImarisCellsViewer", 
                "Imaris Cell");
            m_ModuleDescriptions.Add(
                "ImarisColoc",
                "Imaris Coloc");
            m_ModuleDescriptions.Add(
                "ImarisFilament",
                "Filament Tracer");
            m_ModuleDescriptions.Add(
                "ImarisInPress",
                "Imaris InPress (part of Imaris Vantage)");
            m_ModuleDescriptions.Add(
                "ImarisIPSS",
                "Imaris XT");
            m_ModuleDescriptions.Add(
                "ImarisManualSurface",
                "Imaris Manual Surface (part of Imaris Measurement Pro)");
            m_ModuleDescriptions.Add(
                "ImarisMeasurementPoint",
                "Imaris Measurement Point (part of Imaris Measurement Pro)");
            m_ModuleDescriptions.Add(
                "ImarisReaderBiorad",
                "Bio-Rad MRC (series) file reader (*.pic)");
            m_ModuleDescriptions.Add(
                "ImarisReaderDeltaVision",
                "Applied Precision DeltaVision file reader (*.r3d, *.dv)");
            m_ModuleDescriptions.Add(
                "ImarisReaderGatan",
                "Gatan DigitalMicrograph (series) file reader (*.dm3)");
            m_ModuleDescriptions.Add(
                "ImarisReaderHamamatsu",
                "Hamamatsu Compix SimplePCI file reader (*.cxd)");
            m_ModuleDescriptions.Add(
                "ImarisReaderIII",
                "Imaris version 3.0 reader (*.ims)");
            m_ModuleDescriptions.Add(
                "ImarisReaderIMOD",
                "IMOD MRC file reader (*.mrc, *.st, *.rec)");
            m_ModuleDescriptions.Add(
                "ImarisReaderIPLab",
                "");
            m_ModuleDescriptions.Add(
                "ImarisReaderLeica",
                "Leica file reader (*.lif, *.tif, *.tiff, *.inf, *.info, *.lei, *.raw)");
            m_ModuleDescriptions.Add(
                "ImarisReaderMicroManager",
                "Micro-Manager Image5D file reader (*.tif, *.tiff, *.txt)");
            m_ModuleDescriptions.Add(
                "ImarisReaderNikon",
                "Nikon Image Cytometry Standard and ND2 file reader (*.ics, *.ids, *.nd2)");
            m_ModuleDescriptions.Add(
                "ImarisReaderOlympus",
                "Olympus CellR, Fluoview OIB, OIF, TIFF file reader (*.tif, *.tiff, *.oib, *.oif)");
            m_ModuleDescriptions.Add(
                "ImarisReaderOME",
                "Open Microscopy Environment TIFF and XML file reader (*.tif, *.tiff, *.ome)");
            m_ModuleDescriptions.Add(
                "ImarisReaderPerkinElmerInc",
                "Perkin Elmer Improvision Openlab LIFF (series), RAW and UltraView file reader (*.liff, *.raw, *.tim, *.zpo)");
            m_ModuleDescriptions.Add(
                "ImarisReaderPrairie",
                "Prairie Technologies View file reader (*.xml, *.cfg, *.tif, *.tiff)");
            m_ModuleDescriptions.Add(
                "ImarisReaderTILL",
                "TILL Photonics TILLvisION file reader (*.rbinf)");
            m_ModuleDescriptions.Add(
                "ImarisReaderUniversalImaging",
                "");
            m_ModuleDescriptions.Add(
                "ImarisReaderZeiss",
                "Zeiss AxioVision, CZI, LSM{3|4|5|7}10 file reader (*.zvi, *.czi, *.lsm, *.tif, *.tiff)");
            m_ModuleDescriptions.Add(
                "ImarisSceneViewer",
                "");
            m_ModuleDescriptions.Add(
                "ImarisStatistics",
                "Imaris Statistics (part of Imaris Measurement Pro)");
            m_ModuleDescriptions.Add(
                "ImarisSurpass",
                "");
            m_ModuleDescriptions.Add(
                "ImarisSurpassBase",
                "");
            m_ModuleDescriptions.Add(
                "ImarisTime",
                "Part of Imaris base");
            m_ModuleDescriptions.Add(
                "ImarisTopography",
                "Part of Imaris base");
            m_ModuleDescriptions.Add(
                "ImarisTrack",
                "Imaris Track");
            m_ModuleDescriptions.Add(
                "ImarisVantage",
                "Imaris Vantage");

        }

    }
}
