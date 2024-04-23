//this is going to be an object that we return inside the HTTP response headers
//So we're going to have a header basically called pagination
//When our response goes back to the clients, they're going to be able to fish out the pagination details
//from this pagination header from the HTTP response

﻿namespace API;

public class PaginationHeader
{
    //and we'll create four properties inside here
    //we're going to return this inside an HTTP response
    //we'll create a extension method that's going to extend the HTTP response object or class
    public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
        CurrentPage = currentPage;
        ItemsPerPage = itemsPerPage;
        TotalItems = totalItems;
        TotalPages = totalPages;
    }

    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}