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
        /// MOre human-readable name.
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
    class ModuleCatalog
    {
        private IEnumerable<Module> m_ModuleCatalog;
        private Dictionary<String, String> m_ProductCatalog;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ModuleCatalog()
        {
            // Build the catalog
            Build();
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
        /// products associated to them
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
        /// Build the Module and Product catalogue.
        /// </summary>
        private void Build()
        {
            // This is a catalogue of all known Imaris modules
            this.m_ModuleCatalog = new List<Module> 
            {
                new Module {
                    ID = "ImarisAnalyzer",
                    Name = "Imaris Analyzer",
                    Product = "Obsolete", 
                    Description = "Unknown module" },
                new Module {
                    ID = "ImarisBase",
                    Name = "Imaris",
                    Product = "Imaris",
                    Description = "Imaris" },
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
                    Description = "Part of Imaris Vantage"},
                new Module {
                    ID = "ImarisIPSS",
                    Name = "Imaris XT",
                    Product = "Imaris XT",
                    Description = ""},
                new Module {
                    ID = "ImarisManualSurface",
                    Name = "Imaris Manual Surface", 
                    Product = "Imaris Measurement Pro", 
                    Description = "Part of Imaris Measurement Pro"},
                new Module {
                    ID = "ImarisMeasurementPoint",
                    Name = "Imaris Measurement Point",
                    Product = "Imaris Measurement Pro",
                    Description = "Part of Imaris Measurement Pro"},
                new Module {
                    ID = "ImarisReaderBiorad",
                    Name = "Bio-Rad MRC (series) file reader", 
                    Product = "File Reader", 
                    Description = "Supported extensions: *.pic"},
                new Module {
                    ID = "ImarisReaderDeltaVision",
                    Name = "Applied Precision DeltaVision file reader", 
                    Product = "File Reader", 
                    Description = "Supported extensions: *.r3d, *.dv"},
                new Module {
                    ID = "ImarisReaderGatan",
                    Name = "Gatan DigitalMicrograph (series) file reader", 
                    Product = "File Reader",
                    Description = "Supported extensions: *.dm3"},
                new Module {
                    ID = "ImarisReaderHamamatsu",
                    Name = "Hamamatsu Compix SimplePCI file reader (*.cxd)",
                    Product = "File Reader",
                    Description = ""},
                new Module {
                    ID = "ImarisReaderIII",
                    Name = "Imaris version 3.0 reader (*.ims)",
                    Product = "File Reader",
                    Description = ""},
                new Module {
                    ID = "ImarisReaderIMOD",
                    Name = "IMOD MRC file reader",
                    Product = "File Reader", 
                    Description = "Supported extensions: *.mrc, *.st, *.rec"},
                new Module {
                    ID = "ImarisReaderIPLab",
                    Name = "???",
                    Product = "File Reader",
                    Description = ""},
                new Module {
                    ID = "ImarisReaderLeica",
                    Name = "Leica file reader",
                    Product = "File Reader",
                    Description = "Supported extensions: *.lif, *.tif, *.tiff, *.inf, *.info, *.lei, *.raw"},
                new Module {
                    ID = "ImarisReaderMicroManager",
                    Name = "Micro-Manager Image5D file reader",
                    Product = "File Reader",
                    Description = "Supported extensions: *.tif, *.tiff, *.txt"},
                new Module {
                    ID = "ImarisReaderNikon",
                    Name = "Nikon Image Cytometry Standard and ND2 file reader",
                    Product = "File Reader", 
                    Description = "Supported extensions: *.ics, *.ids, *.nd2"},
                new Module {
                    ID = "ImarisReaderOlympus",
                    Name = "Olympus CellR, Fluoview OIB, OIF, TIFF file reader",
                    Product = "File Reader",
                    Description = "Supported extensions: *.tif, *.tiff, *.oib, *.oif"},
                new Module {
                    ID = "ImarisReaderOME",
                    Name = "Open Microscopy Environment TIFF and XML file reader",
                    Product = "File Reader",
                    Description = "Supported extensions: *.tif, *.tiff, *.ome"},
                new Module {
                    ID = "ImarisReaderPerkinElmerInc",
                    Name = "Perkin Elmer Improvision Openlab LIFF (series), RAW and UltraView file reader",
                    Product = "File Reader",
                    Description = "Supported extensions: *.liff, *.raw, *.tim, *.zpo"},
                new Module {
                    ID = "ImarisReaderPrairie",
                    Name = "Prairie Technologies View file reader",
                    Product = "File Reader",
                    Description = "Supported extensions: *.xml, *.cfg, *.tif, *.tiff"},
                new Module {
                    ID = "ImarisReaderTILL",
                    Name = "TILL Photonics TILLvisION file reader",
                    Product = "File Reader",
                    Description = "Supported extensions: *.rbinf"},
                new Module {
                    ID = "ImarisReaderUniversalImaging",
                    Name = "",
                    Product = "File Reader",
                    Description = ""},
                new Module {
                    ID = "ImarisReaderZeiss",
                    Name = "Zeiss AxioVision, CZI, LSM 310, 410, 510, 710 file reader",
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
                    Description = "Part of Imaris Measurement Pro"},
                new Module {
                    ID = "ImarisSurpass",
                    Name = "Imaris Surpass",
                    Product = "Imaris",
                    Description = "Part of Imaris base"},
                new Module {
                    ID = "ImarisSurpassBase",
                    Name = "Imaris Surpass Base",
                    Product = "Imaris",
                    Description = "Part of Imaris base"},
                new Module {
                    ID = "ImarisTime",
                    Name = "Imaris Time",
                    Product = "Imaris",
                    Description = "Part of Imaris base"},
                new Module {
                    ID = "ImarisTopography",
                    Name = "ImarisTopography",
                    Product = "Imaris",
                    Description = "Part of Imaris base"},
                new Module {
                    ID = "ImarisTrack",
                    Name = "Imaris Track",
                    Product = "Imaris Track",
                    Description = ""},
                new Module {
                    ID = "ImarisVantage",
                    Name = "Imaris Vantage",
                    Product = "Imaris Vantage",
                    Description = "Part of Imaris Vantage"}
            };

            // This is a catalog of the know Imaris products (stored in a Dictionary)
            this.m_ProductCatalog = new Dictionary<String, String>();
            this.m_ProductCatalog.Add(
                "Imaris",
                "3D and 4D Real-Time Interactive Image Visualization");
            this.m_ProductCatalog.Add(
                "Imaris Measurement Pro",
                "The Analysis and Quantification Engine");
            this.m_ProductCatalog.Add(
                "Imaris Track",
                "Discover the Meaning of Motion");
            this.m_ProductCatalog.Add(
                "Imaris Coloc",
                "Isolate, Visualize and Quantify Colocalized Regions");
            this.m_ProductCatalog.Add(
                "Imaris Cell",
                "Analysis and Visualization of intra/inter Cellular Relationships");
            this.m_ProductCatalog.Add(
                "Filament Tracer",
                "Analysis and Visualization of Filamentous Structures");
            this.m_ProductCatalog.Add(
                "Imaris Vantage",
                "Data Plotting (2D to 5D), Interpretation and Mining");
            this.m_ProductCatalog.Add(
                "Imaris XT",
                "Access to the Open Source Community and XTensions. Freedom to customize");
            this.m_ProductCatalog.Add(
                "Imaris Scene Viewer",
                "Viewer for Imaris Scenes");
            this.m_ProductCatalog.Add(
                "Imaris Batch",
                "Automated Image Processing");
            this.m_ProductCatalog.Add(
                "AutoAligner",
                "Image Alignment Made Easy");
            this.m_ProductCatalog.Add(
                "AutoQuant",
                "Advanced and Fully Featured Image Deconvolution");
            this.m_ProductCatalog.Add(
                "File Reader",
                "Proprietary File Readers");

        }

    }
}
