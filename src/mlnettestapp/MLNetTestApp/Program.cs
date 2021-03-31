using Microsoft.ML;
using Microsoft.ML.Data;
using MLNetTestAppML.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLNetTestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //       MemoryStream imageMemoryStream = new MemoryStream();

            //      await imageFile.CopyToAsync(imageMemoryStream);

            var i = Image.FromFile(@"C:\git\eurovision\2euro\Images\Head\k3.png");

            //Convert to Bitmap
            Bitmap bitmapImage = new Bitmap(i); //.FromStream(imageMemoryStream);

            //Set the specific image data into the ImageInputData type used in the DataView
            ImageInputData imageInputData = new ImageInputData { Image = bitmapImage };

            // Create new MLContext
            MLContext mlContext = new MLContext();

            // Load model & create prediction engine
            // error: legacy?
            //ITransformer mlModel = mlContext.Model.Load(@"C:\git\eurovision\2euro\Tensorflow\62872bbe6a6348228956b9fcc5ba8208.TensorFlow.zip", out var modelInputSchema);

            // error: legacy?
            //ITransformer mlModel = mlContext.Model.Load(@"C:\git\eurovision\2euro\ONNXWindowsML\62872bbe6a6348228956b9fcc5ba8208.ONNX.zip", out var modelInputSchema);

            // error: legacy?
            //ITransformer mlModel = mlContext.Model.Load(@"C:\git\eurovision\2euro\Tensorflow\62872bbe6a6348228956b9fcc5ba8208.TensorFlow.zip", out var modelInputSchema);

            // error: legacy?
            //ITransformer mlModel = mlContext.Model.Load(@"C:\git\eurovision\2euro\Tensorflow\62872bbe6a6348228956b9fcc5ba8208.TensorFlow.TensorFlowLiteFloat16.zip", out var modelInputSchema);

            // error: legacy?
            //ITransformer mlModel = mlContext.Model.Load(@"C:\git\eurovision\2euro\Tensorflow\62872bbe6a6348228956b9fcc5ba8208.TensorFlow.TensorFlowLite.zip", out var modelInputSchema);

            //      org      var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
            var predEngine = mlContext.Model.CreatePredictionEngine<ImageInputData, ModelOutput>(mlModel);

            var mo = new ModelOutput();

            predEngine.Predict(imageInputData, ref mo);
        }
    }

    public struct ImageSettings
    {
        public const int imageHeight = 227;
        public const int imageWidth = 227;
        public const float mean = 117;         //offsetImage
        public const bool channelsLast = true; //interleavePixelColors
    }

    // For checking tensor names, you can open the TF model .pb file with tools like Netron: https://github.com/lutzroeder/netron
    public struct TensorFlowModelSettings
    {
        // taken from C:\git\eurovision\2euro\Tensorflow\62872bbe6a6348228956b9fcc5ba8208.TensorFlow.TensorFlowLite

        // input tensor name
        public const string inputTensorName = "Placeholder";

        // output tensor name
        public const string outputTensorName = "model_outputs";
    }

    public class ImageInputData
    {
        //      [ImageType(227, 227)]
        public Bitmap Image { get; set; }
    }

    public class ModelOutput
    {
        // ColumnName attribute is used to change the column name from
        // its default value, which is the name of the field.
        [ColumnName("PredictedLabel")]
        public String Prediction { get; set; }

        public float[] Score { get; set; }
    }
}