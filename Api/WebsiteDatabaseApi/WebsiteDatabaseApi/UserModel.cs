namespace WebsiteDatabaseApi
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public int StreetNumber { get; set; }
        public string City { get; set; }
        public int PostNumber { get; set; }
        public string Email { get; set; }
    }
}
