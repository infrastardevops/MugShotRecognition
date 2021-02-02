using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace MugShotApp
{
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            checksettingsfile();
        }

        void checksettingsfile ()
        {
            if (File.Exists(@"C:\ProgramData\MugShotApp\settings.txt"))
            {
                statustext.Content = "File exists.";
                try
                {
                    System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\settings.txt");
                    File.AppendAllText(@"C:\ProgramData\MugShotApp\debug.log", "\n " + "[" + DateTime.Now.ToString() + "]: " + "OPENED SETTINGS");
                }
                catch(Exception e)
                {
                    statustext.Content = "File exists but couldn't open it.";
                    File.AppendAllText(@"C:\ProgramData\MugShotApp\debug.log", "\n " + "[" + DateTime.Now.ToString() + "]: " + "FILE EXISTS BUT CAN'T OPEN IT. REASON: " + e);
                }
            }
            else
            {
                string[] lines = { ":: No. Of Images Per Screen: 12", ":: No. Of Images To Blur: 3", ":: Hashing Method: 0", ":: No. Of Images To Select From (set one higher than the number you wish to set): 13", ":: Blur Amount: 6" };
                string docPath = @"C:\ProgramData\MugShotApp\";
                using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "settings.txt")))
                {
                    foreach (string line in lines)
                        outputFile.WriteLine(line);
                }
                statustext.Content = "File does not exist. Created a new one.";
                try
                {
                    System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\settings.txt");
                    File.AppendAllText(@"C:\ProgramData\MugShotApp\debug.log", "\n " + "[" + DateTime.Now.ToString() + "]: " + "OPENED SETTINGS FILE");
                }
                catch(Exception e)
                {
                    statustext.Content = "Created file but cannot open.";
                    File.AppendAllText(@"C:\ProgramData\MugShotApp\debug.log", "\n " + "[" + DateTime.Now.ToString() + "]: " + "CREATED SETTINGS FILE BUT CANNOT OPEN FOR REASON: " + e);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\settings.txt");
            }
            catch(Exception b)
            {
                statustext.Content = "Could not open the settings file.";
                File.AppendAllText(@"C:\ProgramData\MugShotApp\debug.log", "\n " + "[" + DateTime.Now.ToString() + "]: " + "COULD NOT OPEN SETTINGS FILE. REASON: " + b);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\debug.log");
            }
            catch
            {
                Console.WriteLine("Could not open the debug log file.");
            }
        }
    }
}
