using NewsSite.Repository;
using NewsSite.Repository.IRepostiory;

namespace NewsSite.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository Categories { get; private set; }
        public INewsRepository News { get; private set; }

        public IAuthRepository Auth { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Categories = new CategoryRepository(_context);
            News = new NewsRepository(_context);
            Auth = new AuthRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
