using System;
using System.Data.Entity;

namespace Models
{
    public class Context : DbContext
    {
        // Constructor
        public Context() : base("MySqlMembershipConnection") { }


        // General

        public DbSet<GestureModel> Gestures { get; set; }

        public DbSet<User> Users { get; set; }


        // Evaluation

        public DbSet<Evaluation> Evaluations { get; set; }

        public DbSet<EvaluationItem> EvaluationItems { get; set; }

        public DbSet<EvaluationItemResult> EvaluationItemResults { get; set; }

        public DbSet<EvaluationResult> EvaluationResults { get; set; }

        public DbSet<EvaluationItemResultDTW> DTWResults { get; set; }


        // Configuration

        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }

        public DbSet<Configuration> Configurations { get; set; }

        public DbSet<ConfigurationGestureRecognizer> GRConfigurations { get; set; }

        public DbSet<ConfigurationDTW> DTWConfigurations { get; set; }

        public DbSet<ConfigurationMovement> MovementConfigurations { get; set; }

        public DbSet<ConfigurationSkeletonPreprocess> SkeletonPreprocessConfigurations { get; set; }

        public DbSet<MindstormConfiguration> MindstormConfigurations { get; set; }
    }
}
