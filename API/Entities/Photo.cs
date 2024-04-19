using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{

    //Create a table photos in the database
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        //To check if the photo is the main photo
        public bool IsMain { get; set; }
        //For photo upload/ store the photos
        public string PublicId { get; set; }

        //For fully defined relationship between photos and users
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}