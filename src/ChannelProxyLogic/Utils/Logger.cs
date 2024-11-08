using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Proxy.Channel.Utils
{
    internal class Logger
    {
        internal static void Log(string mensaje/*, Aci.CRS.ReglasNegocio.Estructuras.ErrorTipo tipo, int idempresa, Guid idusuario, int? idHotel = null*/)
        {
            try
            {
                //using (Aci.CRS.AccesoDatos.BaseDatos dal = new Aci.CRS.AccesoDatos.BaseDatos())
                //{
                //    dal.InsertarParametro("@Mensaje", mensaje);
                //    dal.InsertarParametro("@Tipo", (int)tipo);
                //    if (idempresa > 0)
                //        dal.InsertarParametro("@IDEmpresa", idempresa);
                //    else
                //        dal.InsertarParametro("@IDEmpresa", System.Data.DbType.Int32, System.Data.SqlTypes.SqlInt32.Null);

                //    if (idHotel.HasValue)
                //    {
                //        dal.InsertarParametro("@IDHotel", idHotel.Value);
                //    }

                //    dal.InsertarParametro("@adtFecha", DateTime.Now);
                //    dal.InsertarParametro("@adtUsuario", idusuario);

                //    dal.ExecuteNonQuery("CRS_Logs_Ins", System.Data.CommandType.StoredProcedure);
                //}
            }
            catch { }
        }
    }
}
