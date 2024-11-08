using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACI.Proxy.Channel.Utils;
using System.Configuration;
using ACI.Proxy.Channel.Tokenizer.Models;

namespace ACI.Proxy.Channel.Tokenizer
{
    internal static class Tokenizer
    {
        private static string URLMERCURIOAUTHENTICATE = "https://acisms.es/api/payments/authenticate";
        private static string URLMERCURIOADDCARD = "https://acisms.es/api/payments/addcard";
        private static string URLMERCURIOADDTOKEN = "https://acisms.es/api/payments/addtoken";

        public static string MercurioAuth(string user, string password)
        {
            string authToken = string.Empty;

            bool retry = true;
            int retryCount = 0;
            int maxRetry = 0;
            int timerRetry = 0;
            string urlMercurioAuth = string.Empty;

            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["MercurioAuthRetryTimer"], out timerRetry))
                timerRetry = 0;
            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["MercurioAuthMaxRetries"], out maxRetry))
                maxRetry = 0;

            urlMercurioAuth = System.Configuration.ConfigurationManager.AppSettings.Get("MercurioAuthUrl");
            if (string.IsNullOrEmpty(urlMercurioAuth))
                urlMercurioAuth = URLMERCURIOAUTHENTICATE;

            while (retry)
            {
                retry = false;
                try
                {
                    if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
                    {
                        var obj = new Dictionary<string, string>
                        {
                            { "Username", user },
                            { "Password", password }
                        };

                        var json = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);

                        //authToken = Aci.OTA.Utilidades.PostJson(URLMERCURIOAUTHENTICATE, json).Replace("\"","");
                        authToken = Utilidades.PostJson(urlMercurioAuth, json).Replace("\"", "");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Error en ACI_OTA.ComunicacionesBase, MercurioAuth: " + ex.Message/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Error, 0, new Guid(), idHotel*/);

                    if (maxRetry > 0 && timerRetry > 0)
                    {
                        if (retryCount < maxRetry)
                        {
                            retry = true;
                            retryCount++;
                            System.Threading.Thread.Sleep(timerRetry);
                            Logger.Log("[MercurioAuth] Unhandled error in hotel " /*+ idHotel*/ + ", retry count: " + retryCount/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Information, 0, new Guid(), idHotel*/);
                        }
                    }
                }
            }

            return authToken;
        }

        public static MercurioAddCardResponseModel MercurioAddCard(MercurioAddCardModel mercurioCard, string GateWayPassword, string authToken)
        {
            MercurioAddCardResponseModel addCardResponse = null;

            bool retry = true;
            int retryCount = 0;
            int maxRetry = 0;
            int timerRetry = 0;
            string urlMercurioAddCard = string.Empty;
            bool removeCCAnyways = false;

            if (!bool.TryParse(ConfigurationManager.AppSettings.Get("RemoveCCAnyways"), out removeCCAnyways))
                removeCCAnyways = false;

            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["MercurioAddCardRetryTimer"], out timerRetry))
                timerRetry = 0;
            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["MercurioAddCardMaxRetries"], out maxRetry))
                maxRetry = 0;

            urlMercurioAddCard = System.Configuration.ConfigurationManager.AppSettings.Get("MercurioAddCardUrl");
            if (string.IsNullOrEmpty(urlMercurioAddCard))
                urlMercurioAddCard = URLMERCURIOADDCARD;

            while (retry)
            {
                retry = false;
                try
                {
                    //if (!string.IsNullOrEmpty(GateWay) && !string.IsNullOrEmpty(GateWayPassword) && RemoveDate > DateTime.MinValue)
                    if (!string.IsNullOrEmpty(mercurioCard.Gateway) && !string.IsNullOrEmpty(GateWayPassword))
                    {
                        string Signature = Utilidades.SHA512(mercurioCard.Gateway + mercurioCard.PAN + mercurioCard.CVV + GateWayPassword).ToLower();
                        if (!string.IsNullOrEmpty(Signature))
                        {
                            mercurioCard.Signature = Signature;

                            var json = JsonConvert.SerializeObject(mercurioCard, Newtonsoft.Json.Formatting.Indented);

                            bool logMercurio = false;
                            if (!bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["LogMercurio"], out logMercurio))
                                logMercurio = false;

                            if (!string.IsNullOrEmpty(json) && logMercurio)
                            {
                                if (removeCCAnyways)
                                {
                                    mercurioCard.PAN = "removed";
                                    mercurioCard.CVV = "removed";
                                }
                                var jsonlog = JsonConvert.SerializeObject(mercurioCard, Newtonsoft.Json.Formatting.Indented);
                                Logger.Log(mercurioCard.Locator + ": " + jsonlog/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Information, 0, Guid.NewGuid(), idHotel*/);
                            }

                            Logger.Log(mercurioCard.Locator + ": AuthToken: " + authToken.ToString()/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Information, 0, Guid.NewGuid(), idHotel*/);

                            //string response = Aci.OTA.Utilidades.PostJsonBearer(URLMERCURIOADDCARD, json, false, "", "", authToken);
                            string response = Utilidades.PostJsonBearer(urlMercurioAddCard, json, false, "", "", authToken);

                            if (!string.IsNullOrEmpty(response))
                            {
                                addCardResponse = JsonConvert.DeserializeObject<MercurioAddCardResponseModel>(response);
                            }
                            else
                            {
                                Logger.Log(mercurioCard.Locator + ": Response PostJsonBearer " + response.ToString()/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Information, 0, Guid.NewGuid(), idHotel*/);
                            }
                        }
                        else
                        {
                            Logger.Log("Error en ACI_OTA.ComunicacionesBase, MercurioAuth: Error on generate Signature SHA512"/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Error, 0, new Guid(), idHotel*/);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Error en ACI_OTA.ComunicacionesBase, MercurioAddCard: " + ex.Message/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Error, 0, new Guid(), idHotel*/);

                    if (maxRetry > 0 && timerRetry > 0)
                    {
                        if (retryCount < maxRetry)
                        {
                            retry = true;
                            retryCount++;
                            System.Threading.Thread.Sleep(timerRetry);
                            Logger.Log("[MercurioAddCard] Unhandled error in hotel " /*+ idHotel*/ + ", retry count: " + retryCount/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Information, 0, new Guid(), idHotel*/);
                        }
                    }
                }
            }

            return addCardResponse;
        }

        public static string GetMercurioToken(MercurioInfoModel tokenData, MercurioAddCardModel mercurioCard, DateTime? expirationDate)
        {
            string strToken = null;
            if (!string.IsNullOrEmpty(tokenData.MercurioGateway) && !string.IsNullOrEmpty(tokenData.MercurioGatewayPassword) && !string.IsNullOrEmpty(tokenData.MercurioUser) && !string.IsNullOrEmpty(tokenData.MercurioPassword))
            {
                var authToken = MercurioAuth(tokenData.MercurioUser, tokenData.MercurioPassword);
                if (!string.IsNullOrEmpty(authToken))
                {
                    // ahora mismo el expiration date es la fecha de checkout 
                    // miramos el webconfig y le añadimos x dias, si no le añadimos 15 dias por defecto
                    if (expirationDate.HasValue)
                    {
                        int plusDays = 15;
                        if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["AuthTokenExpirationPlusDays"], out plusDays))
                            plusDays = 15;

                        expirationDate = expirationDate.Value.AddDays(plusDays);

                        // obvio que tiene value, pero por dejarlo igual que antes
                        mercurioCard.RemoveDate = expirationDate.HasValue ? expirationDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") : "";
                    }
                    else
                    {
                        mercurioCard.RemoveDate = DateTime.Now.AddYears(+1).ToString("yyyy-MM-ddTHH:mm:ss.fff");
                    }

                    var cardResult = MercurioAddCard(mercurioCard, tokenData.MercurioGatewayPassword, authToken);
                    //if (cardResult != null && string.IsNullOrEmpty(cardResult.Error))
                    if (cardResult != null && !string.IsNullOrEmpty(cardResult.IdUser) && !string.IsNullOrEmpty(cardResult.TokenUser) && !string.IsNullOrEmpty(cardResult.PanMask))
                    {
                        if (string.IsNullOrEmpty(cardResult.Error))
                        {
                            strToken = JsonConvert.SerializeObject(new { cardResult.IdUser, cardResult.TokenUser, cardResult.PanMask });
                        }
                        else
                        {
                            if (!cardResult.Error.Equals("Unknown error")) strToken = JsonConvert.SerializeObject(new { cardResult.IdUser, cardResult.TokenUser, cardResult.PanMask, cardResult.Error });
                        }
                    }
                }
            }
            return strToken;
        }
    }
}
