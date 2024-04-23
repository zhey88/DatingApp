namespace API.Helpers
{
    //user parameters we can derive from the pagination params class
    public class UserParams : PaginationParams
    {

        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
        //To filter the user to be min of 18 yrs and max of 100 yrs
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;
        public string OrderBy { get; set; } = "lastActive";
    }
}