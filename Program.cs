using C_Project.Classes;
using C_Project.EnumHelpers;
using C_Project.Enums;
using C_Project.JsonHelpers;
using Spectre.Console;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

List<User> admins = new List<User>();
List<Book> bookList = new List<Book>();
List<User> users = new List<User>();
Dictionary<string, List<Book>> favoriteBooksByUser = new Dictionary<string, List<Book>>(StringComparer.OrdinalIgnoreCase);

ResetScreen("Opening the Reading Room & Archives");

await AnsiConsole.Status()
    .StartAsync("[wheat1]Unrolling catalog scrolls and member ledgers...[/]", async ctx =>
    {
        ctx.Spinner(Spinner.Known.Dots);
        ctx.SpinnerStyle(Style.Parse("gold1"));

        users = await SafeReadList<User>(FILE_TYPE.USERS_FILE);
        bookList = await SafeReadList<Book>(FILE_TYPE.BOOKS_FILE);
        admins = await SafeReadList<User>(FILE_TYPE.ADMIN_FILE);
    });

foreach (var u in users)
{
    GetOrCreateFavorites(favoriteBooksByUser, u.Name);
}

foreach (var a in admins)
{
    GetOrCreateFavorites(favoriteBooksByUser, a.Name);
}

while (true)
{
    try
    {
        ResetScreen("Library Foyer");

        AnsiConsole.MarkupLine(
            $"[wheat1]Archives Ready:[/] [bold gold1]{bookList.Count}[/] volumes on shelf · " +
            $"[bold wheat1]{users.Count}[/] readers · [bold sandybrown]{admins.Count}[/] librarians.");
        Console.WriteLine();

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
            ResetScreen("Sign the Guest Ledger");

            string name = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [bold wheat1]Name[/]:")
                    .PromptStyle("gold1")
                    .AllowEmpty()).Trim();

            string password = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [bold wheat1]Passphrase[/]:")
                    .PromptStyle("gold1")
                    .Secret()
                    .AllowEmpty());

            var loggedInAdmin = admins.FirstOrDefault(a => a.Name == name && a.Password == password);
            var loggedInUser = users.FirstOrDefault(u => u.Name == name && u.Password == password);

            if (loggedInAdmin != null)
            {
                var adminFavorites = GetOrCreateFavorites(favoriteBooksByUser, loggedInAdmin.Name);

                bool inAdminMenu = true;
                string? lastAdminMessage = null;

                while (inAdminMenu)
                {
                    ResetScreen($"Librarian Archive Desk — {loggedInAdmin.Name}");

                    AnsiConsole.Write(
                        new Panel(
                            $"[wheat1]Welcome to the archive desk, Librarian [bold gold1]{loggedInAdmin.Name.EscapeMarkup()}[/].[/]\n" +
                            $"[grey italic]Catalog supervision & shelf curation enabled.[/]")
                            .Header("[bold gold1]🗝️  Chief Librarian Authenticated[/]")
                            .Border(BoxBorder.Double)
                            .BorderColor(Color.Gold1));

                    if (!string.IsNullOrEmpty(lastAdminMessage))
                    {
                        AnsiConsole.MarkupLine(lastAdminMessage);
                        Console.WriteLine();
                        lastAdminMessage = null;
                    }

                    string adminAction = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[bold wheat1]Choose an archive task:[/]")
                            .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                            .AddChoices(
                                "1. 📚 Inspect All Cataloged Volumes",
                                "2. ➕ Accession New Book into Catalog",
                                "3. 🗑️  Deaccession / Remove Book from Catalog",
                                "4. 🛠️  Make Another User Admin",
                                "5. 🪪  View Reader Profile",
                                "6. ⭐ Manage Top 4 Favorite Books",
                                "7. 🕯️  Log Out"));

                    if (adminAction.StartsWith("1."))
                    {
                        ResetScreen($"Master Archive Catalog ({bookList.Count} Volumes)");
                        PrintBookTable(bookList, $"Master Archive Catalog ({bookList.Count} Volumes)", Color.Gold1);
                        Console.WriteLine();

                        AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[bold wheat1]Catalog view complete:[/]")
                                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                                .AddChoices("↩️  Back to Archive Desk"));
                    }
                    else if (adminAction.StartsWith("2."))
                    {
                        ResetScreen("Accession New Book");

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
                            STATUS_FILE.HAVENOTREAD_FILE,
                            rating);

                        bookList.Add(newBook);
                        lastAdminMessage = $"[green3_1]Accessioned \"{newBook.Title.EscapeMarkup()}\" into the library catalog (status: Have Not Read)![/]";
                    }
                    else if (adminAction.StartsWith("3."))
                    {
                        ResetScreen("Deaccession Book");

                        if (bookList.Count == 0)
                        {
                            lastAdminMessage = "[red]Book not found: the library catalog is already empty.[/]";
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
                            lastAdminMessage = $"[red]Book not found: no book matching \"{titleToDelete.EscapeMarkup()}\" exists in the catalog.[/]";
                        }
                        else
                        {
                            bookList.Remove(bookToDelete);
                            foreach (var u in users)
                            {
                                u.PersonalBooks.RemoveAll(b => b.Title.Equals(bookToDelete.Title, StringComparison.OrdinalIgnoreCase));
                                GetOrCreateFavorites(favoriteBooksByUser, u.Name)
                                    .RemoveAll(b => b.Title.Equals(bookToDelete.Title, StringComparison.OrdinalIgnoreCase));
                            }
                            foreach (var a in admins)
                            {
                                a.PersonalBooks.RemoveAll(b => b.Title.Equals(bookToDelete.Title, StringComparison.OrdinalIgnoreCase));
                                GetOrCreateFavorites(favoriteBooksByUser, a.Name)
                                    .RemoveAll(b => b.Title.Equals(bookToDelete.Title, StringComparison.OrdinalIgnoreCase));
                            }

                            lastAdminMessage = $"[green3_1]Removed \"{bookToDelete.Title.EscapeMarkup()}\" from the catalog and member shelves.[/]";
                        }
                    }
                    else if (adminAction.StartsWith("4."))
                    {
                        ResetScreen("Promote User to Librarian");

                        string userNameToPromote = AnsiConsole.Prompt(
                            new TextPrompt<string>("Enter exact [bold wheat1]name[/] of user to promote:")
                                .AllowEmpty()).Trim();

                        var userToPromote = users.FirstOrDefault(u =>
                            u.Name.Equals(userNameToPromote, StringComparison.OrdinalIgnoreCase));

                        if (userToPromote == null)
                        {
                            lastAdminMessage = $"[red]User not found: no reader card named \"{userNameToPromote.EscapeMarkup()}\" exists.[/]";
                        }
                        else if (admins.Any(a => a.Name.Equals(userToPromote.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            lastAdminMessage = $"[gold1]\"{userToPromote.Name.EscapeMarkup()}\" is already a librarian.[/]";
                        }
                        else
                        {
                            var promotedAdmin = new User(userToPromote.Name, userToPromote.Password);
                            foreach (var b in userToPromote.PersonalBooks)
                            {
                                promotedAdmin.PersonalBooks.Add(CloneBookWithStatus(b, b.Status));
                            }
                            admins.Add(promotedAdmin);

                            var existingFavs = GetOrCreateFavorites(favoriteBooksByUser, userToPromote.Name);
                            favoriteBooksByUser[promotedAdmin.Name] = existingFavs;

                            lastAdminMessage = $"[green3_1]Promoted \"{userToPromote.Name.EscapeMarkup()}\" to librarian status![/]";
                        }
                    }
                    else if (adminAction.StartsWith("5."))
                    {
                        ViewProfileMenu(loggedInAdmin, users, admins, favoriteBooksByUser, allowOtherProfiles: true);
                    }
                    else if (adminAction.StartsWith("6."))
                    {
                        ManageFavoriteBooksMenu(loggedInAdmin, bookList, adminFavorites);
                    }
                    else if (adminAction.StartsWith("7."))
                    {
                        inAdminMenu = false;
                    }
                }
            }
            else if (loggedInUser != null)
            {
                var userFavorites = GetOrCreateFavorites(favoriteBooksByUser, loggedInUser.Name);

                bool inUserMenu = true;
                string? lastUserMessage = null;

                while (inUserMenu)
                {
                    int activeShelfCount = GetActivePersonalBooks(loggedInUser).Count;
                    ResetScreen($"Reader Study — {loggedInUser.Name}");

                    AnsiConsole.Write(
                        new Panel(
                            $"[wheat1]Pull up a leather armchair by the fire, [bold gold1]{loggedInUser.Name.EscapeMarkup()}[/].[/]\n" +
                            $"[grey italic]Personal shelf holds [bold]{loggedInUser.PersonalBooks.Count}[/] book(s) " +
                            $"([bold]{activeShelfCount}[/] Reading/Already Read) · Favorites: [bold]{userFavorites.Count}/4[/].[/]")
                            .Header("[bold green3_1]☕ Reader Session Active[/]")
                            .Border(BoxBorder.Rounded)
                            .BorderColor(Color.DarkGoldenrod));

                    if (!string.IsNullOrEmpty(lastUserMessage))
                    {
                        AnsiConsole.MarkupLine(lastUserMessage);
                        Console.WriteLine();
                        lastUserMessage = null;
                    }

                    string userAction = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[bold wheat1]Select your next chapter:[/]")
                            .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                            .AddChoices(
                                "1. 🔎 Browse & Search Main Library",
                                "2. 📕 Manage Your Personal Bookshelf & Statuses",
                                "3. ⭐ Manage Top 4 Favorite Books",
                                "4. 🪪 View Profile (Yours or Another Reader)",
                                "5. 🔐 Update Passphrase",
                                "6. 🕯️  Log Out"));

                    if (userAction.StartsWith("1."))
                    {
                        bool inLibraryMenu = true;
                        string? lastCatalogMessage = null;

                        while (inLibraryMenu)
                        {
                            ResetScreen($"Main Library Catalog ({bookList.Count} Volumes)");
                            PrintBookTable(bookList, $"Main Library Catalog ({bookList.Count} Volumes)", Color.DarkGoldenrod);
                            Console.WriteLine();

                            if (!string.IsNullOrEmpty(lastCatalogMessage))
                            {
                                AnsiConsole.MarkupLine(lastCatalogMessage);
                                Console.WriteLine();
                                lastCatalogMessage = null;
                            }

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
                                        "6. ➕ Move Book to Personal Library & Set Status",
                                        "7. ↩️  Return to Reader Study"));

                            if (libChoice.StartsWith("1."))
                            {
                                ResetScreen("Search by Title");
                                string titleQuery = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter [bold wheat1]title[/] keyword:")
                                        .AllowEmpty()).Trim();

                                var byTitle = bookList
                                    .Where(b => b.Title.Contains(titleQuery, StringComparison.OrdinalIgnoreCase))
                                    .ToList();

                                PrintBookMatches(byTitle, $"title \"{titleQuery}\"");
                                Console.WriteLine();
                                AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title("[bold wheat1]Search results:[/]")
                                        .HighlightStyle(new Style(Color.SandyBrown, decoration: Decoration.Bold))
                                        .AddChoices("↩️  Back to Main Library"));
                            }
                            else if (libChoice.StartsWith("2."))
                            {
                                ResetScreen("Search by Author");
                                string authorQuery = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter [bold wheat1]author[/] name:")
                                        .AllowEmpty()).Trim();

                                var byAuthor = bookList
                                    .Where(b => b.Author.Contains(authorQuery, StringComparison.OrdinalIgnoreCase))
                                    .ToList();

                                PrintBookMatches(byAuthor, $"author \"{authorQuery}\"");
                                Console.WriteLine();
                                AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title("[bold wheat1]Search results:[/]")
                                        .HighlightStyle(new Style(Color.SandyBrown, decoration: Decoration.Bold))
                                        .AddChoices("↩️  Back to Main Library"));
                            }
                            else if (libChoice.StartsWith("3."))
                            {
                                ResetScreen("Search by Release Year");
                                int year = AnsiConsole.Prompt(
                                    new TextPrompt<int>("Enter [bold wheat1]release year[/] (e.g., 2006):")
                                        .ValidationErrorMessage("[red]Please enter a valid year.[/]"));

                                var byYear = bookList
                                    .Where(b => b.ReleaseDate.Year == year)
                                    .ToList();

                                PrintBookMatches(byYear, $"year {year}");
                                Console.WriteLine();
                                AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title("[bold wheat1]Search results:[/]")
                                        .HighlightStyle(new Style(Color.SandyBrown, decoration: Decoration.Bold))
                                        .AddChoices("↩️  Back to Main Library"));
                            }
                            else if (libChoice.StartsWith("4."))
                            {
                                ResetScreen("Search by Genre");
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
                                Console.WriteLine();
                                AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title("[bold wheat1]Search results:[/]")
                                        .HighlightStyle(new Style(Color.SandyBrown, decoration: Decoration.Bold))
                                        .AddChoices("↩️  Back to Main Library"));
                            }
                            else if (libChoice.StartsWith("5."))
                            {
                                ResetScreen("Search by Rating");
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
                                Console.WriteLine();
                                AnsiConsole.Prompt(
                                    new SelectionPrompt<string>()
                                        .Title("[bold wheat1]Search results:[/]")
                                        .HighlightStyle(new Style(Color.SandyBrown, decoration: Decoration.Bold))
                                        .AddChoices("↩️  Back to Main Library"));
                            }
                            else if (libChoice.StartsWith("6."))
                            {
                                ResetScreen("Move Book to Personal Library");

                                if (bookList.Count == 0)
                                {
                                    lastCatalogMessage = "[red]Main library has no books left to move to your personal library.[/]";
                                    continue;
                                }

                                string bookTitleToAdd = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter exact or partial [bold wheat1]title[/] to move to your personal library:")
                                        .AllowEmpty()).Trim();

                                var catalogBook = bookList.FirstOrDefault(b =>
                                        b.Title.Equals(bookTitleToAdd, StringComparison.OrdinalIgnoreCase))
                                    ?? bookList.FirstOrDefault(b =>
                                        b.Title.Contains(bookTitleToAdd, StringComparison.OrdinalIgnoreCase));

                                if (catalogBook == null)
                                {
                                    lastCatalogMessage = $"[red]Book not found: no volume matching \"{bookTitleToAdd.EscapeMarkup()}\" exists in the main library.[/]";
                                }
                                else
                                {
                                    // Book enters personal library with default status Have Not Read first, then user sets status
                                    var personalBook = CloneBookWithStatus(catalogBook, STATUS_FILE.HAVENOTREAD_FILE);
                                    bookList.Remove(catalogBook);
                                    loggedInUser.PersonalBooks.Add(personalBook);

                                    var chosenStatus = PromptStatusAfterAdding(personalBook.Title);
                                    personalBook.Status = chosenStatus;

                                    lastCatalogMessage =
                                        $"[green3_1]Moved \"{personalBook.Title.EscapeMarkup()}\" from the main library to your personal library and set status to \"{chosenStatus.GetDisplayName().EscapeMarkup()}\"![/]";
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
                        string? lastShelfMessage = null;

                        while (inPersonalLibraryMenu)
                        {
                            ResetScreen($"{loggedInUser.Name}'s Personal Bookshelf");

                            if (loggedInUser.PersonalBooks.Count == 0)
                            {
                                AnsiConsole.Write(
                                    new Panel("[wheat1 italic]Your personal library is empty. Add books from the Main Library menu (option 1).[/]")
                                        .Header("[bold wheat1]Personal Bookshelf[/]")
                                        .Border(BoxBorder.Rounded));
                            }
                            else
                            {
                                PrintBookTable(
                                    loggedInUser.PersonalBooks,
                                    $"{loggedInUser.Name}'s Personal Bookshelf ({loggedInUser.PersonalBooks.Count} Books)",
                                    Color.DarkGoldenrod);
                            }

                            Console.WriteLine();

                            if (!string.IsNullOrEmpty(lastShelfMessage))
                            {
                                AnsiConsole.MarkupLine(lastShelfMessage);
                                Console.WriteLine();
                                lastShelfMessage = null;
                            }

                            string personalChoice = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[bold wheat1]Bookshelf Actions:[/]")
                                    .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                                    .AddChoices(
                                        "1. 🔄 Set / Change Status on a Book in Personal Library",
                                        "2. ↩️  Return Book to Main Library",
                                        "3. ↩️  Back to Reader Study"));

                            if (personalChoice.StartsWith("1."))
                            {
                                ResetScreen("Set / Change Book Status");

                                if (loggedInUser.PersonalBooks.Count == 0)
                                {
                                    lastShelfMessage = "[red]Your personal library has no books yet. Add a book from the Main Library first.[/]";
                                    continue;
                                }

                                var selectedBook = AnsiConsole.Prompt(
                                    new SelectionPrompt<Book>()
                                        .Title("Select a [bold wheat1]Book[/] in your personal library:")
                                        .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                                        .UseConverter(b => $"{b.Title} [{b.Status.GetDisplayName()}]")
                                        .AddChoices(loggedInUser.PersonalBooks));

                                var newStatus = AnsiConsole.Prompt(
                                    new SelectionPrompt<STATUS_FILE>()
                                        .Title($"Set status for [bold gold1]{selectedBook.Title.EscapeMarkup()}[/]:")
                                        .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                                        .UseConverter(s => s.GetDisplayName())
                                        .AddChoices(Enum.GetValues<STATUS_FILE>()));

                                selectedBook.Status = newStatus;
                                lastShelfMessage = $"[green3_1]Updated \"{selectedBook.Title.EscapeMarkup()}\" to \"{newStatus.GetDisplayName().EscapeMarkup()}\"![/]";
                            }
                            else if (personalChoice.StartsWith("2."))
                            {
                                ResetScreen("Return Book to Main Library");

                                if (loggedInUser.PersonalBooks.Count == 0)
                                {
                                    lastShelfMessage = "[red]Your personal library has no books to return.[/]";
                                    continue;
                                }

                                string bookTitleToReturn = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter exact or partial [bold wheat1]title[/] to return to main library:")
                                        .AllowEmpty()).Trim();

                                var bookToReturn = loggedInUser.PersonalBooks.FirstOrDefault(b =>
                                        b.Title.Equals(bookTitleToReturn, StringComparison.OrdinalIgnoreCase))
                                    ?? loggedInUser.PersonalBooks.FirstOrDefault(b =>
                                        b.Title.Contains(bookTitleToReturn, StringComparison.OrdinalIgnoreCase));

                                if (bookToReturn == null)
                                {
                                    lastShelfMessage = $"[red]Book not found: no book matching \"{bookTitleToReturn.EscapeMarkup()}\" exists in your personal library.[/]";
                                }
                                else
                                {
                                    loggedInUser.PersonalBooks.Remove(bookToReturn);
                                    bookList.Add(CloneBookWithStatus(bookToReturn, STATUS_FILE.HAVENOTREAD_FILE));
                                    lastShelfMessage = $"[green3_1]Returned \"{bookToReturn.Title.EscapeMarkup()}\" to the main library (status reset to Have Not Read).[/]";
                                }
                            }
                            else if (personalChoice.StartsWith("3."))
                            {
                                inPersonalLibraryMenu = false;
                            }
                        }
                    }
                    else if (userAction.StartsWith("3."))
                    {
                        var allAvailableForFavorites = bookList
                            .Concat(loggedInUser.PersonalBooks)
                            .ToList();
                        ManageFavoriteBooksMenu(loggedInUser, allAvailableForFavorites, userFavorites);
                    }
                    else if (userAction.StartsWith("4."))
                    {
                        ViewProfileMenu(loggedInUser, users, admins, favoriteBooksByUser, allowOtherProfiles: true);
                    }
                    else if (userAction.StartsWith("5."))
                    {
                        ResetScreen("Update Passphrase");
                        loggedInUser.ChangePassword();
                        Console.WriteLine();
                        AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[bold wheat1]Passphrase update finished:[/]")
                                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                                .AddChoices("↩️  Back to Reader Study"));
                    }
                    else if (userAction.StartsWith("6."))
                    {
                        inUserMenu = false;
                    }
                }
            }
            else
            {
                throw new UnauthorizedAccessException("User not found or passphrase incorrect: no matching reader or librarian account.");
            }
        }
        else if (mainChoice.StartsWith("2."))
        {
            ResetScreen("Issue a New Reader Card");

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
                ShowFoyerError("Reader name and passphrase cannot be empty.");
                continue;
            }

            if (users.Any(u => u.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)) ||
                admins.Any(a => a.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                ShowFoyerError($"An account with the name \"{newName.EscapeMarkup()}\" already exists.");
                continue;
            }

            var createdUser = new User(newName, newPassword);
            users.Add(createdUser);
            GetOrCreateFavorites(favoriteBooksByUser, createdUser.Name);
            ShowFoyerSuccess($"Reader card issued for [bold]{newName.EscapeMarkup()}[/]! Welcome to the reading room.");
        }
        else if (mainChoice.StartsWith("3."))
        {
            ResetScreen("Hearth & Folio Closed");
            AnsiConsole.Write(
                new Panel("[wheat1 italic]May your next book linger long after the final page. Until next time![/]")
                    .Header("[bold gold1]🕯️  Hearth & Folio Closed[/]")
                    .Border(BoxBorder.Rounded));

            return;
        }
    }
    catch (Exception ex)
    {
        ShowFoyerError(ex.Message.EscapeMarkup());
    }
}

