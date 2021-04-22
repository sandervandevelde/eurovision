// This file was auto-generated by ML.NET Model Builder.

using System;
using System.Diagnostics;
using MLNetTestAppML.Model;

namespace MLNetTestAppML.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            // Create single instance of sample data from first line of dataset for model input
            ModelInput sampleData = new ModelInput()
            {
                ImageSource = @"C:\git\eurovision\2euro\Images\Head\k1.png",
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            sw.Stop();
            Console.WriteLine();
            Console.WriteLine("Using model to make single prediction -- Comparing actual Label with predicted Label from sample data...\n\n");
            Console.WriteLine($"ImageSource: {sampleData.ImageSource}");
            Console.WriteLine($"\n\nPredicted Label value {predictionResult.Prediction} \nPredicted Label scores: [{String.Join(",", predictionResult.Score)}]\n\n");
            Console.WriteLine($"This is a {predictionResult.Prediction} with a reliabilty of {GetPercentage(predictionResult)}% (Time: {sw.ElapsedMilliseconds}ms)");

            sw.Reset();
            sw.Start();
            // Create single instance of sample data from first line of dataset for model input
            sampleData = new ModelInput()
            {
                ImageSource = @"C:\git\eurovision\2euro\Images\Tail\m3.png",
            };

            // Make a single prediction on the sample data and print results
            predictionResult = ConsumeModel.Predict(sampleData);

            sw.Stop();

            Console.WriteLine();
            Console.WriteLine("Using model to make single prediction -- Comparing actual Label with predicted Label from sample data...\n\n");
            Console.WriteLine($"ImageSource: {sampleData.ImageSource}");
            Console.WriteLine($"\n\nPredicted Label value {predictionResult.Prediction} \nPredicted Label scores: [{String.Join(",", predictionResult.Score)}]\n\n");
            Console.WriteLine($"This is a {predictionResult.Prediction} with a reliabilty of {GetPercentage(predictionResult)}% (Time: {sw.ElapsedMilliseconds}ms)");

            sw.Reset();
            sw.Start();
            // Create single instance of sample data from first line of dataset for model input
            sampleData = new ModelInput()
            {
                ImageSource = @"C:\git\eurovision\2euro\Images\Tail\m4.png",
            };

            // Make a single prediction on the sample data and print results
            predictionResult = ConsumeModel.Predict(sampleData);

            sw.Stop();

            Console.WriteLine();
            Console.WriteLine("Using model to make single prediction -- Comparing actual Label with predicted Label from sample data...\n\n");
            Console.WriteLine($"ImageSource: {sampleData.ImageSource}");
            Console.WriteLine($"\n\nPredicted Label value {predictionResult.Prediction} \nPredicted Label scores: [{String.Join(",", predictionResult.Score)}]\n\n");
            Console.WriteLine($"This is a {predictionResult.Prediction} with a reliabilty of {GetPercentage(predictionResult)}% (Time: {sw.ElapsedMilliseconds}ms)");

            sw.Reset();
            sw.Start();
            // Create single instance of sample data from first line of dataset for model input
            sampleData = new ModelInput()
            {
                ImageSource = @"C:\git\eurovision\2euro\Images\Tail\m5.png",
            };

            // Make a single prediction on the sample data and print results
            predictionResult = ConsumeModel.Predict(sampleData);

            sw.Stop();

            Console.WriteLine();
            Console.WriteLine("Using model to make single prediction -- Comparing actual Label with predicted Label from sample data...\n\n");
            Console.WriteLine($"ImageSource: {sampleData.ImageSource}");
            Console.WriteLine($"\n\nPredicted Label value {predictionResult.Prediction} \nPredicted Label scores: [{String.Join(",", predictionResult.Score)}]\n\n");
            Console.WriteLine($"This is a {predictionResult.Prediction} with a reliabilty of {GetPercentage(predictionResult)}% (Time: {sw.ElapsedMilliseconds}ms)");



            /////////////////////////////////////////////

            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
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