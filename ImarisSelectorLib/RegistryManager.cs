using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Win32;

/*
 * Class for registry management
 */
namespace ImarisSelectorLib
{
    /// <summary>
    /// Class for managing registry keys associated to Imaris modules and their license state.
    /// All module and product handling from the client side should be performed through the public
    /// ModuleManager class (which in turns uses RegistryManager).
    /// </summary>
    public class RegistryManager
    {
        // Private backing stores
        private String m_UserSID;
        private String m_ImarisVersionString;
        private List<String> m_InstalledModuleList;
        
        //private ModuleManager m_ModuleCatalog;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ver">Imaris version in the form "Imaris x64 7.6" (no patch version).</param>
        public RegistryManager(String ver)
        {
            // Initialize the module and product catalogs
            //this.m_ModuleCatalog = new ModuleManager();

            // Set user SID and Imaris version for use by other methods
            this.m_UserSID = WindowsIdentity.GetCurrent().User.ToString();
            this.m_ImarisVersionString = ver;

            // Get and store the licenses from the registry
            ScanForInstalledModules();
        }

        /// <summary>
        /// Return the state of the license for the selected module.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>"true" if the license is enabled, "false" if it is disabled, 
        /// and "" if the license could not be found.</returns>
        public bool IsModuleEnabled(String moduleName)
        {
            // Get the licenses key
            RegistryKey licensesKey = GetLicensesKey();
            if (licensesKey == null)
            {
                // TODO Handle this case properly!
                return false;
            }

            // Get the value for the module
            String value = (String)licensesKey.GetValue(moduleName);

            // Close key
            licensesKey.Close();

            if (value == null)
            {
                return false;
            }
            return value.Equals("true");
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
        /// Enable all licenses in the registry.
        /// </summary>
        public void EnableModules(List<String> moduleNames)
        {
            // Iterate over licenses
            foreach (String moduleName in moduleNames)
            {
                // Enable the license
                EnableModule(moduleName);
            }
        }

        /// <summary>
        /// Get all module names (with the exception of ImarisBase and ImarisAnalyzer).
        /// </summary>
        /// <returns>A List of all module names </returns>
        public List<String> GetAllModuleNames()
        {
            return this.m_InstalledModuleList;
        }

        //// PRIVATE METHODS 

        /// <summary>
        /// Gets and stores all licenses from the registry (with the exception of ImarisBase).
        /// </summary>
        private void ScanForInstalledModules()
        {
            // Initialize List
            this.m_InstalledModuleList = new List<String>();

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
                // We hide ImarisAnalyzer, which appears to be an old (inactive) module
                // TODO: Decide what to do with ImarisBase
                if (!module.Equals("ImarisAnalyzer"))
                {
                    this.m_InstalledModuleList.Add(module);
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
    }
}
