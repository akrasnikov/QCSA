using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCSA.Toolkit.Database.Context
{
    public class QCSADBContext : DbContext
    {
        public virtual DbSet Users { get; set; }
        public virtual DbSet Storage { get; set; }
        public QCSADBContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }
    }
}
