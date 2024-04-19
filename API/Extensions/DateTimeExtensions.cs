//For Calculation of the user age
namespace API.Extensions
{

    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            //Calculate the user age
            var age = today.Year - dob.Year;
            //if the user haven't had their birthday yet this year. we will minus 1 year
            if (dob > today.AddYears(-age)) age--;

            return age;
        }
    }
}