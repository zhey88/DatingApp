using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

//when we use the page list, we tell it what t is going to be replaced with
//in our case, for now, it's going to be replaced with member DTO
public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        //If you set page number to be 1, page size to be 3, it will show 3 users(id 1 -- 3)
        //If you set page number to be 2, page size to be 3, it will show 3 users(id 4 -- 6)
        CurrentPage = pageNumber;
        //Total pages = 
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        //PageSize refers to how many users is showing
        PageSize = pageSize;
        TotalCount = count;
        //when we create a new instance of this page list, we can expect to receive all of these different
        //properties back from it, and we'll be able to get access to the items from the page list
        AddRange(items);
    }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    //Static so that we can call our page list
    //This is a query being built up in memory by entity framework, and it's the number of operations that
    //it's going to execute against our database, but not until we use one of these methods, 
    //such as count async, that's going to execute a query against our database
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, 
        int pageNumber, int pageSize)
    {
        //this is going to give us the accounts of the items from our query
        //For example, if we have 10 users, the count will be 10
        var count = await source.CountAsync();
        //we go and get our items using the skip and take operations and then we execute that to a list
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        //returned a new page list, along with the items, the count and the page number
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}