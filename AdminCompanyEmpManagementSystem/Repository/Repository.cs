using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace AdminCompanyEmpManagementSystem.Repository
{
    // Summary:
    // This is a generic repository that can be used with any type or table or data. Here we define 
    // method Add(),Update(),Delete(),GetAll(),Get() and FirstOrDefaut().

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> DbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            DbSet = _context.Set<T>();
        }

        public bool Add(T entity)
        {
            DbSet.Add(entity);
            return Save();
        }

        public bool Delete(int id)
        {
            var getUser = Get(id);
            if(getUser != null) { 
            return Delete(getUser);
            }
            return false;
        }

        public bool Delete(T entity)
        {
            DbSet.Remove(entity);
            return Save();
        }
        public void RemoveRange(IEnumerable<T> entity)
        {
            DbSet.RemoveRange(entity);
        }
        public T? Get(int id)
        {
            return DbSet.Find(id);
        }

        public T? FirstOrDefault(Expression<Func<T, bool>>? filter = null, string? includeTables = null)
        {
            IQueryable<T> query = DbSet;// IQueryable provide functionlity to queries against a
            //specific data source wherein type of data is known
            if (filter != null)
            {
                query = query.Where(filter);    
            }
            if (includeTables != null)
            {
                foreach (var includeTable in includeTables.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeTable);
                }
            }
            return query.FirstOrDefault();
        }

        public ICollection<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeTables = null)
        {
            IQueryable<T> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeTables != null)
            {
                foreach (var includeTable in includeTables.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeTable);
                }
            }
            return query.ToList();
        }

        public virtual bool Update(T entity)
        {
           DbSet.Update(entity);
            return Save();
        }
        public bool Save()
        {
            return _context.SaveChanges() == 1 ? true : false;
        }
    }
}
