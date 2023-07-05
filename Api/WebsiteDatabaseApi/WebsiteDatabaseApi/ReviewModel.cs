namespace WebsiteDatabaseApi
{
    public class ReviewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Text { get; set; }
        public string Timestamp { get; set; }
    }
}
