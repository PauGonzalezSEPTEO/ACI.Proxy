using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Proxy.Channel.Tokenizer.Models
{
    internal class MercurioAddCardResponseModel
    {
        public string IdUser { get; set; }
        public string TokenUser { get; set; }
        public string Error { get; set; }
        public string PanMask { get; set; }
    }
}
