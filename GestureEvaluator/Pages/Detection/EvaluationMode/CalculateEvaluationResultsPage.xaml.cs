using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Algorithms;
using Models;

namespace GestureEvaluator.Pages.Detection.EvaluationMode
{
    /// <summary>
    /// Interaction logic for CalculateEvaluationResultsPage.xaml
    /// </summary>
    public partial class CalculateEvaluationResultsPage : Page
    {
        private Context ctx;
        private Evaluation evaluation;


        public CalculateEvaluationResultsPage()
        {
            InitializeComponent();
            ctx = new Context();

            Dictionary<int, DateTime> dic = ctx.Evaluations.Where(ev => (ev.user.name == HomeWindow.username) && (ev.completed == false)).ToDictionary(ev => ev.ID, ev => ev.date);
            EvaluationListView.ItemsSource = dic;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (EvaluationListView.SelectedItem == null)
                return;

            int evaluationID = ((KeyValuePair<int, DateTime>)EvaluationListView.SelectedItem).Key;
            evaluation = ctx.Evaluations.Find(evaluationID);
            Evaluate();
        }

        public void Evaluate()
        {
            evaluation.started = true;
            ctx.SaveChanges();

            List<EvaluationItem> evaluationItems = ctx.EvaluationItems.Where(e => e.evaluation.ID == evaluation.ID).ToList();
            List<Configuration> confs = evaluation.profiles(ctx);

            foreach (Configuration conf in confs)
            {
                GestureRecognizer gr = new GestureRecognizer(conf.ID);

                foreach (EvaluationItem evi in evaluationItems)
                {
                    Gesture currentGesture = new Gesture(evi.gestureRecorded);

                    if (!ctx.EvaluationItemResults.Any(evr => evr.evaluationItem.ID == evi.ID && evr.conf.ID == conf.ID))
                    {
                        EvaluationItemResult result = new EvaluationItemResult() { conf = conf, evaluationItem = evi };
                        ctx.EvaluationItemResults.Add(result);
                        ctx.SaveChanges();

                        gr.Evaluate(currentGesture, result.ID);
                        result.GestureRecognized = ctx.DTWResults.Where(c => c.EvaluationItemResult.ID == result.ID).ToList().Min().Gesture;
                        result.Result = (result.GestureRecognized.ID == evi.gesturePlayed.ID);
                        ctx.SaveChanges();
                    }
                }
            }

            List<EvaluationResult> evrs = ctx.EvaluationResults.Where(e => e.evaluation.ID == evaluation.ID).ToList();
            if (evrs.Count == 0)
                EvaluateResults();

            evaluation.completed = true;
            evaluation.started = false;
            ctx.SaveChanges();
        }

        public void EvaluateResults()
        {
            List<Configuration> confs = evaluation.profiles(ctx);

            foreach (Configuration conf in confs)
            {
                List<EvaluationItemResult> evirs = ctx.EvaluationItemResults.Where(evir => evir.conf.ID == conf.ID && evir.evaluationItem.evaluation.ID == evaluation.ID).ToList();
                int success = evirs.Count(evir => evir.Result);
                int total = evirs.Count;
                ctx.EvaluationResults.Add(new EvaluationResult() { evaluation = evaluation, conf = conf, Success = success, Total = total });
                ctx.SaveChanges();
            }
        }
    }
}
