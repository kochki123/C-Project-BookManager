using C_Project.Classes;
using C_Project.EnumHelpers;
using C_Project.Enums;
using C_Project.JsonHelpers;
using Spectre.Console;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

AnsiConsole.Clear();
PrintReadingRoomBanner();
AnsiConsole.Write(new Rule("[darkgoldenrod]Opening the Reading Room & Archives[/]").RuleStyle("darkgoldenrod"));

List<User> users = new List<User>();
List<Book> bookList = new List<Book>();
List<User> admins = new List<User>();

await AnsiConsole.Status()
    .StartAsync("[wheat1]Unrolling catalog scrolls and member ledgers...[/]", async ctx =>
    {
        ctx.Spinner(Spinner.Known.Dots);
        ctx.SpinnerStyle(Style.Parse("gold1"));

        users = await SafeReadList<User>(FILE_TYPE.USERS_FILE);
        bookList = await SafeReadList<Book>(FILE_TYPE.BOOKS_FILE);
        admins = await SafeReadList<User>(FILE_TYPE.ADMIN_FILE);
    });

if (admins.Count == 0)
{
    admins = new List<User>
    {
        new User("AdminOne", "admin123"),
        new User("AdminTwo", "admin123"),
        new User("AdminThree", "admin123")
    };
}

AnsiConsole.MarkupLine(
    $"[wheat1]Archives Ready:[/] [bold gold1]{bookList.Count}[/] volumes on shelf · " +
    $"[bold wheat1]{users.Count}[/] readers · [bold sandybrown]{admins.Count}[/] librarians.");

