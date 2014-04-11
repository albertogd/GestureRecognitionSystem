using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Algorithms.DTW;
using Algorithms.Preprocessing;
using Algorithms.Record;
using Models;

namespace Algorithms
{

    public class Gesture
    {
        /// <summary>
        /// Gesture Model ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gesture nanme
        /// </summary>
        public string gesturename { get; private set; }

        // <summary>
        /// Gesture´s user name
        /// </summary>
        public string username { get; private set; }

        /// <summary>
        /// Length of gesture
        /// </summary>
        public int Length { get { return Skeletons.Count; } }

        /// <summary>
        /// Gets or sets the skeletons of the Gesture
        /// </summary>
        public List<Skeleton> Skeletons { get; set; }

        /// <summary>
        /// Kinect recoder skeleton
        /// </summary>
        private KinectRecorder recorder;

        /// <summary>
        /// Kinect replay skeleton
        /// </summary>
        KinectReplay replay;

        /// <summary>
        /// State of movement
        /// </summary>
        public State state;

        /// <summary>
        /// Distance  to detect movement
        /// </summary>
        private Distances movementDetectionDistance;

        /// <summary>
        /// Skeleton Preprocessor for Movement Detector
        /// </summary>
        private Preprocessor movementDetectionPreprocesor;

        /// <summary>
        /// Energy thresholds
        /// </summary>
        // Energy Threshold from Resting to Moving;
        private double ET_RestingToStartMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        private double ET_StartMovingToResting { get; set; }

        // Energy Threshold from Resting to Moving;
        private double ET_StartMovingToMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        private double ET_MovingToStopMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        private double ET_StopMovingToMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        private double ET_StopMovingToResting { get; set; }

        /// <summary>
        /// Time thresholds
        /// </summary>
        // Time Threshold from Resting to Moving;
        private int TT_RestingToStartMoving { get; set; }

        // Time Threshold from Resting to Moving;
        private int TT_StartMovingToResting { get; set; }

        // Time Threshold from Resting to Moving;
        private int TT_StartMovingToMoving { get; set; }

        // Time Threshold from Resting to Moving;
        private int TT_MovingToStopMoving { get; set; }

        // Time Threshold from Resting to Moving;
        private int TT_StopMovingToMoving { get; set; }

        // Time Threshold from Resting to Moving;
        private int TT_StopMovingToResting { get; set; }

        /// <summary>
        /// Energy
        /// </summary>
        public double energy { get; private set; }

        /// <summary>
        /// Frame Counter
        /// </summary>
        // Time counter from Resting to Moving;
        private int TC_RestingToStartMoving { get; set; }

        // Time counter from Resting to Moving;
        private int TC_StartMovingToResting { get; set; }

        // Time counter from Resting to Moving;
        private int TC_StartMovingToMoving { get; set; }

        // Time counterd from Resting to Moving;
        private int TC_MovingToStopMoving { get; set; }

        // Time counter from Resting to Moving;
        private int TC_StopMovingToMoving { get; set; }

        // Time counter from Resting to Moving;
        private int TC_StopMovingToResting { get; set; }
        
        /// <summary>
        /// Last Skeleton Captured
        /// </summary>
        private Skeleton lastSkeleton;

        /// <summary>
        /// List of energies (used only for statistical analysis)
        /// </summary>
        private List<double> energies;

        /// <summary>
        /// Context
        /// </summary>
        private Context ctx;


        /// <summary>
        /// Initializes a new instance of a Gesture
        /// </summary>
        /// <param name="skeletons">Skeletons of Gesture</param>
        public Gesture(string gname, string uname, GestureType type)
        {
            gesturename = gname;
            username = uname;
            Skeletons = new List<Skeleton>(50);
            energies = new List<double>(101);

            ctx = new Context();
            User user = ctx.Users.FirstOrDefault(u => u.name == username);
            GestureModel gesture = new GestureModel() { name = gname, user = user, type = type };
            gesture = ctx.Gestures.Add(gesture);
            ctx.SaveChanges();

            ID = gesture.ID;
            InitializeConf();
        }

