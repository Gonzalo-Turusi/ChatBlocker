using ChatBlocker.Forms;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChatBlocker
{
    internal static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        [STAThread]
        static void Main()
        {
            // Create console window for debugging
            AllocConsole();
            Console.WriteLine("ChatBlocker Debug Console");
            Console.WriteLine("=========================");
            Console.WriteLine("Press Ctrl+C to close console");
            Console.WriteLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                Application.Run(new OverlayForm());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application error: {ex.Message}");
                MessageBox.Show($"Application error: {ex.Message}", "ChatBlocker Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                FreeConsole();
            }
        }
    }
}