static STATUS_FILE PromptStatusAfterAdding(string bookTitle)
{
    return AnsiConsole.Prompt(
        new SelectionPrompt<STATUS_FILE>()
            .Title($"\"{bookTitle.EscapeMarkup()}\" added with default status [bold wheat1]Have Not Read[/]. Choose status to set now:")
            .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
            .UseConverter(s => s.GetDisplayName())
            .AddChoices(
                STATUS_FILE.ALREADY_READ,
                STATUS_FILE.READING_FILE,
                STATUS_FILE.HAVENOTREAD_FILE));
}

static Book CloneBookWithStatus(Book source, STATUS_FILE status)
{
    return new Book(
        source.Title,
        source.Author,
        source.ReleaseDate,
        source.Genre,
        status,
        source.Rating)
    {
        ID = source.ID
    };
}

static bool IsActivePersonalStatus(STATUS_FILE status)
{
    return status == STATUS_FILE.ALREADY_READ || status == STATUS_FILE.READING_FILE;
}

static List<Book> GetActivePersonalBooks(User user)
{
    return user.PersonalBooks
        .Where(b => IsActivePersonalStatus(b.Status))
        .ToList();
}

static List<Book> GetOrCreateFavorites(Dictionary<string, List<Book>> favoritesMap, string userName)
{
    if (!favoritesMap.TryGetValue(userName, out var list))
    {
        list = new List<Book>();
        favoritesMap[userName] = list;
    }

    if (list.Count > 4)
    {
        list.RemoveRange(4, list.Count - 4);
    }

    return list;
}

