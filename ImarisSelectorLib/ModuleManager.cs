using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ImarisSelectorLib
{
    class Module
    {
        private String m_Name;
        private String m_Product;
        private String m_Description;

        public String Name { get { return this.m_Name; } }
        public String Product { get { return this.m_Product; } }
        public String Description { get { return this.m_Description; } }
  
        public Module(String name, String product, String description)
        {
            this.m_Name = name;
            this.m_Product = product;
            this.m_Description = description;
        }
    }

/*    class Product
    {
        private String m_Name;
        private String m_Description;

        public String Name { get { return this.m_Name; } }
        public String Description { get { return this.m_Description; } }

        public Product(String name, String product, String description)
        {
            this.m_Name = name;
            this.m_Description = description;
        }
    }
*/
    class ProductManager
    {
        Hashtable m_KnownProducts;
        Hashtable m_LicensedProducts;

        public ProductManager(List<String> moduleNames)
        {
            // Create list of known products
            FillKnownProducts();

            // Fill licensed products 
            FillLicensedProducts(moduleNames);

        }

        /// <summary>
        /// Fill in all known products with description
        /// </summary>
        private void FillKnownProducts()
        {
            m_KnownProducts = new Hashtable();
            m_KnownProducts.Add(
                "Imaris",
                "3D and 4D Real-Time Interactive Image Visualization");
            m_KnownProducts.Add(
                "Imaris Measurement Pro",
                "The Analysis and Quantification Engine");
            m_KnownProducts.Add(
                "Imaris Track",
                "Discover the Meaning of Motion");
            m_KnownProducts.Add(
                "Imaris Coloc",
                "Isolate, Visualize and Quantify Colocalized Regions");
            m_KnownProducts.Add(
                "Imaris Cell",
                "Analysis and Visualization of intra/inter Cellular Relationships");
            m_KnownProducts.Add(
                "Filament Tracer",
                "Analysis and Visualization of Filamentous Structures");
            m_KnownProducts.Add(
                "Imaris Vantage",
                "Data Plotting (2D to 5D), Interpretation and Mining");
            m_KnownProducts.Add(
                "Imaris XT",
                "Access to the Open Source Community and XTensions. Freedom to customize");
            m_KnownProducts.Add(
                "Imaris Scene Viewer",
                "Viewer for Imaris Scenes");
            m_KnownProducts.Add(
                "Imaris Batch",
                "Automated Image Processing");
            m_KnownProducts.Add(
                "AutoAligner",
                "Image Alignment Made Easy");
            m_KnownProducts.Add(
                "AutoQuant",
                "Advanced and Fully Featured Image Deconvolution");
            m_KnownProducts.Add(
                "File Readers",
                "Proprietary File Readers");
        }

        /// <summary>
        /// Fill in all licensed products based on the module list obtained from the registry
        /// </summary>
        /// <param name="moduleNames">List of module names found in the registry</param>
        private void FillLicensedProducts(List<String> moduleNames)
        {
        }

    }

    class ModuleManager
    {
        Hashtable m_KnownModules;

        public ModuleManager()
        {
            // TODO: Some modules are missing
            m_KnownModules = new Hashtable();
            m_KnownModules.Add(
                "ImarisAnalyzer",
                new Module(
                    "Imaris Analyzer",
                    "Obsolete", 
                    "Unknown module"));
            m_KnownModules.Add(
                "ImarisBase",
                new Module(
                    "Imaris",
                    "Imaris",
                    "Imaris"));
            m_KnownModules.Add(
                "ImarisCellsViewer",
                new Module(
                    "Imaris Cell",
                    "Imaris Cell",
                    ""));
            m_KnownModules.Add(
                "ImarisColoc",
                new Module(
                    "Imaris Coloc",
                    "Imaris Coloc",
                    ""));
            m_KnownModules.Add(
                "ImarisFilament",
                new Module(
                    "Filament Tracer",
                    "Filament Tracer",
                    ""));
            m_KnownModules.Add(
                "ImarisInPress",
                new Module(
                    "Imaris InPress",
                    "Imaris Vantage", 
                    "Part of Imaris Vantage"));
            m_KnownModules.Add(
                "ImarisIPSS",
                new Module(
                    "Imaris XT",
                    "Imaris XT",
                    ""));
            m_KnownModules.Add(
                "ImarisManualSurface",
                new Module(
                    "Imaris Manual Surface", 
                    "Imaris Measurement Pro", 
                    "Part of Imaris Measurement Pro"));
            m_KnownModules.Add(
                "ImarisMeasurementPoint",
                new Module(
                    "Imaris Measurement Point",
                    "Imaris Measurement Pro",
                    "Part of Imaris Measurement Pro"));
            m_KnownModules.Add(
                "ImarisReaderBiorad",
                new Module(
                    "Bio-Rad MRC (series) file reader", 
                    "File Reader", 
                    "Supported extensions: *.pic"));
            m_KnownModules.Add(
                "ImarisReaderDeltaVision",
                new Module(
                    "Applied Precision DeltaVision file reader", 
                    "File Reader", 
                    "Supported extensions: *.r3d, *.dv"));
            m_KnownModules.Add(
                "ImarisReaderGatan",
                new Module(
                    "Gatan DigitalMicrograph (series) file reader", 
                    "File Reader",
                    "Supported extensions: *.dm3"));
            m_KnownModules.Add(
                "ImarisReaderHamamatsu",
                "Hamamatsu Compix SimplePCI file reader (*.cxd)");
            m_KnownModules.Add(
                "ImarisReaderIII",
                "Imaris version 3.0 reader (*.ims)");
            m_KnownModules.Add(
                "ImarisReaderIMOD",
                new Module(
                "IMOD MRC file reader",
                "File Reader", 
                "Supported extensions: *.mrc, *.st, *.rec"));
            m_KnownModules.Add(
                "ImarisReaderIPLab",
                new Module(
                    "???",
                    "File Reader",
                    ""));
            m_KnownModules.Add(
                "ImarisReaderLeica",
                new Module(
                "Leica file reader",
                "File Readeer",
                "Supported extensions: *.lif, *.tif, *.tiff, *.inf, *.info, *.lei, *.raw"));
            m_KnownModules.Add(
                "ImarisReaderMicroManager",
                new Module(
                "Micro-Manager Image5D file reader",
                "File Reader",
                "Supported extensions: *.tif, *.tiff, *.txt"));
            m_KnownModules.Add(
                "ImarisReaderNikon",
                new Module(
                "Nikon Image Cytometry Standard and ND2 file reader",
                "File Reader", 
                "Supported extensions: *.ics, *.ids, *.nd2"));
            m_KnownModules.Add(
                "ImarisReaderOlympus",
                new Module(
                "Olympus CellR, Fluoview OIB, OIF, TIFF file reader",
                "File Reader",
                "Supported extensions: *.tif, *.tiff, *.oib, *.oif"));
            m_KnownModules.Add(
                "ImarisReaderOME",
                new Module(
                "Open Microscopy Environment TIFF and XML file reader",
                "File Reader",
                "Supported extensions: *.tif, *.tiff, *.ome"));
            m_KnownModules.Add(
                "ImarisReaderPerkinElmerInc",
                new Module(
                "Perkin Elmer Improvision Openlab LIFF (series), RAW and UltraView file reader",
                "File Reader",
                "Supported extensions: *.liff, *.raw, *.tim, *.zpo"));
            m_KnownModules.Add(
                "ImarisReaderPrairie",
                new Module(
                "Prairie Technologies View file reader",
                "File Reader",
                "Supported extensions: *.xml, *.cfg, *.tif, *.tiff"));
            m_KnownModules.Add(
                "ImarisReaderTILL",
                new Module(
                "TILL Photonics TILLvisION file reader",
                "File Reader",
                "Supported extensions: *.rbinf"));
            m_KnownModules.Add(
                "ImarisReaderUniversalImaging",
                new Module(
                    "",
                    "File Reader",
                    ""));
            m_KnownModules.Add(
                "ImarisReaderZeiss",
                new Module(
                "Zeiss AxioVision, CZI, LSM 310, 410, 510, 710 file reader",
                "File Reader",
                "Supported extensions: *.zvi, *.czi, *.lsm, *.tif, *.tiff"));
            m_KnownModules.Add(
                "ImarisSceneViewer",
                new Module(
                    "Imaris Scene Viewer",
                    "Imaris Scene Viewer",
                    ""));
            m_KnownModules.Add(
                "ImarisStatistics",
                new Module(
                "Imaris Statistics",
                "Imaris Measurement Pro",
                "Part of Imaris Measurement Pro"));
            m_KnownModules.Add(
                "ImarisSurpass",
                new Module(
                "Imaris Surpass",
                "Imaris",
                "Part of Imaris base"));
            m_KnownModules.Add(
                "ImarisSurpassBase",
                new Module(
                    "Imaris Surpass Base",
                    "Imaris",
                    "Part of Imaris base"));
            m_KnownModules.Add(
                "ImarisTime",
                new Module(
                    "Imaris Time",
                    "Imaris",
                    "Part of Imaris base"));
            m_KnownModules.Add(
                "ImarisTopography",
                new Module(
                    "ImarisTopography",
                    "Imaris",
                    "Part of Imaris base"));
            m_KnownModules.Add(
                "ImarisTrack",
                new Module(
                    "Imaris Track",
                    "Imaris Track",
                    ""));
            m_KnownModules.Add(
                "ImarisVantage",
                new Module(
                    "Imaris Vantage",
                    "Imaris Vantage",
                    "Part of Imaris Vantage"));
        }
    }
}
