using System;
using System.Windows.Forms;

namespace ImarisSelectorAdmin
{
    /// <summary>
    /// The ImarisSelector companion administrator tool.
    /// </summary>
    static class ImarisSelectorAdmin
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
