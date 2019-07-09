#Requires -RunAsAdministrator

$instanceName = "all-our-aggregates-are-wrong"

sqllocaldb stop $instanceName
sqllocaldb delete $instanceName

$databasesPath = "$ENV:UserProfile\$instanceName-databases"
mkdir -Force $databasesPath
rm -Recurse $databasesPath