        /// <summary>
        /// Initializes a new instance of a Gesture
        /// </summary>
        /// <param name="user">Username</param>
        /// <param name="user">Gesture Type</param>
        public Gesture(string user, GestureType type) : this(DateTime.Now.ToString(@"yyyy-MM-dd-hh-mm-ss"), user, type) { } 

        /// <summary>
        /// Initializes a new instance of a Gesture
        /// </summary>
        /// <param name="gesture">Gesture Model</param>
        public Gesture(GestureModel gesture)
        {   
            ID = gesture.ID;
            gesturename = gesture.name;
            username = gesture.user.name;

            if (!String.IsNullOrEmpty(gesture.file) && File.Exists(gesture.file))
            {
                KinectReplay replay = new KinectReplay(gesture.file);
                Skeletons = replay.skeletonReplay.frames.Select(frame => frame.Skeletons.FirstOrDefault(skeleton => skeleton.TrackingState != SkeletonTrackingState.NotTracked)).ToList();
            }
            else
            {
                Skeletons = new List<Skeleton>(50);
            }

            energies = new List<double>(101);
            ctx = new Context();

            InitializeConf();
        }

        public void InitializeConf()
        {
            List<Configuration> confs = ctx.Configurations.ToList();
            SystemConfiguration sysconf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == username);
            ConfigurationMovement conf = sysconf.movementDetectionProfile.movementConfiguration;
            movementDetectionDistance = new Distances(sysconf.movementDetectionProfile.dtwConfiguration.distance);
            movementDetectionPreprocesor = new Preprocessor(sysconf.movementDetectionProfile.ID);
            ET_RestingToStartMoving = conf.ET_RestingToStartMoving;
            ET_StartMovingToResting = conf.ET_StartMovingToResting;
            ET_StartMovingToMoving = conf.ET_StartMovingToMoving;
            ET_MovingToStopMoving = conf.ET_MovingToStopMoving;
            ET_StopMovingToMoving = conf.ET_StopMovingToMoving;
            ET_StopMovingToResting = conf.ET_StopMovingToResting;
            TT_RestingToStartMoving = conf.TT_RestingToStartMoving;
            TT_StartMovingToResting = conf.TT_StartMovingToResting;
            TT_StartMovingToMoving = conf.TT_StartMovingToMoving;
            TT_MovingToStopMoving = conf.TT_MovingToStopMoving;
            TT_StopMovingToMoving = conf.TT_StopMovingToMoving;
            TT_StopMovingToResting = conf.TT_StopMovingToResting;
            ResetCounters();
        }

        public void ResetCounters()
        {
            TC_RestingToStartMoving = 0;
            TC_StartMovingToResting = 0;
            TC_StartMovingToMoving = 0;
            TC_MovingToStopMoving = 0;
            TC_StopMovingToMoving = 0;
            TC_StopMovingToResting = 0;
        }


        public void TrainingRecord()
        {
            Record(GestureType.TrainingEnabled);
        }

        public void EvaluationRecord()
        {
            Record(GestureType.Evaluation);
        }

        public void Record(GestureType gesturetype)
        {
            ResetCounters();

            // Check if replay has been executed.
            // If yes, dispose streams
            if (replay != null)
            {
                replay.Dispose();
                replay = null;
            }

            String baseDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String saveDirectory = System.IO.Path.Combine(baseDirectory, "Data", username);
            String gestureFile = System.IO.Path.Combine(saveDirectory, gesturename);

            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            recorder = new KinectRecorder(gestureFile);
            state = State.Resting;

            GestureModel model = ctx.Gestures.FirstOrDefault(gesture => gesture.user.name == username && gesture.name == gesturename && gesture.type == gesturetype);

            if (model == null)
            {
                User user = ctx.Users.FirstOrDefault(u => u.name == username);
                ctx.Gestures.Add(new GestureModel { name = gesturename, user = user, file = gestureFile, type = gesturetype });
            }
            else
            {
                model.file = gestureFile;
            }

            ctx.SaveChanges();
        }

