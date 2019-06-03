
using Everest.Identity.Core.Infrastructure;

namespace Everest.IdentityTest
{
    public class TestConfiguration: MapConfiguration
    {
        public TestConfiguration()
        {
            Items["authorization:secretKey"] = "eyJ1bmlxdWVfbmFtZSI6ImpvaG5kb2UiLCJleHAiOjE1NDc4NjI2MzUsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6";
            Items["authorization:validIssuer"] = "http://localhost:7000";
            Items["authorization:validAudience"] = "http://localhost:7000";
        }
    }
}
