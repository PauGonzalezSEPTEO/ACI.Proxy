namespace ACI.Proxy.Core.Integrations
{
    public class SiteMinderIntegration : IntegrationBase
    {
        public SiteMinderIntegration()
        {
            Id = 1;
            Name = "SiteMinder";
            AddCustomField("RequestorID_Type", 1);
            AddCustomField("RequestorID_ID", 2);
            AddCustomField("HotelCode", 3);
            AddCustomField("User", 4);
            AddCustomField("Password", 5);
            AddCustomField("Url", 6);
        }
    }
}
