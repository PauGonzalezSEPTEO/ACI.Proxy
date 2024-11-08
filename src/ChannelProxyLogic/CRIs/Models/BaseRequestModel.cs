using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI.Proxy.Channel.CRIs.Suppliers
{
    public class BaseRequestModel
    {
        protected private string _user;
        protected private string _password;
        protected private string _idHotelCri;
        protected private string _url;
        protected private string _mercurioUser;
        protected private bool? _confirmBookings;

        public string User { get { return _user; } set { _user = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public string IdHotelCri { get { return _idHotelCri; } set { _idHotelCri = value; } }
        public string Url { get { return _url; } set { _url = value; } }
        public string MercurioUser { get {  return _mercurioUser; } set { _mercurioUser = value; } }
        public bool? ConfirmBookings { get { return _confirmBookings; } set { _confirmBookings = value; } }
    }
}
// https://www.siteminder.co.uk/webservices/CHANNELPRO