static void ManageFavoriteBooksMenu(User currentUser, List<Book> availableBooks, List<Book> favorites)
{
    const int MaxFavorites = 4;
    string? lastFavMessage = null;

    while (true)
    {
        if (favorites.Count > MaxFavorites)
        {
            favorites.RemoveRange(MaxFavorites, favorites.Count - MaxFavorites);
        }

        ResetScreen($"Top 4 Favorite Books — {currentUser.Name}");

        if (favorites.Count == 0)
        {
            AnsiConsole.Write(
                new Panel("[wheat1 italic]No favorite books selected yet (0 / 4 slots filled). Add up to 4 favorites.[/]")
                    .Header("[bold gold1]⭐ Favorite Four (0/4)[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.DarkGoldenrod));
        }
        else
        {
            PrintBookTable(
                favorites,
                $"{currentUser.Name}'s Favorite Books ({favorites.Count}/{MaxFavorites})",
                Color.Gold1);
        }

        Console.WriteLine();

        if (!string.IsNullOrEmpty(lastFavMessage))
        {
            AnsiConsole.MarkupLine(lastFavMessage);
            Console.WriteLine();
            lastFavMessage = null;
        }

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold wheat1]Favorite Books ({favorites.Count}/{MaxFavorites} slots used):[/]")
                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                .AddChoices(
                    "1. ➕ Add Book to Favorites (Max 4)",
                    "2. 🗑️ Remove Book from Favorites",
                    "3. ↩️  Back"));

        if (choice.StartsWith("1."))
        {
            if (favorites.Count >= MaxFavorites)
            {
                lastFavMessage = $"[red]Favorite limit reached: you already have {MaxFavorites} favorite books (0 to {MaxFavorites} allowed). Remove one first.[/]";
                continue;
            }

            if (availableBooks.Count == 0)
            {
                lastFavMessage = "[red]Book not found: no books available.[/]";
                continue;
            }

            string titleQuery = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter exact or partial [bold wheat1]title[/] to favorite:")
                    .AllowEmpty()).Trim();

            var bookToFav = availableBooks.FirstOrDefault(b =>
                    b.Title.Equals(titleQuery, StringComparison.OrdinalIgnoreCase))
                ?? availableBooks.FirstOrDefault(b =>
                    b.Title.Contains(titleQuery, StringComparison.OrdinalIgnoreCase));

            if (bookToFav == null)
            {
                lastFavMessage = $"[red]Book not found: no book matching \"{titleQuery.EscapeMarkup()}\" exists.[/]";
            }
            else if (favorites.Any(b => b.Title.Equals(bookToFav.Title, StringComparison.OrdinalIgnoreCase)))
            {
                lastFavMessage = $"[gold1]\"{bookToFav.Title.EscapeMarkup()}\" is already in your favorites.[/]";
            }
            else
            {
                favorites.Add(bookToFav);
                lastFavMessage = $"[green3_1]Added \"{bookToFav.Title.EscapeMarkup()}\" to your favorites ({favorites.Count}/{MaxFavorites}).[/]";
            }
        }
        else if (choice.StartsWith("2."))
        {
            if (favorites.Count == 0)
            {
                lastFavMessage = "[red]You have 0 favorite books to remove.[/]";
                continue;
            }

            string titleToRemove = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter exact or partial [bold wheat1]title[/] to remove from favorites:")
                    .AllowEmpty()).Trim();

            var bookToRemove = favorites.FirstOrDefault(b =>
                    b.Title.Equals(titleToRemove, StringComparison.OrdinalIgnoreCase))
                ?? favorites.FirstOrDefault(b =>
                    b.Title.Contains(titleToRemove, StringComparison.OrdinalIgnoreCase));

            if (bookToRemove == null)
            {
                lastFavMessage = $"[red]Book not found: no book matching \"{titleToRemove.EscapeMarkup()}\" is in your favorites.[/]";
            }
            else
            {
                favorites.Remove(bookToRemove);
                lastFavMessage = $"[green3_1]Removed \"{bookToRemove.Title.EscapeMarkup()}\" from your favorites ({favorites.Count}/{MaxFavorites}).[/]";
            }
        }
        else if (choice.StartsWith("3."))
        {
            return;
        }
    }
}

