namespace ACI.Proxy.Core.Integrations
{
    public abstract class IntegrationBase
    {
        public class CustomField
        {
            public string Key { get; set; }
            public int Order { get; set; }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public List<CustomField> CustomFields { get; set; } = new List<CustomField>();

        protected void AddCustomField(string key, int order)
        {
            CustomFields.Add(new CustomField { Key = key, Order = order });
        }
    }
}
