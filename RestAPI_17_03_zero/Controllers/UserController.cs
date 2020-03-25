using RestAPI_17_03_zero.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestAPI_17_03_zero.Controllers
{
    public class UserController : ApiController
    {

        private UserRepository userRepository;

        public UserController()
        {
            this.userRepository = new UserRepository();
        }

        /// <summary>
        /// Método para login do utilizador
        /// </summary>
        /// <param name="username">Nome de utilizador</param>
        /// <param name="password">Palavra-passe</param>
        /// <returns>Retorn um token</returns>
        [Route("fileserver/login/{username}/{password}")]
        [HttpPost]
        public string Login(string username, string password)
        {
            try
            {
                int res = userRepository.AuthUser(username, password);
                if (res == -1) return "-1";
                else return res.ToString();
            }
            catch(Exception e)
            {
                return e.ToString();
            }
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
