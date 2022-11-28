using NewsSite.Data;
using NewsSite.Models;
using NewsSite.Repository.IRepostiory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace NewsSite.Repository
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        private readonly ApplicationDbContext _db;
        public NewsRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

  
        public async Task<News> UpdateAsync(News entity)
        {
            _db.News.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
