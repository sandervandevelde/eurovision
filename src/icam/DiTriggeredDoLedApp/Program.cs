using System;
using System.Threading;
using CamNaviCtrl;

namespace Example6App
{
    /// <summary>
    /// Activate DO LIGHT on DI trigger
    /// </summary>
    internal class Program
    {
        private static Counter _counter = null;

        private static NodeBool _cntOutput = null;

        private static void Main(string[] args)
        {
            //Step1: Create a CameraManager object to manage all cameras

            CameraManager camManager = new CameraManager();

            //Step2: Get the number of cameras

            UInt32 iCameraNum = camManager.GetNumCameras();
            if (iCameraNum == 0) { Console.WriteLine("ERROR: Invalid camera number !!"); return; }

            Console.WriteLine($"Cameras found: {iCameraNum}");

            //Step3: Get the camera id by using index.
            //       The index range is from [0 ~ iCameraNum]
            //       In this example, we defined iIndex to "0" for "first" camera

            string strCaneraID = camManager.GetCameraID(0);
            if (strCaneraID.Length == 0) { Console.WriteLine("ERROR: Invalid camera ID !!"); return; }

            Console.WriteLine($"Camera selected: {strCaneraID}");

            //Step4: Get the Camera object by using camera id
            Camera camera = camManager.GetCamera(strCaneraID);
            if (camera == null) { Console.WriteLine("ERROR: Invalid ICamera !!"); return; }

            Console.WriteLine($"Camera connected");

            //Step5: Register event listener for image ready event

            camera.EvtDiInterrupt += new EventHandler<EvtDiSnapData>(DIInterruptListener);

            Console.WriteLine($"DI event connected");

            //Step6: Get the DigitalInput object

            DigitalInput digitalInput = camera.GetDigitalInput(1);
            if (digitalInput == null) { Console.WriteLine("ERROR: Invalid digitalInput !!"); return; }

            //Step7: Configure Trigger Edge for DI TOE

            NodeEnum diToeTriggerEdge = digitalInput.GetDiToeTriggerEdge();
            if (diToeTriggerEdge == null) { Console.WriteLine("ERROR: Invalid diToeTriggerEdge !!"); return; }

            diToeTriggerEdge.Value = 1;
            Int64 DiToeTriggerEdgeValue = diToeTriggerEdge.Value;

            Console.WriteLine($"DI edge trigger = 1");

            //Step8: Configure DI Debouncer
            NodeInt diDebouncer = digitalInput.GetDiDebouncer();

            if (diDebouncer == null) { Console.WriteLine("ERROR: Invalid diDebouncer !!"); return; }

            diDebouncer.Value = 1;
            Int64 DiDebouncerValue = diDebouncer.Value;
            Int64 DiDebouncerMax = diDebouncer.Max;
            Int64 DiDebouncerMin = diDebouncer.Min;

            Console.WriteLine($"DI debouncer = 1");

            //Step9: Configure DI Input

            NodeBool diIntput = digitalInput.GetDiInputValue();
            if (diIntput == null) { Console.WriteLine("ERROR: Invalid diIntput !!"); return; }

            bool DiIntputValue = diIntput.Value;

            //Step10: Get the DO Counter object

            _counter = camera.GetCounter(2); // 2 = rood  // 1 = Grijs

            if (_counter == null) { Console.WriteLine("ERROR: Invalid counter !!"); return; }

            Console.WriteLine($"DO counter 2 (gray) selected");

            //Step11: Configure Counter Mode

            NodeEnum cntMode = _counter.GetMode();

            if (cntMode == null) { Console.WriteLine("ERROR: Invalid cntMode !!"); return; }

            cntMode.Value = 1;
            Int64 CntModeValue = cntMode.Value;

            //Step12: Configure Counter Invert

            NodeEnum cntInvert = _counter.GetInvert();
            if (cntInvert == null) { Console.WriteLine("ERROR: Invalid cntInvert !!"); return; }

            cntInvert.Value = 0; //  1 = DO is standaard gesloten ; 0 = DO is standaard open
            Int64 CntInvertValue = cntInvert.Value;

            Console.WriteLine($"DO = (0) standard open");

            //Step13: Configure DO Trigger Edge

            NodeEnum cntTriggerEdge = _counter.GetTriggerEdge();
            if (cntTriggerEdge == null) { Console.WriteLine("ERROR: Invalid cntTriggerEdge !!"); return; }

            cntTriggerEdge.Value = 1;
            Int64 CntTriggerEdgeValue = cntTriggerEdge.Value;

            //Step14: Configure DO Delay

            NodeInt cntDelay = _counter.GetDelay();
            if (cntDelay == null) { Console.WriteLine("ERROR: Invalid cntDelay !!"); return; }

            cntDelay.Value = 1;
            Int64 DelayValue = cntDelay.Value;
            Int64 DelayMax = cntDelay.Max;
            Int64 DelayMin = cntDelay.Min;

            //Step15: Configure DO Pulse Width

            NodeInt cntPulseWidth = _counter.GetPulseWidth();
            if (cntPulseWidth == null) { Console.WriteLine("ERROR: Invalid DoPulseWidth !!"); return; }

            cntPulseWidth.Value = 1;
            Int64 PulseWidthValue = cntPulseWidth.Value;
            Int64 PulseWidthMax = cntPulseWidth.Max;
            Int64 PulseWidthMin = cntPulseWidth.Min;

            //Step16: Configure DO Output

            _cntOutput = _counter.GetOutputValue();
            if (_cntOutput == null) { Console.WriteLine("ERROR: Invalid DoOutput !!"); return; }

            bool CntOutputValue = _cntOutput.Value;

            // standaard gesloten indien invert 1 is -> true = verbroken ; FALSE = do nothing
            // standaard verbroken indien invert 0 is => true = sluiten ; FALSE = do nothing

            _cntOutput.SetValue(false);

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

            do
            {
                Thread.Sleep(1000);
                //  diInvert.Value = 0;
                //   Thread.Sleep(1000);
                //diInvert.Value = 1;
            }
            while (!Console.KeyAvailable);
        }

        private static void DIInterruptListener(object sender, EvtDiSnapData e)
        {
            Console.WriteLine("\n\nMy DI interrupt Process !!");

            //Step1: Get the DI snap state according to DI interrupt

            Console.WriteLine("State = [{0}]", e.GetState(1));

            //Step2: Get the DI snap trigger edge according to DI interrupt

            Console.WriteLine("TriggerEdge = [{0}]", e.GetTriggerEdge(1));
            Console.WriteLine("DI Interrupt is found");
            Console.WriteLine("Please press any key to quit !");

            if (_cntOutput != null)
            {
                _cntOutput.SetValue(Convert.ToBoolean(e.GetState(1)));
            }
        }
    }
}