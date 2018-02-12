using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rebus.Manager.Testing.Contracts.Messages;
using Rebus.Workflow;

namespace Rebus.Api.Testing.Controllers
{
    [ApiVersion("1")]
    public class CheckIpAddressApiController : Controller
    {
        private readonly IFlowBuilderFactory m_flowBuilderFactory;

        public CheckIpAddressApiController(IFlowBuilderFactory flowBuilderFactory)
        {
            m_flowBuilderFactory = flowBuilderFactory;
        }

        [HttpGet("api/ipaddress/public/check")]
        public async Task<IActionResult> RunCheck()
        {
            var builder = m_flowBuilderFactory.Create();

            builder.AddStep(new GetExternalIpAddressMessage());

            await builder.Send();

            return new OkResult();
        }
    }
}
