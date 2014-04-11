using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Evaluation
    {
        // EvaluationID
        public int ID { get; set; }

        // User
        public virtual User user { get; set; }

        // Evaluation Type
        public EvaluationType type { get; set; }

        // Evaluation Datetime
        public DateTime date { get; set; }

        // If evaluation result has been started
        public bool started { get; set; }

        // If evaluation result has been completed
        public bool completed { get; set; }

        // Evaluation Profiles
        public string profileSerialized { get; set; }

        public List<Configuration> profiles(Context ctx)
        {
            if (String.IsNullOrEmpty(profileSerialized))
                return new List<Configuration>();
            else
                return profileSerialized.Split(':').Select(id => ctx.Configurations.Find(Int32.Parse(id))).ToList();
        }

        public void profiles(params Configuration[] profiles)
        {
            if (profiles.Length == 0)
                profileSerialized = "";
            else if (profiles.Length == 1)
                profileSerialized = profiles[0].ID.ToString();
            else
                profileSerialized = profiles.Select(g => g.ID.ToString()).Aggregate((a, b) => a + ":" + b);
        }
    }
}
