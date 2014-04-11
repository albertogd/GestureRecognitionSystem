using System;
using System.IO;
using Microsoft.Kinect;

namespace Algorithms.Record
{
    public class KinectRecorder
    {
        string file;
        
        // Streams
        MemoryStream recordStream;
        BinaryWriter writer;
        
        // Time
        DateTime previousFlushDate;

        // Recorders
        SkeletonRecorder skeletonRecorder;


        public KinectRecorder(string filepath)
        {
            file = filepath;
            recordStream = new MemoryStream();
            writer = new BinaryWriter(recordStream);
            skeletonRecorder = new SkeletonRecorder(writer);
            previousFlushDate = DateTime.Now;
        }

        public void Record(SkeletonFrame frame)
        {
            if (writer == null)
                throw new Exception("This recorder is stopped");

            if (skeletonRecorder == null)
                throw new Exception("Skeleton recording is not actived on this KinectRecorder");

            skeletonRecorder.Record(frame);
            Flush();
        }

        void Flush()
        {
            var now = DateTime.Now;

            if (now.Subtract(previousFlushDate).TotalSeconds > 60)
            {
                previousFlushDate = now;
                writer.Flush();
            }
        }

        public void Stop()
        {
            if (writer == null)
                throw new Exception("This recorder is already stopped");

            writer.Flush();
            using (FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                recordStream.Seek(0, SeekOrigin.Begin);
                recordStream.CopyTo(fileStream);
            }

            writer.Close();
            writer.Dispose();

            recordStream.Close();
            recordStream.Dispose();
            recordStream = null;
        }

        public void Restart()
        {
            writer.Close();
            writer.Dispose();

            recordStream.Close();
            recordStream.Dispose();
            
            recordStream = new MemoryStream();
            writer = new BinaryWriter(recordStream);
            skeletonRecorder.Restart(writer);
            previousFlushDate = DateTime.Now;
        }

    }
}
