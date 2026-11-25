using Spectre.Console;

namespace C_Project.Classes
{
    internal class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public List<Book> PersonalBooks { get; set; } = new List<Book>();

        public User(string name, string password)
        {
            Id = new Random().Next(1, 1000);
            Name = name;
            Password = password;
        }

        public bool ChangePassword()
        {
            AnsiConsole.Write(new Rule("[yellow]Change Password[/]").RuleStyle("yellow"));

            string currentPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter your [bold yellow]Current Password[/]:")
                    .PromptStyle("yellow")
                    .Secret()
                    .AllowEmpty());

            if (currentPassword != Password)
            {
                AnsiConsole.MarkupLine("[red]Current password is incorrect.[/]");
                return false;
            }

            string newPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter your [bold green]New Password[/]:")
                    .PromptStyle("green")
                    .Secret()
                    .AllowEmpty());

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                AnsiConsole.MarkupLine("[red]New password cannot be empty.[/]");
                return false;
            }

            Password = newPassword;
            AnsiConsole.MarkupLine("[green]Password changed successfully![/]");
            return true;
        }

        public override string ToString()
        {
            return $"{Name}|{Password}|";
        }
    }
}