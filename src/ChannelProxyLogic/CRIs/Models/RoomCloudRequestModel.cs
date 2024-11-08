using ACI.Proxy.Channel.CRIs.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Proxy.Channel.CRIs.Models
{
    public class RoomCloudRequestModel : BaseRequestModel
    {
        private decimal? _gdsVersion;


        public decimal? GdsVersion { get { return _gdsVersion; } set { _gdsVersion = value; } }
    }
}
