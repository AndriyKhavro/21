Docker MySQL master-slave replication 
========================

MySQL 8.0 master-slave replication with using Docker. 

Based on https://github.com/vbabak/docker-mysql-master-slave.

## Run

To run this examples you will need to start containers with "docker-compose" 
and after starting setup replication. See commands inside ./build.sh. 

#### Create 1 master and 2 slave MySQL containers with master-slave row-based replication 

```bash
./build.sh
```

#### Run a script that writes data to database.

```bash
./run.sh
```

#### Read changes from slave

```bash
docker exec mysql_slave-1 sh -c "export MYSQL_PWD=111; mysql -u root mydb -e 'select count(*) from users \G'"
```

#### Stop one slave
```bash
docker compose stop mysql_slave-1
```

Turning off and on a slave doesn't stop the replication. The slave picks up replication from the point it stopped. However, with the write script running, it has a replication lag. 

#### Drop the last column on slave
```bash
docker exec mysql_slave-2 sh -c "export MYSQL_PWD=111; mysql -u root mydb -e 'ALTER TABLE users DROP COLUMN date_of_birth \G'"
```

#### Drop a column from the middle on slave
```bash
docker exec mysql_slave-2 sh -c "export MYSQL_PWD=111; mysql -u root mydb -e 'ALTER TABLE users DROP COLUMN email \G'"
```

Notice that dropping a column from the table under replication is allowed. If the last column is dropped, it doesn't break the replication. The replication continues with the remaining columns. However, dropping a column from the middle stops the replication. It's described in MySql documentation: [Replication with More Columns on Source or Replica](https://dev.mysql.com/doc/mysql-replication-excerpt/8.0/en/replication-features-more-columns.html).