namespace MySqlTest;

public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }

    public User(string username, string email, string firstName, string lastName, DateTime dateOfBirth)
    {
        Username = username;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }
}