using PlutoData.Specifications;
using PlutoData.Specifications.Builder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlutoData.Test.Repositorys
{
    public class ProjectPagingSpecification : Specification<Blog>
    {
        
        public ProjectPagingSpecification()
        {
            Query.Where(e => e.Id >0);
            Query.Include(e => e.Posts);
            Query.Search(x => x.Url, "%g_81%", 1);
            Query.OrderByDescending(e => e.Id).ThenBy(x => x.Title);
        }

    }
}
