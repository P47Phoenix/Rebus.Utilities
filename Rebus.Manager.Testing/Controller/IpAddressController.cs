using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Rebus.Manager.Testing
{
    [ApiVersion("1")]
    public class IpAddressController : Controller
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
