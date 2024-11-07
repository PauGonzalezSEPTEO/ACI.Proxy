using System;
using System.Data;
using DbUp.Engine;

namespace ACI.Proxy.Database.Scripts.Code
{
    public class S002_ModifySomeRows : IScript
    {
        public string ProvideScript(Func<IDbCommand> dbCommandFactory)
        {
            using var cmd = dbCommandFactory();
            cmd.CommandText = "SELECT COUNT(*) FROM Test";
            int count = (int)cmd.ExecuteScalar();
            return @$"UPDATE Test SET Type = Type + '+{count}' WHERE Id = 2";
        }
    }
}
