using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Win32;
using ImarisSelectorLib;

/*
 * Class for registry management
 */
namespace ImarisSelector
{
    /// <summary>
    /// Class for managing registry keys associated to Imaris modules and their license state.
    /// </summary>
    public class RegistryManager
    {
        // Private backing stores
        private String m_UserSID;
        private String m_ImarisVersionString;
        private List<String> m_InstalledModuleList;
        private List<String> m_InstalledProductList;
        
        private ModuleCatalog m_ModuleCatalog;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ver">Imaris version in the form "Imaris x64 7.6" (no patch version).</param>
        public RegistryManager(String ver)
        {
            // Initialize the module and product catalogs
            this.m_ModuleCatalog = new ModuleCatalog();

            // Set user SID and Imaris version for use by other methods
            this.m_UserSID = WindowsIdentity.GetCurrent().User.ToString();
            this.m_ImarisVersionString = ver;

            // Get and store the licenses from the registry
            GetInstalledModuleList();
            GetInstalledProductList();
        }

        /// <summary>
        /// Returns true if license information could be found in the registry.
        /// </summary>
        /// <returns>True if license information could be found, false otherwise.</returns>
        public bool LicenseInformationFound()
        {
            return (this.m_InstalledModuleList.Count() > 0);
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
        /// Disable all licenses in the registry (with the exception of Imaris base).
        /// </summary>
        public void DisableProducts(List<String> productNames)
        {
            // For each of the products get the corresponding modules
            foreach (String product in productNames)
            {
                List<String> modules = this.m_ModuleCatalog.GetModulesForProduct(product);
                DisableModules(modules);
            }

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
        /// Enable all licenses in the registry.
        /// </summary>
        public void EnableModules(List<String> moduleNames)
        {
            // Iterate over licenses
            foreach (String moduleName in moduleNames)
            {
                // Check it the license is disabled
                if (!IsModuleEnabled(moduleName))
                {
                    // Enable the license
                    EnableModule(moduleName);
                }
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

        /// <summary>
        /// Get selected module names for simple view.
        /// </summary>
        /// <returns></returns>
        public List<String> GetProductNames()
        {
            return this.m_InstalledProductList;
        }

        /// <summary>
        /// Returns true if the product is enabled. It at least one of the
        /// modules in a product are enabled, the product is enabled as well.
        /// </summary>
        /// <param name="productName">Name of the module.</param>
        /// <returns>Product name.</returns>
        public bool IsProductEnabled(String productName)
        {
            // Get modules for the product
            List<String> moduleNames =
                this.m_ModuleCatalog.GetModulesForProduct(productName);

            // Check the module states
            foreach (String module in moduleNames)
            {
                if (IsModuleEnabled(module))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns name of the product to which the module belongs.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>Product name.</returns>
        public String GetProductForModule(String moduleName)
        {
            return this.m_ModuleCatalog.GetProductForModule(moduleName);
        }


        /// <summary>
        /// Returns the description for a given product.
        /// </summary>
        /// <returns>Product description.</returns>
        public String GetProductDescription(String productName)
        {
            return this.m_ModuleCatalog.GetProductDescription(productName);
        }

        /// <summary>
        /// Returns the description for a given module.
        /// </summary>
        /// <returns>Module description.</returns>
        public String GetModuleDescription(String moduleName)
        {
            return this.m_ModuleCatalog.GetModuleDescription(moduleName);
        }

        /// <summary>
        /// Returns the modules that belong to a given product.
        /// </summary>
        /// <returns>List of modules.</returns>
        public List<String> GetModulesForProduct(String productName)
        {
            return this.m_ModuleCatalog.GetModulesForProduct(productName);
        }

        //// PRIVATE METHODS 

        /// <summary>
        /// Gets and stores all licenses from the registry (with the exception of ImarisBase).
        /// </summary>
        private void GetInstalledModuleList()
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
        /// Stores a subset of all module names to be displayed in the simple view
        /// </summary>
        private void GetInstalledProductList()
        {            
            // Get the list of products
            this.m_InstalledProductList = this.m_ModuleCatalog.GetProductsForModules(this.m_InstalledModuleList);
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
