using C_Project.Enums;



namespace C_Project.EnumHelpers
{
    internal class EnumHelper
    {

        public static string GetDisplayName(this STATUS_FILE status)
        {
            return status switch
            {
                STATUS_FILE.HAVENOTREAD_FILE => "Have Not Read",
                STATUS_FILE.READING_FILE => "Reading",
                STATUS_FILE.ALREADY_READ => "Already Read",
                _ => status.ToString()
            };
        }

        public static string GetDisplayName(this RATING_FILE rating)
        {
            return rating switch
            {
                RATING_FILE.POOR_RATING_FILE => "1 Star",
                RATING_FILE.AVERAGE_RATING_FILE => "2 Stars",
                RATING_FILE.GOOD_RATING_FILE => "3 Stars",
                RATING_FILE.EXCELLENT_RATING_FILE => "4 Stars",
                _ => rating.ToString()
            };
        }

        public static string GetDisplayName(this GENRE_FILE genre)
        {
            return genre switch
            {
                GENRE_FILE.FANTASY_FILE => "Fantasy",
                GENRE_FILE.DETECTIVE_FILE => "Detective",
                GENRE_FILE.ROMANCE_FILE => "Romance",
                GENRE_FILE.THRILLER_FILE => "Thriller",
                _ => genre.ToString()
            };
        }
    }
}

