using System.Runtime.Serialization;

namespace ACI.Proxy.Api
{
    public class ErrorResponse
    {
        public IEnumerable<string> Messages { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Exception { get; set; }
    }
}
