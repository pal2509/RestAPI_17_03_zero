using RestAPI_17_03_zero.Models;
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
            return UserRepository.LoginUser(username, password);
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
            return UserRepository.LogoutUser(int.Parse(token));
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
            if (UserRepository.TokenIsValid(int.Parse(token)))
            {
                string username = UserRepository.GetUsername(int.Parse(token));
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + username))
                {
                    DirectoryInfo di = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + username);
                }
                return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + UserRepository.GetUsername(int.Parse(token))).GetFiles().Select(d => d.Name).ToArray();
                //Leitura de todos os ficherios no directorio atual
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
        [HttpPost]
        public string FileDownload(string token)
        {
            if (UserRepository.TokenIsValid(int.Parse(token)))//verificação se o token é valido
            {
                var request = HttpContext.Current.Request;
                var filepath = AppDomain.CurrentDomain.BaseDirectory + UserRepository.GetUsername(int.Parse(token))+ "\\" + request.Headers["filename"];

                var res = HttpContext.Current.Response;

                using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))//Stream para leitura do ficherio
                {
                    using (Stream requestStream = res.OutputStream)//
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
        [HttpPost]
        public string FileUpload(string token)
        {
            if (UserRepository.TokenIsValid(int.Parse(token)))//Verificação se o token é valido
            {
                var request = HttpContext.Current.Request;//Pedido
                string username = UserRepository.GetUsername(int.Parse(token));
                var filePath = AppDomain.CurrentDomain.BaseDirectory + username + "\\" + request.Headers["filename"];//Caminho do ficheiro
                string ttl = request.Headers["ttl"];
                if(!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + username))
                {
                    DirectoryInfo di = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + username);
                }
                TimeSpan time;
                if(TimeSpan.TryParse(ttl,out time))
                {
                    DataBaseManager db = new DataBaseManager();
                    db.AddFileTime(request.Headers["filename"], time);
                }
                using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))//Stream pra escrita do ficheiro
                {
                    request.InputStream.CopyTo(fs);
                }
                return "Sucesso";
            }
            else return "-1";

        }

        [Route("fileserver/FileDelete/{token}")]
        [HttpPost]
        public int FileDelete(string token)
        {
            if (UserRepository.TokenIsValid(int.Parse(token)))//Verificação se o token é valido
            {
                var request = HttpContext.Current.Request;//Pedido
                string username = UserRepository.GetUsername(int.Parse(token));
                if (username != null)
                { 
                    var filePath = AppDomain.CurrentDomain.BaseDirectory + username + "\\" + request.Headers["filename"];//Caminho do ficheiro
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return 1;
                    }
                    else return -2;
                }
                else return -1;
            }
            else return -1;

        }

        [Route("fileserver/SignUp")]
        [HttpPost]
        public int SignUp()
        {
            var request = HttpContext.Current.Request;
            string username = request.Headers["username"];
            string password = request.Headers["password"];           
            return UserRepository.RegistrationRequest(username, password);
        }

        [Route("fileserver/RequestList/{token}")]
        [HttpGet]
        public List<string> RequestList(string token)
        {
            return UserRepository.RequestList(int.Parse(token));
        }

        [Route("fileserver/RequestManagement/{token}")]
        [HttpPost]
        public int RequestManagement(string token)
        {
            var request = HttpContext.Current.Request;
            return UserRepository.RequestAcception(int.Parse(token), request.Headers["username"]);
        }

        [Route("fileserver/FileCopy/{token}")]
        [HttpPost]
        public int FileCopy(string token)
        {
            var request = HttpContext.Current.Request;
            return 1;
        }


    }
}
