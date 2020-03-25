using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestAPI_17_03_zero.Controllers
{
    public class AulaController : ApiController
    {


        [Route("fileserver/login/{username}/{password}")]
        [HttpPost]
        public string Login(string username, string password)
        {
      
            return "";
        }

        [Route("fileserver/dir/{token}")]
        [HttpGet]
        public IEnumerable<string> FileList(string token)
        {


            return new string[] { "Aluno A", "Aluno B" };
        }

        //Usar um token para permitir a autenticaçao que está guardado na api

        [Route("fileserver/logout/{username}/{password}")]
        [HttpPost]
        public string Logout(string username, string password)
        {



            return "";
        }

        [Route("fileserver/dir/FileDownload/{token}")]
        [HttpGet]
        public IEnumerable<string> FileDownload(string token)
        {


            return new string[] { "Aluno A", "Aluno B" };
        }

        [Route("fileserver/dir/FileUpload/{token}")]
        [HttpPost]
        public IEnumerable<string> FileUpload(string token)
        {


            return new string[] { "Aluno A", "Aluno B" };
        }


    }
}
