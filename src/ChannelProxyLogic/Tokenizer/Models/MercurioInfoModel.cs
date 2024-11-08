using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Proxy.Channel.Tokenizer.Models
{
    internal class MercurioInfoModel
    {
        public string MercurioGateway { get; set; }
        public string MercurioGatewayPassword { get; set; }
        public string MercurioUser { get; set; }
        public string MercurioPassword { get; set; }
        public bool? SendSms { get; set; }
    }
}
