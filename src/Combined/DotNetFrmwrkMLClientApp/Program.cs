﻿using MLNetTestAppML.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetFrmwrkMLClientApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Start on .Net Framework app");

            TestImage(@"C:\git\eurovision\2euro\Images\Head\k1.png");
            TestImage(@"C:\git\eurovision\2euro\Images\Tail\m3.png");
            TestImage(@"C:\git\eurovision\2euro\Images\Tail\m4.png");
            TestImage(@"C:\git\eurovision\2euro\Images\Tail\m5.png");
            /////////////////////////////////////////////

            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }
        private static void TestImage(string fileName)
        {
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
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Using model to make single prediction '{fileName}' -- Comparing actual Label with predicted Label from sample data...");
            Console.WriteLine($"ImageSource: {sampleData.ImageSource}");
            Console.WriteLine($"Predicted Label value {predictionResult.Prediction} \nPredicted Label scores: [{String.Join(",", predictionResult.Score)}]");
            Console.WriteLine($"This is a {predictionResult.Prediction} with a reliabilty of {GetPercentage(predictionResult)}% (Time: {sw.ElapsedMilliseconds}ms)");
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
