using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ACI.Proxy.Channel.Utils
{
    internal static class Utilidades
    {
        public static string PostJsonBearer(string url, string rq, bool basicHeader = false, string user = "", string pass = "", string token = "")
        {
            string rs = string.Empty;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                //request.ContentLength = bytes.Length;
                if (basicHeader)
                {
                    httpWebRequest.Headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pass)));
                }
                else
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                    }
                }

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

                using (var stremWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = rq;
                    stremWriter.Write(json);
                    stremWriter.Flush();
                    stremWriter.Close();
                }

                Logger.Log("PostJsonBearer getting response..."/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Error, 0, Guid.NewGuid()*/);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    rs = streamReader.ReadToEnd();
                    Logger.Log("PostJsonBearer streamReader: " + rs.ToString()/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Error, 0, Guid.NewGuid()*/);
                }

            }
            catch (WebException wex)
            {
                string rstmp = string.Empty;
                if (wex?.Response != null)
                {
                    using (var stream = wex.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            rstmp += reader.ReadToEnd();
                        }
                    }
                }
                Logger.Log("PostJsonBearer KO wex: " + wex.ToString() + ", rstmp=> " + rstmp/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Error, 0, Guid.NewGuid()*/);

                throw wex;
            }
            catch (Exception ex)
            {
                Logger.Log("PostJsonBearer KO ex: " + ex.ToString() + ", rs=> " + rs/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Error, 0, Guid.NewGuid()*/);

                throw ex;
            }

            return rs;
        }

        public static string PostJson(string url, string rq, bool basicHeader = false, string user = "", string pass = "", string token = "")
        {
            string rs = string.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                //request.ContentLength = bytes.Length;
                if (basicHeader)
                {
                    httpWebRequest.Headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pass)));
                }
                else
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                    }
                }

                using (var stremWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = rq;
                    stremWriter.Write(json);
                    stremWriter.Flush();
                    stremWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    rs = streamReader.ReadToEnd();
                }

            }
            catch (WebException wex)
            {
                if (wex?.Response != null)
                {
                    using (var stream = wex.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            rs += reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return rs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="obj"></param>
        /// <param name="getError"></param>
        /// <returns></returns>
        public static string Serializar(Type tipo, object obj, bool getError)
        {
            string result = string.Empty;
            System.IO.MemoryStream ms = null;
            System.Xml.XmlWriter xw = null;
            System.IO.StreamReader sr = null;

            try
            {
                ms = new System.IO.MemoryStream();
                xw = System.Xml.XmlWriter.Create(ms);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(tipo);
                serializer.Serialize(xw, obj);
                xw.Flush();
                ms.Seek(0, System.IO.SeekOrigin.Begin);

                sr = new System.IO.StreamReader(ms, System.Text.Encoding.UTF8);
                result = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (getError)
                {
                    return ex.Message;
                }
                else
                {
                    return string.Empty;
                }
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                if (ms != null)
                    ms.Close();
            }
            return result;
        }
        /// <summary>
        /// Serializar un objeto en XML
        /// </summary>
        /// <param name="tipo">Tipo para serializar el objeto.</param>
        /// <param name="obj">Objeto que queremos serializar.</param>
        /// <returns>Objeto serializado en XML</returns>
        public static string Serializar(Type tipo, object obj)
        {
            return Serializar(tipo, obj, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="obj"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static string Serializar(Type tipo, object obj, XmlSerializerNamespaces namespaces)
        {
            string result = string.Empty;
            System.IO.MemoryStream ms = null;
            System.Xml.XmlWriter xw = null;
            System.IO.StreamReader sr = null;

            try
            {
                ms = new System.IO.MemoryStream();
                xw = System.Xml.XmlWriter.Create(ms);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(tipo);
                serializer.Serialize(xw, obj, namespaces);
                xw.Flush();
                ms.Seek(0, System.IO.SeekOrigin.Begin);

                sr = new System.IO.StreamReader(ms, System.Text.Encoding.UTF8);
                result = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                if (ms != null)
                    ms.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="obj"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static string Serializar(Type tipo, object obj, string namespaces)
        {
            string result = string.Empty;
            System.IO.MemoryStream ms = null;
            System.Xml.XmlWriter xw = null;
            System.IO.StreamReader sr = null;

            try
            {
                ms = new System.IO.MemoryStream();
                xw = System.Xml.XmlWriter.Create(ms);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(tipo, namespaces);
                serializer.Serialize(xw, obj);
                xw.Flush();
                ms.Seek(0, System.IO.SeekOrigin.Begin);

                sr = new System.IO.StreamReader(ms, System.Text.Encoding.UTF8);
                result = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                if (ms != null)
                    ms.Close();
            }
            return result;
        }
        /// <summary>
        /// Desserializar un XML a una clase Aci.OTA.
        /// </summary>
        /// <param name="xml">XML a DeSerializar</param>
        /// <param name="tipo">Tipo que queremos generar</param>
        /// <returns>Objeto deSerializado</returns>
        public static object DeSerializar(string xml, Type tipo)
        {
            object obj = null;
            try
            {
                System.IO.StringReader sr = new System.IO.StringReader(xml);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (System.Xml.XmlReader xw = System.Xml.XmlReader.Create((sr)))
                    {
                        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(tipo);
                        obj = serializer.Deserialize(xw);
                    }
                }
            }
            catch (Exception Exception)
            {

            }

            return obj;
        }
        /// <summary>
        /// Desserializar un XML a una clase Aci.OTA.
        /// </summary>
        /// <param name="xml">XML a DeSerializar</param>
        /// <param name="tipo">Tipo que queremos generar</param>
        /// <returns>Objeto deSerializado</returns>
        public static object DeSerializar(string xml, Type tipo, bool getError, out string error)
        {
            error = String.Empty;
            object obj = null;

            try
            {
                System.IO.StringReader sr = new System.IO.StringReader(xml);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (System.Xml.XmlReader xw = System.Xml.XmlReader.Create((sr)))
                    {
                        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(tipo);
                        obj = serializer.Deserialize(xw);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return obj;
        }
        /// <summary>
        /// Desserializar un XML a una clase Aci.OTA.
        /// </summary>
        /// <param name="xml">XML a DeSerializar</param>
        /// <param name="tipo">Tipo que queremos generar</param>
        /// <param name="namespace">XML namespace</param>
        /// <returns>Objeto deSerializado</returns>
        public static object DeSerializar(string xml, Type tipo, string xmlNamespace)
        {
            object obj;
            try
            {
                System.IO.StringReader sr = new System.IO.StringReader(xml);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (System.Xml.XmlReader xw = System.Xml.XmlReader.Create((sr)))
                    {
                        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(tipo, xmlNamespace);
                        obj = serializer.Deserialize(xw);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return obj;
        }

        /// <summary>
        /// Limpiamos los namespace porque no lo reconecen "http://www.w3.org/2001/XMLSchema-instance"
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string limpiarNamespace(string xml)
        {
            return xml.Replace(@" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""", "");
        }

        public static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        /// <summary>
        /// ToDo - Obtener Codigo ISO2 del pais
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="cod"></param>
        /// <returns></returns>
        public static string GetPaisISO2(string tipo, string cod)
        {
            string codPaisISO2 = string.Empty;
            if (!string.IsNullOrEmpty(tipo) && !string.IsNullOrEmpty(cod))
            {
                //CRS.ReglasNegocio.Tipos.DS_Paises paises = GetPaises(0, 999999999);
                //Aci.CRS.ReglasNegocio.Tipos.DS_Paises.crs_PaisesRow[] rowsPaises = null;
                //switch (tipo)
                //{
                //    case "codpais":
                //        rowsPaises = (Aci.CRS.ReglasNegocio.Tipos.DS_Paises.crs_PaisesRow[])paises.crs_Paises.Select("CodPais = '" + cod.Replace("'", "''") + "'");
                //        if (rowsPaises != null && rowsPaises.Length > 0)
                //            codPaisISO2 = rowsPaises[0].CodPais;
                //        break;
                //    case "codtelefono":
                //        rowsPaises = (Aci.CRS.ReglasNegocio.Tipos.DS_Paises.crs_PaisesRow[])paises.crs_Paises.Select("CodTelefono = '" + cod.Replace("'", "''") + "'");
                //        if (rowsPaises != null && rowsPaises.Length > 0)
                //            codPaisISO2 = rowsPaises[0].CodPais;
                //        break;
                //    case "nompais":
                //        rowsPaises = (Aci.CRS.ReglasNegocio.Tipos.DS_Paises.crs_PaisesRow[])paises.crs_Paises.Select("NomPais = '" + cod.Replace("'", "''") + "'");
                //        if (rowsPaises != null && rowsPaises.Length > 0)
                //            codPaisISO2 = rowsPaises[0].CodPais;
                //        break;
                //    case "codprestige":
                //        rowsPaises = (Aci.CRS.ReglasNegocio.Tipos.DS_Paises.crs_PaisesRow[])paises.crs_Paises.Select("CodPrestige = '" + cod.Replace("'", "''") + "'");
                //        if (rowsPaises != null && rowsPaises.Length > 0)
                //            codPaisISO2 = rowsPaises[0].CodPais;
                //        break;
                //}
            }
            return codPaisISO2;
        }

        public static string SanitizePhoneNumber(string number, string codPais)
        {
            string sanitizedNumer = string.Empty;

            if (!string.IsNullOrEmpty(number))
            {
                PhoneNumbers.PhoneNumberUtil phoneUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
                try
                {
                    sanitizedNumer = CleanPhoneNumber(number);
                    if (!string.IsNullOrEmpty(sanitizedNumer))
                    {
                        PhoneNumbers.PhoneNumber parsedNumber = phoneUtil.Parse(
                            (!sanitizedNumer.StartsWith("+") ? "+" : "") + (sanitizedNumer.Length == 9 ? "34" : "") + sanitizedNumer,
                            codPais);
                        if (parsedNumber != null)
                        {
                            string countryCode = parsedNumber.CountryCode > 0 ? parsedNumber.CountryCode.ToString() : "34";
                            sanitizedNumer = countryCode + parsedNumber.NationalNumber.ToString();

                            //PhoneNumbers.PhoneNumberOfflineGeocoder geocoder = PhoneNumbers.PhoneNumberOfflineGeocoder.GetInstance();
                            //string from = geocoder.GetDescriptionForNumber(parsedNumber, PhoneNumbers.Locale.English);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Error en ACI_OTA.ComunicacionesBase, SanitizePhoneNumber (number: " + number + ", codPais:" + codPais + "): " + ex.ToString()/*, CRS.ReglasNegocio.Estructuras.ErrorTipo.Error, 0, new Guid()*/);
                }
            }

            return sanitizedNumer;
        }

        private static string CleanPhoneNumber(string number)
        {
            string cleanedNumber = string.Empty;

            if (!string.IsNullOrEmpty(number))
            {
                cleanedNumber = System.Text.RegularExpressions.Regex.Replace(number, "[^\\d]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).TrimStart('0');
            }

            return cleanedNumber;
        }
    }
}
