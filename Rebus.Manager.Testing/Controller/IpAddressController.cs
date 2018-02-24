using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Rebus.Manager.Testing.Controller
{
    [ApiVersion("1")]
    public class IpAddressController : Microsoft.AspNetCore.Mvc.Controller
    {
        internal static IPAddress ExternalIpAddress { get; set; }


        [HttpGet("api/ipaddress/external")]
        public async Task<IActionResult> Get()
        {
            return new OkObjectResult(new
            {
                ExternalIpAddress = ExternalIpAddress.ToString()
            });
        }
    }
}
