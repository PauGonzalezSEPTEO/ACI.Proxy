namespace ACI.Proxy.Mail.Dtos
{
    public class GetTemplateByNameDto
    {
        public List<string>? CustomFields { get; set; }

        public string? HTMLContent { get; set; }
    }
}
