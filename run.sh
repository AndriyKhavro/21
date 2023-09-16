create_table_script=$(cat "./CreateTable.sql")
docker exec mysql_master sh -c "export MYSQL_PWD=111; mysql -u root mydb -e '$create_table_script \G'"

dotnet run --project ./MySqlTest.DataGenerator/MySqlTest.DataGenerator.csproj