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
        [HttpGet]
        public int Login(string username, string password)
        {
            //UserRepository.CreateSomeUsers();
            //return 1;
            int res = UserRepository.LoginUser(username, password);
            return res;
         
        }

        /// <summary>
        /// Logout do User
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("fileserver/logout/{token}")]
        [HttpGet]
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
                return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles().Select(d => d.Name).ToArray();//Leitura de todos os ficherios
                                                                                                      //no directorio atual
                //return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
            }
            else
            {
                string[] r = { "-1", "Erro na leitura da pasta" };
                return r;
            }
            
        }

        /// <summary>
        /// Metodo para o download de ficheiros
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns></returns>

        [Route("fileserver/FileDownload/{token}")]
        [HttpGet]
        public string FileDownload(string token)
        {
            if (UserRepository.TokenIsValid(token))//verificação se o token é valido
            {
                var request = HttpContext.Current.Response;
                var filepath = AppDomain.CurrentDomain.BaseDirectory + request.Headers["filename"];

                using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))//Stream para leitura do ficherio
                {
                    using (Stream requestStream = request.OutputStream)//
                    {
                        byte[] buffer = new byte[1024 * 4];
                        int bytesLeft = 0;

                        while ((bytesLeft = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            requestStream.Write(buffer, 0, bytesLeft);
                        }
                        return "Sucesso";
                    }
                }
            }
            else return "Token inválido!!!";        
        }

        /// <summary>
        /// Metodo para upload de ficheiros
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("fileserver/FileUpload/{token}")]
        [HttpGet]
        public string FileUpload(string token)
        {
            if (UserRepository.TokenIsValid(token))//Verificação se o token é valido
            {
                var request = HttpContext.Current.Request;//Pedido
                var filePath = AppDomain.CurrentDomain.BaseDirectory + request.Headers["filename"];//Caminho do ficheiro
                using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))//Stream pra escrita do ficheiro
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
