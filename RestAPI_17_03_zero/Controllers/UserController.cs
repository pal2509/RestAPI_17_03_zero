using Antlr.Runtime.Tree;
using RestAPI_17_03_zero.Models;
using RestAPI_17_03_zero.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.UI;

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
                UserRepository.VerifyUFilesDate(UserRepository.GetUserId(int.Parse(token)));
                string username = UserRepository.GetUsername(int.Parse(token));
                return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Users\\" + UserRepository.GetUsername(int.Parse(token))).GetFiles().Select(d => d.Name).ToArray();
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
        /// Método para o download de ficheiros
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns></returns>
        [Route("fileserver/FileDownload/{token}")]
        [HttpPost]
        public int FileDownload(string token)
        {
            var res = HttpContext.Current.Response;
            if (UserRepository.TokenIsValid(int.Parse(token)))//verificação se o token é valido
            {
                var request = HttpContext.Current.Request;
                var filepath = AppDomain.CurrentDomain.BaseDirectory + "Users\\" + UserRepository.GetUsername(int.Parse(token)) + "\\" + request.Headers["filename"];

                if (File.Exists(filepath) && !UserRepository.VerifyUFilesDate(UserRepository.GetUserId(int.Parse(token)), request.Headers["filename"]))
                {
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
                            requestStream.Close();
                            res.Headers.Add("r", "1");

                        }

                    }
                }
                else res.Headers.Add("r", "-2");
            }
            else res.Headers.Add("r", "-1");
            return 1;
        }

        /// <summary>
        /// Método para upload de ficheiros
        /// </summary>
        /// <param name="token">Token de acesso</param>
        /// <returns></returns>
        [Route("fileserver/FileUpload/{token}")]
        [HttpPost]
        public int FileUpload(string token)
        {
            if (UserRepository.TokenIsValid(int.Parse(token)))//Verificação se o token é valido
            {
                var request = HttpContext.Current.Request;//Pedido
                string username = UserRepository.GetUsername(int.Parse(token));
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "Users\\" + username + "\\" + request.Headers["filename"];//Caminho do ficheiro
                string ttl = request.Headers["ttl"];
                TimeSpan time;
                //Adiciona o tempo de vida aos ficheiros
                if (TimeSpan.TryParse(ttl, out time))
                {
                    DataBaseManager db = new DataBaseManager();
                    db.AddFileTime(UserRepository.GetUserId(int.Parse(token)), request.Headers["filename"], time);
                }
                using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))//Stream pra escrita do ficheiro
                {
                    request.InputStream.CopyTo(fs);
                }
                return 1;
            }
            else return -1;

        }

        /// <summary>
        /// Metodo para eliminar um ficheiro
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Retorna 1 em caso de sucesso, -2 se o ficheiro não existe, -1 se o token é inválido, -3 outro erro</returns>
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
                    var filePath = AppDomain.CurrentDomain.BaseDirectory + "Users\\" + username + "\\" + request.Headers["filename"];//Caminho do ficheiro
                    if (File.Exists(filePath) && !UserRepository.VerifyUFilesDate(UserRepository.GetUserId(int.Parse(token)), request.Headers["filename"]))
                    {
                        DataBaseManager db = new DataBaseManager();
                        db.RemoveFilettl(UserRepository.GetUserId(int.Parse(token)), request.Headers["filename"]);
                        File.Delete(filePath);
                        return 1;
                    }
                    else return -2;
                }
                else return -3;
            }
            else return -1;

        }

        /// <summary>
        /// Método para efetuar o pedido de registo
        /// </summary>
        /// <returns>Retorna 1 se o pedido foi feito com sucesso, -1 se já existe esse nome de utilizador</returns>
        [Route("fileserver/SignUp")]
        [HttpPost]
        public int SignUp()
        {
            var request = HttpContext.Current.Request;
            string username = request.Headers["username"];
            string password = request.Headers["password"];
            return UserRepository.RegistrationRequest(username, password);
        }

        /// <summary>
        /// Método que retorna a lista de pedidos de registo
        /// NOTA: O utilizador para poder  utilizar este método deve ter um nivel de acesso elevado
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Uma lista de strings em que cada uma é o nome de utilizador do pedido de registo</returns>
        [Route("fileserver/RequestList/{token}")]
        [HttpGet]
        public List<string> RequestList(string token)
        {
            return UserRepository.RequestList(int.Parse(token));
        }

        /// <summary>
        /// Método para aceitar um pedido de registo que rebece o nome de utilizador do pedido de registo a registar
        /// NOTA: O utilizador para poder  utilizar este método deve ter um nivel de acesso elevado
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("fileserver/RequestManagement/{token}")]
        [HttpPost]
        public int RequestManagement(string token)
        {
            var request = HttpContext.Current.Request;
            return UserRepository.RequestAcception(int.Parse(token), request.Headers["username"]);
        }

        /// <summary>
        /// Método para copiar ficheiros
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("fileserver/FileCopy/{token}")]
        [HttpPost]
        public int FileCopy(string token)
        {
            var request = HttpContext.Current.Request;
            if (UserRepository.TokenIsValid(int.Parse(token)))
            {
                string username = UserRepository.GetUsername(int.Parse(token));
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "Users\\" + username + "\\" + request.Headers["filename"];
                string f1 = request.Headers["filename"];
                string f2 = request.Headers["newfile"];
                if (File.Exists(filepath))
                {
                    File.Copy(filepath, AppDomain.CurrentDomain.BaseDirectory + "Users\\" + username + "\\" + f2);
                    return 1;
                }
                return -2;
            }
            return -1;
        }

        /// <summary>
        /// Retorna os canais que esse utilizador está subscrito
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("messages/GetUChannels/{token}")]
        [HttpGet]
        public string[] GetUChannels(string token)
        {
            var request = HttpContext.Current.Request;
            if (UserRepository.TokenIsValid(int.Parse(token)))
            {
                DataBaseManager db = new DataBaseManager();
                return db.GetSubedChannels(UserRepository.GetUserId(int.Parse(token)));
            }
            string[] r = { "-1", "Token inválido" };
            return r;
        }


        /// <summary>
        /// Retorna todos os canais disponiveis para subscrever
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("messages/GetAllChannels/{token}")]
        [HttpGet]
        public string[] GetAllChannels(string token)
        {
            var request = HttpContext.Current.Request;
            if (UserRepository.TokenIsValid(int.Parse(token)))
            {
                DataBaseManager db = new DataBaseManager();
                return db.GetChannels();
            }
            string[] r = { "-1", "Token inválido" };
            return r;
        }


        /// <summary>
        /// Permite ao utilizador mandar mensagens para um canal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("messages/SendMessage/{token}")]
        [HttpPost]
        public int SendMessage(string token)
        {
            var request = HttpContext.Current.Request;
            if (UserRepository.TokenIsValid(int.Parse(token)))
            {
                string channel = request.Headers["channel"];
                if (UserRepository.IsUserSub(UserRepository.GetUserId(int.Parse(token)), channel))
                {
                    DataBaseManager db = new DataBaseManager();
                    string message = UserRepository.GetUsername(int.Parse(token)) + "@" + DateTime.Now.ToString() + ": " + request.Headers["message"] + "\n";
                    string file = db.GetChannelFile(channel);
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Canais\\" + file, message);
                    return 1;
                }
                return -2;
            }
            return -1;
        }

        /// <summary>
        /// Retorna as mensagens de um canal 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("messages/GetMessage/{token}")]
        [HttpPost]
        public string[] GetMessage(string token)
        {
            string[] r = { "-1", "Token inválido" };
            var request = HttpContext.Current.Request;
            if (UserRepository.TokenIsValid(int.Parse(token)))
            {

                string channel = request.Headers["channel"];
                if (UserRepository.IsUserSub(UserRepository.GetUserId(int.Parse(token)), channel))
                {
                    DataBaseManager db = new DataBaseManager();

                    string file = db.GetChannelFile(channel);//Buscar o nome do ficheiro á base de dados
                    //Leitura do ficheiro
                    IEnumerable<string> chat = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "Canais\\" + file);

                    if (chat.Count() <= 10) return chat.ToArray();
                    else
                    {
                        //Ir buscar as ultimas 10 linhas do chat para retornar
                        r = new string[10];
                        int j = 0;
                        int length = chat.Count();
                        for (int i = length - 10; i < length; i++)
                        {
                            r[j] = chat.ElementAt(i);
                            j++;
                        }

                        return r;
                    }

                }
                return r;
            }
            return r;
        }

        /// <summary>
        /// Método para subscrever a um canal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("messages/SubChannel/{token}")]
        [HttpPost]
        public int SubChannel(string token)
        {
            var request = HttpContext.Current.Request;
            if (UserRepository.TokenIsValid(int.Parse(token)))
            {
                string channel = request.Headers["channel"];
                DataBaseManager db = new DataBaseManager();
                if (UserRepository.ChannelExists(channel))//Verifica se o canal existe
                {
                    if (db.IsUserSub(UserRepository.GetUserId(int.Parse(token)), channel) == -1)//Verificar se o utilizador já é sub
                    {
                        db.SubToChannel(channel, UserRepository.GetUserId(int.Parse(token)));
                        return 1;
                    }
                    return -3;
                }
                return -2;
            }
            return -1;
        }

    }
}

