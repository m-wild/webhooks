$ErrorActionPreference = "stop"

$db_container = "webhook-db"
$db_image = "mariadb:latest"
$db_password = "rHZoqvshst8TcrzHMQc8X7QH"
$db_migrations = ".\sql\migrations"

## waits for mysql to be fully started and ready to accept connections
function __waitfordb() {
    write-host "Waiting for mysql to start.." -nonewline
    do {
        write-host '.' -nonewline
        sleep -Seconds 2
    } until (docker exec -it $db_container /usr/bin/mysqladmin ping --silent --password=$db_password)
    write-host ''
}

## run sql migrations
function __migratedb() {
    $migrations = ls $db_migrations -Filter "*.sql" | sort {$_.name}

    foreach ($mig in $migrations) {
        write-host "Running database migrations $mig"
        cat $db_migrations\$mig | docker exec -i $db_container /usr/bin/mysql --password=$db_password
    }
}



write-host "Creating Docker container $db_container -> $db_image"
docker run --name $db_container -e MYSQL_ROOT_PASSWORD=$db_password -d --rm -p 3306:3306 $db_image

__waitfordb

__migratedb

write-host "Done"