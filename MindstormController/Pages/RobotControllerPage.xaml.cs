using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Models;
using MonoBrick.EV3;
using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using Algorithms;
using Algorithms.Audio;
using MindstormController.Helpers;
using MoreLinq;

namespace MindstormController.Pages
{
    /// <summary>
    /// Interaction logic for RobotControllerPage.xaml
    /// </summary>
    public partial class RobotControllerPage : Page
    {
        private BackgroundWorker bw1;
        private BackgroundWorker bw2;
        private Context ctx;
        private MindstormConfiguration conf;
        private Brick<Sensor, Sensor, Sensor, Sensor> ev3;
        private KinectSensor sensor;
        private SpeechRecognizer speechRecognizer;
        private Gesture gesture;
        private GestureRecognizer gestureRecognizer;
        private bool enableRecord;
        private MotorState leftArm, rightArm, leftFoot, rightFoot;


        /// <summary>
        /// Constructor
        /// </summary>
        public RobotControllerPage()
        {
            InitializeComponent();

            Thread.Sleep(10000);
            leftArm = rightArm = leftFoot = rightFoot = MotorState.Down;
            ctx = new Context();
            ctx.Gestures.ToList();
            ctx.Configurations.ToList();
            conf = ctx.MindstormConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);
            string connection = (conf.connection == ConnectionType.USB) ? "usb" : "wifi";
            ev3 = new Brick<Sensor, Sensor, Sensor, Sensor>(connection);
            speechRecognizer = new SpeechRecognizer(SpeechRecognized, HomeWindow.language);

            if (conf.recognition == RecognitionType.Visual)
            {
                enableRecord = false;

                this.Loaded += (s, e) => { NavigationService.Navigating += NavigationService_Navigating; };

                try
                {
                    sensor = KinectSensor.KinectSensors.FirstOrDefault(k => k.Status == KinectStatus.Connected);

                    if (sensor == null)
                    {
                        ErrorMsg.Text = "Error: No se ha detectado ningún Kinect en el equipo";
                        ErrorMsg.Visibility = System.Windows.Visibility.Visible;

                        return;
                    }
                }
                catch
                {
                    ErrorMsg.Text = "Error: No se ha detectado ningún Kinect en el equipo";
                    ErrorMsg.Visibility = System.Windows.Visibility.Visible;

                    return;
                }

                ctx = new Context();
                ConfigurationMovement movConf = ctx.Configurations.FirstOrDefault(c => c.user.name == HomeWindow.username).movementConfiguration;
                EnergyBar.Maximum = movConf.ET_RestingToStartMoving * 10;

                bw1 = new BackgroundWorker();
                bw1.DoWork += new DoWorkEventHandler(startGestureRecognizer);
                bw1.RunWorkerAsync(conf.profile.ID);

                gesture = new Gesture(HomeWindow.username, GestureType.Evaluation);
                gesture.EvaluationRecord();
                enableRecord = true;

                sensor.SkeletonFrameReady += sensor_SkeletonFrameReadyRecord;
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                sensor.SkeletonStream.Enable();
                sensor.Start();
            }
            else if (conf.recognition == RecognitionType.Speech)
            {
                painter.Visibility = Visibility.Hidden;
                EnergyBar.Visibility = Visibility.Hidden;
                speechRecognizer.start();
            }
        }

