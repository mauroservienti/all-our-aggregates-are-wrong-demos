#Requires -RunAsAdministrator

$instanceName = "all-our-aggregates-are-wrong"

$serverName = "(localdb)\" + $instanceName
sqlcmd -S $serverName -i ".\Teardown-Databases.sql"

sqllocaldb stop $instanceName
sqllocaldb delete $instanceName