static void ViewProfileMenu(
    User currentAccount,
    List<User> users,
    List<User> admins,
    Dictionary<string, List<Book>> favoriteBooksByUser,
    bool allowOtherProfiles)
{
    if (!allowOtherProfiles)
    {
        ResetScreen($"Profile — {currentAccount.Name}");
        PrintUserProfile(currentAccount, admins, GetOrCreateFavorites(favoriteBooksByUser, currentAccount.Name));
        Console.WriteLine();
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold wheat1]Profile view:[/]")
                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                .AddChoices("↩️  Back"));
        return;
    }

    ResetScreen("Reader Profiles");

    string profileChoice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold wheat1]Whose profile would you like to view?[/]")
            .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
            .AddChoices(
                "1. 🪪 My Profile",
                "2. 🔎 Another Reader's Profile"));

    if (profileChoice.StartsWith("1."))
    {
        ResetScreen($"Profile — {currentAccount.Name}");
        PrintUserProfile(currentAccount, admins, GetOrCreateFavorites(favoriteBooksByUser, currentAccount.Name));
        Console.WriteLine();
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold wheat1]Profile view:[/]")
                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                .AddChoices("↩️  Back"));
    }
    else if (profileChoice.StartsWith("2."))
    {
        ResetScreen("Lookup Reader Profile");

        string targetName = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter exact [bold wheat1]reader name[/]:")
                .AllowEmpty()).Trim();

        var targetUser = users.FirstOrDefault(u => u.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
            ?? admins.FirstOrDefault(a => a.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

        if (targetUser == null)
        {
            ResetScreen("Profile Not Found");
            AnsiConsole.Write(
                new Panel($"[red]User not found: no profile found for \"{targetName.EscapeMarkup()}\".[/]")
                    .Header("[bold red]Error[/]")
                    .Border(BoxBorder.Heavy)
                    .BorderColor(Color.Red));
            Console.WriteLine();
            AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold wheat1]Profile lookup:[/]")
                    .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                    .AddChoices("↩️  Back"));
            return;
        }

        ResetScreen($"Profile — {targetUser.Name}");
        PrintUserProfile(targetUser, admins, GetOrCreateFavorites(favoriteBooksByUser, targetUser.Name));
        Console.WriteLine();
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold wheat1]Profile view:[/]")
                .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
                .AddChoices("↩️  Back"));
    }
}

