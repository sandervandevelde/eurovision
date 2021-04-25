using System;
using System.Threading;

namespace TriggeredImageTakerApp
{
    internal partial class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello Triggered Image Maker!");

            var icamCamera = new Icam7000();

            icamCamera.SetLightRingColor(LightRingColor.Green);

            Console.WriteLine("Ready for action!");

            do
            {
                Thread.Sleep(1000);
                //  diInvert.Value = 0;
                //   Thread.Sleep(1000);
                //diInvert.Value = 1;
            }
            while (!Console.KeyAvailable);
        }
    }
}