        public void Record(SkeletonFrame frame)
        {
            if (recorder == null)
                throw new Exception("Try to record gesture without startirn KinectRecord");

            if(frame.SkeletonArrayLength == 0)
                return;

            Skeleton[] sks = new Skeleton[frame.SkeletonArrayLength];
            frame.CopySkeletonDataTo(sks);
            Skeleton actualSkeleton = sks.FirstOrDefault(skeleton => skeleton.TrackingState != SkeletonTrackingState.NotTracked);

            if (actualSkeleton == null)
                return;

            if (lastSkeleton == null)
            {
                lastSkeleton = actualSkeleton;
                return;
            }

            // Update state
            energy = movementDetectionDistance.Distance(movementDetectionPreprocesor.Preprocess(lastSkeleton), movementDetectionPreprocesor.Preprocess(actualSkeleton));

            if (Double.IsNaN(energy) || Double.IsInfinity(energy))
                energy = 0;
            
            energies.Add(energy);

            switch (state)
            {
                case State.Resting:

                    if (energy > ET_RestingToStartMoving)
                    {
                        TC_RestingToStartMoving++;

                        if (TC_RestingToStartMoving > TT_RestingToStartMoving)
                        {
                            // Save skeleton to this object
                            Skeletons.Add(actualSkeleton);

                            // Save frame to recorder
                            recorder.Record(frame);

                            // Change State
                            state = State.StartMoving;
                            
                            // Reset New State Counters
                            TC_StartMovingToResting = 0;
                            TC_StartMovingToMoving = 0;
                        }
                    }

                    break;

                case State.StartMoving:

                    if (energy < ET_StartMovingToResting)
                    {
                        // Increase StartMoving to Resting counter
                        TC_StartMovingToResting++;

                        if (TC_StartMovingToResting > TT_StartMovingToResting)
                        {
                            state = State.Resting;
                            TC_RestingToStartMoving = 0;
                            Skeletons.Clear();
                            recorder.Restart();
                        }
                    }
                    else
                    {
                        // Save skeleton to this object
                        Skeletons.Add(actualSkeleton);

                        // Save frame to recorder
                        recorder.Record(frame);

                        if (energy > ET_StartMovingToMoving)
                        {
                            // Increase StartMoving to Moving counter
                            TC_StartMovingToMoving++;

                            if (TC_StartMovingToMoving > TT_StartMovingToMoving)
                            {
                                state = State.Moving;
                                TC_MovingToStopMoving = 0;
                            }
                        }
                    }

                    break;

                case State.Moving:

                    // Save skeleton to this object
                    Skeletons.Add(actualSkeleton);

                    // Save frame to recorder
                    recorder.Record(frame);

                    if (energy < ET_MovingToStopMoving)
                    {
                        // Increase StartMoving to Moving counter
                        TC_MovingToStopMoving++;

                        if (TC_MovingToStopMoving > TT_MovingToStopMoving)
                        {
                            state = State.StopMoving;
                            TC_StopMovingToMoving = 0;
                            TC_StopMovingToResting = 0;
                        }
                        
                    }

                    break;

                case State.StopMoving:

                    // Save skeleton to this object
                    Skeletons.Add(actualSkeleton);

                    // Save frame to recorder
                    recorder.Record(frame);

                    if (energy < ET_StopMovingToResting)
                    {
                        TC_StopMovingToResting++;

                        if (TC_StopMovingToResting > TT_StopMovingToResting)
                        {
                            state = State.Finish;
                            recorder.Stop();
                        }

                    }
                    else
                    {
                        if (energy > ET_StopMovingToMoving)
                        {
                            TC_StopMovingToMoving++;

                            if (TC_StopMovingToMoving > TT_StopMovingToMoving)
                            {
                                state = State.Moving;
                                TC_MovingToStopMoving = 0;
                            }
                        }
                    }

                    break;
            }

            lastSkeleton = actualSkeleton;
        }

