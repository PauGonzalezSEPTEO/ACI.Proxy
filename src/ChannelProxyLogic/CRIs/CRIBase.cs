using ACI.Proxy.Channel.CRIs.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Proxy.Channel.CRIs
{
    public abstract class CRIBase
    {
        internal CRIBase()
        {
        }
        public abstract string RecuperarReservas(BaseRequestModel datosPeticion);
        protected abstract string ModifCvv(string xml);
    }
}