while (true)
{
    try
    {
        PrintBookshelfDivider();
        AnsiConsole.Write(new Rule("[gold1]✦ Library Foyer ✦[/]").RuleStyle("darkgoldenrod"));

        string mainChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold wheat1]🕯️  Welcome to the Hearth & Folio Reading Room. How may I assist you?[/]")
                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                .AddChoices(
                    "1. 📖 Log-In to Your Account",
                    "2. ✨ Register a Reader Card (Sign-Up)",
                    "3. 🚪 Step Out of the Library (Exit)"));

        if (mainChoice.StartsWith("1."))
        {
            string roleChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold wheat1]🪪  Which desk are you approaching?[/]")
                    .HighlightStyle(new Style(Color.SandyBrown, decoration: Decoration.Bold))
                    .AddChoices(
                        "1. 📚 Reader Desk (User)",
                        "2. 🗝️  Chief Librarian Desk (Admin)"));

            AnsiConsole.Write(new Rule("[sandybrown]Sign the Guest Ledger[/]").RuleStyle("darkgoldenrod"));

            string name = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [bold wheat1]Name[/]:")
                    .PromptStyle("gold1")
                    .AllowEmpty()).Trim();

            string password = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [bold wheat1]Passphrase[/]:")
                    .PromptStyle("gold1")
                    .Secret()
                    .AllowEmpty());

            if (roleChoice.StartsWith("1."))
            {
                var loggedInUser = users.FirstOrDefault(u => u.Name == name && u.Password == password);

                if (loggedInUser == null)
                {
                    throw new UnauthorizedAccessException("No matching reader card found for that name and passphrase.");
                }

                AnsiConsole.Write(
                    new Panel(
                        $"[wheat1]Pull up a leather armchair by the fire, [bold gold1]{loggedInUser.Name.EscapeMarkup()}[/].[/]\n" +
                        $"[grey italic]Personal shelf currently holds [bold]{loggedInUser.PersonalBooks.Count}[/] book(s).[/]")
                        .Header("[bold green3_1]☕ Reader Session Active[/]")
                        .Border(BoxBorder.Rounded)
                        .BorderColor(Color.DarkGoldenrod));

                bool inUserMenu = true;
                while (inUserMenu)
                {
                    PrintBookshelfDivider();
                    AnsiConsole.Write(new Rule($"[gold1]📚 Reader Study — {loggedInUser.Name.EscapeMarkup()}[/]").RuleStyle("darkgoldenrod"));

                    string userAction = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[bold wheat1]Select your next chapter:[/]")
                            .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                            .AddChoices(
                                "1. 🔎 Browse & Search the Grand Catalog",
                                "2. 📕 Manage Your Personal Bookshelf",
                                "3. 🔐 Update Passphrase",
                                "4. 🕯️  Log Out"));

                    if (userAction.StartsWith("1."))
                    {
                        PrintBookTable(bookList, $"Grand Library Catalog ({bookList.Count} Volumes)", Color.DarkGoldenrod);

                        bool inLibraryMenu = true;
                        while (inLibraryMenu)
                        {
                            Console.WriteLine();
                            AnsiConsole.Write(new Rule("[sandybrown]Catalog Index & Discovery[/]").RuleStyle("darkgoldenrod"));

                            string libChoice = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[bold wheat1]How would you like to search the shelves?[/]")
                                    .HighlightStyle(new Style(Color.SandyBrown, decoration: Decoration.Bold))
                                    .AddChoices(
                                        "1. 🏷️  Search by Title",
                                        "2. ✍️  Search by Author",
                                        "3. 📅 Search by Release Year",
                                        "4. 🎭 Search by Genre",
                                        "5. ⭐ Search by Rating",
                                        "6. ➕ Shelve Book in Personal Library",
                                        "7. ↩️  Return to Reader Study"));

                            if (libChoice.StartsWith("1."))
                            {
                                string titleQuery = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter [bold wheat1]title[/] keyword:")
                                        .AllowEmpty()).Trim();

                                var byTitle = bookList
                                    .Where(b => b.Title.Contains(titleQuery, StringComparison.OrdinalIgnoreCase))
                                    .ToList();

                                PrintBookMatches(byTitle, $"title \"{titleQuery}\"");
                            }
                            else if (libChoice.StartsWith("2."))
                            {
                                string authorQuery = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter [bold wheat1]author[/] name:")
                                        .AllowEmpty()).Trim();

                                var byAuthor = bookList
                                    .Where(b => b.Author.Contains(authorQuery, StringComparison.OrdinalIgnoreCase))
                                    .ToList();

                                PrintBookMatches(byAuthor, $"author \"{authorQuery}\"");
                            }
                            else if (libChoice.StartsWith("3."))
                            {
                                int year = AnsiConsole.Prompt(
                                    new TextPrompt<int>("Enter [bold wheat1]release year[/] (e.g., 2006):")
                                        .ValidationErrorMessage("[red]Please enter a valid year.[/]"));

                                var byYear = bookList
                                    .Where(b => b.ReleaseDate.Year == year)
                                    .ToList();

                                PrintBookMatches(byYear, $"year {year}");
                            }
                            else if (libChoice.StartsWith("4."))
                            {
                                var selectedGenre = AnsiConsole.Prompt(
                                    new SelectionPrompt<GENRE_FILE>()
                                        .Title("Choose a [bold wheat1]Genre[/]:")
                                        .HighlightStyle(new Style(Color.SandyBrown, decoration: Decoration.Bold))
                                        .UseConverter(g => g.GetDisplayName())
                                        .AddChoices(Enum.GetValues<GENRE_FILE>()));

                                var byGenre = bookList
                                    .Where(b => b.Genre == selectedGenre)
                                    .ToList();

                                PrintBookMatches(byGenre, $"genre {selectedGenre.GetDisplayName()}");
                            }
                            else if (libChoice.StartsWith("5."))
                            {
                                var selectedRating = AnsiConsole.Prompt(
                                    new SelectionPrompt<RATING_FILE>()
                                        .Title("Choose a [bold wheat1]Rating[/]:")
                                        .HighlightStyle(new Style(Color.SandyBrown, decoration: Decoration.Bold))
                                        .UseConverter(r => r.GetDisplayName())
                                        .AddChoices(Enum.GetValues<RATING_FILE>()));

                                var byRating = bookList
                                    .Where(b => b.Rating == selectedRating)
                                    .ToList();

                                PrintBookMatches(byRating, $"rating {selectedRating.GetDisplayName()}");
                            }
                            else if (libChoice.StartsWith("6."))
                            {
                                string bookTitleToAdd = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter exact or partial [bold wheat1]title[/] to add:")
                                        .AllowEmpty()).Trim();

                                var bookToAdd = bookList.FirstOrDefault(b =>
                                        b.Title.Equals(bookTitleToAdd, StringComparison.OrdinalIgnoreCase))
                                    ?? bookList.FirstOrDefault(b =>
                                        b.Title.Contains(bookTitleToAdd, StringComparison.OrdinalIgnoreCase));

                                if (bookToAdd == null)
                                {
                                    AnsiConsole.MarkupLine($"[gold1]No volume matching \"{bookTitleToAdd.EscapeMarkup()}\" was found in the catalog.[/]");
                                }
                                else if (loggedInUser.PersonalBooks.Contains(bookToAdd))
                                {
                                    AnsiConsole.MarkupLine($"[gold1]\"{bookToAdd.Title.EscapeMarkup()}\" is already on your personal shelf.[/]");
                                }
                                else
                                {
                                    loggedInUser.PersonalBooks.Add(bookToAdd);
                                    AnsiConsole.MarkupLine($"[green3_1]Shelved \"{bookToAdd.Title.EscapeMarkup()}\" in your personal library![/]");
                                }
                            }
                            else if (libChoice.StartsWith("7."))
                            {
                                inLibraryMenu = false;
                            }
                        }
                    }
                    else if (userAction.StartsWith("2."))
                    {
                        bool inPersonalLibraryMenu = true;
                        while (inPersonalLibraryMenu)
                        {
                            Console.WriteLine();
                            AnsiConsole.Write(
                                new Rule($"[gold1]📕 {loggedInUser.Name.EscapeMarkup()}'s Personal Bookshelf[/]")
                                    .RuleStyle("darkgoldenrod"));

                            if (loggedInUser.PersonalBooks.Count == 0)
                            {
                                AnsiConsole.Write(
                                    new Panel("[wheat1 italic]Your personal shelf is quiet and empty. Browse the catalog to pick your first read![/]")
                                        .Header("[bold wheat1]Personal Bookshelf[/]")
                                        .Border(BoxBorder.Rounded));
                            }
                            else
                            {
                                PrintBookTable(
                                    loggedInUser.PersonalBooks,
                                    $"{loggedInUser.Name}'s Personal Bookshelf ({loggedInUser.PersonalBooks.Count})",
                                    Color.DarkGoldenrod);
                            }

                            string personalChoice = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[bold wheat1]Bookshelf Actions:[/]")
                                    .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                                    .AddChoices(
                                        "1. ➕ Add Book to Personal Shelf",
                                        "2. 🗑️ Remove Book from Personal Shelf",
                                        "3. 🛠️ Make Another User Admin",
                                        "4. ↩️  Back to Reader Study"));

                            if (personalChoice.StartsWith("1."))
                            {
                                if (bookList.Count == 0)
                                {
                                    AnsiConsole.MarkupLine("[gold1]The main catalog has no books available right now.[/]");
                                    continue;
                                }

                                string bookTitleToAdd = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter exact or partial [bold wheat1]title[/] to add:")
                                        .AllowEmpty()).Trim();

                                var bookToAdd = bookList.FirstOrDefault(b =>
                                        b.Title.Equals(bookTitleToAdd, StringComparison.OrdinalIgnoreCase))
                                    ?? bookList.FirstOrDefault(b =>
                                        b.Title.Contains(bookTitleToAdd, StringComparison.OrdinalIgnoreCase));

                                if (bookToAdd == null)
                                {
                                    AnsiConsole.MarkupLine($"[gold1]No book matching \"{bookTitleToAdd.EscapeMarkup()}\" was found.[/]");
                                }
                                else if (loggedInUser.PersonalBooks.Contains(bookToAdd))
                                {
                                    AnsiConsole.MarkupLine($"[gold1]\"{bookToAdd.Title.EscapeMarkup()}\" is already on your shelf.[/]");
                                }
                                else
                                {
                                    loggedInUser.PersonalBooks.Add(bookToAdd);
                                    AnsiConsole.MarkupLine($"[green3_1]Added \"{bookToAdd.Title.EscapeMarkup()}\" to your personal shelf![/]");
                                }
                            }
                            else if (personalChoice.StartsWith("2."))
                            {
                                if (loggedInUser.PersonalBooks.Count == 0)
                                {
                                    AnsiConsole.MarkupLine("[gold1]Your personal shelf has no books to remove.[/]");
                                    continue;
                                }

                                string bookTitleToDelete = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter exact or partial [bold wheat1]title[/] to remove:")
                                        .AllowEmpty()).Trim();

                                var bookToDelete = loggedInUser.PersonalBooks.FirstOrDefault(b =>
                                        b.Title.Equals(bookTitleToDelete, StringComparison.OrdinalIgnoreCase))
                                    ?? loggedInUser.PersonalBooks.FirstOrDefault(b =>
                                        b.Title.Contains(bookTitleToDelete, StringComparison.OrdinalIgnoreCase));

                                if (bookToDelete == null)
                                {
                                    AnsiConsole.MarkupLine($"[gold1]No book matching \"{bookTitleToDelete.EscapeMarkup()}\" was found on your shelf.[/]");
                                }
                                else
                                {
                                    loggedInUser.PersonalBooks.Remove(bookToDelete);
                                    AnsiConsole.MarkupLine($"[green3_1]Removed \"{bookToDelete.Title.EscapeMarkup()}\" from your personal shelf.[/]");
                                }
                            }

                            else if (personalChoice.StartsWith("3."))
                            {
                                string userNameToPromote = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter exact [bold wheat1]name[/] of user to promote:")
                                        .AllowEmpty()).Trim();
                                var userToPromote = users.FirstOrDefault(u =>
                                    u.Name.Equals(userNameToPromote, StringComparison.OrdinalIgnoreCase));
                                if (userToPromote == null)
                                {
                                    AnsiConsole.MarkupLine($"[gold1]No reader card found for \"{userNameToPromote.EscapeMarkup()}\".[/]");
                                }
                                else if (admins.Any(a => a.Name.Equals(userToPromote.Name, StringComparison.OrdinalIgnoreCase)))
                                {
                                    AnsiConsole.MarkupLine($"[gold1]\"{userToPromote.Name.EscapeMarkup()}\" is already a librarian.[/]");
                                }
                                else
                                {
                                    admins.Add(new User(userToPromote.Name, userToPromote.Password));
                                    AnsiConsole.MarkupLine($"[green3_1]Promoted \"{userToPromote.Name.EscapeMarkup()}\" to librarian status![/]");
                                }
                            }

                            else if (personalChoice.StartsWith("4."))
                            {
                                inPersonalLibraryMenu = false;
                            }
                        }
                    }
                    else if (userAction.StartsWith("3."))
                    {
                        loggedInUser.ChangePassword();
                    }
                    else if (userAction.StartsWith("4."))
                    {
                        inUserMenu = false;
                        AnsiConsole.MarkupLine("[wheat1 italic]Bookmarks placed. You have logged out of the study.[/]");
                    }
                }
            }
            else if (roleChoice.StartsWith("2."))
            {
                var loggedInAdmin = admins.FirstOrDefault(a => a.Name == name && a.Password == password);

                if (loggedInAdmin == null)
                {
                    throw new UnauthorizedAccessException("Invalid librarian credentials.");
                }

                AnsiConsole.Write(
                    new Panel(
                        $"[wheat1]Welcome to the archive desk, Librarian [bold gold1]{loggedInAdmin.Name.EscapeMarkup()}[/].[/]\n" +
                        $"[grey italic]Catalog supervision & shelf curation enabled.[/]")
                        .Header("[bold gold1]🗝️  Chief Librarian Authenticated[/]")
                        .Border(BoxBorder.Double)
                        .BorderColor(Color.Gold1));

                bool inAdminMenu = true;
                while (inAdminMenu)
                {
                    PrintBookshelfDivider();
                    AnsiConsole.Write(new Rule($"[gold1]🗝️  Librarian Archive Desk — {loggedInAdmin.Name.EscapeMarkup()}[/]").RuleStyle("darkgoldenrod"));

                    string adminAction = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[bold wheat1]Choose an archive task:[/]")
                            .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                            .AddChoices(
                                "1. 📚 Inspect All Cataloged Volumes",
                                "2. ➕ Accession New Book into Catalog",
                                "3. 🗑️  Deaccession / Remove Book from Catalog",
                                "4. 🕯️  Log Out"));

                    if (adminAction.StartsWith("1."))
                    {
                        PrintBookTable(bookList, $"Master Archive Catalog ({bookList.Count} Volumes)", Color.Gold1);
                    }
                    else if (adminAction.StartsWith("2."))
                    {
                        string title = AnsiConsole.Prompt(
                            new TextPrompt<string>("Enter [bold wheat1]Book Title[/]:")
                                .PromptStyle("gold1")).Trim();

                        string author = AnsiConsole.Prompt(
                            new TextPrompt<string>("Enter [bold wheat1]Author[/]:")
                                .PromptStyle("gold1")).Trim();

                        int year = AnsiConsole.Prompt(
                            new TextPrompt<int>("Enter [bold wheat1]Release Year[/] (e.g., 2020):")
                                .ValidationErrorMessage("[red]Please enter a valid year.[/]"));

                        var genre = AnsiConsole.Prompt(
                            new SelectionPrompt<GENRE_FILE>()
                                .Title("Select [bold wheat1]Genre[/]:")
                                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                                .UseConverter(g => g.GetDisplayName())
                                .AddChoices(Enum.GetValues<GENRE_FILE>()));

                        var status = AnsiConsole.Prompt(
                            new SelectionPrompt<STATUS_FILE>()
                                .Title("Select [bold wheat1]Reading Status[/]:")
                                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                                .UseConverter(s => s.GetDisplayName())
                                .AddChoices(Enum.GetValues<STATUS_FILE>()));

                        var rating = AnsiConsole.Prompt(
                            new SelectionPrompt<RATING_FILE>()
                                .Title("Select [bold wheat1]Rating[/]:")
                                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                                .UseConverter(r => r.GetDisplayName())
                                .AddChoices(Enum.GetValues<RATING_FILE>()));

                        var newBook = new Book(
                            title,
                            author,
                            new DateTime(year, 1, 1),
                            genre,
                            status,
                            rating);

                        bookList.Add(newBook);
                        AnsiConsole.MarkupLine($"[green3_1]Accessioned \"{newBook.Title.EscapeMarkup()}\" into the library catalog![/]");
                    }
                    else if (adminAction.StartsWith("3."))
                    {
                        if (bookList.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[gold1]The library catalog is already empty.[/]");
                            continue;
                        }

                        string titleToDelete = AnsiConsole.Prompt(
                            new TextPrompt<string>("Enter exact or partial [bold wheat1]title[/] to remove:")
                                .AllowEmpty()).Trim();

                        var bookToDelete = bookList.FirstOrDefault(b =>
                                b.Title.Equals(titleToDelete, StringComparison.OrdinalIgnoreCase))
                            ?? bookList.FirstOrDefault(b =>
                                b.Title.Contains(titleToDelete, StringComparison.OrdinalIgnoreCase));

                        if (bookToDelete == null)
                        {
                            AnsiConsole.MarkupLine($"[gold1]No book matching \"{titleToDelete.EscapeMarkup()}\" was found in the catalog.[/]");
                        }
                        else
                        {
                            bookList.Remove(bookToDelete);
                            foreach (var u in users)
                            {
                                u.PersonalBooks.Remove(bookToDelete);
                            }

                            AnsiConsole.MarkupLine($"[green3_1]Removed \"{bookToDelete.Title.EscapeMarkup()}\" from the catalog and member shelves.[/]");
                        }
                    }
                    else if (adminAction.StartsWith("4."))
                    {
                        inAdminMenu = false;
                        AnsiConsole.MarkupLine("[wheat1 italic]Archive keys returned. You have logged out.[/]");
                    }
                }
            }
        }
        else if (mainChoice.StartsWith("2."))
        {
            AnsiConsole.Write(new Rule("[wheat1]Issue a New Reader Card[/]").RuleStyle("darkgoldenrod"));

            string newName = AnsiConsole.Prompt(
                new TextPrompt<string>("Choose a [bold wheat1]Reader Name[/]:")
                    .PromptStyle("gold1")
                    .AllowEmpty()).Trim();

            string newPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("Create a [bold wheat1]Passphrase[/]:")
                    .PromptStyle("gold1")
                    .Secret()
                    .AllowEmpty());

            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newPassword))
            {
                AnsiConsole.MarkupLine("[red]Reader name and passphrase cannot be empty.[/]");
                continue;
            }

            if (users.Any(u => u.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                AnsiConsole.MarkupLine("[gold1]A reader card with that name is already registered.[/]");
                continue;
            }

            users.Add(new User(newName, newPassword));
            AnsiConsole.MarkupLine($"[green3_1]Reader card issued for [bold]{newName.EscapeMarkup()}[/]! Welcome to the reading room.[/]");
        }
        else if (mainChoice.StartsWith("3."))
        {
            Console.WriteLine();
            AnsiConsole.Write(
                new Panel("[wheat1 italic]May your next book linger long after the final page. Until next time![/]")
                    .Header("[bold gold1]🕯️  Hearth & Folio Closed[/]")
                    .Border(BoxBorder.Rounded));

            return;
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.Write(
            new Panel($"[red]{ex.Message.EscapeMarkup()}[/]")
                .Header("[red]Reading Room Notice[/]")
                .Border(BoxBorder.Heavy)
                .BorderColor(Color.Red));
    }
}

