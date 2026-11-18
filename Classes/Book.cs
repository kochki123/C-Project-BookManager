using C_Project.Enums;


namespace C_Project.Classes
{
    internal class Book
    {
        public int ID { get; set; }

        private int counter_ = 0;

        public Book(string title, string author, DateTime releaseYear, GENRE_FILE genre, STATUS_FILE status, RATING_FILE rating)
        {
            ID = counter_++;
            Title = title;
            Author = author;
            ReleaseYear = releaseYear;
            Genre = genre;
            Status = status;
        }

        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime ReleaseYear { get; set; } = DateTime.UtcNow;
        public GENRE_FILE Genre { get; set; }
        public STATUS_FILE Status { get; set; }
        public RATING_FILE Rating { get; set; }

        public override string ToString()
        {
            return $"{Title}|{Author}|{ReleaseYear}|{Status}|{Rating}";
        }
    }
}
