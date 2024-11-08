using ACI.Proxy.Channel.CRIs.Suppliers;
using ACI.Proxy.Channel.Tokenizer.Models;
using ACI.Proxy.Channel.Utils;
using Suppliers.SiteMinder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ACI.Proxy.Channel.CRIs
{
    public class SiteMinder : CRIBase
    {
        public const decimal GDS_Version = 1.0M;
        private const string textType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText";
        public const string SM_OTA = "http://www.opentravel.org/OTA/2003/05";

        public const string GDS_RequestorID_ID = "CHANNELPRO";
        public const string GDS_RequestorID_Type = "22";
        public const string URL_FORMAT = @"https://ws.siteminder.com/pmsxchangev2/services/{0}";

        public override string RecuperarReservas(BaseRequestModel requestData)
        {
            if (requestData.GetType() != typeof(SiteMinderRequestModel))
                throw new ArgumentException("An object of type SiteMinderRequestModel was expected.");

            if (requestData == null || (requestData as SiteMinderRequestModel) == null)
                throw new ArgumentNullException("Null entry");

            return RecuperarReservas(requestData as SiteMinderRequestModel);
        }

        private string RecuperarReservas(SiteMinderRequestModel requestData)
        {
            string xmlRs = string.Empty;
            OTA_ResRetrieveRS rs = null;

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

                    SourceType source = new SourceType()
                    {
                        RequestorID = new SourceTypeRequestorID() 
                        { 
                            Type = !string.IsNullOrEmpty(requestData.GdsRequestorType) ? requestData.GdsRequestorType : GDS_RequestorID_Type,
                            ID = !string.IsNullOrEmpty(requestData.GdsRequestorId) ? requestData.GdsRequestorId : GDS_RequestorID_ID
                        }
                    };

                    rq.POS = new List<SourceType> { source }.ToArray();
                    rq.ReadRequests = new OTA_ReadRQReadRequests();
                    OTA_ReadRQReadRequestsHotelReadRequest hotelReadRq = new OTA_ReadRQReadRequestsHotelReadRequest()
                    {
                        SelectionCriteria = new OTA_ReadRQReadRequestsHotelReadRequestSelectionCriteria()
                        {
                            SelectionTypeSpecified = true,
                            SelectionType = OTA_ReadRQReadRequestsHotelReadRequestSelectionCriteriaSelectionType.Undelivered
                        },
                        HotelCode = requestData.IdHotelCri
                    };
                    rq.ReadRequests.Items = new List<OTA_ReadRQReadRequestsHotelReadRequest> { hotelReadRq }.ToArray();

                    using (PmsXchangeServiceClient pmsXchangeServiceClient = new PmsXchangeServiceClient())
                    {
                        pmsXchangeServiceClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(GetUrl(requestData));

                        //ReadRQResponse readRQResponse = pmsXchangeServiceClient.ReadRQAsync(securityHeaderType, rq).Result;
                        ReadRQResponse readRQResponse = pmsXchangeServiceClient.ReadRQAsync(CreateSecurityHeader(requestData.User, requestData.Password), rq).Result;

                        if (readRQResponse != null && readRQResponse.OTA_ResRetrieveRS != null && readRQResponse.OTA_ResRetrieveRS.Items != null && readRQResponse.OTA_ResRetrieveRS.Items.Count() > 0)
                        {
                            string result = Utilidades.Serializar(typeof(ReadRQResponse), readRQResponse, "http://www.opentravel.org/OTA/2003/05");
                            if (!string.IsNullOrEmpty(result))
                                xmlRs = ModifCvv(result);
                        }
                    }


                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return xmlRs;
        }

        private string GetUrl(SiteMinderRequestModel requestData)
        {
            if (requestData != null && !string.IsNullOrEmpty(requestData.Url))
                return requestData.Url;
            else if (requestData != null && !string.IsNullOrEmpty(requestData.GdsRequestorId))
                return string.Format(URL_FORMAT, requestData.GdsRequestorId);
            else
                return string.Format(URL_FORMAT, GDS_RequestorID_ID);
        }

        protected override string ModifCvv(string xml)
        {
            string result = string.Empty;
            MercurioInfoModel mercurioInfo = null;

            try
            {
                if (!string.IsNullOrEmpty(xml))
                {
                    // somehow, manage to load mercurioInfo properties
                    mercurioInfo = new MercurioInfoModel();
                    if (mercurioInfo != null)
                    {
                        PaymentCardType otaPayCard = null;
                        ReadRQResponse readRQResponse = (ReadRQResponse)Utilidades.DeSerializar(xml, typeof(ReadRQResponse), "http://www.opentravel.org/OTA/2003/05");
                        if (readRQResponse != null && readRQResponse.OTA_ResRetrieveRS != null && readRQResponse.OTA_ResRetrieveRS.Items != null && readRQResponse.OTA_ResRetrieveRS.Items.Length > 0)
                        {
                            foreach (object objList in readRQResponse.OTA_ResRetrieveRS.Items)
                            {
                                if (objList != null && objList is OTA_ResRetrieveRSReservationsList)
                                {
                                    OTA_ResRetrieveRSReservationsList otaResList = objList as OTA_ResRetrieveRSReservationsList;
                                    if (otaResList != null && otaResList.Items != null && otaResList.Items.Length > 0)
                                    {
                                        foreach (object objRes in otaResList.Items)
                                        {
                                            if (objRes is HotelReservationType)
                                            {
                                                HotelReservationType otaHotRes = objRes as HotelReservationType;
                                                if (otaHotRes?.ResGlobalInfo?.Guarantee?.GuaranteesAccepted != null && otaHotRes.ResGlobalInfo.Guarantee.GuaranteesAccepted.Length > 0)
                                                {
                                                    GuaranteeTypeGuaranteeAccepted otaGuarantee = otaHotRes.ResGlobalInfo.Guarantee.GuaranteesAccepted[0];
                                                    if (otaGuarantee.Item != null && otaGuarantee.Item is PaymentCardType)
                                                    {
                                                        string code = "VI";
                                                        string type = "1";
                                                        string cvv = "";

                                                        otaPayCard = new PaymentCardType();
                                                        otaPayCard = (PaymentCardType)otaGuarantee.Item;

                                                        if (!string.IsNullOrEmpty(otaPayCard.CardType))
                                                            type = otaPayCard.CardType;

                                                        if (!string.IsNullOrEmpty(otaPayCard.CardCode))
                                                            code = otaPayCard.CardCode;

                                                        if (!string.IsNullOrEmpty(otaPayCard.SeriesCode))
                                                            cvv = otaPayCard.SeriesCode;

                                                        #region GetEmail
                                                        string email = string.Empty;
                                                        if (otaHotRes != null && otaHotRes.ResGlobalInfo != null && otaHotRes.ResGlobalInfo.Profiles != null && otaHotRes.ResGlobalInfo.Profiles.Length > 0
                                                            && otaHotRes.ResGlobalInfo.Profiles.Any(x => x != null && x.Profile != null && x.Profile.Customer != null && x.Profile.Customer.Email != null && x.Profile.Customer.Email.Length > 0 && x.Profile.Customer.Email.Any(z => z != null && !string.IsNullOrEmpty(z.Value))))
                                                        {
                                                            email = otaHotRes.ResGlobalInfo.Profiles
                                                                .Where(x => x != null && x.Profile != null && x.Profile.Customer != null && x.Profile.Customer.Email != null && x.Profile.Customer.Email.Length > 0 && x.Profile.Customer.Email.Any(y => y != null && !string.IsNullOrEmpty(y.Value)))
                                                                .Select(x => x.Profile.Customer.Email.Where(y => y != null && !string.IsNullOrEmpty(y.Value)).First().Value)
                                                                .FirstOrDefault();
                                                        }
                                                        #endregion GetEmail

                                                        #region GetCheckOutDate
                                                        DateTime? removeDate = null;
                                                        if (otaHotRes != null && otaHotRes.RoomStays != null && otaHotRes.RoomStays.Length > 0
                                                            && otaHotRes.RoomStays.Any(x => x != null && x.TimeSpan != null && !string.IsNullOrEmpty(x.TimeSpan.End)))
                                                        {
                                                            removeDate = otaHotRes.RoomStays
                                                                .Where(x => x != null && x.TimeSpan != null && !string.IsNullOrEmpty(x.TimeSpan.End))
                                                                .Select(x => DateTime.ParseExact(x.TimeSpan.End, "yyyy-MM-dd", null))
                                                                .OrderBy(x => x)
                                                                .LastOrDefault();
                                                        }
                                                        #endregion GetCheckOutDate

                                                        #region GetCheckInDate
                                                        DateTime? checkInDate = null;
                                                        if (otaHotRes != null && otaHotRes.RoomStays != null && otaHotRes.RoomStays.Length > 0
                                                            && otaHotRes.RoomStays.Any(x => x != null && x.TimeSpan != null && !string.IsNullOrEmpty(x.TimeSpan.Start)))
                                                        {
                                                            checkInDate = otaHotRes.RoomStays
                                                                .Where(x => x != null && x.TimeSpan != null && !string.IsNullOrEmpty(x.TimeSpan.Start))
                                                                .Select(x => DateTime.ParseExact(x.TimeSpan.Start, "yyyy-MM-dd", null))
                                                                .OrderBy(x => x)
                                                                .FirstOrDefault();
                                                        }
                                                        #endregion GetCheckInDate

                                                        #region isVCC
                                                        bool? isVCC = null;

                                                        if (otaHotRes != null && otaHotRes.ResGlobalInfo != null && otaHotRes.ResGlobalInfo.Guarantee != null && otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription != null
                                                            && otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription.Count() > 0 && otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription[0] != null
                                                            && otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription[0].Items != null && otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription[0].Items.Count() > 0
                                                            && otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription[0].Items[0] != null
                                                            && (otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription[0].Items[0] as FormattedTextTextType) != null
                                                            && !string.IsNullOrEmpty((otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription[0].Items[0] as FormattedTextTextType).Value)
                                                            && ((otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription[0].Items[0] as FormattedTextTextType).Value.ToLower().Contains("prepay") ||
                                                                (otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription[0].Items[0] as FormattedTextTextType).Value.ToLower().Contains("virtual creditcard"))
                                                                )
                                                        {
                                                            string prePayType = GetPrePayTypeFromDescription((otaHotRes.ResGlobalInfo.Guarantee.GuaranteeDescription[0].Items[0] as FormattedTextTextType).Value);
                                                            isVCC = (!string.IsNullOrEmpty(prePayType) && prePayType == "VCC");
                                                        }
                                                        #endregion isVCC

                                                        #region Lang
                                                        string lang = string.Empty;
                                                        if (otaHotRes != null)
                                                        {
                                                            CustomerType customer = null;
                                                            // first take resguest
                                                            if (otaHotRes.ResGuests != null && otaHotRes.ResGuests.Count() > 0)
                                                            {
                                                                if (otaHotRes.ResGuests.Any(a => a != null && a.PrimaryIndicatorSpecified && a.PrimaryIndicator && a.Profiles != null && a.Profiles.Count() > 0 && a.Profiles.Any(n => n != null && n.Profile != null && n.Profile.Customer != null)))
                                                                {
                                                                    // take firs resguest with PrimaryIndicator = true
                                                                    customer = otaHotRes.ResGuests.Where(w => w != null && w.PrimaryIndicatorSpecified && w.PrimaryIndicator && w.Profiles != null && w.Profiles.Count() > 0 && w.Profiles.Any(n => n != null && n.Profile != null && n.Profile.Customer != null)).FirstOrDefault().Profiles.FirstOrDefault().Profile.Customer;
                                                                }
                                                                else if (otaHotRes.ResGuests.Any(a => a != null && a.Profiles != null && a.Profiles.Count() > 0 && a.Profiles.Any(n => n != null && n.Profile != null && n.Profile.Customer != null)))
                                                                {
                                                                    // if any resguest with PrimaryIndicator = true, take firs resguest
                                                                    customer = otaHotRes.ResGuests.Where(w => w != null && w.Profiles != null && w.Profiles.Count() > 0 && w.Profiles.Any(n => n != null && n.Profile != null && n.Profile.Customer != null)).FirstOrDefault().Profiles.FirstOrDefault().Profile.Customer;
                                                                }
                                                            }
                                                            if (customer != null && customer.Address != null && customer.Address.Count() > 0 && customer.Address[0] != null && customer.Address[0].CountryName != null && !string.IsNullOrEmpty(customer.Address[0].CountryName.Code))
                                                            {
                                                                string codpais = customer.Address[0].CountryName.Code;
                                                                lang = Utilidades.GetPaisISO2("codpais", codpais);
                                                                if (string.IsNullOrEmpty(lang))
                                                                    lang = Utilidades.GetPaisISO2("nompais", codpais);
                                                            }
                                                            else if (otaHotRes.ResGlobalInfo != null && otaHotRes.ResGlobalInfo.Profiles != null && otaHotRes.ResGlobalInfo.Profiles.Count() > 0 && otaHotRes.ResGlobalInfo.Profiles[0] != null && otaHotRes.ResGlobalInfo.Profiles[0].Profile != null && otaHotRes.ResGlobalInfo.Profiles[0].Profile.Customer != null)
                                                            {
                                                                // if found customer (or not found) hasnt codpais, try with resglobalinfo customer 
                                                                customer = otaHotRes.ResGlobalInfo.Profiles[0].Profile.Customer;
                                                                if (customer != null && customer.Address != null && customer.Address.Count() > 0 && customer.Address[0] != null && customer.Address[0].CountryName != null && !string.IsNullOrEmpty(customer.Address[0].CountryName.Value))
                                                                {
                                                                    string nompais = customer.Address[0].CountryName.Value;
                                                                    lang = Utilidades.GetPaisISO2("nompais", nompais);
                                                                    if (string.IsNullOrEmpty(lang))
                                                                        lang = Utilidades.GetPaisISO2("codpais", nompais);
                                                                }
                                                            }
                                                        }
                                                        #endregion Lang

                                                        #region GetTelephone
                                                        string telefono = string.Empty;
                                                        if (otaHotRes != null && otaHotRes.ResGlobalInfo != null && otaHotRes.ResGlobalInfo.Profiles != null && otaHotRes.ResGlobalInfo.Profiles.Length > 0
                                                            && otaHotRes.ResGlobalInfo.Profiles.Any(x => x != null && x.Profile != null && x.Profile.Customer != null && x.Profile.Customer.Telephone != null && x.Profile.Customer.Telephone.Length > 0 && x.Profile.Customer.Telephone.Any(z => z != null && !string.IsNullOrEmpty(z.PhoneNumber))))
                                                        {
                                                            telefono = otaHotRes.ResGlobalInfo.Profiles
                                                                .Where(x => x != null && x.Profile != null && x.Profile.Customer != null && x.Profile.Customer.Telephone != null && x.Profile.Customer.Telephone.Length > 0 && x.Profile.Customer.Telephone.Any(y => y != null && !string.IsNullOrEmpty(y.PhoneNumber)))
                                                                .Select(x => x.Profile.Customer.Telephone.Where(y => y != null && !string.IsNullOrEmpty(y.PhoneNumber)).First().PhoneNumber)
                                                                .FirstOrDefault();

                                                            // intentamos sanitizar el numero
                                                            string sanitizedPhone = Utilidades.SanitizePhoneNumber(telefono, lang);
                                                            if (!string.IsNullOrEmpty(sanitizedPhone))
                                                                telefono = sanitizedPhone;
                                                        }

                                                        #endregion GetTelephone

                                                        #region Guest
                                                        string guest = string.Empty;
                                                        if (otaHotRes != null)
                                                        {
                                                            CustomerType customer = null;
                                                            // first take resguest
                                                            if (otaHotRes.ResGuests != null && otaHotRes.ResGuests.Count() > 0)
                                                            {
                                                                if (otaHotRes.ResGuests.Any(a => a != null && a.PrimaryIndicatorSpecified && a.PrimaryIndicator && a.Profiles != null && a.Profiles.Count() > 0 && a.Profiles.Any(n => n != null && n.Profile != null && n.Profile.Customer != null)))
                                                                {
                                                                    // take firs resguest with PrimaryIndicator = true
                                                                    customer = otaHotRes.ResGuests.Where(w => w != null && w.PrimaryIndicatorSpecified && w.PrimaryIndicator && w.Profiles != null && w.Profiles.Count() > 0 && w.Profiles.Any(n => n != null && n.Profile != null && n.Profile.Customer != null)).FirstOrDefault().Profiles.FirstOrDefault().Profile.Customer;
                                                                }
                                                                else if (otaHotRes.ResGuests.Any(a => a != null && a.Profiles != null && a.Profiles.Count() > 0 && a.Profiles.Any(n => n != null && n.Profile != null && n.Profile.Customer != null)))
                                                                {
                                                                    // if any resguest with PrimaryIndicator = true, take firs resguest
                                                                    customer = otaHotRes.ResGuests.Where(w => w != null && w.Profiles != null && w.Profiles.Count() > 0 && w.Profiles.Any(n => n != null && n.Profile != null && n.Profile.Customer != null)).FirstOrDefault().Profiles.FirstOrDefault().Profile.Customer;
                                                                }

                                                                if (customer != null && customer.PersonName != null && customer.PersonName.Length > 0 && customer.PersonName[0] != null)
                                                                {
                                                                    if (customer.PersonName[0].GivenName != null && customer.PersonName[0].GivenName.Length > 0 && !string.IsNullOrEmpty(customer.PersonName[0].GivenName[0]))
                                                                        guest += customer.PersonName[0].GivenName[0].ToUpper();

                                                                    if (!string.IsNullOrEmpty(customer.PersonName[0].Surname))
                                                                    {
                                                                        if (!string.IsNullOrEmpty(guest))
                                                                            guest += " ";

                                                                        guest += customer.PersonName[0].Surname.ToUpper();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        #endregion Guest

                                                        #region Adults&Children
                                                        int? adults = 0;
                                                        int? children = 0;

                                                        if (otaHotRes != null && otaHotRes.RoomStays != null && otaHotRes.RoomStays.Count() > 0 && otaHotRes.RoomStays[0] != null
                                                            && otaHotRes.RoomStays[0].GuestCounts != null && otaHotRes.RoomStays[0].GuestCounts.GuestCount != null && otaHotRes.RoomStays[0].GuestCounts.GuestCount.Count() > 0)
                                                        {
                                                            foreach (var unGuestCount in otaHotRes.RoomStays[0].GuestCounts.GuestCount)
                                                            {
                                                                switch (unGuestCount.AgeQualifyingCode)
                                                                {
                                                                    //case "7":
                                                                    //    infants = ConvFct.GetIntValue(unGuestCount.Count);
                                                                    //    break;
                                                                    case "8":
                                                                        if (!string.IsNullOrEmpty(unGuestCount.Count))
                                                                        {
                                                                            int childrenOut;
                                                                            if (int.TryParse(unGuestCount.Count, out childrenOut))
                                                                                children = childrenOut;
                                                                        }
                                                                        break;
                                                                    default:
                                                                    case "10":
                                                                        if (!string.IsNullOrEmpty(unGuestCount.Count))
                                                                        {
                                                                            int adultOut;
                                                                            if (int.TryParse(unGuestCount.Count, out adultOut))
                                                                                adults = adultOut;
                                                                        }
                                                                        break;
                                                                }
                                                            }
                                                            //try to distribute the ocuppancy correctly
                                                            if (otaHotRes.RoomStays[0].RoomRates != null && otaHotRes.RoomStays[0].RoomRates != null && otaHotRes.RoomStays[0].RoomRates.RoomRate != null
                                                                && otaHotRes.RoomStays[0].RoomRates.RoomRate[0] != null && !string.IsNullOrEmpty(otaHotRes.RoomStays[0].RoomRates.RoomRate[0].NumberOfUnits))
                                                            {
                                                                int noOfRooms = 1;
                                                                if (!int.TryParse(otaHotRes.RoomStays[0].RoomRates.RoomRate[0].NumberOfUnits, out noOfRooms))
                                                                    noOfRooms = 1;

                                                                if (adults.HasValue)
                                                                    adults = (int)decimal.Round(Convert.ToDecimal(adults.Value) /
                                                                                                Convert.ToDecimal(noOfRooms));
                                                                if (children.HasValue)
                                                                    children = (int)decimal.Round(Convert.ToDecimal(children.Value) /
                                                                                                  Convert.ToDecimal(noOfRooms));
                                                            }
                                                        }
                                                        #endregion Adults&Children

                                                        #region RoomType&Board
                                                        string roomType = string.Empty;
                                                        string board = string.Empty;

                                                        if (otaHotRes != null && otaHotRes.RoomStays != null && otaHotRes.RoomStays.Count() > 0 && otaHotRes.RoomStays[0] != null)
                                                        {
                                                            if (otaHotRes.RoomStays[0].RoomTypes != null && otaHotRes.RoomStays[0].RoomTypes.Count() > 0 && otaHotRes.RoomStays[0].RoomTypes[0] != null)
                                                            {
                                                                if (!string.IsNullOrEmpty(otaHotRes.RoomStays[0].RoomTypes[0].RoomType))
                                                                    roomType += otaHotRes.RoomStays[0].RoomTypes[0].RoomType;

                                                                // cant get board name
                                                            }
                                                        }
                                                        #endregion RoomType&Board

                                                        #region Total
                                                        decimal? total = null;
                                                        if (otaHotRes != null && otaHotRes.ResGlobalInfo != null && otaHotRes.ResGlobalInfo.Total != null && otaHotRes.ResGlobalInfo.Total.AmountAfterTaxSpecified && otaHotRes.ResGlobalInfo.Total.AmountAfterTax > 0)
                                                        {
                                                            total = otaHotRes.ResGlobalInfo.Total.AmountAfterTax/*.ToString()*/;
                                                        }
                                                        #endregion

                                                        #region HasSendMercurioSms
                                                        bool smsEnabled = mercurioInfo.SendSms.HasValue && mercurioInfo.SendSms.Value;
                                                        #endregion HasSendMercurioSms

                                                        DateTime ccExpiredDate = DateTime.ParseExact(otaPayCard.ExpireDate, "MMyy", System.Globalization.CultureInfo.InvariantCulture);

                                                        string loc = GetReservaBono(otaHotRes);

                                                        MercurioAddCardModel mercurioCard = new MercurioAddCardModel()
                                                        {
                                                            Gateway = mercurioInfo.MercurioGateway,
                                                            CardHolderName = otaPayCard.CardHolderName,
                                                            PAN = otaPayCard.CardNumber,
                                                            CVV = otaPayCard.SeriesCode,
                                                            ExpDate = ccExpiredDate.ToString("MMyy"),
                                                            Checkin = checkInDate.HasValue ? checkInDate.Value.ToString("yyyy-MM-dd") : "",
                                                            Email = !string.IsNullOrEmpty(email) ? email : "",
                                                            Mobile = !string.IsNullOrEmpty(telefono) ? telefono : "",
                                                            Locator = !string.IsNullOrEmpty(loc) ? loc : "",
                                                            VirtualCard = isVCC.HasValue ? isVCC.Value.ToString() : "",
                                                            SMSEnabled = smsEnabled.ToString(),
                                                            Lang = !string.IsNullOrEmpty(lang) ? lang : "",
                                                            Guest = !string.IsNullOrEmpty(guest) ? guest : "",
                                                            Checkout = removeDate.HasValue ? removeDate.Value.ToString("yyyy-MM-dd") : "",
                                                            Board = !string.IsNullOrEmpty(board) ? board : "",
                                                            Adults = adults.HasValue ? adults.Value.ToString() : "",
                                                            Children = children.HasValue ? children.Value.ToString() : "",
                                                            //Total = !string.IsNullOrEmpty(total) ? total : "", 
                                                            Total = total.HasValue ? total.Value : (decimal?)null,
                                                            RoomType = !string.IsNullOrEmpty(roomType) ? roomType : ""
                                                        };

                                                        string token = Tokenizer.Tokenizer.GetMercurioToken(mercurioInfo, mercurioCard, removeDate);

                                                        otaPayCard.CardNumber = ""; //token;
                                                        otaPayCard.SeriesCode = "000";
                                                        otaPayCard.ExpireDate = "0000";
                                                        otaPayCard.CardHolderName = "Removed by engine";

                                                        if (!string.IsNullOrEmpty(token))
                                                            otaPayCard.CardNumber = token;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        result = Utilidades.Serializar(typeof(ReadRQResponse), readRQResponse, "http://www.opentravel.org/OTA/2003/05");
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        private static string GetPrePayTypeFromDescription(string descriptionText)
        {
            string prePayType = string.Empty;

            if (!string.IsNullOrEmpty(descriptionText))
            {
                if (descriptionText.ToLower().Contains("payout type: virtual credit card") || descriptionText.ToLower().Contains("virtual creditcard"))
                    prePayType = "VCC";

                if (descriptionText.ToLower().Contains("payout type: banktransfer"))
                    prePayType = "BankTransfer";
            }

            return prePayType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otaHotRes"></param>
        /// <returns></returns>
        private static string GetReservaBono(HotelReservationType otaHotRes)
        {
            List<HotelReservationIDsTypeHotelReservationID> otaHotResIDList;
            string bono = string.Empty;

            if (otaHotRes != null && otaHotRes.ResGlobalInfo != null &&
                otaHotRes.ResGlobalInfo.HotelReservationIDs != null)
            {
                otaHotResIDList = new List<HotelReservationIDsTypeHotelReservationID>();
                otaHotResIDList.AddRange(otaHotRes.ResGlobalInfo.HotelReservationIDs);

                if (otaHotResIDList.Count > 0)
                {
                    foreach (HotelReservationIDsTypeHotelReservationID otaHotResID in otaHotResIDList)
                    {
                        bono = otaHotResID.ResID_Value;
                    }
                }
            }
            return bono;
        }

        // not mine
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
    }
}
