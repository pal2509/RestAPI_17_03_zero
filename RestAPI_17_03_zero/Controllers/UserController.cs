using RestAPI_17_03_zero.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace RestAPI_17_03_zero.Controllers
{
    public class UserController : ApiController
    {

        /// <summary>
        /// Método para login do utilizador
        /// </summary>
        /// <param name="username">Nome de utilizador</param>
        /// <param name="password">Palavra-passe</param>
        /// <returns>Retorn um token, se o token for -1 o username e/ou password não existem, -2 se ja está loged in</returns>
        [Route("fileserver/login/{username}/{password}")]
        [HttpPost]
        public string Login(string username, string password)
        {
            //UserRepository.CreateSomeUsers();
            //return 1;
            string res = UserRepository.LoginUser(username, password);
            return res;
         
        }

        /// <summary>
        /// Logout do User
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("fileserver/logout/{token}")]
        [HttpPost]
        public bool Logout(string token)
        { 
            return UserRepository.LogoutUser(token);
        }


        /// <summary>
        /// Lista o diretório
        /// </summary>
        /// <param name="token">Token de acesso</param>
        /// <returns></returns>
        [Route("fileserver/dir/{token}")]
        [HttpGet]
        public string[] FileList(string token)
        {
            if (UserRepository.TokenIsValid(token))
            {
                return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
            }
            else
            {
                string[] r = { "-1" };
                return r;
            }
            
        }


        [Route("fileserver/dir/FileDownload/{token}")]
        [HttpGet]
        public string FileDownload(string token)
        {
            if (UserRepository.TokenIsValid(token))
            {
                var request = HttpContext.Current.Request;
                var filePath = AppDomain.CurrentDomain.BaseDirectory + request.Headers["filename"];
                using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    request.InputStream.CopyTo(fs);
                }
                return "Sucesso";
            }
            else return "-1";

           
        }

  




        [Route("fileserver/FileUpload/{token}")]
        [HttpPost]
        public string FileUpload(string token)
        {
            if (UserRepository.TokenIsValid(token))
            {
                var request = HttpContext.Current.Request;
                var filePath = AppDomain.CurrentDomain.BaseDirectory + request.Headers["filename"];
                using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    request.InputStream.CopyTo(fs);
                }
                return "Sucesso";
            }
            else return "-1";

        }


        /*
        [Route("api/myfileupload")]
        [HttpPost]
        public string MyFileUpload()
        {
            var request = HttpContext.Current.Request;
            var filePath = AppDomain.CurrentDomain.BaseDirectory + request.Headers["filename"];
            using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                request.InputStream.CopyTo(fs);
            }
            return "uploaded";
        }
        */


    }
}
