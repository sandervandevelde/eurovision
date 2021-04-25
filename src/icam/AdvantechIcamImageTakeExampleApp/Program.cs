using System;
using System.Threading;
using CamNaviCtrl;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace AdvantechExampleApp
{
    class Program
    {
        public static Bitmap CreateGrayBitmap(byte[] rawBytes, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            Console.WriteLine("rawBytes.Length = 0x{0:X} or {1}\n", rawBytes.Length, rawBytes.Length);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                                            ImageLockMode.WriteOnly, bitmap.PixelFormat);

            Marshal.Copy(rawBytes, 0, bitmapData.Scan0, (width * height));
            bitmap.UnlockBits(bitmapData);

            //create a 256B grayscale palette.
            var pal = bitmap.Palette;
            for (int i = 0; i < 256; i++) pal.Entries[i] = Color.FromArgb(i, i, i);
            bitmap.Palette = pal;

            return bitmap;
        }

        static void Main(string[] args)
        {
            //Step1: Create a CameraManager object to manage all cameras
            CameraManager camManager = new CameraManager();

            //Step2: Get the number of cameras
            UInt32 iCameraNum = camManager.GetNumCameras();
            if (iCameraNum == 0) 
            { 
                Console.WriteLine("ERROR: Invalid camera number !!"); return; 
            }
            else
            { 
                Console.WriteLine("iCameraNum = {0} \n", iCameraNum); 
            }

            Console.WriteLine($"Cameras found: {iCameraNum}");

            //Step3: Get the camera id by using index.  
            //       The index range is from [0 ~ iCameraNum]
            //       In this example, we defined iIndex to "0" for "first" camera
            string strCaneraID = camManager.GetCameraID(0);
            if (strCaneraID.Length == 0) 
            {
                Console.WriteLine("ERROR: Invalid camera ID !!"); return;
            }
            else
            { 
                Console.WriteLine("strCaneraID = {0} \n", strCaneraID);
            }

            Console.WriteLine($"Camera selected: {strCaneraID}");

            //Step4: Get the Camera object by using camera id
            Camera camera = camManager.GetCamera(strCaneraID);
            if (camera == null)
            {
                Console.WriteLine("ERROR: Invalid ICamera !!"); return;
            }

            Console.WriteLine($"Camera connected");

            //Step5: Get the CameraConfigure object for camera configuration
            CameraConfigure camConfigure = camera.GetCameraConfigure();
            if (camConfigure == null)
            {
                Console.WriteLine("ERROR: Invalid ICamConfigure !!"); return; 
            }
            else
            {
                Console.WriteLine("camConfigure.Height and Width = {0} {1} \n", camConfigure.Height.Value, camConfigure.Width.Value); 
            }

            Console.WriteLine("Camera configuration available");

            //STEP 5.1: From Camera configuration: Leds
            NodeEnum pLEDSelector = camConfigure.GetEnumNode("LedSelector");
            if (pLEDSelector == null)
            {
                Console.WriteLine("ERROR: Invalid LED Selector !!"); return; 
            }

            // Mode 0: Continuous, 1: GPO, 2: Level Toggle, 3: Toggle
            NodeEnum pLEDMode = camConfigure.GetEnumNode("LedMode");

            // cnl_set_int_value(pLEDMode, 3) in toggle mode
            NodeBool pLEDToggle = camConfigure.GetBoolNode("LedToggleEnable");
            NodeInt pLEDTogglePeriod = camConfigure.GetIntNode("LedToggleRate");
            NodeBool pLEDGpo = camConfigure.GetBoolNode("LedGpo");

            Console.WriteLine("Access to LED objects");

            for (int i = 0; i < 3; i++)
            {
                // 0: Blue, 1: Green, 2: Orange
                pLEDSelector.SetValue(i); // Select LED
                pLEDMode.SetValue(1); // Set LED to GPO mode
                pLEDGpo.SetValue(false); // LED GPO off
            }

            Console.WriteLine("All three Leds diable");

            pLEDSelector.SetValue(2); // orange
            pLEDGpo.SetValue(true); // put led on

            Console.WriteLine("Orange LED (2) activated");

            //Step6: The TLParamsLocked should be configured as "1" ("Unlock") for starting camera
            if (camConfigure.TLParamsLocked == null) { Console.WriteLine("ERROR: Invalid TLParamsLocked node !!"); return; }
            camConfigure.TLParamsLocked.Value = 1;

            //Step7: Define the AcquisitionMode as "2" ("Continues Acquisition Mode")
            if (camConfigure.AcquisitionMode == null) { Console.WriteLine("ERROR: Invalid AcquisitionMode node !!"); return; }
            camConfigure.AcquisitionMode.Value = 2;

            Console.WriteLine("camConfigure.GetPixelFormat = 0x{0:X} \n", camConfigure.GetPixelFormat().Value);
            //Console.WriteLine("camConfigure.GetPayloadSize = 0x{0:X} \n", camConfigure.GetPayloadSize().Value);

            //Step8: Start the camera image acquisition function
            if (camConfigure.AcquisitionStart == null) { Console.WriteLine("ERROR: Invalid AcquisitionStart node !!"); return; }
            camConfigure.AcquisitionStart.Execute();

            int iAcq_Counter = 0;

            while (true)
            { 
                //Step9: Start Acquisition
                camera.StartAcq(1); //for 1 shot in this case; the continuous snapshot speed depends on Camera settings.

                // Step10: Do anything you are interesting while the device is working.
                //int iAcq_Counter = 0;

                //Step11: Get the image object with timeout (1000ms)
                EvtImgData evtimage = camera.GetImage(1000);
                if (evtimage == null)
                {
                    Console.WriteLine("Null image or no more image. Press any key to leave this loop.\n");
                    Thread.Sleep(500);
                    continue;
                }
                else
                {
                    iAcq_Counter++;
                    Console.WriteLine("Got a new Image Data {0}\n", iAcq_Counter);
                }

                //Step12: Get the image buffer size. (not the real image size)
                //Console.WriteLine("buffer size = 0x{0:X} \n", evtimage.ImageSize);

                //Step13: Get the image buffer base address
                byte[] byBuffer = evtimage.ImageBase;

                //avoid handing an empty bufer
                if (byBuffer != null && byBuffer.Length > 0)
                {
                    int bmpWidth = (int)camConfigure.Width.Value;
                    int bmpHeight = (int)camConfigure.Height.Value;
                    Bitmap bitmap = null;

                    //Get and check image format and call proper convert functions.
                    long pixel_format = camConfigure.GetPixelFormat().Value;

                    //MONO8 - one of iCam-7000 model
                    if (pixel_format == (long)GEV_PIEXL_FORMAT.MONO8)
                    {
                        bitmap = CreateGrayBitmap(byBuffer, bmpWidth, bmpHeight);

                        Console.WriteLine("The Camera's Pixel format: {0} 0x{1:X}\n", bitmap.PixelFormat.ToString(), pixel_format);
                    }
                    else
                    {
                        //RGB8
                        //BAYER_GR_8
                        //BAYER_RG_8
                        //BAYER_GB_8
                        //BAYER_BG_8

                        //Step14: Display first 3 image data
                        //Console.WriteLine("Output image header bytes \n");
                        //Console.WriteLine(string.Format("DATA[{0:X}]", byBuffer[0]));
                        //Console.WriteLine(string.Format("DATA[{0:X}]", byBuffer[1]));
                        //Console.WriteLine(string.Format("DATA[{0:X}]", byBuffer[2]));
                        Console.WriteLine("Pixel format Not supported yet: 0x{0:X} \n", pixel_format);
                    }

                    if (bitmap != null)
                    {
                        var today = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}";

                        Directory.CreateDirectory(@"C:\temp\" + today);


                        try
                        {
                            string bmpfile = @"C:\temp\"+ today + @"\"  + Convert.ToString(iAcq_Counter) + ".bmp";

                            bitmap.Save(bmpfile, System.Drawing.Imaging.ImageFormat.Bmp);
                            //bitmap.Save(@"D:\a.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                            //bitmap.Save(@"D:\a.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            //bitmap.Save(@"D:\a.bmp", System.Drawing.Imaging.ImageFormat.Png);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("There was a problem saving the file.\n");
                        }
                    }
                }
                //Step14: Release image buffer
                evtimage.ReleaseImage();

                //            } while (!Console.KeyAvailable);



                //Step15: Stop the camera image acquisition function
                if (camConfigure.AcquisitionStop == null) { Console.WriteLine("ERROR: Invalid AcquisitionStop node !!"); }
                camConfigure.AcquisitionStop.Execute();

                //Step16: Stop Acquisition
                camera.StopAcq();


                Console.WriteLine("Image taken");

                Thread.Sleep(500);
            }
        }
    }
}
