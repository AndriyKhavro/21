# #!/bin/bash

# Set files as read-only to avoid "World-writable config file '/etc/mysql/conf.d/mysql.conf.cnf' is ignored" error.
chmod 444 ./master/conf/mysql.conf.cnf
chmod 444 ./slave/conf/mysql.conf.cnf

docker-compose down -v
rm -rf ./master/data/*
rm -rf ./slave/data-1/*
rm -rf ./slave/data-2/*
docker-compose build
docker-compose up -d

until docker exec mysql_master sh -c 'export MYSQL_PWD=111; mysql -u root -e ";"'
do
    echo "Waiting for mysql_master database connection..."
    sleep 4
done

priv_stmt='CREATE USER "mydb_slave_user"@"%" IDENTIFIED BY "mydb_slave_pwd"; GRANT REPLICATION SLAVE ON *.* TO "mydb_slave_user"@"%"; FLUSH PRIVILEGES;'
docker exec mysql_master sh -c "export MYSQL_PWD=111; mysql -u root -e '$priv_stmt'"

until docker-compose exec mysql_slave-1 sh -c 'export MYSQL_PWD=111; mysql -u root -e ";"'
do
    echo "Waiting for mysql_slave-1 database connection..."
    sleep 4
done

until docker-compose exec mysql_slave-2 sh -c 'export MYSQL_PWD=111; mysql -u root -e ";"'
do
    echo "Waiting for mysql_slave-2 database connection..."
    sleep 4
done

MS_STATUS=`docker exec mysql_master sh -c 'export MYSQL_PWD=111; mysql -u root -e "SHOW MASTER STATUS"'`
CURRENT_LOG=`echo $MS_STATUS | awk '{print $6}'`
CURRENT_POS=`echo $MS_STATUS | awk '{print $7}'`

start_slave_stmt="CHANGE MASTER TO MASTER_HOST='mysql_master',MASTER_USER='mydb_slave_user',MASTER_PASSWORD='mydb_slave_pwd',MASTER_LOG_FILE='$CURRENT_LOG',MASTER_LOG_POS=$CURRENT_POS; START SLAVE;"
start_slave_cmd='export MYSQL_PWD=111; mysql -u root -e "'
start_slave_cmd+="$start_slave_stmt"
start_slave_cmd+='"'
docker exec mysql_slave-1 sh -c "$start_slave_cmd"
docker exec mysql_slave-2 sh -c "$start_slave_cmd"

docker exec mysql_slave-1 sh -c "export MYSQL_PWD=111; mysql -u root -e 'SHOW SLAVE STATUS \G'"
docker exec mysql_slave-2 sh -c "export MYSQL_PWD=111; mysql -u root -e 'SHOW SLAVE STATUS \G'"
