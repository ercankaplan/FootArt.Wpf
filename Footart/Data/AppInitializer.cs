using FootArt.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Footart.Data
{
    public class AppInitializer : CreateDatabaseIfNotExists<AppDbContext>
    {
    }
}
