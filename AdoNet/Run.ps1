$user = "sa"
$password = "12345(!)a"
$database = "grains"
chcp 65001

sqlcmd -S localhost -U $user -P $password -Q "CREATE DATABASE $database"
sqlcmd -S localhost -U $user -P $password -d $database -i SQLServer-Main.sql
sqlcmd -S localhost -U $user -P $password -d $database -i SQLServer-Clustering.sql
sqlcmd -S localhost -U $user -P $password -d $database -i SQLServer-Persistence.sql
sqlcmd -S localhost -U $user -P $password -d $database -i SQLServer-Reminders.sql
          