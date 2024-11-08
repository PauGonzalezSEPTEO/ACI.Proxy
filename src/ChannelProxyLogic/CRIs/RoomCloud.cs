using ACI.Proxy.Channel.CRIs.Models;
using ACI.Proxy.Channel.CRIs.Suppliers;
using ACI.Proxy.Channel.Utils;
using Suppliers.SiteMinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ACI.Proxy.Channel.CRIs
{
    public class RoomCloud : CRIBase
    {
        private const string OTA_RESERVATIONS = @"https://api.roomcloud.net/be/xml/Ota2011B.jsp";
        public const decimal GDS_Version = 1016;
        private const string textType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText";
        private const string wsseNs = @"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wsswssecurity-secext-1.0.xsd";
        private const string wsuNs = @"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wsswssecurity-utility-1.0.xsd";

        public override string RecuperarReservas(BaseRequestModel requestData)
        {
            if (requestData.GetType() != typeof(RoomCloudRequestModel))
                throw new ArgumentException("An object of type RoomCloudRequestModel was expected.");

            if (requestData == null || (requestData as RoomCloudRequestModel) == null)
                throw new ArgumentNullException("Null entry");

            return RecuperarReservas(requestData as RoomCloudRequestModel);
        }

        private string RecuperarReservas(RoomCloudRequestModel requestData)
        {
            string xmlRs = string.Empty;

            try
            {
                if (requestData != null && !string.IsNullOrEmpty(requestData.User) && !string.IsNullOrEmpty(requestData.Password) && !string.IsNullOrEmpty(requestData.IdHotelCri))
                {
                    OTA_ReadRQ rq = new OTA_ReadRQ()
                    {
                        EchoToken = Guid.NewGuid().ToString(),
                        TimeStamp = DateTime.UtcNow,//use GMT
                        TimeStampSpecified = true,
                        Version = requestData.GdsVersion.HasValue ? requestData.GdsVersion.Value : GDS_Version
                    };

                    rq.ReadRequests = new OTA_ReadRQReadRequests();
                    OTA_ReadRQReadRequestsHotelReadRequest hotelReadRq = new OTA_ReadRQReadRequestsHotelReadRequest()
                    {
                        SelectionCriteria = new OTA_ReadRQReadRequestsHotelReadRequestSelectionCriteria()
                        {
                            SelectionTypeSpecified = true,
                            SelectionType = OTA_ReadRQReadRequestsHotelReadRequestSelectionCriteriaSelectionType.Undelivered,

                            Start = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd"),
                            End = DateTime.Now.ToString("yyyy-MM-dd")
                        },
                        HotelCode = requestData.IdHotelCri
                    };
                    rq.ReadRequests.Items = new List<OTA_ReadRQReadRequestsHotelReadRequest> { hotelReadRq }.ToArray();

                    

                    //using (HttpClient httpClient = new HttpClient())
                    //{
                    //    HttpContent httpContent = new StringContent(GetContent(rq, requestData.User, requestData.Password), Encoding.UTF8, "text/xml");
                    //    //httpClient.BaseAddress = new Uri(OTA_RESERVATIONS);
                    //    httpContent.Headers.Add("SOAPAction", "http://tempuri.org/HelloWorld");
                    //    var httpResponse = httpClient.PostAsync(OTA_RESERVATIONS, httpContent).Result;
                    //    httpResponse.EnsureSuccessStatusCode();

                    //    xmlRs = httpResponse.Content.ReadAsStringAsync().Result;
                    //}

                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return xmlRs;
        }

        private static string GetContent(OTA_ReadRQ rq, string user, string pass)
        {
            string content = string.Empty;

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass)) 
            {
                // Crear un nuevo documento XML
                XmlDocument doc = new XmlDocument();

                // Crear un espacio de nombres para manejar los prefijos
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("soapenv", "http://www.w3.org/2003/05/soap-envelope");
                nsmgr.AddNamespace("ns", "http://www.opentravel.org/OTA/2003/05");
                nsmgr.AddNamespace("wsa", "http://www.w3.org/2005/08/addressing");
                nsmgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wsswssecurity-secext-1.0.xsd");
                nsmgr.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wsswssecurity-utility-1.0.xsd");

                // Crear el elemento raíz <soapenv:Envelope>
                XmlElement envelopeElement = doc.CreateElement("soapenv", "Envelope", nsmgr.LookupNamespace("soapenv"));
                envelopeElement.SetAttribute("xmlns:ns", nsmgr.LookupNamespace("ns"));

                // Crear el elemento <soapenv:Header>
                XmlElement headerElement = doc.CreateElement("soapenv", "Header", nsmgr.LookupNamespace("soapenv"));
                headerElement.SetAttribute("xmlns:wsa", nsmgr.LookupNamespace("wsa"));

                // Crear el elemento <wsse:Security>
                XmlElement securityElement = doc.CreateElement("wsse", "Security", nsmgr.LookupNamespace("wsse"));
                securityElement.SetAttribute("xmlns:wsu", nsmgr.LookupNamespace("wsu"));

                // Crear el elemento <wsse:UsernameToken>
                XmlElement usernameTokenElement = doc.CreateElement("wsse", "UsernameToken", nsmgr.LookupNamespace("wsse"));

                // Crear y añadir el atributo wsu:Id al elemento <wsse:UsernameToken>
                XmlAttribute idAttribute = doc.CreateAttribute("wsu", "Id", nsmgr.LookupNamespace("wsu"));
                idAttribute.Value = "UsernameToken-45";
                usernameTokenElement.Attributes.Append(idAttribute);

                // Crear el elemento <wsse:Username>
                XmlElement usernameElement = doc.CreateElement("wsse", "Username", nsmgr.LookupNamespace("wsse"));
                XmlText usernameText = doc.CreateTextNode(user);
                usernameElement.AppendChild(usernameText);

                // Crear el elemento <wsse:Password>
                XmlElement passwordElement = doc.CreateElement("wsse", "Password", nsmgr.LookupNamespace("wsse"));
                passwordElement.SetAttribute("Type", "PasswordText");
                XmlText passwordText = doc.CreateTextNode(pass);
                passwordElement.AppendChild(passwordText);

                // Añadir <wsse:Username> y <wsse:Password> a <wsse:UsernameToken>
                usernameTokenElement.AppendChild(usernameElement);
                usernameTokenElement.AppendChild(passwordElement);

                // Añadir <wsse:UsernameToken> a <wsse:Security>
                securityElement.AppendChild(usernameTokenElement);

                // Añadir <wsse:Security> a <soapenv:Header>
                headerElement.AppendChild(securityElement);

                // Crear el elemento <soapenv:Body>
                XmlElement bodyElement = doc.CreateElement("soapenv", "Body", nsmgr.LookupNamespace("soapenv"));

                bodyElement.InnerXml = Utilidades.Serializar(typeof(OTA_ReadRQ), rq).Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");

                // Añadir <soapenv:Header> y <soapenv:Body> a <soapenv:Envelope>
                envelopeElement.AppendChild(headerElement);
                envelopeElement.AppendChild(bodyElement);

                // Añadir <soapenv:Envelope> al documento
                doc.AppendChild(envelopeElement);

                // Guardar el documento en un string
                using (StringWriter stringWriter = new StringWriter())
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    doc.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                    content = stringWriter.ToString();
                }
            }

            return content;
        }

        static private SecurityHeaderType CreateSecurityHeader(string usernameAuthenticate, string passwordAuthenticate)
        {
            SecurityHeaderType securityHeader = new SecurityHeaderType()
            {
                Any = CreateUserNameToken(usernameAuthenticate, passwordAuthenticate)
            };

            return (securityHeader);
        }

        static private System.Xml.XmlElement[] CreateUserNameToken(string usernameAuthenticate, string passwordAuthenticate)
        {
            //
            // Not all the SOAP envelope elements are available through the request block intellisense.
            // Those that aren't must be added as XmlElements.
            //

            XmlDocument doc = new XmlDocument();
            XmlElement usernametoken = doc.CreateElement("UsernameToken");
            XmlElement password = doc.CreateElement("Password");
            XmlElement username = doc.CreateElement("Username");

            //
            // Password is transmitted in plain text.
            //

            password.SetAttribute("Type", textType);

            XmlText usernameText = doc.CreateTextNode(usernameAuthenticate);
            XmlText passwordText = doc.CreateTextNode(passwordAuthenticate);

            username.AppendChild(usernameText);
            password.AppendChild(passwordText);
            usernametoken.AppendChild(username);
            usernametoken.AppendChild(password);

            return (new XmlElement[] { usernametoken });
        }

        protected override string ModifCvv(string xml)
        {
            throw new NotImplementedException();
        }
    }
}
