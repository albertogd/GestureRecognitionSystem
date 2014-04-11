using System;
using System.IO;
using System.Threading;

namespace Algorithms.Record
{
    public class KinectReplay : IDisposable
    {
        BinaryReader reader;
        Stream stream;
        readonly SynchronizationContext synchronizationContext;

        // Events
        public event EventHandler<ReplaySkeletonFrameReadyEventArgs> SkeletonFrameReady;

        // Replay
        public ReplaySystem<ReplaySkeletonFrame> skeletonReplay;

        public bool Started { get; internal set; }

        public bool IsFinished
        {
            get
            {
                if (skeletonReplay != null && !skeletonReplay.IsFinished)
                    return false;

                return true;
            }
        }

        public KinectReplay(string file)
        {
            stream = File.OpenRead(file);
            reader = new BinaryReader(stream);

            synchronizationContext = SynchronizationContext.Current;
            skeletonReplay = new ReplaySystem<ReplaySkeletonFrame>();

            while (reader.BaseStream.Position != reader.BaseStream.Length)
                skeletonReplay.AddFrame(reader);
        }

        public void Start(double speed)
        {
            if (Started)
                throw new Exception("KinectReplay already started");

            Started = true;

            if (skeletonReplay != null)
            {
                skeletonReplay.Start(speed);

                skeletonReplay.FrameReady += frame => synchronizationContext.Send(state =>
                {
                    if (SkeletonFrameReady != null)
                        SkeletonFrameReady(this, new ReplaySkeletonFrameReadyEventArgs { SkeletonFrame = frame });
                }, null);
            }
        }

        public void Stop()
        {
            if (skeletonReplay != null)
                skeletonReplay.Stop();

            Started = false;
        }

        public void Dispose()
        {
            Stop();

            skeletonReplay = null;

            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }
    }
}
