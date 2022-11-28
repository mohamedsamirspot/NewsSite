using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsSite.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<News> News { get; set; }
        public IEnumerable<Category> Category { get; set; }

    }
}
