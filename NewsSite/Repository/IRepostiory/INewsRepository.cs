using NewsSite.Models;
using System.Linq.Expressions;

namespace NewsSite.Repository.IRepostiory
{
    public interface INewsRepository : IRepository<News>
    {
      
        //Task<News> UpdateAsync(News entity);
  
    }
}
