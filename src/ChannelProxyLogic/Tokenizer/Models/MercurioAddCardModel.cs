using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Proxy.Channel.Tokenizer.Models
{
    internal class MercurioAddCardModel
    {
        public string Gateway { get; set; }
        public string PAN { get; set; }
        public string ExpDate { get; set; }
        public string CVV { get; set; }
        public string CardHolderName { get; set; }
        public string Signature { get; set; }
        public string RemoveDate { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Locator { get; set; }
        public string VirtualCard { get; set; }
        public string SMSEnabled { get; set; }
        public string Checkin { get; set; }
        public string Lang { get; set; }

        public string Guest { get; set; }
        public string Checkout { get; set; }
        public string Board { get; set; }
        public string RoomType { get; set; }
        public string Adults { get; set; }
        public string Children { get; set; }
        public decimal? Total { get; set; }
        public bool ShouldSerializeTotal()
        {
            return TotalSpecified;
        }
        [JsonIgnore]
        public bool TotalSpecified { get { return Total.HasValue; } }
    }
}
