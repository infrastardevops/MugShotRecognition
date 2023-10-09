using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using CoenM.ImageHash.HashAlgorithms;
using CoenM.ImageHash;
using System.Drawing;
using System.Windows.Media.Effects;
using System.Drawing.Imaging;
using Rectangle = System.Drawing.Rectangle;

namespace MugShotApp
{
    // Alexander Walford 2021
    // Mugshot Image Recognition App For C# WPF

    // Liscenced for use by Infrastar LTD 2023 through the creative commons attribute by Alexander Walford.

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetElements();
            getprefs();
            errorlogfile();
        }

        // public variables
        int numperscreen = 0;
        int numofimagesblur = 0;
        int hashmethod = 0;
        int imagescountreal = 1;
        int imagerange = 13;
        int bluramount = 0;
        int maxnumofimagestoblur = 8;
        int passmark = 90;

        // refactored: store the randomly selected image paths, requires further implementation
        List<string> randomblurimages = new List<string>();

        // refactored: array of Image objects for UI
        List<System.Windows.Controls.Image> image_elements = new List<System.Windows.Controls.Image>();

        // refactored: array of Label objects for UI difference hashes
        List<System.Windows.Controls.Label> hash_text_elements = new List<System.Windows.Controls.Label>();

        // refactored: array of Label objects for UI hashes
        List<System.Windows.Controls.Label> hash_norm_text_elements = new List<System.Windows.Controls.Label>();

        // create a public logger object reference
        Logger logger = new Logger();

        // for the refacorisation of the addition of an Image element array
        // this is better practice, reduces code and faster in general
        void SetElements()
        {
            // add all the image elements to the list
            image_elements.Add(image1);
            image_elements.Add(image2);
            image_elements.Add(image3);
            image_elements.Add(image4);
            image_elements.Add(image5);
            image_elements.Add(image6);
            image_elements.Add(image7);
            image_elements.Add(image8);
            image_elements.Add(image9);
            image_elements.Add(image10);
            image_elements.Add(image11);
            image_elements.Add(image12);
            // add all the Label elements to the list for the differences
            hash_text_elements.Add(similaritytext1);
            hash_text_elements.Add(similaritytext2);
            hash_text_elements.Add(similaritytext3);
            hash_text_elements.Add(similaritytext4);
            hash_text_elements.Add(similaritytext5);
            hash_text_elements.Add(similaritytext6);
            // add the Label elements to the list for the hashes
            hash_norm_text_elements.Add(image1hash);
            hash_norm_text_elements.Add(image2hash);
            hash_norm_text_elements.Add(image3hash);
            hash_norm_text_elements.Add(image4hash);
            hash_norm_text_elements.Add(image5hash);
            hash_norm_text_elements.Add(image6hash);
            hash_norm_text_elements.Add(image7hash);
            hash_norm_text_elements.Add(image8hash);
            hash_norm_text_elements.Add(image9hash);
            hash_norm_text_elements.Add(image10hash);
            hash_norm_text_elements.Add(image11hash);
            hash_norm_text_elements.Add(image12hash);
        }

        void NoImagesDetected ()
        {
            statustext.Content = "Error: No images detected.";
            var win = new NeedToAddImages(); // show the window for informing the user that they need to add images
            logger.Log("[ X ] NO IMAGES DETECTED");
            win.Show();
        }

        void errorlogfile()
        {
            if (Directory.Exists(@"C:\ProgramData\MugShotApp\"))
            {
                try
                {
                    if (File.Exists(@"C:\ProgramData\MugShotApp\debug.log"))
                    {
                        logger.Log("[ ! ] APPLICATION STARTED"); // log the application starting
                    }
                    else
                    {
                        string[] lines = { "### CUSTOM DEBUGGING FILE ###", "# Important application messages will appear here.", "# The latest messages are located at the bottom of the file. #", " ", ":: LOG START ::" };
                        string docPath = @"C:\ProgramData\MugShotApp\";
                        using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "debug.log")))
                        {
                            foreach (string line in lines)
                            {
                                outputFile.WriteLine(line); // write line to output file
                            } 
                        }
                    }
                }
                catch(Exception e)
                {
                    var win = new ERROR();
                    win.Show();
                    logger.Log("[ X ] MAJOR ERROR: " + e.Message.ToString());
                }
            }
        }

        // get the user preferences from the user preferences file
        async void getprefs ()
        {
            statustext.Content = "Selecting images based on user perferences..";
            try
            {
                logger.Log("SELECTING IMAGES BASED ON USER PREFERENCES");
            }
            catch
            {
            }

            retest.Visibility = Visibility.Hidden; // hide the re-test button

            if (File.Exists(@"C:\ProgramData\MugShotApp\output.txt"))
            {
                outputfilebutton.Visibility = Visibility.Visible; // show the output file button
            }
            else
            {
                if (Directory.Exists(@"C:\ProgramData\MugShotApp\"))
                {
                    File.Create(@"C:\ProgramData\MugShotApp\output.txt"); // create the output file
                }
                else
                {
                    Directory.CreateDirectory(@"C:\ProgramData\MugShotApp\");
                    File.Create(@"C:\ProgramData\MugShotApp\output.txt");
                }
                outputfilebutton.Visibility = Visibility.Visible; // show the output file button
            }

            if (Directory.Exists(@"C:\ProgramData\MugShotApp\"))  // check if application directory exists
            {
                Console.WriteLine("Directory already exists.");
                if (File.Exists(@"C:\ProgramData\MugShotApp\settings.txt"))
                {
                    Console.WriteLine("Preferences file already exists.");
                    int counter = 0;
                    string line;
                    // Read the file and display it line by line.  
                    System.IO.StreamReader file = new System.IO.StreamReader(@"C:\ProgramData\MugShotApp\settings.txt");
                    while ((line = file.ReadLine()) != null)
                    {
                        // needs to be refactored
                        if (line.StartsWith(":: No. Of Images Per Screen:"))
                        {
                            string b = string.Empty;
                            int val = 0;
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (Char.IsDigit(line[i]))
                                {
                                    b += line[i];
                                }
                            }
                            if (b.Length > 0)
                            {
                                val = int.Parse(b);
                            }
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
                            {
                                val = int.Parse(b);
                            }
                            imagerange = val;
                            int f = val - 1;
                            rangetext.Content = "Image Range: " + f.ToString();
                            if (f == 12)
                            {
                                rangetext.Content = "Image Range: " + f.ToString() + " (default)";
                            }
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
                    logger.Log("NUM OF IMAGES PER SCREEN: " + numperscreen.ToString());
                    logger.Log("NUM OF IMAGES TO BLUR: " + numofimagesblur.ToString());
                    logger.Log("HASHING METHOD: " + hashmethod.ToString());
                    await Task.Delay(3000);
                    calchash();
                }
                else
                {
                    string[] lines = { ":: No. Of Images Per Screen: 12", ":: No. Of Images To Blur: 3", ":: Hashing Method: 0", ":: No. Of Images To Select From (set one higher than the number you wish to set): 13", ":: Blur Amount: 6" };
                    string docPath = @"C:\ProgramData\MugShotApp\";
                    using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "settings.txt")))
                    {
                        foreach (string line in lines)
                        {
                            outputFile.WriteLine(line);
                        }
                    }
                    Console.WriteLine("Created the preferences file.");
                    statustext.Content = "Created preferences file..";
                    await Task.Delay(3000);
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
                    using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "settings.txt")))
                    {
                        foreach (string line in lines) // iterate through each of the lines of the settings file
                        {
                            outputFile.WriteLine(line);
                        }
                    }
                    Console.WriteLine("Created the preferences file.");
                    statustext.Content = "Created the preferences file..";
                    await Task.Delay(3000);
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

        // detmine the hashing algorithm from the settings file
        void calchash ()
        {
            if (Directory.Exists(@"C:\ProgramData\MugShotApp\images\"))
            {
                Console.WriteLine("Images folder exists.");
                logger.Log("Found the images folder.");

                // refactored
                // determines which hashing mode to use based on the user's preferences in the settings file
                switch(hashmethod)
                {
                    case 0:
                        PerceptualHash();
                        hashmodetext.Content = "Hashing Mode: Perceptual (default)";
                        statustext.Content = "Getting hashes using mode perceptual..";
                        logger.Log("HASHING MODE: PERCEPTUAL");
                        break;
                    case 1:
                        Console.WriteLine("(DifferenceHash)");
                        DifferenceHash();
                        hashmodetext.Content = "Hashing Mode: DifferenceHash";
                        statustext.Content = "Getting hashes using mode Difference..";
                        logger.Log("HASHING MODE: DIFFERENCE");
                        break;
                    case 2:
                        Console.WriteLine("(AverageHash)");

                        AverageHash();
                        hashmodetext.Content = "Hashing Mode: AverageHash";
                        statustext.Content = "Getting hashes using mode Average..";
                        logger.Log("HASHING MODE: AVERAGEHASH");
                        break;
                    default:
                        Console.WriteLine("ERROR, INVALID HASHMETHOD IN SETTINGS FILE.");
                        statustext.Content = "ERROR, INVALID HASHMETHOD IN SETTINGS FILE.";
                        logger.Log("INVALID HASHMETHOD IN SETTINGS FILE");
                        var win = new ERROR();
                        win.Show();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Images folder does not exist. Creating..");
                logger.Log("CREATING IMAGES FOLDER..");
                try
                {
                    Directory.CreateDirectory(@"C:\ProgramData\MugShotApp\images\");
                    Console.WriteLine("Correctly created the images folder.");
                    statustext.Content = "Correctly created the images folder.";
                    logger.Log("CREATED IMAGES FOLDER"); // log
                    var win = new NeedToAddImages(); // show the "add images" window
                    win.Show();
                }
                catch
                {
                    logger.Log("ERROR CREATING IMAGES FOLDER");
                    var win = new ERROR();
                    win.Show();
                }
            }
        }

        // compare hashes method
        void comparehashes ()
        {
            try
            {
                if (!File.Exists(@"C:\ProgramData\MugShotApp\output.txt"))
                {
                    try
                    {
                        File.Create(@"C:\ProgramData\MugShotApp\output.txt");
                    }
                    catch (Exception e)
                    {
                        logger.Log("FAILED TO CREATED OUTPUT FILE. REASON: " + e);
                    }
                }
            }
            catch(Exception e)
            {
                logger.Log("FAILED TO DETECT FILE. REASON: " + e.Message.ToString()); // could not read the file or create it, likely a permission error
            }

            calchashesbutton.Visibility = Visibility.Hidden; // hide the calculate hashes button
            statustext.Content = "Comparing image hashes against originals..";
            logger.Log("RE-COMPARING HASHES"); // log action

            // refactored
            // iterates through the number of images to blur, renders the blurred image if required and generates the similarity hash
            if (numofimagesblur < maxnumofimagestoblur)
            {
                var hashAlgorithm = new AverageHash(); // new average hash object

                List<string> file_names = new List<string>();

                // regenerate hashes for all images on canvas and save them into a string list
                foreach (System.Windows.Controls.Image f in image_elements)
                {
                    string filename = f.Source.ToString().Replace("file:///", ""); // get the image path from the canvas
                    file_names.Add(filename);
                }

                // now that we have all the paths, we can compare the hashes
                // iterate through every element in the canvas
                for (int i = 0; i < 6; i++) 
                {
                    string path1 = file_names[i]; // get the top image's path
                    string path2 = file_names[i + 6]; // get the image below it

                    var stream = File.OpenRead(path1); // open the filestream for the first image
                    Console.WriteLine(path1); // print the filename
                    ulong imageHash = hashAlgorithm.Hash(stream); // generate the image hash for the file stream
                    var stream2 = File.OpenRead(path2); // create a new filestream for the comparision image
                    Console.WriteLine(path2); // print the second file's filename
                    ulong imageHash2 = hashAlgorithm.Hash(stream2); // generate the hash for the second image

                    // generate the similarity hash percentage
                    double percentageImageSimilarity = CompareHash.Similarity(imageHash, imageHash2); 
                    
                    // check if higher than the required passmark %
                    if (percentageImageSimilarity > passmark) 
                    {
                        // likely recognised
                        hash_text_elements[i].Content = percentageImageSimilarity.ToString() + "% (Recognised)";
                        logger.LogOuput(i.ToString() + "_blurred.png was correctly recognised.");
                    }
                    else 
                    {
                        // not recognised
                        hash_text_elements[i].Content = percentageImageSimilarity.ToString() + "% (NOT Recognised)";
                        logger.LogOuput(i.ToString() + "_blurred.png was not recognised.");
                    }
                }
                // done comparing the hashes
                statustext.Content = "Comparison completed.";
                logger.Log("[ ! ] COMPLETED HASH COMPARISON.");
                retest.Visibility = Visibility.Visible; // make the retest button visible
            }
            else
            {
                int lc = 0;
                foreach (string s in randomblurimages) // iterate through all the images
                {
                    var hashAlgorithm = new AverageHash(); // create a new average hash object
                    string filename = @"C:\ProgramData\MugShotApp\images\" + s + ".png"; // image 1 filename
                    string filename2 = @"C:\ProgramData\MugShotApp\images\"+ s + "_blurred.png"; // image 2 filename

                    var stream = File.OpenRead(filename); // open image 1
                    Console.WriteLine(filename); // print image 1 filename
                    ulong imageHash = hashAlgorithm.Hash(stream); // hash image 1
                    var stream2 = File.OpenRead(filename2); // open image 2
                    Console.WriteLine(filename2); // print image 2 filename
                    ulong imageHash2 = hashAlgorithm.Hash(stream2); // hash image 2
                    double percentageImageSimilarity = CompareHash.Similarity(imageHash, imageHash2); // compare the hashes, returns a % simularity
                    if (percentageImageSimilarity > passmark) // determine if the hash is high enough to pass
                    {
                        similaritytext1.Content = percentageImageSimilarity.ToString() + "% (Recognised)"; // recognised
                    }
                    else
                    {
                        similaritytext1.Content = percentageImageSimilarity.ToString() + "% (NOT Recognised)"; // not recognised
                    }

                    // vertically compare the images
                    image_elements[1].Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\" + s + ".png")); // get the file name of the randomly blurred original image
                    image_elements[lc / 2].Source = new BitmapImage(new Uri(@"C:\ProgramData\MugShotApp\images\ " + s + "_blurred.png")); // get the file name of the same image, but blurred and set the element horizontally below it

                    statustext.Content = "Comparison completed.";
                    retest.Visibility = Visibility.Visible; // show the retest button

                    logger.Log("INVALID SETTING: NUMOFIMAGESTOBLUR"); // invalid setting log
                    lc++;
                }
            }
        }

        // generate a average hash for all of the images
        void AverageHash()
        {
            Console.WriteLine("(AverageHash)");
            var hashAlgorithm = new AverageHash(); // new hash object
            Random rnd = new Random(); // new random object
            string numselected = rnd.Next(1, 13).ToString(); // randomly select a number between 1 and 12 and convert to a string, used for selecting the image
            string numrange = rnd.Next(1, imagerange).ToString(); // randomly select a number between 1 and 12 and then convert it to a string, used for determining the range of the images
            string filename = @"C:\ProgramData\MugShotApp\images\" + numrange + ".png"; // set the path string
            var stream = File.OpenRead(filename); // open the file
            Console.WriteLine(filename); // print the filename to the console
            ulong imageHash = hashAlgorithm.Hash(stream); // hash the image from the file stream in a ulong variable

            // refactored
            for (int i = 1; i < 11; i++) // iterate through every element
            { 
                hash_text_elements[i].Content = imageHash; // set the Label content to the hash value
                image_elements[i].Source = new BitmapImage(new Uri(filename)); // set the image source as the selected file
                imagescountreal = i; // set the number of images that are real
            }

            Console.WriteLine("Hashing completed.");
            statustext.Content = "Hashing completed. Waiting for user interaction to blur.."; // print the console about the hashing being completed
            blurbutton.Visibility = Visibility.Visible; // set the blue button to visible
        }

        // generate the difference hashes
        async void DifferenceHash()
        {
            Console.WriteLine("(DifferenceHash)");
            var hashAlgorithm = new DifferenceHash(); // generate a difference hash

            // refactored
            for (int i = 1; i < 12; i++) // iterate through every element
            {
                string filename = @"C:\ProgramData\MugShotApp\images\" + i.ToString() + ".png"; // generate the filename
                var stream = File.OpenRead(filename); // set the path string
                Console.WriteLine(filename); // print the filename to the console
                ulong imageHash = hashAlgorithm.Hash(stream); // hash the image from the file stream in a ulong variable

                hash_text_elements[i].Content = imageHash; // set the label value as the image hash
                image_elements[i].Source = new BitmapImage(new Uri(filename)); // set the source of the image to the filename
                imagescountreal = i; // set the number of images that are real
            }

            Console.WriteLine("Hashing completed.");
            statustext.Content = "Hashing completed. Waiting for user interaction to blur.."; // print text to the status label
            logger.Log("DIFFERENCE HASHING COMPLETED"); // log the hashing finished
            blurbutton.Visibility = Visibility.Visible; // make the blur button visible
            await Task.Delay(3000); // wait 5 seconds async
            blur(); // call blur function for image elements
        }

        // generate the perceptual hash for each image and render it to the canvas
        async void PerceptualHash()
        {
            Console.WriteLine("(PerceptualHash)"); // print the hashing method to the console
            statustext.Content = "Started hashing..";

            try
            {
                var hashAlgorithm = new PerceptualHash(); // create a perceptual hash object

                // refactored
                for (int i = 1; i < 13; i++) // iterate through every element
                {
                    string filename = @"C:\ProgramData\MugShotApp\images\" + i + ".png"; // generate the filename
                    var stream = File.OpenRead(filename); // set the path string
                    Console.WriteLine(filename); // print the filename
                    ulong imageHash = hashAlgorithm.Hash(stream); // hash the image from the file stream in a ulong variable 

                    hash_norm_text_elements[i - 1].Content = imageHash; // set the label value as the image hash
                    image_elements[i - 1].Source = new BitmapImage(new Uri(filename)); // set the source of the image to the filename
                    imagescountreal = i; // set the number of images that are real
                }

                statustext.Content = "Hashing completed. Waiting for user interaction to blur..";
                logger.Log("PERCEPTUAL HASHING COMPLETED"); // log into the log file
                blurbutton.Visibility = Visibility.Visible; // set the blur button to visible
                await Task.Delay(3000); // wait 5 seconds
                blur();
            }
            catch(Exception e)
            {
                // error message for no images detected
                Console.WriteLine("ERROR WHEN GENERATING PERCEPTUAL HASH: " + e.Message.ToString()); // print the error to the command line
                NoImagesDetected(); // show the no images detected window
            }
        }

        // blur defined images
        async void blur ()
        {
            try
            {
                logger.Log("STARTED BLURING"); // log
                blurbutton.Visibility = Visibility.Hidden; // hide the blur button
                statustext.Content = "Bluring " + numofimagesblur.ToString() + " images at random.."; // state how many images are being blurred
                logger.Log("Bluring " + numofimagesblur.ToString() + " images at random.."); // log how many images we are bluring
                BlurBitmapEffect myBlurEffect = new BlurBitmapEffect(); // new blurbitmapeffect object
                myBlurEffect.Radius = 4; // set the radius
                myBlurEffect.KernelType = KernelType.Box; // set the kernel type

                // refactored
                if (numofimagesblur < maxnumofimagestoblur) // if number of images to blur is less than max number of images to blur (within correct range)
                {
                    logger.Log("NUM OF IMAGES TO BLUR IS LESS THAN MAX, CONTINUE WITH DEFINED PREFERENCE");
                    Random rnd = new Random(); // new random object
                    for (int i = 1; i < numofimagesblur + 1; i++)
                    {
                        randomblurimages.Add(rnd.Next(numofimagesblur, numperscreen).ToString());  // add a new random number to the random blur images list, using the variables for number of images to blur and number per screen as the max value
                        Bitmap bitmap = new Bitmap(@"C:\ProgramData\MugShotApp\images\" + randomblurimages[i - 1] + ".png"); // create a new bitmap object and assign the random blur image to it
                        bitmap = Blur(bitmap, bluramount); // blur the image and return the value as the original bitmap object
                        string path = @"C:\ProgramData\MugShotApp\images\" + randomblurimages[i - 1] + "_blurred.png";
                        if (!File.Exists(path)) // to prevent errors with generic errors in GDI+
                        {
                            bitmap.Save(path); // save the bitmap blurred in the same directory as the normal images with a _blurred suffix at the end
                            // add the image path to the list
                            randomblurimages[i] = path;
                        }
                        image_elements[i - 1].Source = new BitmapImage(new Uri(path)); // set the image element source as the blurred image path
                    }
                }
                else
                {
                    // invalid
                    logger.Log("[ ! ] Invalid setting for number of images.");
                }

                statustext.Content = "The images have been blurred. Waiting for user interaction..";
                checkagain.Visibility = Visibility.Visible; // show the check again button
                await Task.Delay(3000);
                recheckhash();
            }
            catch (Exception e)
            {
                // error detected
                Console.WriteLine("ERROR DURING BLURING: "+ e.Message.ToString()); // print the error to the command line
            }
        }

        private static Bitmap Blur(Bitmap image, Int32 blurSize) // blur method, takes bitmap object and blursize
        {
            return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize); // return the blurred bitmap image object and call the main blur method
        }

        private unsafe static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize) // actually blurs the bitmap image, takes the formatted rectangle object as an argument
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height); // create new bitmap the same size of the passed in image object
            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred)) // get the windows graphics blurred library
            {
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel); // draw the blur rectangle over the image into a new bitmap object
            }
            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);
            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = System.Drawing.Image.GetPixelFormatSize(blurred.PixelFormat);
            // Get pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();
            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++) // rect transform for the rectangle
                {
                    // default vars
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
            // unlock the bits
            blurred.UnlockBits(blurredData);
            return blurred; // return the blurred BitmapData object
        }

        // recheck all of the image hashes
        async void recheckhash ()
        {
            // refactored
            checkagain.Visibility = Visibility.Hidden; // make the check button visible
            statustext.Content = "Re-Checking new hash values for the images.."; // set the status label text
            logger.Log("[ ! ] RE-HASHING ALL IMAGES");
            var hashAlgorithm = new PerceptualHash(); // new perceptual hash object

            // regenerate hashes for all images on canvas
            int lc = 0;
            foreach (System.Windows.Controls.Image f in image_elements)
            {
                string filename = f.Source.ToString().Replace("file:///", ""); // get the image path from the canvas
                Console.WriteLine(filename); // print the filename to the console
                var stream = File.OpenRead(filename); // create filestream object with filename argument
                ulong imageHash = hashAlgorithm.Hash(stream); // hash the stream image object and save it in a ulong variable
                hash_norm_text_elements[lc].Content = imageHash; // set the label value as the image hash
                lc = lc + 1;
            }

            // log for completion
            statustext.Content = "Re-Hashing completed.";
            logger.Log("RE-HASHING COMEPLETED"); // log to file
            calchashesbutton.Visibility = Visibility.Visible; // make the calulate hashes button visible
            await Task.Delay(3000); // wait 5 seconds async
            comparehashes(); // call compare hashes method
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // make the window draggable
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            logger.Log("[ ! ] APPLICATION SHUTDOWN..");
            this.Close();
            System.Windows.Application.Current.Shutdown(); // close button
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Images folder
            var win = new Images();
            win.Show(); // show the images window
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Settings
            var win = new Settings();
            win.Show(); // show the settings window
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            blur(); // blur button, call method
        }

        private void checkagain_Click(object sender, RoutedEventArgs e)
        {
            recheckhash(); // recheck hash button, call method
        }

        private void calchashesbutton_Click(object sender, RoutedEventArgs e)
        {
            comparehashes(); // compare hashes button, call method
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Directory.Delete(@"C:\ProgramData\MugShotApp"); // factory reset button, delete the directory
        }

        private void retest_Click(object sender, RoutedEventArgs e)
        {
            // restart application
            logger.Log("APPLICATION RESTARTED");
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void retest_Click_1(object sender, RoutedEventArgs e)
        {
            recheckhash(); // rechech hash button, call method
        }

        private void outputfilebutton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"C:\ProgramData\MugShotApp\output.txt")) // check if the output file exists
            {
                System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\output.txt"); // open the output file in the default text editor
            }
            else
            {
                File.Create(@"C:\ProgramData\MugShotApp\output.txt"); // create the file
                System.Diagnostics.Process.Start(@"C:\ProgramData\MugShotApp\output.txt"); // open the output file in the default text editor
            }
        }
    }
}