static void PrintUserProfile(User profileUser, List<User> admins, List<Book> favorites)
{
    bool isAdmin = admins.Any(a => a.Name.Equals(profileUser.Name, StringComparison.OrdinalIgnoreCase));
    var activePersonalBooks = GetActivePersonalBooks(profileUser);
    var alreadyReadBooks = activePersonalBooks
        .Where(b => b.Status == STATUS_FILE.ALREADY_READ)
        .ToList();

    int favCount = Math.Min(favorites.Count, 4);

    AnsiConsole.Write(
        new Panel(
            $"[wheat1]Reader:[/] [bold gold1]{profileUser.Name.EscapeMarkup()}[/] · [grey]ID #{profileUser.Id}[/]\n" +
            $"[wheat1]Role:[/] {(isAdmin ? "[bold sandybrown]Librarian (Admin)[/]" : "[wheat1]Reader[/]")}\n" +
            $"[wheat1]Already Read Books Count:[/] [bold green3_1]{alreadyReadBooks.Count}[/]\n" +
            $"[wheat1]Personal Library (Reading / Already Read):[/] [bold wheat1]{activePersonalBooks.Count}[/]\n" +
            $"[wheat1]Favorite Books:[/] [bold gold1]{favCount}[/] / 4")
            .Header($"[bold gold1]🪪  Profile — {profileUser.Name.EscapeMarkup()}[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.DarkGoldenrod));

    Console.WriteLine();
    if (favCount == 0)
    {
        AnsiConsole.MarkupLine("[wheat1 italic]Favorite Books (0/4): None selected yet.[/]");
    }
    else
    {
        PrintBookTable(
            favorites.Take(4),
            $"{profileUser.Name}'s Top Favorites ({favCount}/4)",
            Color.Gold1);
    }

    Console.WriteLine();
    if (profileUser.PersonalBooks.Count == 0)
    {
        AnsiConsole.MarkupLine("[wheat1 italic]Personal Library: No books added yet.[/]");
        return;
    }

    PrintBookTable(
        profileUser.PersonalBooks,
        $"{profileUser.Name}'s Personal Library ({profileUser.PersonalBooks.Count})",
        Color.DarkGoldenrod);
}

static void ShowFoyerError(string markupMessage)
{
    ResetScreen("Reading Room Notice");
    AnsiConsole.Write(
        new Panel($"[red]{markupMessage}[/]")
            .Header("[bold red]Error[/]")
            .Border(BoxBorder.Heavy)
            .BorderColor(Color.Red));
    Console.WriteLine();
    AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold wheat1]Notice:[/]")
            .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
            .AddChoices("↩️  Return to Foyer"));
}

static void ShowFoyerSuccess(string markupMessage)
{
    ResetScreen("Reader Card Issued");
    AnsiConsole.MarkupLine($"[green3_1]{markupMessage}[/]");
    Console.WriteLine();
    AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold wheat1]Continue:[/]")
            .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.Bold))
            .AddChoices("↩️  Return to Foyer"));
}

static void ResetScreen(string sectionTitle)
{
    AnsiConsole.Clear();
    PrintReadingRoomBanner();
    AnsiConsole.Write(new Rule($"[gold1]{sectionTitle.EscapeMarkup()}[/]").RuleStyle("darkgoldenrod"));
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

static void PrintBookMatches(List<Book> matches, string criteriaDescription)
{
    if (matches.Count == 0)
    {
        AnsiConsole.MarkupLine($"[red]Book not found: no volumes turned up for {criteriaDescription.EscapeMarkup()}.[/]");
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