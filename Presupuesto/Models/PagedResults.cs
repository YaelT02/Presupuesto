namespace Presupuesto.Models
{
    public class PagedResults<T>
    {
        public PagedResults(IEnumerable<T> items, int totalItems, int pageNumber, int pageSize, int draw, int maxNavigationPages = 5)
        {
            Items = items;
            TotalItems = totalItems;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            PageNumber = pageNumber;
            MaxNavigationPages = maxNavigationPages;
            Draw = draw;

            StartPage = Math.Max(1, pageNumber - (maxNavigationPages / 2));
            EndPage = Math.Min(TotalPages, StartPage + maxNavigationPages - 1);

            // Adjust start page and end page if they are out of range
            if (EndPage - StartPage < maxNavigationPages - 1)
            {
                StartPage = Math.Max(1, EndPage - maxNavigationPages + 1);
            }

            PageNumbers = Enumerable.Range(StartPage, EndPage - StartPage + 1);
        }

        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int MaxNavigationPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public IEnumerable<int> PageNumbers { get; set; }
        public int Draw { get; set; }
    }
}