        public void Replay(ReplaySkeletonFrame frame)
        {
            if (frame.Skeletons.Count() == 0)
                return;

            Skeleton[] sks = frame.Skeletons;
            Skeleton actualSkeleton = sks.FirstOrDefault(skeleton => skeleton.TrackingState != SkeletonTrackingState.NotTracked);

            if (actualSkeleton == null)
                return;

            if (lastSkeleton == null)
            {
                lastSkeleton = actualSkeleton;
                return;
            }

            // Update state
            energy = movementDetectionDistance.Distance(movementDetectionPreprocesor.Preprocess(lastSkeleton), movementDetectionPreprocesor.Preprocess(actualSkeleton));

            if (Double.IsNaN(energy) || Double.IsInfinity(energy))
                energy = 0;
            
            energies.Add(energy);

            switch (state)
            {
                case State.Resting:

                    if (energy > ET_RestingToStartMoving)
                    {
                        TC_RestingToStartMoving++;

                        if (TC_RestingToStartMoving > TT_RestingToStartMoving)
                        {
                            state = State.StartMoving;
                            TC_StartMovingToResting = 0;
                            TC_StartMovingToMoving = 0;
                        }
                    }

                    break;

                case State.StartMoving:

                    if (energy < ET_StartMovingToResting)
                    {
                        // Increase StartMoving to Resting counter
                        TC_StartMovingToResting++;

                        if (TC_StartMovingToResting > TT_StartMovingToResting)
                        {
                            state = State.Resting;
                            TC_RestingToStartMoving = 0;
                        }
                    }
                    else
                    {

                        if (energy > ET_StartMovingToMoving)
                        {
                            // Increase StartMoving to Moving counter
                            TC_StartMovingToMoving++;

                            if (TC_StartMovingToMoving > TT_StartMovingToMoving)
                            {
                                state = State.Moving;
                                TC_MovingToStopMoving = 0;
                            }
                        }
                    }

                    break;

                case State.Moving:

                    if (energy < ET_MovingToStopMoving)
                    {
                        // Increase StartMoving to Moving counter
                        TC_MovingToStopMoving++;

                        if (TC_MovingToStopMoving > TT_MovingToStopMoving)
                        {
                            state = State.StopMoving;
                            TC_StopMovingToMoving = 0;
                            TC_StopMovingToResting = 0;
                        }

                    }

                    break;

                case State.StopMoving:

                    if (energy < ET_StopMovingToResting)
                    {
                        TC_StopMovingToResting++;

                        if (TC_StopMovingToResting > TT_StopMovingToResting)
                        {
                            state = State.Finish;
                        }

                    }
                    else
                    {
                        if (energy > ET_StopMovingToMoving)
                        {
                            TC_StopMovingToMoving++;

                            if (TC_StopMovingToMoving > TT_StopMovingToMoving)
                            {
                                state = State.Moving;
                                TC_MovingToStopMoving = 0;
                            }
                        }
                    }

                    break;
            }

            lastSkeleton = actualSkeleton;
        }

        public void Replay(EventHandler<ReplaySkeletonFrameReadyEventArgs> eventHandler)
        {
            Replay(eventHandler, 1);
        }

        public void Replay(EventHandler<ReplaySkeletonFrameReadyEventArgs> eventHandler, double speed)
        {
            ResetCounters();
            
            // In replay mode, we start in Start Moving State
            state = State.StartMoving;
            string gestureFile = ctx.Gestures.Find(ID).file;

            if (String.IsNullOrEmpty(gestureFile) || !File.Exists(gestureFile))
                return;

            replay = new KinectReplay(gestureFile);

            if (ctx.Gestures.Find(ID).type == GestureType.Evaluation)
            {
                List<Skeleton> skes = replay.skeletonReplay.frames.Select(frame => frame.Skeletons.FirstOrDefault(skeleton => skeleton.TrackingState != SkeletonTrackingState.NotTracked)).ToList();
                int t = 0;
            }
            replay.SkeletonFrameReady += eventHandler;
            replay.Start(speed);
        }

        public void Delete()
        {
            GestureModel gesture = ctx.Gestures.FirstOrDefault(g => g.user.name == username && g.name == gesturename && g.type == GestureType.Evaluation);

            if (gesture == null)
                return;

            try
            {
                if (File.Exists(gesture.file))
                    File.Delete(gesture.file);

                ctx.Gestures.Remove(gesture);
                ctx.SaveChanges();
            }
            catch
            {
            }
        }

        public void DeleteSkeletonFile()
        {
            GestureModel gesture = ctx.Gestures.FirstOrDefault(g => g.user.name == username && g.name == gesturename);

            if (gesture == null)
                return;

            try
            {
                if (File.Exists(gesture.file))
                    File.Delete(gesture.file);

                gesture.file = null;
                ctx.SaveChanges();
            }
            catch
            {
            }
        }

    }
}
