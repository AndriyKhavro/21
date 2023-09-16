using MySqlTest;
using MySqlTest.Library;
using Polly;

int batchSize = 10000;
int totalUsers = 40000000;

var dataGenerator = new DataGenerator();
using var userRepository = new UserRepository("Server=localhost;Port=4406;Database=mydb;Uid=root;Pwd=111;");
userRepository.OpenConnection();

var policy = Policy.Handle<Exception>().RetryForever();

for (int i = 0; i < totalUsers; i += batchSize)
{
    var batch = dataGenerator.GenerateUserBatch(i, batchSize);
    Console.WriteLine($"Generated batch #{i}");
    policy.Execute(() => userRepository.InsertBatch(batch));
    Console.WriteLine($"Inserted batch #{i}");
}
