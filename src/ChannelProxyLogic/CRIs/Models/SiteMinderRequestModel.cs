using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Proxy.Channel.CRIs.Suppliers
{
    public class SiteMinderRequestModel : BaseRequestModel
    {
        private decimal? _gdsVersion;
        private string? _gdsRequestorType;
        private string? _gdsRequestorId;

        public decimal? GdsVersion { get { return _gdsVersion; } set { _gdsVersion = value; } }
        public string? GdsRequestorType { get {  return _gdsRequestorType; } set { _gdsRequestorType = value; } }
        public string? GdsRequestorId { get {  return _gdsRequestorId; } set { _gdsRequestorId = value; } }
    }
}
