using System;
using CamNaviCtrl;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace TriggeredImageTakerApp
{
    public class Icam7000
    {
        private NodeBool _counterDoRed = null;

        private NodeBool _LedGpo = null;

        private NodeEnum _LedSelector = null;

        private CameraManager _camManager;

        private Camera camera = null;

        private CameraConfigure camConfigure = null;

        private int iAcq_Counter = 0;

        private string today = string.Empty;

        public Icam7000()
        {
            today = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}";

            Directory.CreateDirectory(@"C:\temp\" + today);

            //Step1: Create a CameraManager object to manage all cameras

            _camManager = new CameraManager();

            //Step2: Get the number of cameras

            UInt32 iCameraNum = _camManager.GetNumCameras();
            if (iCameraNum == 0) { Console.WriteLine("ERROR: Invalid camera number !!"); return; }

            Console.WriteLine($"Cameras found: {iCameraNum}");

            //Step3: Get the camera id by using index.
            //       The index range is from [0 ~ iCameraNum]
            //       In this example, we defined iIndex to "0" for "first" camera

            string strCaneraID = _camManager.GetCameraID(0);
            if (strCaneraID.Length == 0) { Console.WriteLine("ERROR: Invalid camera ID !!"); return; }

            Console.WriteLine($"Camera selected: {strCaneraID}");

            //Step4: Get the Camera object by using camera id
            camera = _camManager.GetCamera(strCaneraID);
            if (camera == null) { Console.WriteLine("ERROR: Invalid ICamera !!"); return; }

            Console.WriteLine($"Camera connected");

            ConfigureLedRing(camera);

            SetLightRingColor(LightRingColor.Orange);

            //Step5: Register event listener for image ready event

            camera.EvtDiInterrupt += new EventHandler<EvtDiSnapData>(DIInterruptListener);

            Console.WriteLine($"DI event connected");

            // STEP 6-9

            var digitalInput = ConfigureDigitalInput(camera);

            // STEP 10-16

            ConfigureDigitalOutput(camera);

            ConfigureImageAcquisition(camera);

            // STEP 17

            ActivateDigitalInput(digitalInput);
        }

        private void ConfigureImageAcquisition(Camera camera)
        {
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
        }

        private void ActivateDigitalInput(DigitalInput digitalInput)
        {
            //Step17-1 DI Interrupt: Enable DI Intrrupt

            //NodeEnum diInterrupt = digitalInput.GetDiInterrupt();
            //if (diInterrupt == null) { Console.WriteLine("ERROR: Invalid DiInterrupt !!"); return; }

            //diInterrupt.Value = 1;

            //Step17-2 DI Interrupt: Enable DI Intrrupt Trigger Edge with "Both" edge

            // SVDV these lines start listening for DI trigger

            NodeEnum diIntTriggerEdge = digitalInput.GetDiIntTriggerEdge();
            if (diIntTriggerEdge == null) { Console.WriteLine("ERROR: Invalid DiIntTriggerEdge !!"); return; }
            diIntTriggerEdge.Value = 3; //1 = triggerstate=1 binnengaan flank // 2= verlaten flank //3= both

            //Step17-3 DI Interrupt: Configure DI invert

            ////NodeEnum diInvert = digitalInput.GetDiInvert();
            ////if (diInvert == null) { Console.WriteLine("ERROR: Invalid DiInvert !!"); return; }

            //Step17-4 DI Interrupt: Verify the DI interrupt function.
        }

        private void ConfigureDigitalOutput(Camera camera)
        {
            //Step10: Get the DO Counter object

            Counter counterDoRed = camera.GetCounter(2); // 2 = red  // 1 = Grey

            if (counterDoRed == null) { Console.WriteLine("ERROR: Invalid counter !!"); return; }

            Console.WriteLine($"DO counter 2 (gray) selected");

            //Step11: Configure Counter Mode

            NodeEnum cntMode = counterDoRed.GetMode();

            if (cntMode == null) { Console.WriteLine("ERROR: Invalid cntMode !!"); return; }

            cntMode.Value = 1;
            Int64 CntModeValue = cntMode.Value;

            //Step12: Configure Counter Invert

            NodeEnum cntInvert = counterDoRed.GetInvert();
            if (cntInvert == null) { Console.WriteLine("ERROR: Invalid cntInvert !!"); return; }

            cntInvert.Value = 0; //  1 = DO is standaard gesloten ; 0 = DO is standaard open
            Int64 CntInvertValue = cntInvert.Value;

            Console.WriteLine($"DO = (0) standard open");

            //Step13: Configure DO Trigger Edge

            NodeEnum cntTriggerEdge = counterDoRed.GetTriggerEdge();
            if (cntTriggerEdge == null) { Console.WriteLine("ERROR: Invalid cntTriggerEdge !!"); return; }

            cntTriggerEdge.Value = 1;
            Int64 CntTriggerEdgeValue = cntTriggerEdge.Value;

            //Step14: Configure DO Delay

            NodeInt cntDelay = counterDoRed.GetDelay();
            if (cntDelay == null) { Console.WriteLine("ERROR: Invalid cntDelay !!"); return; }

            cntDelay.Value = 1;
            Int64 DelayValue = cntDelay.Value;
            Int64 DelayMax = cntDelay.Max;
            Int64 DelayMin = cntDelay.Min;

            //Step15: Configure DO Pulse Width

            NodeInt cntPulseWidth = counterDoRed.GetPulseWidth();
            if (cntPulseWidth == null) { Console.WriteLine("ERROR: Invalid DoPulseWidth !!"); return; }

            cntPulseWidth.Value = 1;
            Int64 PulseWidthValue = cntPulseWidth.Value;
            Int64 PulseWidthMax = cntPulseWidth.Max;
            Int64 PulseWidthMin = cntPulseWidth.Min;

            //Step16: Configure DO Output

            _counterDoRed = counterDoRed.GetOutputValue();
            if (_counterDoRed == null) { Console.WriteLine("ERROR: Invalid DoOutput !!"); return; }

            bool CntOutputValue = _counterDoRed.Value;

            // standaard gesloten indien invert 1 is -> true = verbroken ; FALSE = do nothing
            // standaard verbroken indien invert 0 is => true = sluiten ; FALSE = do nothing

            _counterDoRed.SetValue(false);
        }

        private DigitalInput ConfigureDigitalInput(Camera camera)
        {
            Console.WriteLine($"DI event connected");

            //Step6: Get the DigitalInput object

            DigitalInput digitalInput = camera.GetDigitalInput(1);
            if (digitalInput == null) { Console.WriteLine("ERROR: Invalid digitalInput !!"); return null; }

            //Step7: Configure Trigger Edge for DI TOE

            NodeEnum diToeTriggerEdge = digitalInput.GetDiToeTriggerEdge();
            if (diToeTriggerEdge == null) { Console.WriteLine("ERROR: Invalid diToeTriggerEdge !!"); return null; }

            diToeTriggerEdge.Value = 1;
            Int64 DiToeTriggerEdgeValue = diToeTriggerEdge.Value;

            Console.WriteLine($"DI edge trigger = 1");

            //Step8: Configure DI Debouncer
            NodeInt diDebouncer = digitalInput.GetDiDebouncer();

            if (diDebouncer == null) { Console.WriteLine("ERROR: Invalid diDebouncer !!"); return null; }

            diDebouncer.Value = 1;
            Int64 DiDebouncerValue = diDebouncer.Value;
            Int64 DiDebouncerMax = diDebouncer.Max;
            Int64 DiDebouncerMin = diDebouncer.Min;

            Console.WriteLine($"DI debouncer = 1");

            //Step9: Configure DI Input

            NodeBool diIntput = digitalInput.GetDiInputValue();
            if (diIntput == null) { Console.WriteLine("ERROR: Invalid diIntput !!"); return null; }

            bool DiIntputValue = diIntput.Value;

            return digitalInput;
        }

        private void ConfigureLedRing(Camera camera)
        {
            //Step5: Get the CameraConfigure object for camera configuration
            camConfigure = camera.GetCameraConfigure();
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
            _LedSelector = camConfigure.GetEnumNode("LedSelector");
            if (_LedSelector == null)
            {
                Console.WriteLine("ERROR: Invalid LED Selector !!"); return;
            }

            // Mode 0: Continuous, 1: GPO, 2: Level Toggle, 3: Toggle
            NodeEnum pLEDMode = camConfigure.GetEnumNode("LedMode");

            // cnl_set_int_value(pLEDMode, 3) in toggle mode
            NodeBool pLEDToggle = camConfigure.GetBoolNode("LedToggleEnable");
            NodeInt pLEDTogglePeriod = camConfigure.GetIntNode("LedToggleRate");
            _LedGpo = camConfigure.GetBoolNode("LedGpo");

            Console.WriteLine("Access to LED objects");

            for (int i = 0; i < 3; i++)
            {
                // 0: Blue, 1: Green, 2: Orange
                _LedSelector.SetValue(i); // Select LED
                pLEDMode.SetValue(1); // Set LED to GPO mode
                _LedGpo.SetValue(false); // LED GPO off
                Thread.Sleep(500);
            }

            Console.WriteLine("All three Leds diabled");
        }

        public void SetLightRingColor(LightRingColor lightRingColor)
        {
            _LedGpo.SetValue(false); // put led on
            _LedSelector.SetValue((int)lightRingColor); // 2 = orange
            _LedGpo.SetValue(true); // put led on

            Console.WriteLine($"Ring light {lightRingColor} ({(int)lightRingColor}) activated");
        }

        private Bitmap CreateGrayBitmap(byte[] rawBytes, int width, int height)
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

        private void DIInterruptListener(object sender, EvtDiSnapData e)
        {
            Console.WriteLine("\nDI interrupt Processing:");

            //Step1: Get the DI snap state according to DI interrupt

            Console.WriteLine($"State = [{e.GetState(1)}]");

            //Step2: Get the DI snap trigger edge according to DI interrupt

            Console.WriteLine($"TriggerEdge = [{e.GetTriggerEdge(1)}]");
            Console.WriteLine("DI Interrupt is found");
            Console.WriteLine("Please press any key to quit !");

            if (_counterDoRed != null)
            {
                _counterDoRed.SetValue(Convert.ToBoolean(e.GetState(1)));
            }

            if (e.GetState(1) == 1)
            {
                AcquireImage();
            }
        }

        private void AcquireImage()
        {
            SetLightRingColor(LightRingColor.Blue);

            //Step9: Start Acquisition
            camera.StartAcq(1); //for 1 shot in this case; the continuous snapshot speed depends on Camera settings.

            // Step10: Do anything you are interesting while the device is working.
            //int iAcq_Counter = 0;

            //Step11: Get the image object with timeout (1000ms)
            EvtImgData evtimage = camera.GetImage(1000);
            if (evtimage == null)
            {
                Console.WriteLine("Null image or no more image. Press any key to leave this loop.\n");

                SetLightRingColor(LightRingColor.Orange);

                return;
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
                    try
                    {
                        string bmpfile = @"C:\temp\" + today + @"\" + Convert.ToString(iAcq_Counter) + ".bmp";

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

            //Step15: Stop the camera image acquisition function
            if (camConfigure.AcquisitionStop == null) { Console.WriteLine("ERROR: Invalid AcquisitionStop node !!"); }
            camConfigure.AcquisitionStop.Execute();

            //Step16: Stop Acquisition
            camera.StopAcq();

            Console.WriteLine("Image taken");
            SetLightRingColor(LightRingColor.Green);
        }
    }
}