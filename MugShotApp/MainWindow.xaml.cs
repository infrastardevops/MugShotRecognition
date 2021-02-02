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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using CoenM.ImageHash.HashAlgorithms;
using CoenM.ImageHash;
using System.Windows.Threading;
using System.Threading;
using System.Drawing;
using System.Windows.Media.Effects;
using System.Drawing.Imaging;
using Rectangle = System.Drawing.Rectangle;

namespace MugShotApp
{
    // Alexander Walford 2021
    // Mugshot Image Recognition App For C# WPF

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            statustext.Content = "Selecting images based on user perferences..";
            getprefs();
        }

        int numperscreen = 0;
        int numofimagesblur = 0;
        int hashmethod = 0;
        int imagescountreal = 1;
        int imagerange = 13;
        int bluramount = 0;

        void getprefs ()
        {
            if (Directory.Exists(@"C:\ProgramData\MugShotApp\"))
            {
                Console.WriteLine("Directory already exists.");
                if (File.Exists(@"C:\ProgramData\MugShotApp\settings.pref"))
                {
                    Console.WriteLine("Preferences file already exists.");

                    int counter = 0;
                    string line;

                    // Read the file and display it line by line.  
                    System.IO.StreamReader file = new System.IO.StreamReader(@"C:\ProgramData\MugShotApp\settings.pref");
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.StartsWith(":: No. Of Images Per Screen:"))
                        {
                            string b = string.Empty;
                            int val = 0;

                            for (int i = 0; i < line.Length; i++)
                            {
                                if (Char.IsDigit(line[i]))
                                    b += line[i];
                            }

                            if (b.Length > 0)
                                val = int.Parse(b);

                            Console.WriteLine("Images per screen: " + val.ToString());
                            numperscreen = val;
                        }
                        else if (line.StartsWith(":: No. Of Images To Blur:"))
                        {
                            string b = string.Empty;
                            int val = 0;

                            for (int i = 0; i < line.Length; i++)
                            {
                                if (Char.IsDigit(line[i]))
                                    b += line[i];
                            }

                            if (b.Length > 0)
                                val = int.Parse(b);

                            Console.WriteLine("Images to blur: " + val.ToString());
                            numofimagesblur = val;
                        }
                        else if (line.StartsWith(":: Hashing Method:"))
                        {
                            string b = string.Empty;
                            int val = 0;

                            for (int i = 0; i < line.Length; i++)
                            {
                                if (Char.IsDigit(line[i]))
                                    b += line[i];
                            }

                            if (b.Length > 0)
                                val = int.Parse(b);
                            hashmethod = val;

                            Console.WriteLine("Mehtod Hashing Method: " + val.ToString());
                        }
                        else if (line.StartsWith(":: No. Of Images To Select From"))
                        {
                            string b = string.Empty;
                            int val = 0;

                            for (int i = 0; i < line.Length; i++)
                            {
                                if (Char.IsDigit(line[i]))
                                    b += line[i];
                            }

                            if (b.Length > 0)
                                val = int.Parse(b);
                            imagerange = val;
                            rangetext.Content = "Image Range: " + val;

                            Console.WriteLine("No. Of Images In Database: " + val.ToString());
                        }
                        else if (line.StartsWith(":: Blur Amount:"))
                        {
                            string b = string.Empty;
                            int val = 0;

                            for (int i = 0; i < line.Length; i++)
                            {
                                if (Char.IsDigit(line[i]))
                                    b += line[i];
                            }

                            if (b.Length > 0)
                                val = int.Parse(b);
                            bluramount = val;
                            statustext.Content = "Blur Amount: " + val;
                        }
                        else
                        {
                            Console.WriteLine("ERROR, INVALID SETTINGS FILE.");
                            var win = new ERROR();
                            win.Show();
                        }
                        counter++;
                    }

                    file.Close();
                    Console.WriteLine(numperscreen);
                    Console.WriteLine(numofimagesblur);
                    Console.WriteLine(hashmethod);
                    calchash();
  
                }
                else
                {
                    string[] lines = { ":: No. Of Images Per Screen: 12", ":: No. Of Images To Blur: 3", ":: Hashing Method: 0", ":: No. Of Images To Select From (set one higher than the number you wish to set): 13", ":: Blur Amount: 6" };
                    string docPath = @"C:\ProgramData\MugShotApp\";
                    using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "settings.pref")))
                    {
                        foreach (string line in lines)
                        outputFile.WriteLine(line);
                    }
                    Console.WriteLine("Created the preferences file.");
                    statustext.Content = "Created preferences file..";
                    getprefs();
                }
            }

            else
            {
                try
                {
                    Directory.CreateDirectory(@"C:\ProgramData\MugShotApp\");
                    Console.WriteLine("Created the directory.");
                    string[] lines = { ":: No. Of Images Per Screen: 12", ":: No. Of Images To Blur: 3", ":: Hashing Method: 0", ":: No. Of Images To Select From (set one higher than the number you wish to set): 13", ":: Blur Amount: 6" };
                    string docPath = @"C:\ProgramData\MugShotApp\";
                    using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "settings.pref")))
                    {
                        foreach (string line in lines)
                            outputFile.WriteLine(line);
                    }
                    Console.WriteLine("Created the preferences file.");
                    statustext.Content = "Created the preferences file..";
                    getprefs();
                }
                catch
                {
                    Console.WriteLine("Error creating the directory.");
                    statustext.Content = "Error creating the directory.";
                    var win = new ERROR();
                    win.Show();
                }
            }
        }


        void calchash ()
        {
            if (Directory.Exists(@"C:\ProgramData\MugShotApp\images\"))
            {
                Console.WriteLine("Images folder exists.");

                if (hashmethod == 0)
                {
                    PerceptualHash();
                    hashmodetext.Content = "Hashing Mode: Perceptual (default)";
                    statustext.Content = "Getting hashes using mode perceptual..";
                }

                else if (hashmethod == 1)
                {
                    Console.WriteLine("(DifferenceHash)");

                    var hashAlgorithm = new DifferenceHash();

                    string filename = "your filename";
                    var stream = File.OpenRead(filename);

                    ulong imageHash = hashAlgorithm.Hash(stream);
                }
                else if (hashmethod == 2)
                {
                    Console.WriteLine("(AverageHash)");

                    var hashAlgorithm = new AverageHash();

                    string filename = "your filename";
                    var stream = File.OpenRead(filename);

                    ulong imageHash = hashAlgorithm.Hash(stream);
                }
                else
                {
                    Console.WriteLine("ERROR, INVALID HASHMETHOD IN SETTINGS FILE.");
                    statustext.Content = "ERROR, INVALID HASHMETHOD IN SETTINGS FILE.";
                    var win = new ERROR();
                    win.Show();
                }
            }

            else
            {
                Console.WriteLine("Images folder does not exist. Creating..");
                try
                {
                    Directory.CreateDirectory(@"C:\ProgramData\MugShotApp\images\");
                    Console.WriteLine("Correctly created the images folder.");
                    statustext.Content = "Correctly created the images folder.";
                    var win = new NeedToAddImages();
                    win.Show();
                }
                catch
                {
                    var win = new ERROR();
                    win.Show();
                }
            }
        }


        void comparehashes ()
        {
            statustext.Content = "Comparing image hashes against originals..";



        }

        async void PerceptualHash()
        {
            Console.WriteLine("(PerceptualHash)");

            int stopat = numperscreen + 1;

            if (imagescountreal == stopat) {

                Console.WriteLine("Hashing completed.");
                await Task.Delay(2000);
                statustext.Content = "Hashing completed. Waiting for user interaction to blur..";
                blurbutton.Visibility = Visibility.Visible;
                imagescountreal = 1;
            }

            else
            {
                try
                {
                    var hashAlgorithm = new PerceptualHash();

                    Random rnd = new Random();
                    string numselected = rnd.Next(1, 13).ToString();
                    string numrange = rnd.Next(1, imagerange).ToString();

                    string filename = @"C:\ProgramData\MugShotApp\images\" + numrange + ".png";
                    var stream = File.OpenRead(filename);
                    Console.WriteLine(filename);
                    ulong imageHash = hashAlgorithm.Hash(stream);

                    if (imagescountreal == 1)
                    {
                        image1hash.Content = imageHash;
                        image1.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 2)
                    {
                        image2hash.Content = imageHash;
                        image2.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 3)
                    {
                        image3hash.Content = imageHash;
                        image3.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 4)
                    {
                        image4hash.Content = imageHash;
                        image4.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 5)
                    {
                        image5hash.Content = imageHash;
                        image5.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 6)
                    {
                        image6hash.Content = imageHash;
                        image6.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 7)
                    {
                        image7hash.Content = imageHash;
                        image7.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 8)
                    {
                        image8hash.Content = imageHash;
                        image8.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 9)
                    {
                        image9hash.Content = imageHash;
                        image9.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 10)
                    {
                        image10hash.Content = imageHash;
                        image10.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 11)
                    {
                        image11hash.Content = imageHash;
                        image11.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                    else if (imagescountreal == 12)
                    {
                        image12hash.Content = imageHash;
                        image12.Source = new BitmapImage(new Uri(filename));
                        imagescountreal++;
                        PerceptualHash();
                    }
                }
                catch
                {
                    statustext.Content = "Error: No images detected.";
                    var win = new NeedToAddImages();
                    win.Show();
                }
            }
        }

        void blur ()
        {
            blurbutton.Visibility = Visibility.Hidden;
            statustext.Content = "Bluring " + numofimagesblur.ToString() + " images at random..";

            BlurBitmapEffect myBlurEffect = new BlurBitmapEffect();

            myBlurEffect.Radius = 4;

            myBlurEffect.KernelType = KernelType.Box;

            if (numofimagesblur == 1)
            {
                Bitmap bitmap = new Bitmap(@"C:\ProgramData\MugShotApp\images\1.png");
                bitmap = Blur(bitmap, bluramount);
                bitmap.Save(@"C:\ProgramData\MugShotApp\images\1_blured.png");

                image1.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\1_blured.png"));
            }
            else if (numofimagesblur == 2)
            {
                Bitmap bitmap = new Bitmap(@"C:\ProgramData\MugShotApp\images\1.png");
                bitmap = Blur(bitmap, bluramount);
                bitmap.Save(@"C:\ProgramData\MugShotApp\images\1_blured.png");

                Bitmap bitmap2 = new Bitmap(@"C:\ProgramData\MugShotApp\images\2.png");
                bitmap2 = Blur(bitmap2, bluramount);
                bitmap2.Save(@"C:\ProgramData\MugShotApp\images\2_blured.png");

                image1.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\1_blured.png"));
                image2.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\2_blured.png"));

            }
            else if (numofimagesblur == 3)
            {
                Random rnd = new Random();
                int imageselector = rnd.Next(1, numperscreen);
                int imageselector2 = rnd.Next(1, numperscreen);
                int imageselector3 = rnd.Next(1, numperscreen);

                Bitmap bitmap = new Bitmap(@"C:\ProgramData\MugShotApp\images\" + imageselector + ".png");
                bitmap = Blur(bitmap, bluramount);
                bitmap.Save(@"C:\ProgramData\MugShotApp\images\1_blured.png");

                Bitmap bitmap2 = new Bitmap(@"C:\ProgramData\MugShotApp\images\" + imageselector2 + ".png");
                bitmap2 = Blur(bitmap2, bluramount);
                bitmap2.Save(@"C:\ProgramData\MugShotApp\images\2_blured.png");

                Bitmap bitmap3 = new Bitmap(@"C:\ProgramData\MugShotApp\images\" + imageselector3 + ".png");
                bitmap3 = Blur(bitmap3, bluramount);
                bitmap3.Save(@"C:\ProgramData\MugShotApp\images\3_blured.png");

                image1.Source = new BitmapImage(new Uri (@"C:\ProgramData\MugShotApp\images\1_blured.png"));
                image2.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\2_blured.png"));
                image3.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\3_blured.png"));
            }

            else if (numofimagesblur == 4)
            {
                Bitmap bitmap = new Bitmap(@"C:\ProgramData\MugShotApp\images\1.png");
                bitmap = Blur(bitmap, bluramount);
                bitmap.Save(@"C:\ProgramData\MugShotApp\images\1_blured.png");

                Bitmap bitmap2 = new Bitmap(@"C:\ProgramData\MugShotApp\images\2.png");
                bitmap2 = Blur(bitmap2, bluramount);
                bitmap2.Save(@"C:\ProgramData\MugShotApp\images\2_blured.png");

                Bitmap bitmap3 = new Bitmap(@"C:\ProgramData\MugShotApp\images\3.png");
                bitmap3 = Blur(bitmap3, bluramount);
                bitmap3.Save(@"C:\ProgramData\MugShotApp\images\3_blured.png");

                Bitmap bitmap4 = new Bitmap(@"C:\ProgramData\MugShotApp\images\4.png");
                bitmap4 = Blur(bitmap4, bluramount);
                bitmap4.Save(@"C:\ProgramData\MugShotApp\images\4_blured.png");

                image1.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\1_blured.png"));
                image2.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\2_blured.png"));
                image3.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\3_blured.png"));
                image4.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\4_blured.png"));
            }

            else if (numofimagesblur == 5)
            {
                Bitmap bitmap = new Bitmap(@"C:\ProgramData\MugShotApp\images\1.png");
                bitmap = Blur(bitmap, bluramount);
                bitmap.Save(@"C:\ProgramData\MugShotApp\images\1_blured.png");

                Bitmap bitmap2 = new Bitmap(@"C:\ProgramData\MugShotApp\images\2.png");
                bitmap2 = Blur(bitmap2, bluramount);
                bitmap2.Save(@"C:\ProgramData\MugShotApp\images\2_blured.png");

                Bitmap bitmap3 = new Bitmap(@"C:\ProgramData\MugShotApp\images\3.png");
                bitmap3 = Blur(bitmap3, bluramount);
                bitmap3.Save(@"C:\ProgramData\MugShotApp\images\3_blured.png");

                Bitmap bitmap4 = new Bitmap(@"C:\ProgramData\MugShotApp\images\4.png");
                bitmap4 = Blur(bitmap4, bluramount);
                bitmap4.Save(@"C:\ProgramData\MugShotApp\images\4_blured.png");

                Bitmap bitmap5 = new Bitmap(@"C:\ProgramData\MugShotApp\images\5.png");
                bitmap5 = Blur(bitmap5, bluramount);
                bitmap5.Save(@"C:\ProgramData\MugShotApp\images\5_blured.png");

                image1.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\1_blured.png"));
                image2.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\2_blured.png"));
                image3.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\3_blured.png"));
                image4.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\4_blured.png"));
                image5.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\5_blured.png"));
            }

            else if (numofimagesblur == 6)
            {
                Bitmap bitmap = new Bitmap(@"C:\ProgramData\MugShotApp\images\1.png");
                bitmap = Blur(bitmap, bluramount);
                bitmap.Save(@"C:\ProgramData\MugShotApp\images\1_blured.png");

                Bitmap bitmap2 = new Bitmap(@"C:\ProgramData\MugShotApp\images\2.png");
                bitmap2 = Blur(bitmap2, bluramount);
                bitmap2.Save(@"C:\ProgramData\MugShotApp\images\2_blured.png");

                Bitmap bitmap3 = new Bitmap(@"C:\ProgramData\MugShotApp\images\3.png");
                bitmap3 = Blur(bitmap3, bluramount);
                bitmap3.Save(@"C:\ProgramData\MugShotApp\images\3_blured.png");

                Bitmap bitmap4 = new Bitmap(@"C:\ProgramData\MugShotApp\images\4.png");
                bitmap4 = Blur(bitmap4, bluramount);
                bitmap4.Save(@"C:\ProgramData\MugShotApp\images\4_blured.png");

                Bitmap bitmap5 = new Bitmap(@"C:\ProgramData\MugShotApp\images\5.png");
                bitmap5 = Blur(bitmap5, bluramount);
                bitmap5.Save(@"C:\ProgramData\MugShotApp\images\5_blured.png");

                Bitmap bitmap6 = new Bitmap(@"C:\ProgramData\MugShotApp\images\6.png");
                bitmap6 = Blur(bitmap6, bluramount);
                bitmap6.Save(@"C:\ProgramData\MugShotApp\images\6_blured.png");

                image1.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\1_blured.png"));
                image2.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\2_blured.png"));
                image3.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\3_blured.png"));
                image4.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\4_blured.png"));
                image5.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\5_blured.png"));
                image6.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\6_blured.png"));
            }

            else
            {
                Random rnd = new Random();
                int imageselector = rnd.Next(1,numperscreen);

                statustext.Content = "ERROR: Invalid settings detected. Bluring 3 images only. Maximum to blur is 6.";

                Bitmap bitmap = new Bitmap(@"C:\ProgramData\MugShotApp\images\" + imageselector + ".png");
                bitmap = Blur(bitmap, bluramount);
                bitmap.Save(@"C:\ProgramData\MugShotApp\images\1_blured.png");

                Bitmap bitmap2 = new Bitmap(@"C:\ProgramData\MugShotApp\images\" + imageselector + ".png");
                bitmap2 = Blur(bitmap2, bluramount);
                bitmap2.Save(@"C:\ProgramData\MugShotApp\images\2_blured.png");

                Bitmap bitmap3 = new Bitmap(@"C:\ProgramData\MugShotApp\images\" + imageselector + ".png");
                bitmap3 = Blur(bitmap3, bluramount);
                bitmap3.Save(@"C:\ProgramData\MugShotApp\images\3_blured.png");

                image1.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\1_blured.png"));
                image2.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\2_blured.png"));
                image3.Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\3_blured.png"));
            }

            statustext.Content = "The images have been blurred. Waiting for user interaction..";
            checkagain.Visibility = Visibility.Visible;
        }

        private static Bitmap Blur(Bitmap image, Int32 blurSize)
        {
            return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);
        }

        private unsafe static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = System.Drawing.Image.GetPixelFormatSize(blurred.PixelFormat);

            // Get pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            avgB += data[0]; // Blue
                            avgG += data[1]; // Green
                            avgR += data[2]; // Red

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                    {
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            // Change values
                            data[0] = (byte)avgB;
                            data[1] = (byte)avgG;
                            data[2] = (byte)avgR;
                        }
                    }
                }
            }

            // Unlock the bits
            blurred.UnlockBits(blurredData);

            return blurred;
        }

        void recheckhash ()
        {
            checkagain.Visibility = Visibility.Hidden;
            statustext.Content = "Re-Checking new hash values for the images..";

            int stopat = numperscreen + 1;

            if (imagescountreal == stopat)
            {

                Console.WriteLine("Re-Hashing completed.");
                statustext.Content = "Re-Hashing completed.";
                imagescountreal = 0;
                calchashesbutton.Visibility = Visibility.Visible;
            }

            else
            {
                try
                {
                    var hashAlgorithm = new PerceptualHash();

                    Random rnd = new Random();
                    string numselected = rnd.Next(1, 13).ToString();
                    string numrange = rnd.Next(1, numofimagesblur).ToString();

                    string filename = @"C:\ProgramData\MugShotApp\images\" + numrange + "_blured.png";
                    var stream = File.OpenRead(filename);
                    Console.WriteLine(filename);
                    ulong imageHash = hashAlgorithm.Hash(stream);

                    if (imagescountreal == 1)
                    {
                        image1hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 2)
                    {
                        image2hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 3)
                    {
                        image3hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 4)
                    {
                        image4hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 5)
                    {
                        image5hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 6)
                    {
                        image6hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 7)
                    {
                        image7hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 8)
                    {
                        image8hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 9)
                    {
                        image9hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 10)
                    {
                        image10hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 11)
                    {
                        image11hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                    else if (imagescountreal == 12)
                    {
                        image12hash.Content = imageHash;
                        imagescountreal++;
                        recheckhash();
                    }
                }
                catch
                {
                    statustext.Content = "Error: No images detected.";
                    var win = new NeedToAddImages();
                    win.Show();
                }
            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Images folder
            System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\images\");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Settings
            System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\settings.pref");
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            blur();
        }

        private void checkagain_Click(object sender, RoutedEventArgs e)
        {
            recheckhash();
        }

        private void calchashesbutton_Click(object sender, RoutedEventArgs e)
        {
            comparehashes();
        }
    }
}
