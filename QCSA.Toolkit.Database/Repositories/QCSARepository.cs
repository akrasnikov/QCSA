using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QCSA.Toolkit.Database.Context;
using System.Data.Entity;

namespace QCSA.Toolkit.Database.Repositories
{
    public class QCSARepository<T> : IQCSARepository<T> where T : class
    {
        private readonly QCSADBContext dbContext;
        private IDbSet<T> dbSet => dbContext.Set<T>();
        public IQueryable<T> Entities => dbSet;
        public QCSARepository(QCSADBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
    }
}


