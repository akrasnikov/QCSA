using System.Linq;

namespace QCSA.Toolkit.Database.Repositories
{
    public interface IQCSARepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        void Remove(T entity);
        void Add(T entity);
    }
}