using ACI.Proxy.Channel.CRIs.Models;
using ACI.Proxy.Channel.CRIs.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Proxy.Channel.CRIs
{
    internal class Dingus : CRIBase
    {
        public override string RecuperarReservas(BaseRequestModel requestData)
        {
            if (requestData.GetType() != typeof(DingusRequestModel))
                throw new ArgumentException("An object of type DingusRequestModel was expected.");

            if (requestData == null || (requestData as DingusRequestModel) == null)
                throw new ArgumentNullException("Null entry");

            return RecuperarReservas(requestData as DingusRequestModel);
        }

        private string RecuperarReservas(DingusRequestModel requestData)
        {
            string xmlRs = string.Empty;

            return xmlRs;
        }

        protected override string ModifCvv(string xml)
        {
            throw new NotImplementedException();
        }
    }
}
