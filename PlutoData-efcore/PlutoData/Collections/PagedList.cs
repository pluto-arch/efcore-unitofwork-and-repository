using System;
using System.Collections.Generic;
using System.Linq;


namespace PlutoData.Collections
{
    public class PagedList<T> : IPagedList<T>
    {
       
        public int PageIndex { get; set; }
       
        public int PageSize { get; set; }
       
        public int TotalCount { get; set; }
       

      
        public IList<T> Items { get; set; }



        internal PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (source==null)
            {
                PageIndex = default;
                PageSize = default;
                TotalCount = default;
                Items = default;
            }
            if (pageIndex<1)
            {
                throw new ArgumentException($"页码不能小于1");
            }

            if (source is IQueryable<T> querable)
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = querable.Count();
                Items = querable.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
            }
            else
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = source.Count();
                Items = source.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
            }
        }

        internal PagedList() => Items = new T[0];
    }


    
    internal class PagedList<TSource, TResult> : IPagedList<TResult>
    {
       
        public int PageIndex { get; }
      
        public int PageSize { get; }
     
        public int TotalCount { get; }
       

      
        public IList<TResult> Items { get; }


      
        public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize)
        {
            if (source == null)
            {
                PageIndex = default;
                PageSize = default;
                TotalCount = default;
                Items = default;
            }
            if (pageIndex < 1)
            {
                throw new ArgumentException($"页码不能小于1");
            }

            if (source is IQueryable<TSource> querable)
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = querable.Count();
                var items = querable.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToArray();
                Items = new List<TResult>(converter(items));
            }
            else
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = source.Count();
                var items = source.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToArray();
                Items = new List<TResult>(converter(items));
            }
        }

      
        public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            PageIndex = source.PageIndex;
            PageSize = source.PageSize;
            TotalCount = source.TotalCount;
            Items = new List<TResult>(converter(source.Items));
        }
    }



   
    public static class PagedList
    {
       
        public static IPagedList<T> Empty<T>() => new PagedList<T>();
      
        public static IPagedList<TResult> From<TResult, TSource>(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter) => new PagedList<TSource, TResult>(source, converter);
    }

}