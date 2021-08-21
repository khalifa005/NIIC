using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Application.Interfaces
{
    public enum OrderOperator
    {
        None = 0,
        Ascending,
        Descending
    }

    public abstract class FilterUI<T> where T : class
    {
        public M DeserializeJson<M>(string json)
        {
            var unescaped = Compression.Decompress(Uri.UnescapeDataString(json));

            using (var input = new StringReader(unescaped))
            {
                return Json.Deserialize<M>(input.ReadToEnd());
            }
        }

        public abstract T GetFilter();

        public string SerializeJson(object obj)
        {
            var queryString = Json.Serialize(obj);

            return Uri.EscapeDataString(Compression.Compress(queryString));
        }

        public bool IsEmpty()
        {
            foreach (var pi in GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    var value = (string?)pi.GetValue(this);
                    if (!string.IsNullOrEmpty(value)) return false;
                }
                else if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    var val = pi.GetValue(this);
                    if (val != null) return false;
                }
            }

            return true;
        }

        public abstract string SerializeJson();
    }

    public interface IFilter<T>
    {
        IQueryable<T> Filter(IQueryable<T> query);
    }


    public interface ISort<T>
    {
        IOrderedQueryable<T> Sort(IQueryable<T> query);
    }


    public class QueryCondition<T>
    {
        public IFilter<T> Filter { get; set; }

        public QueryCondition()
        {
        }

        public QueryCondition(IFilter<T> f)
        {
            Filter = f;
        }
    }

   
    public class PagingQueryCondition<T> : QueryCondition<T>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public PagingQueryCondition()
        {
        }

        public PagingQueryCondition(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public PagingQueryCondition(IFilter<T> f, int page, int pageSize) : base(f)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
   
    public class SortedPagingQueryCondition<T> : PagingQueryCondition<T>
    {
        public ISort<T> Sorter { get; set; }

        public SortedPagingQueryCondition()
        {
        }

        public SortedPagingQueryCondition(IFilter<T> f, ISort<T> s, int page, int pageSize) : base(f, page, pageSize)
        {
            Sorter = s;
        }
    }

    public static class QueryBuilder
    {
        public static PagingSpec<T> Paging<T>(IQueryable<T> query, PagingQueryCondition<T> qi) where T : class
        {
            //pass query from _context.activity.include ...AsiQuerable for filtering with related data 
            if (qi.Filter != null)
                query = qi.Filter.Filter(query);

            var total = query;
            var result = query.Take(qi.PageSize).Skip((qi.Page - 1) * qi.PageSize);

            return new PagingSpec<T>
            {
                Count = total,
                Listing = result
            };
        }

        public static PagingSpec<T> Paging<T>(IQueryable<T> query, SortedPagingQueryCondition<T> qi) where T : class
        {
            if (qi.Sorter == null)
                throw new ArgumentNullException($"{nameof(qi.Sorter)} cannot be null");

            if (qi.Filter != null)
                query = qi.Filter.Filter(query);

            var sorted = qi.Sorter.Sort(query);

            var total = sorted;
            var result = sorted.Take(qi.PageSize).Skip((qi.Page - 1) * qi.PageSize);

            return new PagingSpec<T>
            {
                Count = total,
                Listing = result
            };
        }
    }

    public class ManySpec<T>
    {
        public IQueryable<T> Listing { get; set; }
    }

    public class PagingSpec<T> : ManySpec<T>
    {
        public IQueryable<T> Count { get; set; }
    }

    public class PagingInfo
    {
        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total results.
        /// </summary>
        /// <value>The total results.</value>
        public int TotalResults { get; set; }

        /// <summary>
        /// Gets the total number of pages
        /// </summary>
        public int TotalPages
        {
            get
            {
                if (PageSize == 0)
                    return 1;

                int totalPages = Math.DivRem(TotalResults, PageSize, out int remainder);

                if (remainder > 0)
                    totalPages++;

                return totalPages;
            }
        }
    }
}
