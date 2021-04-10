using PlutoData.Specifications;
using PlutoData.Specifications.Builder;

namespace PlutoData.Test
{
    public class BlogSpecification: Specification<Blog>
    {
        public BlogSpecification()
        {
            Query.Where(x=>x.Id>0)
                .OrderByDescending(x=>x.Id);
        }
    }

    public class Blog2Specification: Specification<Blog,string>
    {
        public Blog2Specification()
        {
            Query.Select(x => x.Title).Where(x=>x.Id>0);
        }
    }
}