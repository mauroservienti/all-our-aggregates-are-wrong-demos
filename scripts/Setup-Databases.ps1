#Requires -RunAsAdministrator

$instanceName = "all-our-aggregates-are-wrong"

sqllocaldb create $instanceName
sqllocaldb share $instanceName $instanceName
sqllocaldb start $instanceName
sqllocaldb info $instanceName

$serverName = "(localdb)\" + $instanceName
$databasesPath = "$ENV:UserProfile\$instanceName-databases"
mkdir -Force $databasesPath
$pathParameter = '"{0}"' -f $databasesPath
sqlcmd -S $serverName -v UserPath=$pathParameter -i "$PSScriptRoot\Setup-Databases.sql" 
