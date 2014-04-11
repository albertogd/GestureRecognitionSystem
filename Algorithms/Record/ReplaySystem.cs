using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Algorithms.Record
{
    public class ReplaySystem<T> where T : ReplayFrame, new()
    {
        internal event Action<T> FrameReady;
        public readonly List<T> frames = new List<T>();

        CancellationTokenSource cancellationTokenSource;

        public bool IsFinished
        {
            get;
            private set;
        }

        internal void AddFrame(BinaryReader reader)
        {
            T frame = new T();

            frame.CreateFromReader(reader);

            frames.Add(frame);
        }

        public void Start(double speed)
        {
            Stop();

            IsFinished = false;

            cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = cancellationTokenSource.Token;

            Task.Factory.StartNew(() =>
            {
                foreach (T frame in frames.Skip(1))
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(speed * frame.TimeStamp));

                    if (token.IsCancellationRequested)
                        break;

                    if (FrameReady != null)
                        FrameReady(frame);
                }

                IsFinished = true;
            }, token);
        }

        public void Stop()
        {
            if (cancellationTokenSource == null)
                return;

            cancellationTokenSource.Cancel();
        }
    }
}
