using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestAPI_17_03_zero.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "Aluno A", "Aluno B" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return id.ToString();
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public string Put(int id, [FromBody]string value)
        {
            return id.ToString();
        }

        // DELETE api/values/5
        public string Delete(int id)
        {
            return id.ToString();
        }
    }
}
