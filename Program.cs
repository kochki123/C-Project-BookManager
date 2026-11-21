using C_Project.Classes;
using C_Project.Enums;
using C_Project.JsonHelpers;
using Spectre.Console;

// ============================================================================
// 1. STARTUP & DATA LOADING
// ============================================================================
AnsiConsole.Clear();
AnsiConsole.Write(
    new FigletText("Digital Library")
        .Centered()
        .Color(Color.Cyan1));
AnsiConsole.Write(new Rule("[grey]Loading Library Data[/]").RuleStyle("grey"));

List<User> users = new List<User>();
List<Book> bookList = new List<Book>();

await AnsiConsole.Status()
    .StartAsync("Reading users and books from JSON...", async ctx =>
    {
        ctx.Spinner(Spinner.Known.Dots);
        ctx.SpinnerStyle(Style.Parse("cyan1"));

        users = await JsonHelper<User>.ReadFromJsonAsync(FILE_TYPE.USERS_FILE);
        bookList = await JsonHelper<Book>.ReadFromJsonAsync(FILE_TYPE.BOOKS_FILE);
    });

AnsiConsole.MarkupLine($"[green]Loaded[/] [bold]{users.Count}[/] users and [bold]{bookList.Count}[/] books.");

// ============================================================================
// 2. MAIN APPLICATION LOOP (LOG-IN / SIGN-UP / EXIT)
// ============================================================================
while (true)
{
    try
    {
        Console.WriteLine();
        AnsiConsole.Write(new Rule("[cyan1]Main Entrance[/]").RuleStyle("cyan1"));

        string mainChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold cyan1]Welcome to your DIGITAL LIBRARY! What would you like to do?[/]")
                .HighlightStyle(new Style(Color.Cyan1, decoration: Decoration.Bold))
                .AddChoices("1. Log-In", "2. Sign-Up", "3. Exit"));

        // --------------------------------------------------------------------
        // 2A. LOG-IN FLOW
        // --------------------------------------------------------------------
        if (mainChoice.StartsWith("1."))
        {
            AnsiConsole.Write(new Rule("[yellow]Log-In[/]").RuleStyle("yellow"));

            string name = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [bold yellow]Your Name[/]:")
                    .PromptStyle("yellow")
                    .AllowEmpty());

            string password = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [bold yellow]Your Password[/]:")
                    .PromptStyle("yellow")
                    .Secret()
                    .AllowEmpty());

            var loggedInUser = users.FirstOrDefault(u => u.Name == name && u.Password == password);

            if (loggedInUser == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password!");
            }

            AnsiConsole.Write(
                new Panel($"Welcome back, [bold cyan1]{loggedInUser.Name.EscapeMarkup()}[/]!")
                    .Header("[green]Login Successful[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Green));

            // ----------------------------------------------------------------
            // 3. ROLE SELECTION (USER / ADMIN)
            // ----------------------------------------------------------------
            string roleChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Select Your Role:[/]")
                    .HighlightStyle(new Style(Color.Green, decoration: Decoration.Bold))
                    .AddChoices("1. User", "2. Admin"));

            if (roleChoice.StartsWith("1."))
            {
                // ------------------------------------------------------------
                // 4. USER MENU LOOP
                // ------------------------------------------------------------
                bool inUserMenu = true;
                while (inUserMenu)
                {
                    Console.WriteLine();
                    AnsiConsole.Write(new Rule($"[cyan1]User Menu — {loggedInUser.Name.EscapeMarkup()}[/]").RuleStyle("cyan1"));

                    string userAction = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[bold cyan1]Choose an option:[/]")
                            .HighlightStyle(new Style(Color.Cyan1, decoration: Decoration.Bold))
                            .AddChoices(
                                "1. Browse & Search Library",
                                "2. See Your Personal Library",
                                "3. Log Out"));

                    if (userAction.StartsWith("1."))
                    {
                        // Show full catalog in a styled table
                        PrintBookTable(bookList, $"All Library Books ({bookList.Count})", Color.Cyan1);

                        // ----------------------------------------------------
                        // 5. LIBRARY SEARCH & ADD SUBMENU LOOP
                        // ----------------------------------------------------
                        bool inLibraryMenu = true;
                        while (inLibraryMenu)
                        {
                            Console.WriteLine();
                            AnsiConsole.Write(new Rule("[magenta1]Library Actions[/]").RuleStyle("magenta1"));

                            string libChoice = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("[bold magenta1]What would you like to do in the library?[/]")
                                    .HighlightStyle(new Style(Color.Magenta1, decoration: Decoration.Bold))
                                    .AddChoices(
                                        "1. Search Book By Title",
                                        "2. Search Book By Author",
                                        "3. Search Book By Release Year",
                                        "4. Search Book By Genre",
                                        "5. Search Book By Rating",
                                        "6. Add Book To Your Personal Library",
                                        "7. Back to User Menu"));

                            // --- 5.1 Search by Title ---
                            if (libChoice.StartsWith("1."))
                            {
                                string titleQuery = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter [bold]title[/] to search:")
                                        .AllowEmpty()).Trim();

                                var byTitle = bookList
                                    .Where(b => b.Title.Contains(titleQuery, StringComparison.OrdinalIgnoreCase))
                                    .ToList();

                                PrintBookMatches(byTitle, $"title \"{titleQuery}\"");
                            }
                            // --- 5.2 Search by Author ---
                            else if (libChoice.StartsWith("2."))
                            {
                                string authorQuery = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter [bold]author[/] to search:")
                                        .AllowEmpty()).Trim();

                                var byAuthor = bookList
                                    .Where(b => b.Author.Contains(authorQuery, StringComparison.OrdinalIgnoreCase))
                                    .ToList();

                                PrintBookMatches(byAuthor, $"author \"{authorQuery}\"");
                            }
                            // --- 5.3 Search by Release Year ---
                            else if (libChoice.StartsWith("3."))
                            {
                                int year = AnsiConsole.Prompt(
                                    new TextPrompt<int>("Enter [bold]release year[/] (e.g., 2006):")
                                        .ValidationErrorMessage("[red]Please enter a valid 4-digit year.[/]"));

                                var byYear = bookList
                                    .Where(b => b.ReleaseDate.Year == year)
                                    .ToList();

                                PrintBookMatches(byYear, $"year {year}");
                            }
                            // --- 5.4 Search by Genre ---
                            else if (libChoice.StartsWith("4."))
                            {
                                var genres = Enum.GetValues<GENRE_FILE>();
                                var selectedGenre = AnsiConsole.Prompt(
                                    new SelectionPrompt<GENRE_FILE>()
                                        .Title("Select [bold]Genre[/]:")
                                        .HighlightStyle(new Style(Color.Magenta1, decoration: Decoration.Bold))
                                        .AddChoices(genres));

                                var byGenre = bookList
                                    .Where(b => b.Genre == selectedGenre)
                                    .ToList();

                                PrintBookMatches(byGenre, $"genre {selectedGenre}");
                            }
                            // --- 5.5 Search by Rating ---
                            else if (libChoice.StartsWith("5."))
                            {
                                var ratings = Enum.GetValues<RATING_FILE>();
                                var selectedRating = AnsiConsole.Prompt(
                                    new SelectionPrompt<RATING_FILE>()
                                        .Title("Select [bold]Rating[/]:")
                                        .HighlightStyle(new Style(Color.Magenta1, decoration: Decoration.Bold))
                                        .AddChoices(ratings));

                                var byRating = bookList
                                    .Where(b => b.Rating == selectedRating)
                                    .ToList();

                                PrintBookMatches(byRating, $"rating {selectedRating}");
                            }
                            // --- 5.6 Add Book to Personal Library ---
                            else if (libChoice.StartsWith("6."))
                            {
                                string bookTitleToAdd = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter exact or partial [bold]title[/] of the book to add:")
                                        .AllowEmpty()).Trim();

                                var bookToAdd = bookList.FirstOrDefault(b =>
                                        b.Title.Equals(bookTitleToAdd, StringComparison.OrdinalIgnoreCase))
                                    ?? bookList.FirstOrDefault(b =>
                                        b.Title.Contains(bookTitleToAdd, StringComparison.OrdinalIgnoreCase));

                                if (bookToAdd == null)
                                {
                                    AnsiConsole.MarkupLine($"[yellow]No book matching \"{bookTitleToAdd.EscapeMarkup()}\" was found in the library.[/]");
                                }
                                else if (loggedInUser.PersonalBooks.Contains(bookToAdd))
                                {
                                    AnsiConsole.MarkupLine($"[yellow]\"{bookToAdd.Title.EscapeMarkup()}\" is already in your personal library.[/]");
                                }
                                else
                                {
                                    loggedInUser.PersonalBooks.Add(bookToAdd);
                                    AnsiConsole.MarkupLine($"[green]Added \"{bookToAdd.Title.EscapeMarkup()}\" to your personal library![/]");
                                }
                            }
                            // --- 5.7 Back to User Menu ---
                            else if (libChoice.StartsWith("7."))
                            {
                                inLibraryMenu = false;
                            }
                        }
                    }
                    else if (userAction.StartsWith("2."))
                    {
                        // Show user's personal library
                        if (loggedInUser.PersonalBooks.Count == 0)
                        {
                            AnsiConsole.Write(
                                new Panel("[grey]Your personal library is currently empty. Add books from Browse & Search Library![/]")
                                    .Header("[cyan1]Your Personal Library[/]")
                                    .Border(BoxBorder.Rounded)
                                    .BorderColor(Color.Grey));
                        }
                        else
                        {
                            PrintBookTable(
                                loggedInUser.PersonalBooks,
                                $"{loggedInUser.Name}'s Personal Library ({loggedInUser.PersonalBooks.Count})",
                                Color.Green);
                        }
                    }
                    else if (userAction.StartsWith("3."))
                    {
                        inUserMenu = false;
                        AnsiConsole.MarkupLine("[grey]You have been logged out.[/]");
                    }
                }
            }
            else if (roleChoice.StartsWith("2."))
            {
                AnsiConsole.Write(
                    new Panel("[yellow]Admin menu is not implemented yet.[/]")
                        .Header("[yellow]Admin[/]")
                        .Border(BoxBorder.Rounded)
                        .BorderColor(Color.Yellow));
            }
        }
        // --------------------------------------------------------------------
        // 2B. SIGN-UP FLOW
        // --------------------------------------------------------------------
        else if (mainChoice.StartsWith("2."))
        {
            AnsiConsole.Write(new Rule("[green]Sign-Up[/]").RuleStyle("green"));

            string newName = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [bold green]Your Name[/]:")
                    .PromptStyle("green")
                    .AllowEmpty()).Trim();

            string newPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [bold green]Your Password[/]:")
                    .PromptStyle("green")
                    .Secret()
                    .AllowEmpty());

            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newPassword))
            {
                AnsiConsole.MarkupLine("[red]Name and password cannot be empty.[/]");
                continue;
            }

            if (users.Any(u => u.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                AnsiConsole.MarkupLine("[yellow]A user with that name already exists.[/]");
                continue;
            }

            users.Add(new User(newName, newPassword));
            AnsiConsole.MarkupLine($"[green]Account created for [bold]{newName.EscapeMarkup()}[/]! You can now log in.[/]");
        }
        // --------------------------------------------------------------------
        // 2C. EXIT APPLICATION
        // --------------------------------------------------------------------
        else if (mainChoice.StartsWith("3."))
        {
            AnsiConsole.MarkupLine("[cyan1]Goodbye! Happy reading.[/]");
            return;
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.Write(
            new Panel($"[red]{ex.Message.EscapeMarkup()}[/]")
                .Header("[red]Error[/]")
                .Border(BoxBorder.Heavy)
                .BorderColor(Color.Red));
    }
}

