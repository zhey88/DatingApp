// get the pagination properties
//We just need to derive from the pagination params class that we just created

﻿namespace API.Helpers
{
    //specify the properties that we're interested in in addition to the pagination params
    public class LikesParams : PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }

        
    }
}