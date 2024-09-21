# Wait to be sure that SQL Server came up
sleep 10s

echo "running set up script"
#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools18/bin/sqlcmd -C -S mssql,1433 -U sa -P .acisa159753 -d master -i db-init.sql
