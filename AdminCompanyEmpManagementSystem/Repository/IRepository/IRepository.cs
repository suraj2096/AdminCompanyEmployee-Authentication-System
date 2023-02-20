using System.Linq.Expressions;

namespace AdminCompanyEmpManagementSystem.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        public bool Add(T entity);
        public bool Delete(int id);
        public bool Delete(T entity);
        public void RemoveRange(IEnumerable<T> entity);
        public  bool Update(T entity);
        public T? Get(int id);
        public T? FirstOrDefault(Expression<Func<T, bool>>? filter=null, string? includeTables = null);

        public ICollection<T> GetAll(Expression<Func<T,bool>>? filter=null,string? includeTables = null);
        public bool Save();
        
    }
}
