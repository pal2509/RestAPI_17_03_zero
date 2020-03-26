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

        //private UserRepository userRepository;
        /*
        public UserController()
        {
            this.userRepository = new UserRepository();
        }
        */

        /// <summary>
        /// Método para login do utilizador
        /// </summary>
        /// <param name="username">Nome de utilizador</param>
        /// <param name="password">Palavra-passe</param>
        /// <returns>Retorn um token, se o token for -1 o username e/ou password não existem, -2 se ja está loged in</returns>
        [Route("fileserver/login/{username}/{password}")]
        [HttpPost]
        public int Login(string username, string password)
        {
            //UserRepository.CreateSomeUsers();
            //return 1;
            int res = UserRepository.LoginUser(username, password);
            return res;
         
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
