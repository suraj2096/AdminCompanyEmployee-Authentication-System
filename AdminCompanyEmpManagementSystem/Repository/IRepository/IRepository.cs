namespace AdminCompanyEmpManagementSystem.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        public T GetAll();
        public T GetById(int id);
        
    }
}
