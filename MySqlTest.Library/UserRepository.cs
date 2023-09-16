using MySql.Data.MySqlClient;

namespace MySqlTest.Library;

public sealed class UserRepository : IDisposable
{
    private readonly MySqlConnection _connection;
    private bool _isConnectionOpen;

    public UserRepository(string connectionString)
    {
        _connection = new MySqlConnection(connectionString);
    }

    public void OpenConnection()
    {
        _connection.Open();

        _isConnectionOpen = true;

        Console.WriteLine($"Connection opened. Connection Timeout: {_connection.ConnectionTimeout}.");
    }

    public void InsertBatch(IEnumerable<User> users)
    {
        EnsureConnectionOpened();

        using var transaction = _connection.BeginTransaction();
        using var command = CreateInsertCommand();
        command.Transaction = transaction;

        foreach (var user in users)
        {
            SetUser(command, user);

            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public void InsertOne(User user)
    {
        EnsureConnectionOpened();

        using var command = CreateInsertCommand();
        SetUser(command, user);
        command.ExecuteNonQuery();
    }

    public IReadOnlyCollection<User> SelectUsersForMonth()
    {
        EnsureConnectionOpened();

        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE date_of_birth BETWEEN '1991-08-01' AND '1991-08-31'";
        var users = new List<User>();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new User(
                reader.GetString("username"),
                reader.GetString("email"),
                reader.GetString("first_name"),
                reader.GetString("last_name"),
                reader.GetDateTime("date_of_birth")
            ));
        }

        return users;
    }

    public IReadOnlyCollection<User> SelectUsersForSpecificDate()
    {
        EnsureConnectionOpened();

        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE date_of_birth = '1991-08-24'";
        var users = new List<User>();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new User(
                reader.GetString("username"),
                reader.GetString("email"),
                reader.GetString("first_name"),
                reader.GetString("last_name"),
                reader.GetDateTime("date_of_birth")
            ));
        }

        return users;
    }

    public void CreateBtreeIndex()
    {
        EnsureConnectionOpened();

        using var command = _connection.CreateCommand();
        command.CommandText = "CREATE INDEX idx_dob ON users (date_of_birth);";
        command.CommandTimeout = 180;
        command.ExecuteNonQuery();
    }

    public void CreateHashIndex()
    {
        EnsureConnectionOpened();

        using var command = _connection.CreateCommand();
        command.CommandText = "CREATE INDEX idx_dob ON users (date_of_birth) USING HASH;";
        command.CommandTimeout = 180;
        command.ExecuteNonQuery();
    }

    public void DropIndex()
    {
        EnsureConnectionOpened();

        using var command = _connection.CreateCommand();
        command.CommandText = "DROP INDEX idx_dob ON users;";
        command.ExecuteNonQuery();
        Console.WriteLine("INDEX DROPped");
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    private void EnsureConnectionOpened()
    {
        if (!_isConnectionOpen)
        {
            OpenConnection();
        }
    }

    private static void SetUser(MySqlCommand command, User user)
    {
        command.Parameters["@username"].Value = user.Username;
        command.Parameters["@email"].Value = user.Email;
        command.Parameters["@first_name"].Value = user.FirstName;
        command.Parameters["@last_name"].Value = user.LastName;
        command.Parameters["@date_of_birth"].Value = user.DateOfBirth;
    }

    private MySqlCommand CreateInsertCommand()
    {
        var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO users (username, email, first_name, last_name, date_of_birth) " +
                              "VALUES (@username, @email, @first_name, @last_name, @date_of_birth)";

        command.Parameters.Add("@username", MySqlDbType.VarChar);
        command.Parameters.Add("@email", MySqlDbType.VarChar);
        command.Parameters.Add("@first_name", MySqlDbType.VarChar);
        command.Parameters.Add("@last_name", MySqlDbType.VarChar);
        command.Parameters.Add("@date_of_birth", MySqlDbType.Date);
        return command;
    }
}