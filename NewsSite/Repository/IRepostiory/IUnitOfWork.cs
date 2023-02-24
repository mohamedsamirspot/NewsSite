using NewsSite.Models;

namespace NewsSite.Repository.IRepostiory
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        INewsRepository News { get; }
        IAuthRepository Auth { get; }

        int Complete();
    }
}
