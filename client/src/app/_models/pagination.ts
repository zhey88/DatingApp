export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

//So in order to use this, when we get the response back from the API, we're going to need to take a
//look at the header, fish out the pagination information and create a new paginated result class and
//populate this property with the pagination information and we'll set the results to the list of items.

//T will be the list of items, members
export class PaginatedResult<T> {
    result?: T;
    pagination?: Pagination
}