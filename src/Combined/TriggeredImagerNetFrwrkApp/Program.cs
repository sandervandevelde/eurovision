using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TriggeredImagerNetFrwrkApp
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

            }
            while (!Console.KeyAvailable);
        }
    }
}