// ============================================================================
// 6. HELPER FUNCTIONS (TABLE & SEARCH RESULT DISPLAY)
// ============================================================================
static void PrintBookMatches(List<Book> matches, string criteriaDescription)
{
    if (matches.Count == 0)
    {
        AnsiConsole.MarkupLine($"[yellow]No books found matching {criteriaDescription.EscapeMarkup()}.[/]");
        return;
    }

    PrintBookTable(
        matches,
        $"Found {matches.Count} Book(s) — {criteriaDescription}",
        Color.Yellow);
}

static void PrintBookTable(IEnumerable<Book> books, string title, Color borderColor)
{
    var table = new Table()
        .Title($"[bold]{title.EscapeMarkup()}[/]")
        .Border(TableBorder.Rounded)
        .BorderColor(borderColor);

    table.AddColumn(new TableColumn("[bold]Title[/]"));
    table.AddColumn(new TableColumn("[bold]Author[/]"));
    table.AddColumn(new TableColumn("[bold]Year[/]").Centered());
    table.AddColumn(new TableColumn("[bold]Genre[/]"));
    table.AddColumn(new TableColumn("[bold]Status[/]"));
    table.AddColumn(new TableColumn("[bold]Rating[/]"));

    foreach (var book in books)
    {
        table.AddRow(
            book.Title.EscapeMarkup(),
            book.Author.EscapeMarkup(),
            book.ReleaseDate.Year.ToString(),
            book.Genre.ToString().EscapeMarkup(),
            book.Status.ToString().EscapeMarkup(),
            book.Rating.ToString().EscapeMarkup());
    }

    AnsiConsole.Write(table);
}