using System;
using System.Windows.Forms;

namespace ImarisSelector
{
    /// <summary>
    /// ImarisSelector allows picking the module licenses to check out when Imaris starts.
    /// </summary>
    static class ImarisSelector
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
