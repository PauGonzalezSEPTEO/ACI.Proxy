namespace ACI.HAM.Settings
{
    public class PingWebsiteSettings
    {
        public Uri Url { get; set; } = new Uri("https://acigrup.com");

        public int TimeIntervalInMinutes { get; set; } = 60;
    }
}
