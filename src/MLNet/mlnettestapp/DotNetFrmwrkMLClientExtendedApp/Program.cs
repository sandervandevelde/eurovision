using MLNetTestAppML.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TriggeredImagerNetFrwrkApp;

namespace DotNetFrmwrkMLClientApp
{
    internal class Program
    {
        private static Icam7000 icamCamera;

        private static void Main(string[] args)
        {
            Console.WriteLine("Start on .Net Framework app");

            Console.WriteLine("Hello Triggered Image Maker!");

            icamCamera = new Icam7000(@"c:\temp");

            icamCamera.ImageTaken += IcamCamera_ImageTaken;

            icamCamera.SetLightRingColor(LightRingColor.Green);

            Console.WriteLine("Ready for action!");

            do
            {
                Thread.Sleep(1000);
            }
            while (!Console.KeyAvailable);
        }

        private static void IcamCamera_ImageTaken(object sender, string imageName)
        {
            TestImage(imageName);
        }

        private static void TestImage(string fileName)
        {
            icamCamera.ToggleDigitalOut(0);

            var sw = new Stopwatch();
            sw.Start();
            // Create single instance of sample data from first line of dataset for model input
            ModelInput sampleData = new ModelInput()
            {
                ImageSource = fileName,
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            sw.Stop();
            Console.WriteLine($"Using model to make single prediction '{fileName}' -- Comparing actual Label with predicted Label from sample data...");
            Console.WriteLine($"ImageSource: {sampleData.ImageSource}");
            Console.WriteLine($"Predicted Label value {predictionResult.Prediction} \nPredicted Label scores: [{String.Join(",", predictionResult.Score)}]");
            Console.WriteLine($"This is a --{predictionResult.Prediction.ToUpper()}-- with a reliabilty of {GetPercentage(predictionResult)}% (Time: {sw.ElapsedMilliseconds}ms)");

            if (predictionResult.Prediction.ToLower() == "head")
            {
                icamCamera.ToggleDigitalOut(1);
                Console.WriteLine("Towerlight RED DO activated");
            }
            else
            {
                icamCamera.ToggleDigitalOut(2);
                Console.WriteLine("Towerlight Green DO activated");
            }
        }

        private static double GetPercentage(ModelOutput predictionResult)
        {
            if (predictionResult.Prediction.ToUpper() == "HEAD")
            {
                return predictionResult.Score[0] * 100;
            }

            if (predictionResult.Prediction.ToUpper() == "TAIL")
            {
                return predictionResult.Score[1] * 100;
            }

            return -1;
        }
    }
}
