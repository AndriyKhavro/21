namespace MySqlTest;

internal class DataGenerator
{
    public List<User> GenerateUserBatch(int startId, int batchSize)
    {
        var usersBatch = new List<User>();
        for (int i = startId; i < startId + batchSize; i++)
        {
            string username = $"user{i}";
            string email = $"user{i}@example.com";
            string firstName = $"First{i}";
            string lastName = $"Last{i}";

            usersBatch.Add(new User(username, email, firstName, lastName, GetRandomDate()));
        }
        return usersBatch;
    }

    private static DateTime GetRandomDate()
    {
        // Create a random number generator
        var random = new Random();

        // Get the current year
        int currentYear = DateTime.Now.Year;

        // Generate a random year between 1900 and the current year
        int randomYear = random.Next(1900, currentYear + 1);

        // Generate a random month between 1 and 12
        int randomMonth = random.Next(1, 13);

        // Generate a random day between 1 and the maximum number of days in the selected month
        int randomDay = random.Next(1, DateTime.DaysInMonth(randomYear, randomMonth) + 1);

        // Create a random date using the generated year, month, and day
        return new DateTime(randomYear, randomMonth, randomDay);
    }
}