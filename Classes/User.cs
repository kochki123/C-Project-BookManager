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

        public override string ToString()
        {
            return $"{Name}|{Password}|";
        }
    }
}
