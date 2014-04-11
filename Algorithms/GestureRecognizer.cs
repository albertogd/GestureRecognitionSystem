using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Algorithms.DTW;
using Algorithms.Preprocessing;
using Models;

namespace Algorithms
{
    public class GestureRecognizer
    {
        private Configuration conf;
        private Context ctx;
        private List<Gesture> trainingGestures;


        public GestureRecognizer(int ConfigurationID)
        {
            ctx = new Context();
            conf = ctx.Configurations.Find(ConfigurationID);
            User user = conf.user;

            trainingGestures = ctx.Gestures.Where(gesture => gesture.user.ID == user.ID && gesture.type == GestureType.TrainingEnabled).ToList().Select(g => new Gesture(g)).ToList();

            // Preprocessor
            Preprocessor p = new Preprocessor(conf.ID);
            trainingGestures.ForEach(gesture => gesture.Skeletons = gesture.Skeletons.Select(s => p.Preprocess(s)).ToList());
        }

        public void Evaluate(Gesture gesture, int EvaluationResultID)
        {
            if (gesture.Skeletons.Count == 0)
                return;

            // Preprocessor
            Preprocessor p = new Preprocessor(conf.ID);
            gesture.Skeletons = gesture.Skeletons.Select(s => p.Preprocess(s)).ToList();

            foreach (Gesture training in trainingGestures)
            {
                Dtw dtw = new Dtw(gesture, training, conf.dtwConfiguration);
                EvaluationItemResultDTW evirdtw = new EvaluationItemResultDTW() { EvaluationItemResult = ctx.EvaluationItemResults.Find(EvaluationResultID), Gesture = ctx.Gestures.Find(training.ID), distance = dtw.GetCost() };
                ctx.DTWResults.Add(evirdtw);
            }

            ctx.SaveChanges();
        }

        public Gesture Recognize(Gesture gesture)
        {
            Dictionary<Gesture, double> distances = GetDistances(gesture);
            double? threshold = conf.gestureRecognizerConfiguration.distanceRecognizerThreshold;

            if (threshold.HasValue)
            {
                Dictionary<Gesture, double> orderDistances = distances.OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value);
                double distance1 = orderDistances.First().Value;
                double distance2 = orderDistances.Skip(1).First().Value;

                return (distance2 - distance1 > conf.gestureRecognizerConfiguration.distanceRecognizerThreshold) ? orderDistances.First().Key : null;
            }
            else
            {
                return distances.MinBy(x => x.Value).Key;
            }
        }

        public Dictionary<Gesture, double> GetDistances(Gesture gesture)
        {
            if (gesture.Skeletons.Count == 0)
                return null;
            Dictionary<Gesture, double> distances = new Dictionary<Gesture, double>(trainingGestures.Count);

            // Preprocessor
            Preprocessor p = new Preprocessor(conf.ID);
            gesture.Skeletons = gesture.Skeletons.Select(s => p.Preprocess(s)).ToList();

            foreach (Gesture trainingGesture in trainingGestures)
            {
                Dtw dtw = new Dtw(gesture, trainingGesture, conf.dtwConfiguration);
                double cost = dtw.GetCost();
                distances.Add(trainingGesture, cost);
            }

            return distances;
        }
    }
}
