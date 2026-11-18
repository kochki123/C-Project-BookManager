using C_Project.Classes;
using C_Project.Enums;
using Spectre.Console;

List<User> users = new List<User>()
{
    new User("Alice", "Password123"),
    new User("Bob", "SecurePassword456"),
    new User("Charlie", "MySecretPassword789"),
    new User("Diana", "DianaPass!2026"),
    new User("Ethan", "EthanCode#88" ),
    new User("Fiona", "FionaSecret*99"),
    new User("George", "GeorgePass_11"),
    new User("Hannah", "HannahKey$44"),
    new User("Ian", "IanSecure#77"),
    new User("Julia", "JuliaPwd@55")
};





List<Book> bookList = new List<Book>
{
    // Fantasy
    new Book("The Hobbit", "J.R.R. Tolkien", new DateTime(1937, 9, 21), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Fellowship of the Ring", "J.R.R. Tolkien", new DateTime(1954, 7, 29), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Two Towers", "J.R.R. Tolkien", new DateTime(1954, 11, 11), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Return of the King", "J.R.R. Tolkien", new DateTime(1955, 10, 20), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Harry Potter and the Sorcerer's Stone", "J.K. Rowling", new DateTime(1997, 6, 26), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Harry Potter and the Chamber of Secrets", "J.K. Rowling", new DateTime(1998, 7, 2), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Harry Potter and the Prisoner of Azkaban", "J.K. Rowling", new DateTime(1999, 7, 8), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Harry Potter and the Goblet of Fire", "J.K. Rowling", new DateTime(2000, 7, 8), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("A Game of Thrones", "George R.R. Martin", new DateTime(1996, 8, 6), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("A Clash of Kings", "George R.R. Martin", new DateTime(1998, 11, 16), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("A Storm of Swords", "George R.R. Martin", new DateTime(2000, 8, 8), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Way of Kings", "Brandon Sanderson", new DateTime(2010, 8, 31), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Mistborn: The Final Empire", "Brandon Sanderson", new DateTime(2006, 7, 17), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Name of the Wind", "Patrick Rothfuss", new DateTime(2007, 3, 27), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Wise Man's Fear", "Patrick Rothfuss", new DateTime(2011, 3, 1), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Lion, the Witch and the Wardrobe", "C.S. Lewis", new DateTime(1950, 10, 16), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("American Gods", "Neil Gaiman", new DateTime(2001, 6, 19), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Neverwhere", "Neil Gaiman", new DateTime(1996, 9, 1), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Colour of Magic", "Terry Pratchett", new DateTime(1983, 11, 24), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Mort", "Terry Pratchett", new DateTime(1987, 11, 1), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Priory of the Orange Tree", "Samantha Shannon", new DateTime(2019, 2, 26), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Fifth Season", "N.K. Jemisin", new DateTime(2015, 8, 4), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Poppy War", "R.F. Kuang", new DateTime(2018, 5, 1), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Ocean at the End of the Lane", "Neil Gaiman", new DateTime(2013, 6, 18), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Stardust", "Neil Gaiman", new DateTime(1999, 4, 1), GENRE_FILE.FANTASY_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),

    // Detective
    new Book("A Study in Scarlet", "Arthur Conan Doyle", new DateTime(1887, 11, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Sign of the Four", "Arthur Conan Doyle", new DateTime(1890, 2, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Adventures of Sherlock Holmes", "Arthur Conan Doyle", new DateTime(1892, 10, 14), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Hound of the Baskervilles", "Arthur Conan Doyle", new DateTime(1902, 3, 25), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Mysterious Affair at Styles", "Agatha Christie", new DateTime(1920, 10, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Murder of Roger Ackroyd", "Agatha Christie", new DateTime(1926, 6, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Murder on the Orient Express", "Agatha Christie", new DateTime(1934, 1, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Death on the Nile", "Agatha Christie", new DateTime(1937, 11, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("And Then There Were None", "Agatha Christie", new DateTime(1939, 11, 6), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The ABC Murders", "Agatha Christie", new DateTime(1936, 1, 6), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Big Sleep", "Raymond Chandler", new DateTime(1939, 2, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Farewell, My Lovely", "Raymond Chandler", new DateTime(1940, 10, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Long Goodbye", "Raymond Chandler", new DateTime(1953, 3, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Maltese Falcon", "Dashiell Hammett", new DateTime(1930, 2, 14), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Thin Man", "Dashiell Hammett", new DateTime(1934, 1, 8), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("In the Woods", "Tana French", new DateTime(2007, 5, 17), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Likeness", "Tana French", new DateTime(2008, 7, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Cuckoo's Calling", "Robert Galbraith", new DateTime(2013, 4, 30), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Silkworm", "Robert Galbraith", new DateTime(2014, 6, 19), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Magpie Murders", "Anthony Horowitz", new DateTime(2016, 10, 6), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Word Is Murder", "Anthony Horowitz", new DateTime(2018, 8, 28), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Moonstone", "Wilkie Collins", new DateTime(1868, 1, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The No. 1 Ladies' Detective Agency", "Alexander McCall Smith", new DateTime(1998, 1, 1), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Da Vinci Code", "Dan Brown", new DateTime(2003, 3, 18), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Angels & Demons", "Dan Brown", new DateTime(2000, 5, 29), GENRE_FILE.DETECTIVE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),

    // Romance
    new Book("Pride and Prejudice", "Jane Austen", new DateTime(1813, 1, 28), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Sense and Sensibility", "Jane Austen", new DateTime(1811, 1, 1), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Jane Eyre", "Charlotte Bronte", new DateTime(1847, 10, 16), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Outlander", "Diana Gabaldon", new DateTime(1991, 6, 1), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Notebook", "Nicholas Sparks", new DateTime(1996, 10, 1), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("A Walk to Remember", "Nicholas Sparks", new DateTime(1999, 10, 1), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Me Before You", "Jojo Moyes", new DateTime(2012, 1, 5), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Fault in Our Stars", "John Green", new DateTime(2012, 1, 10), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Red, White & Royal Blue", "Casey McQuiston", new DateTime(2019, 5, 14), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Hating Game", "Sally Thorne", new DateTime(2016, 8, 9), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Beach Read", "Emily Henry", new DateTime(2020, 5, 19), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Book Lovers", "Emily Henry", new DateTime(2022, 5, 3), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("People We Meet on Vacation", "Emily Henry", new DateTime(2021, 5, 11), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("It Ends with Us", "Colleen Hoover", new DateTime(2016, 8, 2), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Verity", "Colleen Hoover", new DateTime(2018, 12, 7), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Ugly Love", "Colleen Hoover", new DateTime(2014, 11, 4), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Duke and I", "Julia Quinn", new DateTime(2000, 1, 1), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Viscount Who Loved Me", "Julia Quinn", new DateTime(2000, 12, 1), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Bridgerton: An Offer From a Gentleman", "Julia Quinn", new DateTime(2001, 9, 1), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Normal People", "Sally Rooney", new DateTime(2018, 8, 28), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Conversations with Friends", "Sally Rooney", new DateTime(2017, 5, 25), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Spanish Love Deception", "Elena Armas", new DateTime(2021, 2, 22), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Twisted Love", "Ana Huang", new DateTime(2021, 4, 22), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Redwood Love", "Kylie Scott", new DateTime(2017, 6, 13), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Wuthering Heights", "Emily Bronte", new DateTime(1847, 12, 1), GENRE_FILE.ROMANCE_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),

    // Thriller
    new Book("The Silence of the Lambs", "Thomas Harris", new DateTime(1988, 5, 1), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Red Dragon", "Thomas Harris", new DateTime(1981, 10, 1), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Gone Girl", "Gillian Flynn", new DateTime(2012, 5, 24), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Sharp Objects", "Gillian Flynn", new DateTime(2006, 9, 26), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Dark Places", "Gillian Flynn", new DateTime(2009, 5, 5), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Girl on the Train", "Paula Hawkins", new DateTime(2015, 1, 13), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Into the Water", "Paula Hawkins", new DateTime(2017, 5, 2), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Girl with the Dragon Tattoo", "Stieg Larsson", new DateTime(2005, 8, 1), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Girl Who Played with Fire", "Stieg Larsson", new DateTime(2006, 6, 26), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("The Girl Who Kicked the Hornets' Nest", "Stieg Larsson", new DateTime(2007, 5, 25), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Shutter Island", "Dennis Lehane", new DateTime(2003, 4, 15), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE),
    new Book("Mystic River", "Dennis Lehane", new DateTime(2001, 2, 1), GENRE_FILE.THRILLER_FILE, STATUS_FILE.HAVENOTREAD_FILE, RATING_FILE.POOR_RATING_FILE)
};




while (true)
{
    try
    {
        Console.WriteLine("Welcome!! pls sign-up or log-in to accsess your DIGITAL LIBRARY ");

        Console.WriteLine("1.Log-In");

        Console.WriteLine("2.Sign-In");

        int input = Convert.ToInt32(Console.ReadLine());


        switch (input)
        {
            case 1:
                Console.Write("Enter Your Name:");

                string name = Console.ReadLine();

                Console.Write("Enter Your Password:");
                string password = Console.ReadLine();

                // Search the list for a user matching both name and password
                var loggedInUser = users.FirstOrDefault(u => u.Name == name && u.Password == password);

                if (loggedInUser == null)
                {
                    // Throws an error (stops execution and crashes with an exception)
                    throw new UnauthorizedAccessException("Error: Invalid username or password!");
                }
                else
                {
                    // Success!
                    Console.WriteLine($"Login successful! Welcome To Your Digital Library : {loggedInUser.Name}.");

                    Console.WriteLine("Select Your");

                }


                break;


            case 2:

                Console.Write("Enter Your Name:");

                string name1 = Console.ReadLine();

                Console.Write("Enter Your Password:");

                string password1 = Console.ReadLine();

                new User(name1, password1);

                users.Add(new User(name1, password1));

                break;

        }


    }

    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
    }
}
