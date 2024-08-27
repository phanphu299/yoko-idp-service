namespace IdpServer.Application.User.Model
{
    public class UserPatchDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public int DateTimeFormatId { get; set; }
        public int TimezoneId { get; set; }
        public string LanguageCode { get; set; }
    }
}