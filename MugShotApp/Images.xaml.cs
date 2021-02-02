using System;
using System.Collections.Generic;
using System.IO;
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

namespace MugShotApp
{
    public partial class Images : Window
    {
        public Images()
        {
            InitializeComponent();
            getlist();
        }

        void getlist ()
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(@"C:\ProgramData\MugShotApp\images\");//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.png");
                string str = "";
                foreach (FileInfo file in Files)
                {
                    str = str + ", " + file.Name;
                    imageslist.Text = imageslist.Text + file.Name + "\n";
                }
            }
            catch(Exception f)
            {
                imageslist.Text = imageslist.Text + "\n Could not get the files in the images folder.";
                File.AppendAllText(@"C:\ProgramData\MugShotApp\debug.log", "\n " + "[" + DateTime.Now.ToString() + "]: " + "COULD NOT GET FILES IN THE IMAGES FOLDER. REASON: " + f);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\images\");
                imageslist.Text = imageslist.Text + "\n Opened the images folder.";
                File.AppendAllText(@"C:\ProgramData\MugShotApp\debug.log", "\n " + "[" + DateTime.Now.ToString() + "]: " + "OPENED IMAGES FOLDER");
            }
            catch(Exception d)
            {
                imageslist.Text = imageslist.Text + "\n Could not open the images folder.";
                File.AppendAllText(@"C:\ProgramData\MugShotApp\debug.log", "\n " + "[" + DateTime.Now.ToString() + "]: " + "COULD NOT OPEN IMAGES FOLDER. REASON: " + d);
            }
        }
    }
}