static async Task<List<T>> SafeReadList<T>(FILE_TYPE fileType)
{
    try
    {
        return await JsonHelper<T>.ReadFromJsonAsync(fileType);
    }
    catch
    {
        return new List<T>();
    }
}

static void PrintReadingRoomBanner()
{
    AnsiConsole.MarkupLine("[darkgoldenrod]╔══════════════════════════════════════════════════════════════════════════╗[/]");
    AnsiConsole.MarkupLine("[darkgoldenrod]║[/] [gold1 bold]        🕯️  HEARTH & FOLIO  ·  DIGITAL READING ROOM & ARCHIVE  🕯️        [/] [darkgoldenrod]║[/]");
    AnsiConsole.MarkupLine("[darkgoldenrod]║[/] [wheat1 italic]              \"A room without books is like a body without a soul.\"         [/] [darkgoldenrod]║[/]");
    AnsiConsole.MarkupLine("[darkgoldenrod]╚══════════════════════════════════════════════════════════════════════════╝[/]");
}

static void PrintBookshelfDivider()
{
    Console.WriteLine();
    AnsiConsole.MarkupLine("[darkgoldenrod]━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━[/]");
    AnsiConsole.MarkupLine("[darkgoldenrod]  │█║▌│█║▌│█║▌│  [wheat1]✦  Quiet Shelves & Warm Lanterns  ✦[/]  │█║▌│█║▌│█║▌│[/]");
    AnsiConsole.MarkupLine("[darkgoldenrod]━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━[/]");
}

