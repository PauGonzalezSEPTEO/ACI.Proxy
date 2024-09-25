# Actively wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
until /opt/mssql-tools18/bin/sqlcmd -C -S mssql,1433 -U sa -P .acisa159753 -Q "SELECT 1" &> /dev/null
do
  echo "SQL Server is not ready, waiting 5 seconds..."
  sleep 5s
done

echo "running set up script"
#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools18/bin/sqlcmd -C -S mssql,1433 -U sa -P .acisa159753 -d master -i db-init.sql
