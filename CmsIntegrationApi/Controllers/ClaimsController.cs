using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ClaimMessages;
using Microsoft.AspNetCore.Mvc;

namespace CmsIntegrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private static IEnumerable<MyClaim> claims = new List<MyClaim>
            {
                new MyClaim {
                    ClaimId = "A001",
                    ClientName = "Bob Mary",
                    WorkName = "Roller Stone",
                    ReferenceNum = "Ref-A001"
                },
                new MyClaim {
                    ClaimId = "B001",
                    ClientName = "Carpenter",
                    WorkName = "Roller Stone",
                    ReferenceNum = "Ref-B001"
                },
            };
        private static int getCount = 0;
        private static int postCount = 0;
        public ClaimsController()
        {
        }

        [HttpGet]
        public ActionResult<IEnumerable<MyClaim>> Get()
        {
            getCount++;
            if (getCount % 2 == 1)
            {
                throw new HttpRequestException();
            }
            return Ok(claims);
        }

        [HttpGet("{id}")]
        public ActionResult<MyClaim> Get(string id)
        {
            var claim = claims.First(c => c.ClaimId == id);
            return Ok(claim);
        }

        [HttpPost]
        public ActionResult<string> Post([FromBody] MyClaim claim)
        {
            var token = Request.Headers["Authorization"];
            if(!token.ToString().Equals("Bearer valid_token"))
            {
                return Ok("401");
            }

            claim.ReferenceNum = "ref-" + claim.ClaimId;
            claims = claims.Append(claim);
            return Ok(claim.ReferenceNum);
        }
    }
}