static void PrintBookMatches(List<Book> matches, string criteriaDescription)
{
    if (matches.Count == 0)
    {
        AnsiConsole.MarkupLine($"[gold1]No volumes turned up for {criteriaDescription.EscapeMarkup()}.[/]");
        return;
    }

    PrintBookTable(
        matches,
        $"Catalog Matches ({matches.Count}) — {criteriaDescription}",
        Color.SandyBrown);
}

static void PrintBookTable(IEnumerable<Book> books, string title, Color borderColor)
{
    var table = new Table()
        .Title($"[bold gold1]📖 {title.EscapeMarkup()}[/]")
        .Border(TableBorder.Rounded)
        .BorderColor(borderColor);

    table.AddColumn(new TableColumn("[bold wheat1]Title[/]"));
    table.AddColumn(new TableColumn("[bold wheat1]Author[/]"));
    table.AddColumn(new TableColumn("[bold sandybrown]Year[/]").Centered());
    table.AddColumn(new TableColumn("[bold gold1]Genre[/]"));
    table.AddColumn(new TableColumn("[bold wheat1]Status[/]"));
    table.AddColumn(new TableColumn("[bold gold1]Rating[/]"));

    foreach (var book in books)
    {
        table.AddRow(
            $"[wheat1]{book.Title.EscapeMarkup()}[/]",
            $"[wheat1]{book.Author.EscapeMarkup()}[/]",
            $"[sandybrown]{book.ReleaseDate.Year}[/]",
            $"[gold1]{book.Genre.GetDisplayName().EscapeMarkup()}[/]",
            $"[wheat1]{book.Status.GetDisplayName().EscapeMarkup()}[/]",
            $"[gold1]{book.Rating.GetDisplayName().EscapeMarkup()}[/]");
    }

    AnsiConsole.Write(table);
}