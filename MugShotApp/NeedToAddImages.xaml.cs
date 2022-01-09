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
    /// <summary>
    /// Interaction logic for NeedToAddImages.xaml
    /// </summary>
    public partial class NeedToAddImages : Window
    {
        public NeedToAddImages()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\images\");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var win = new Settings();
            win.Show();
            this.Hide();
        }
    }
}
