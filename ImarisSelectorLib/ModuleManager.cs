using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ImarisSelectorLib
{
    /// <summary>
    /// A class to hold information about modules and products. 
    /// </summary>
    class Module
    {
        /// <summary>
        /// Module ID corresponding to its name in the registry.
        /// </summary>
        public String ID { get; set; }

        /// <summary>
        /// More human-readable name.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Name of the product to which the module belongs.
        /// </summary>
        public String Product { get; set; }

        /// <summary>
        /// Module description.
        /// </summary>
        public String Description { get; set; }
    }

    /// <summary>
    /// A class to manage a catalog of modules and products. 
    /// </summary>
    public class ModuleManager
    {
        private IEnumerable<Module> m_ModuleCatalog;
        private Dictionary<String, String> m_ProductCatalog;

        private Settings m_Settings;

        private RegistryManager m_RegistryManager;

        /// <summary>
        /// Constructor.
        /// <param name="settings">Application settings (read from disk).</param>
        /// </summary>
        public ModuleManager(Settings settings)
        {
            // Store the settings
            this.m_Settings = settings;

            // Build the catalog
            Build();

            // Initialize the Registry Manager
            this.m_RegistryManager = new RegistryManager(this.m_Settings.ImarisVersion);

            // Make sure the Imaris product is enabled
            EnableProducts(new List<String> { "Imaris" });
        }

        /// <summary>
        /// Returns true if license information could be found in the registry.
        /// </summary>
        /// <returns>True if license information could be found, false otherwise.</returns>
        public bool LicenseInformationFound()
        {
            return (this.m_RegistryManager.GetInstalledModuleNames().Count > 0);
        }

        /// <summary>
        /// Returns a dictionary of (name, description) for all known product names.
        /// </summary>
        /// <returns>Name and description for all known Imaris products.</returns>
        public Dictionary<String, String> GetProductCatalog()
        {
            return this.m_ProductCatalog;
        }

        /// <summary>
        /// Return a list of names for all products that are installed on the machine.
        /// </summary>
        /// <returns>List of installed product names.</returns>
        public List<String> GetInstalledProductList()
        {
            // Return the list of installed products
            return GetProductsForModules(this.m_RegistryManager.GetInstalledModuleNames());
        }

        /// <summary>
        /// Return a list of names for all products that are installed on the machine
        /// and filtered by the admin selection in ImarisSelectorAdmin.
        /// </summary>
        /// <returns>List of installed and filtered product names.</returns>
        public List<String> GetInstalledAndFilteredProductList()
        {            
            // Get the list of installed products
            List<String> installedProducts =
                GetProductsForModules(this.m_RegistryManager.GetInstalledModuleNames());

            // Now filter by the admin selections
            var results =
                from productName in installedProducts.AsEnumerable()
                join productWithState in this.m_Settings.ProductsWithEnabledState
                on productName equals productWithState.Key
                where productWithState.Value == true && !productName.Equals("Imaris")
                orderby productName ascending
                select new { productName };

            // Now get a list
            var names = results.Distinct().ToList();

            // Make sure to cast properly
            List<String> installedAndFilteredProducts = new List<string>();
            foreach (var name in names)
            {
                installedAndFilteredProducts.Add(name.productName.ToString());
            }

            return installedAndFilteredProducts;
        }

        /// <summary>
        /// Get the description for product with specific name.
        /// </summary>
        /// <param name="productName">Name of the product.</param>
        /// <returns>Product description.</returns>
        public String GetProductDescription(String productName) 
        {
            String descr;
            if (m_ProductCatalog.TryGetValue(productName, out descr))
            {
                return descr;
            }
            else
            {
                return "Unknown module.";
            }

        }

        /// <summary>
        /// Get the human-readable name for module with specific name.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>Module human-readable name.</returns>
        public String GetModuleName(String moduleName)
        {
            // Query the module catalog for the module name and get its description.
            var results =
                from knownModule in this.m_ModuleCatalog
                where knownModule.ID.Equals(moduleName)
                select new { knownModule.Name };

            // Return
            if (results.Count() == 0)
            {
                return "Unknown module.";
            }
            else
            {
                return results.ElementAt(0).Name.ToString();
            }
        }

        /// <summary>
        /// Get the description for module with specific name.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>Module description.</returns>
        public String GetModuleDescription(String moduleName)
        {
            // Query the module catalog for the module name and get its description.
            var results =
                from knownModule in this.m_ModuleCatalog
                where knownModule.ID.Equals(moduleName)
                select new { knownModule.Description };

            // Return
            if (results.Count() == 0)
            {
                return "Unknown module.";
            }
            else
            {
                return results.ElementAt(0).Description.ToString();
            }
        }

        /// <summary>
        /// Get the product to which the module belongs.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>Product nam.</returns>
        public String GetProductForModule(String moduleName)
        {
            // Query the module catalog for the module name and get its description.
            var results =
                from knownModule in this.m_ModuleCatalog
                where knownModule.ID.Equals(moduleName)
                select new { knownModule.Product };

            // Return
            if (results.Count() == 0)
            {
                return "Unknown module.";
            }
            else
            {
                return results.ElementAt(0).Product.ToString();
            }
        }

        /// <summary>
        /// Query the module catalog for input module names and return the 
        /// products associated to them.
        /// </summary>
        /// <param name="moduleNames">List of module names</param>
        /// <returns>List of product names</returns>
        public List<String> GetProductsForModules(List<String> moduleNames)
        {
            // Instantiate a list of product names
            List<String> productNames = new List<String>();

            // Query the module catalog for all modules in the input parameter
            // and return the (unique) product names for the modules found.
            var results =
                from moduleName in moduleNames.AsEnumerable()
                join knownModule in this.m_ModuleCatalog
                on moduleName equals knownModule.ID
                orderby knownModule.Product ascending
                select new { knownModule.Product };

            var names = results.Distinct().ToList();

            // Make sure to cast properly
            foreach (var name in names)
            {
                productNames.Add(name.Product.ToString());
            }

            return productNames;
        }

        /// <summary>
        /// Query the module catalog for input product name and return the 
        /// modules associated to it
        /// </summary>
        /// <param name="productName">Product name</param>
        /// <returns>List of module names</returns>
        public List<String> GetModulesForProduct(String productName)
        {
            // Instantiate a list of product names
            List<String> moduleNames = new List<String>();

            // Query the module catalog for the product name and get all modules
            // and return the (unique) product names for the modules found.
            var results =
                from knownModule in this.m_ModuleCatalog
                where knownModule.Product.Equals(productName)
                select new { knownModule.ID };

            var names = results.Distinct().ToList();

            // Make sure to cast properly
            foreach (var name in names)
            {
                moduleNames.Add(name.ID.ToString());
            }

            return moduleNames;
        }

        /// <summary>
        /// Returns a list of all modules associated to a list of product names.
        /// </summary>
        /// <param name="productNames">List of product names.</param>
        /// <returns>List of module names</returns>
        public List<String> GetModulesForProducts(List<String> productNames)
        {
            List<String> moduleNames = new List<String>();

            foreach (String productName in productNames)
            {
                moduleNames.AddRange(GetModulesForProduct(productName));
            }

            // Return the complete list
            return moduleNames;
        }

        /// <summary>
        /// Enable module of given name.
        /// </summary>
        /// <param name="moduleName">Name of the module to be enabled.</param>
        public void EnableModule(String moduleName)
        {
            this.m_RegistryManager.EnableModule(moduleName);
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
        /// Enable all licenses in the registry.
        /// // TODO: Decide what to do with ImarisBase.
        /// </summary>
        public void EnableAllModules()
        {
            // Get all licenses
            List<String> allModuleNames = GetAllModuleNames();

            // Diable all modules
            EnableModules(allModuleNames);
        }

        /// <summary>
        /// Disable module of given name.
        /// </summary>
        /// <param name="moduleName">Name of the module to be disabled.</param>
        public void DisableModule(String moduleName)
        {
            this.m_RegistryManager.DisableModule(moduleName);
        }

        /// <summary>
        /// Disable all licenses in the registry.
        /// TODO: Decide what to do with ImarisBase.
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
        /// Disable all licenses in the registry.
        /// // TODO: Decide what to do with ImarisBase.
        /// </summary>
        public void DisableAllModules()
        {
            // Get all licenses
            List<String> allModuleNames = GetAllModuleNames();

            // Diable all modules
            DisableModules(allModuleNames);
        }

        /// <summary>
        /// Enable modules belonging to specific products.
        /// </summary>
        public void EnableProducts(List<String> productNames)
        {
            // For each of the products get the corresponding modules
            foreach (String product in productNames)
            {
                List<String> modules = GetModulesForProduct(product);
                EnableModules(modules);
            }
        }

        /// <summary>
        /// Disable modules belonging to specific products.
        /// </summary>
        public void DisableProducts(List<String> productNames)
        {
            // For each of the products get the corresponding modules
            foreach (String product in productNames)
            {
                List<String> modules = GetModulesForProduct(product);
                DisableModules(modules);
            }
        }

        /// <summary>
        /// Get all filtered module names.
        /// </summary>
        /// <returns>A List of filtered module names </returns>
        public List<String> GetFilteredModuleNames()
        {
            return GetModulesForProducts(GetInstalledAndFilteredProductList());
        }

        /// <summary>
        /// Get all module names.
        /// </summary>
        /// <returns>A List of all module names </returns>
        public List<String> GetAllModuleNames()
        {
            return this.m_RegistryManager.GetInstalledModuleNames();
        }

        /// <summary>
        /// Get list of products.
        /// </summary>
        /// <returns>List of product names.</returns>
        public List<String> GetAllProductNames()
        {
            return new List<string>(this.m_ProductCatalog.Keys);
        }

        /// <summary>
        /// Return the state of the license for the selected module.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>"true" if the license is enabled, "false" if it is disabled, 
        /// and "" if the license could not be found.</returns>
        public bool IsModuleEnabled(String moduleName)
        {
            return this.m_RegistryManager.IsModuleEnabled(moduleName);
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
            List<String> moduleNames = GetModulesForProduct(productName);

            // Check the module states
            foreach (String module in moduleNames)
            {
                if (this.m_RegistryManager.IsModuleEnabled(module))
                {
                    return true;
                }
            }
            return false;
        }

        // PRIVATE METHODS

        /// <summary>
        /// Build the Module and Product catalogue.
        /// </summary>
        private void Build()
        {
            // This is a catalogue of all known Imaris modules
            this.m_ModuleCatalog = new List<Module> 
            {
                /* ImarisAnalyzer is obsolete
                new Module {
                    ID = "ImarisAnalyzer",
                    Name = "Imaris Analyzer",
                    Product = "Obsolete", 
                    Description = "Unknown module" },
                */
                new Module {
                    ID = "ImarisBase",
                    Name = "Imaris Base",
                    Product = "Imaris",
                    Description = "" },
                new Module {
                    ID = "ImarisSurpass",
                    Name = "Imaris Surpass",
                    Product = "Imaris",
                    Description = ""},
                new Module {
                    ID = "ImarisSurpassBase",
                    Name = "Imaris Surpass Base",
                    Product = "Imaris",
                    Description = ""},
                new Module {
                    ID = "ImarisTime",
                    Name = "Imaris Time",
                    Product = "Imaris",
                    Description = ""},
                new Module {
                    ID = "ImarisTopography",
                    Name = "ImarisTopography",
                    Product = "Imaris",
                    Description = ""},
                new Module {
                    ID = "ImarisCellsViewer",
                    Name = "Imaris Cell",
                    Product = "Imaris Cell",
                    Description = ""},
                new Module {
                    ID = "ImarisColoc",
                    Name = "Imaris Coloc",
                    Product = "Imaris Coloc",
                    Description = ""},
                new Module {
                    ID = "ImarisFilament",
                    Name = "Filament Tracer",
                    Product = "Filament Tracer",
                    Description = ""},
                new Module {
                    ID = "ImarisInPress",
                    Name = "Imaris InPress",
                    Product = "Imaris Vantage", 
                    Description = ""},
                new Module {
                    ID = "ImarisIPSS",
                    Name = "Imaris XT",
                    Product = "Imaris XT",
                    Description = ""},
                new Module {
                    ID = "ImarisManualSurface",
                    Name = "Imaris Manual Surface", 
                    Product = "Imaris Measurement Pro", 
                    Description = ""},
                new Module {
                    ID = "ImarisMeasurementPoint",
                    Name = "Imaris Measurement Point",
                    Product = "Imaris Measurement Pro",
                    Description = ""},
                new Module {
                    ID = "ImarisReaderBiorad",
                    Name = "Bio-Rad MRC (series)", 
                    Product = "File Reader", 
                    Description = "Supported extensions: *.pic"},
                new Module {
                    ID = "ImarisReaderDeltaVision",
                    Name = "Applied Precision DeltaVision", 
                    Product = "File Reader", 
                    Description = "Supported extensions: *.r3d, *.dv"},
                new Module {
                    ID = "ImarisReaderGatan",
                    Name = "Gatan DigitalMicrograph (series)", 
                    Product = "File Reader",
                    Description = "Supported extensions: *.dm3"},
                new Module {
                    ID = "ImarisReaderHamamatsu",
                    Name = "Hamamatsu Compix SimplePCI",
                    Product = "File Reader",
                    Description = "Supported extensions: *.cxd"},
                new Module {
                    ID = "ImarisReaderIII",
                    Name = "Imaris version 3.0 reader",
                    Product = "File Reader",
                    Description = "Supported extensions: *.ims"},
                new Module {
                    ID = "ImarisReaderIMOD",
                    Name = "IMOD MRC",
                    Product = "File Reader", 
                    Description = "Supported extensions: *.mrc, *.st, *.rec"},
                new Module {
                    ID = "ImarisReaderIPLab",
                    Name = "???",
                    Product = "File Reader",
                    Description = "Supported extesions: *.???"},
                new Module {
                    ID = "ImarisReaderLeica",
                    Name = "Leica",
                    Product = "File Reader",
                    Description = "Supported extensions: *.lif, *.tif, *.tiff, *.inf, *.info, *.lei, *.raw"},
                new Module {
                    ID = "ImarisReaderMicroManager",
                    Name = "Micro-Manager Image5D",
                    Product = "File Reader",
                    Description = "Supported extensions: *.tif, *.tiff, *.txt"},
                new Module {
                    ID = "ImarisReaderNikon",
                    Name = "Nikon Image Cytometry Standard and ND2",
                    Product = "File Reader", 
                    Description = "Supported extensions: *.ics, *.ids, *.nd2"},
                new Module {
                    ID = "ImarisReaderOlympus",
                    Name = "Olympus CellR, Fluoview OIB, OIF, TIFF",
                    Product = "File Reader",
                    Description = "Supported extensions: *.tif, *.tiff, *.oib, *.oif"},
                new Module {
                    ID = "ImarisReaderOME",
                    Name = "Open Microscopy Environment TIFF and XML",
                    Product = "File Reader",
                    Description = "Supported extensions: *.tif, *.tiff, *.ome"},
                new Module {
                    ID = "ImarisReaderPerkinElmerInc",
                    Name = "Perkin Elmer Improvision Openlab LIFF (series), RAW and UltraView",
                    Product = "File Reader",
                    Description = "Supported extensions: *.liff, *.raw, *.tim, *.zpo"},
                new Module {
                    ID = "ImarisReaderPrairie",
                    Name = "Prairie Technologies View",
                    Product = "File Reader",
                    Description = "Supported extensions: *.xml, *.cfg, *.tif, *.tiff"},
                new Module {
                    ID = "ImarisReaderTILL",
                    Name = "TILL Photonics TILLvisION",
                    Product = "File Reader",
                    Description = "Supported extensions: *.rbinf"},
                new Module {
                    ID = "ImarisReaderUniversalImaging",
                    Name = "???",
                    Product = "File Reader",
                    Description = "Supported extensions: *.???"},
                new Module {
                    ID = "ImarisReaderZeiss",
                    Name = "Zeiss AxioVision, CZI, LSM 310, 410, 510, 710",
                    Product = "File Reader",
                    Description = "Supported extensions: *.zvi, *.czi, *.lsm, *.tif, *.tiff"},
                new Module {
                    ID = "ImarisSceneViewer",
                    Name = "Imaris Scene Viewer",
                    Product = "Imaris Scene Viewer",
                    Description = ""},
                new Module {
                    ID = "ImarisStatistics",
                    Name = "Imaris Statistics",
                    Product = "Imaris Measurement Pro",
                    Description = ""},
                new Module {
                    ID = "ImarisTrack",
                    Name = "Imaris Track",
                    Product = "Imaris Track",
                    Description = ""},
                new Module {
                    ID = "ImarisVantage",
                    Name = "Imaris Vantage",
                    Product = "Imaris Vantage",
                    Description = ""}
            };

            // This is a catalog of the know Imaris products (stored in a Dictionary)
            this.m_ProductCatalog = new Dictionary<String, String>();
            this.m_ProductCatalog.Add(
                "Imaris",
                "3D and 4D Real-Time Interactive Image Visualization."); 
            this.m_ProductCatalog.Add(
                "Imaris Measurement Pro",
                "The Analysis and Quantification Engine.");
            this.m_ProductCatalog.Add(
                "Imaris Track",
                "Discover the Meaning of Motion.");
            this.m_ProductCatalog.Add(
                "Imaris Coloc",
                "Isolate, Visualize and Quantify Colocalized Regions.");
            this.m_ProductCatalog.Add(
                "Imaris Cell",
                "Analysis and Visualization of intra/inter Cellular Relationships.");
            this.m_ProductCatalog.Add(
                "Filament Tracer",
                "Analysis and Visualization of Filamentous Structures.");
            this.m_ProductCatalog.Add(
                "Imaris Vantage",
                "Data Plotting (2D to 5D), Interpretation and Mining.");
            this.m_ProductCatalog.Add(
                "Imaris XT",
                "Access to the Open Source Community and XTensions. Freedom to customize.");
            this.m_ProductCatalog.Add(
                "Imaris Scene Viewer",
                "Viewer for Imaris Scenes.");
            this.m_ProductCatalog.Add(
                "Imaris Batch",
                "Automated Image Processing.");
            this.m_ProductCatalog.Add(
                "AutoAligner",
                "Image Alignment Made Easy.");
            this.m_ProductCatalog.Add(
                "AutoQuant",
                "Advanced and Fully Featured Image Deconvolution.");
            this.m_ProductCatalog.Add(
                "File Reader",
                "Proprietary File Readers.");

        }

    }
}