        private void sensor_SkeletonFrameReadyRecord(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                if (enableRecord == true)
                {
                    gesture.Record(frame);

                    if (gesture.state == State.Finish)
                    {
                        enableRecord = false;
                        bw1 = new BackgroundWorker();
                        bw1.DoWork += new DoWorkEventHandler(finishGesture);
                        bw1.RunWorkerAsync();
                    }
                }

                EnergyBar.Value = (gesture.energy > EnergyBar.Maximum) ? EnergyBar.Maximum : gesture.energy;

                Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                frame.CopySkeletonDataTo(skeletons);
                painter.Draw(skeletons);
            }
        }

        private void finishGesture(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Msg.Visibility = Visibility.Visible;
                Msg.Text = "DETECTANDO GESTO...";
            }));

            GestureModel model;
            Gesture detected;

            if (gestureRecognizer != null)
            {
                detected = gestureRecognizer.Recognize(gesture);
                model = ctx.Gestures.Find(detected.ID);
            }
            else
            {
                detected = null;
                model = null;
            }

            gesture.Delete();

            if (detected != null)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Msg.Visibility = Visibility.Visible;
                    Msg.Text = "GESTO DETECTADO: " + detected.gesturename;
                }));

                if ((conf.rightArmMove != null) && (model.ID == conf.rightArmMove.ID) )
                {
                    MoveDual("MUEVE MANO DERECHA", "C", 50);
                }

                if ((conf.rightArmUp != null) && (model.ID == conf.rightArmUp.ID) )
                {
                    Move("MANO DERECHA ARRIBA", "C", -50);
                }

                if ((conf.rightArmDown != null) && (model.ID == conf.rightArmDown.ID))
                {
                    Move("MANO DERECHA ABAJO", "C", 50);
                }

                if ((conf.leftArmMove != null) && (model.ID == conf.leftArmMove.ID))
                {
                    MoveDual("MUEVE MANO IZQUIERDA", "A", 50);     
                }

                if ((conf.leftArmUp != null) && (model.ID == conf.leftArmUp.ID))
                {
                    Move("MANO IZQUIERDA ARRIBA", "A", -50);
                }

                if ((conf.leftArmDown != null) && (model.ID == conf.leftArmDown.ID))
                {
                    Move("MANO IZQUIERDA ABAJO", "A", 50);
                }

                if ((conf.bothArmsMove != null) && (model.ID == conf.bothArmsMove.ID))
                {
                    BothArmsMove();
                }

                if ((conf.bothArmsUp != null) && (model.ID == conf.bothArmsUp.ID))
                {
                    BothArmsUp();
                }

                if ((conf.bothArmsDown != null) && (model.ID == conf.bothArmsDown.ID))
                {
                    BothArmsDown();
                }

                if ((conf.rightFootMove != null) && (model.ID == conf.rightFootMove.ID))
                {
                    MoveDual("MUEVE PIE DERECHO", "D", 50);
                }

                if ((conf.rightFootForward != null) && (model.ID == conf.rightFootForward.ID))
                {
                    Move("PIE DERECHO ADELANTE", "D", -50);
                }

                if ((conf.rightFootBackward != null) && (model.ID == conf.rightFootBackward.ID))
                {
                    Move("PIE DERECHO ATRÁS", "D", 50);
                }

                if ((conf.leftFootMove != null) && (model.ID == conf.leftFootMove.ID))
                {
                    MoveDual("MUEVE PIE IZQUIERDO", "B", 50);
                }

                if ((conf.leftFootForward != null) && (model.ID == conf.leftFootForward.ID))
                {
                    Move("PIE IZQUIERDO ADELANTE", "B", -50);
                }

                if ((conf.leftFootBackward != null) && (model.ID == conf.leftFootBackward.ID))
                {
                    Move("PIE IZQUIERDO ATRÁS", "B", 50);
                }

                if ((conf.bothFeetMove != null) && (model.ID == conf.bothFeetMove.ID))
                {
                    BothFeetMove();
                }

                if ((conf.bothFeetForward != null) && (model.ID == conf.bothFeetForward.ID))
                {
                    BothFeetForward();
                }

                if ((conf.bothFeetBackward != null) && (model.ID == conf.bothFeetBackward.ID))
                {
                    BothFeetBackward();
                }

                Thread.Sleep(2000);
            }

            this.Dispatcher.Invoke((Action)(() =>
            {
                Msg.Visibility = Visibility.Hidden;
                gesture = new Gesture(HomeWindow.username, GestureType.Evaluation);
                gesture.EvaluationRecord();
                enableRecord = true;
            }));
        }

        // <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "GO FORWARD":
                        BothFeetForward();
                        break;

                    case "GO BACKWARD":
                        BothFeetBackward();
                        break;

                    case "TURN LEFT":
                        Move("GIRAR IZQUIERDA", "B", -50);
                        break;

                    case "TURN RIGHT":
                        Move("GIRAR DERECHA", "D", -50);
                        break;

                    case "RIGHT ARM UP":
                        Move("MANO DERECHA ARRIBA", "C", -50);
                        break;

                    case "RIGHT ARM DOWN":
                        Move("MANO DERECHA ABAJO", "C", 50);
                        break;

                    case "LEFT ARM UP":
                        Move("MANO IZQUIERDA ARRIBA", "A", -50);
                        break;

                    case "LEFT ARM DOWN":
                        Move("MANO IZQUIERDA ABAJO", "A", 50);
                        break;

                    case "BOTH ARMS UP":
                        BothArmsUp();
                        break;

                    case "BOTH ARMS DOWN":
                        BothArmsDown();
                        break;
                }
            }
        }

        // Move arm/foot
        private void Move(string name, string motor, sbyte speed)
        {
            WriteText(name);
            var command = Tuple.Create<string, sbyte>(motor, speed);
            bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(SendCommand);
            bw2.RunWorkerAsync(command);
        }

        private void MoveDual(string name, string motor, sbyte speed)
        {
            WriteText(name);
            var command = Tuple.Create<string, sbyte>(motor, speed);
            bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(SendMoveCommand);
            bw2.RunWorkerAsync(command);
        }

        // Both Arms Move
        private void BothArmsMove()
        {
            WriteText("MUEVE AMBAS MANOS");
            bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(BothArmsMoveCommand);
            bw2.RunWorkerAsync();
        }

        private void BothArmsMoveCommand(object sender, DoWorkEventArgs e)
        {
            try
            {
                ev3.Connection.Open();

                sbyte speed = 50;

                if (leftArm == MotorState.Down)
                    speed = -50;
                else
                    speed = 50;
                
                ev3.MotorSync.BitField = OutputBitfield.OutA | OutputBitfield.OutC;
                ev3.MotorSync.On(speed, 0);
                Thread.Sleep(1000);
                ev3.MotorSync.Off();

                if (leftArm == MotorState.Down)
                    leftArm = rightArm = MotorState.Up;
                else
                    leftArm = rightArm = MotorState.Down;
            }
            catch
            {
            }
            finally
            {
                ev3.Connection.Close();
            }
        }

        // Both Arms Up
        private void BothArmsUp()
        {
            WriteText("AMBAS MANOS ARRIBA");
            bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(BothArmsUpCommand);
            bw2.RunWorkerAsync();
        }

        private void BothArmsUpCommand(object sender, DoWorkEventArgs e)
        {
            try
            {
                ev3.Connection.Open();
                ev3.MotorSync.BitField = OutputBitfield.OutA | OutputBitfield.OutC;
                ev3.MotorSync.On(-50, 0);
                Thread.Sleep(1000);
                ev3.MotorSync.Off();
            }
            catch
            {
            }
            finally
            {
                ev3.Connection.Close();
            }
        }

        private void BothArmsDown()
        {
            WriteText("AMBAS MANOS ABAJO");
            bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(BothArmsDownCommand);
            bw2.RunWorkerAsync();
        }

        private void BothArmsDownCommand(object sender, DoWorkEventArgs e)
        {
            try
            {
                ev3.Connection.Open();
                ev3.MotorSync.BitField = OutputBitfield.OutA | OutputBitfield.OutC;
                ev3.MotorSync.On(50, 0);
                Thread.Sleep(1000);
                ev3.MotorSync.Off();
            }
            catch
            {
            }
            finally
            {
                ev3.Connection.Close();
            }
        }

        // Both Feet
        private void BothFeetMove()
        {
            WriteText("MUEVE AMBOS PIES");
            bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(BothFeetMoveCommand);
            bw2.RunWorkerAsync();
        }

        private void BothFeetMoveCommand(object sender, DoWorkEventArgs e)
        {
            try
            {
                ev3.Connection.Open();

                sbyte speed = 50;

                if (leftFoot == MotorState.Down)
                    speed = 50;
                else
                    speed = -50;
                
                ev3.MotorSync.BitField = OutputBitfield.OutB | OutputBitfield.OutD;
                ev3.MotorSync.On(speed, 0);
                Thread.Sleep(1000);
                ev3.MotorSync.Off();

                if (leftFoot == MotorState.Down)
                    leftFoot = rightFoot = MotorState.Up;
                else
                    leftFoot = rightFoot = MotorState.Down;
            }
            catch
            {
            }
            finally
            {
                ev3.Connection.Close();
            }
        }

        private void BothFeetForward()
        {
            WriteText("ADELANTE");
            bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(BothFeetForwardCommand);
            bw2.RunWorkerAsync();
        }

        private void BothFeetForwardCommand(object sender, DoWorkEventArgs e)
        {
            try
            {
                ev3.Connection.Open();
                ev3.MotorSync.BitField = OutputBitfield.OutB | OutputBitfield.OutD;
                ev3.MotorSync.On(50, 0);
                Thread.Sleep(1000);
                ev3.MotorSync.Off();
            }
            catch
            {
            }
            finally
            {
                ev3.Connection.Close();
            }
        }

        private void BothFeetBackward()
        {
            WriteText("ATRÁS");
            bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(BothFeetBackwardCommand);
            bw2.RunWorkerAsync();
        }

        private void BothFeetBackwardCommand(object sender, DoWorkEventArgs e)
        {
            try
            {
                ev3.Connection.Open();
                ev3.MotorSync.BitField = OutputBitfield.OutB | OutputBitfield.OutD;
                ev3.MotorSync.On(-50, 0);
                Thread.Sleep(1000);
                ev3.MotorSync.Off();
            }
            catch
            {
            }
            finally
            {
                ev3.Connection.Close();
            }
        }

        private void SendCommand(object sender, DoWorkEventArgs e)
        {
            Tuple<string, sbyte> argument = e.Argument as Tuple<string, sbyte>;
            string motor = argument.Item1;
            sbyte speed = argument.Item2;

            try
            {
                ev3.Connection.Open();

                switch (motor)
                {
                    case "A":
                        ev3.MotorA.On(speed);
                        Thread.Sleep(1800);
                        ev3.MotorA.Off();
                        break;

                    case "B":
                        ev3.MotorB.On(speed);
                        Thread.Sleep(1000);
                        ev3.MotorB.Off();
                        break;

                    case "C":
                        ev3.MotorC.On(speed);
                        Thread.Sleep(1800);
                        ev3.MotorC.Off();
                        break;

                    case "D":
                        ev3.MotorD.On(speed);
                        Thread.Sleep(1000);
                        ev3.MotorD.Off();
                        break;
                }
                
            }
            catch
            {
            }
            finally
            {
                ev3.Connection.Close();
            }
        }

        private void SendMoveCommand(object sender, DoWorkEventArgs e)
        {
            Tuple<string, sbyte> argument = e.Argument as Tuple<string, sbyte>;
            string motor = argument.Item1;
            sbyte speed = argument.Item2;

            try
            {
                ev3.Connection.Open();

                switch (motor)
                {
                    case "A":
                        speed = (leftArm == MotorState.Down) ? (sbyte)(-1 * Math.Abs(speed)) : Math.Abs(speed);
                        ev3.MotorA.On(speed);
                        Thread.Sleep(1800);
                        ev3.MotorA.Off();
                        leftArm = (leftArm == MotorState.Down) ? MotorState.Up : MotorState.Down;
                        break;

                    case "B":
                        speed = (leftFoot == MotorState.Down) ? (sbyte)(-1 * Math.Abs(speed)) : Math.Abs(speed);
                        ev3.MotorB.On(speed);
                        Thread.Sleep(1000);
                        ev3.MotorB.Off();
                        leftFoot = (leftFoot == MotorState.Down) ? MotorState.Up : MotorState.Down;
                        break;

                    case "C":
                        speed = (rightArm == MotorState.Down) ? (sbyte)(-1 * Math.Abs(speed)) : Math.Abs(speed);
                        ev3.MotorC.On(speed);
                        Thread.Sleep(1800);
                        ev3.MotorC.Off();
                        rightArm = (rightArm == MotorState.Down) ? MotorState.Up : MotorState.Down;
                        break;

                    case "D":
                        speed = (rightFoot == MotorState.Down) ? (sbyte)(-1 * Math.Abs(speed)) : Math.Abs(speed);
                        ev3.MotorD.On(speed);
                        Thread.Sleep(1000);
                        ev3.MotorD.Off();
                        rightFoot = (rightFoot == MotorState.Down) ? MotorState.Up : MotorState.Down;
                        break;
                }

            }
            catch
            {
            }
            finally
            {
                ev3.Connection.Close();
            }
        }

        private void WriteText(string text)
        {
            this.Dispatcher.Invoke((Action)(() => { Msg.Text = text; }));
            Thread.Sleep(1000);
            this.Dispatcher.Invoke((Action)(() => { Msg.Text = ""; }));
        }

        private void startGestureRecognizer(object sender, DoWorkEventArgs e)
        {
            int confID;
            bool result = Int32.TryParse(e.Argument.ToString(), out confID);

            if (result)
                gestureRecognizer = new GestureRecognizer(confID);
        }

        public void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if(gesture != null)
                    gesture.Delete();

                if (sensor != null)
                    sensor.Stop();

                try
                {
                    ev3.Connection.Open();

                    if (leftArm == MotorState.Up)
                    {
                        ev3.MotorA.On(50);
                        Thread.Sleep(1800);
                        ev3.MotorA.Off();
                    }

                    if (rightArm == MotorState.Up)
                    {
                        ev3.MotorC.On(50);
                        Thread.Sleep(1800);
                        ev3.MotorC.Off();
                    }
                }
                catch
                {
                }
                finally
                {
                    ev3.Connection.Close();
                }
            }
        }

        public static void WaitForMotorToStop(Brick<Sensor, Sensor, Sensor, Sensor> ev3, string motor)
        {
            Thread.Sleep(500);

            switch (motor)
            {
                case "A":
                    while (ev3.MotorA.IsRunning()) { Thread.Sleep(50); }
                    break;
                case "B":
                    while (ev3.MotorB.IsRunning()) { Thread.Sleep(50); }
                    break;
                case "C":
                    while (ev3.MotorC.IsRunning()) { Thread.Sleep(50); }
                    break;
                case "D":
                    while (ev3.MotorD.IsRunning()) { Thread.Sleep(50); }
                    break;
            }
        }
    }
}
