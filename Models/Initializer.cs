using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace Models
{
    //public class Initializer : DropCreateDatabaseAlways<Context>
    public class Initializer : DropCreateDatabaseIfModelChanges<Context>
    {

        protected override void Seed(Context ctx)
        {
            User.AddDefaultUser("Alber", "1234", Language.Spanish, ctx);
        }
    }
}
