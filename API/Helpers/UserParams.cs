namespace API.Helpers
{
    public class UserParams
    {
        //set a maxi page size the user can request
        private const int MaxPageSize = 50;
        //So we'd always return the first page
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            //This will get the value of 10
            get => _pageSize;
            //check the value against the maximum page size
            //check to see if this is greater than the max page size
            //if greater, we will return the max page size, if not, return pageSize
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
        //To filter the user to be min of 18 yrs and max of 100 yrs
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;
        public string OrderBy { get; set; } = "lastActive";
    }
}