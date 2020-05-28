using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestAPI_17_03_zero.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Http;
using Npgsql;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Web.WebSockets;

namespace RestAPI_17_03_zero.Services
{
    static class UserRepository
    {

        #region Attributes
        static List<Token> tokens = new List<Token>(); //Lista para guradar os tokens de cada id que está loged in
        static Random rand = new Random();//Gerador de números á sorte para os tokens
        #endregion
        #region Methods

        /// <summary>
        /// Verfica se os ficheiros de um dado utilizador estão dentro do prazo de vida
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool VerifyUFilesDate(int uid, string filename)
        {
            DataBaseManager db = new DataBaseManager();
            List<Filettl> f = db.GetUFiles(uid);
            if (f != null)
            {
                foreach (Filettl t in f)
                {
                    if (t.FName.CompareTo(filename) == 0)
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan diff = now.Subtract(t.FVal);
                        if (diff > new TimeSpan(0, 0, 0))
                        {
                            DeleteUserFile(t.Uid, t.FName);
                            db.RemoveFilettl(uid, t.FName);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Verifica o tempo de vida de todos os ficheiros
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static bool VerifyUFilesDate(int uid)
        {
            DataBaseManager db = new DataBaseManager();
            List<Filettl> f = db.GetUFiles(uid);
            List<Filettl> toRemove = f.FindAll(x => DateTime.Now.Subtract(x.FVal) > new TimeSpan(0, 0, 0));
            if (f != null)
            {
                toRemove.ForEach(x =>
                {
                    DeleteUserFile(x.Uid, x.FName);
                    db.RemoveFilettl(x.Uid, x.FName);
                });
                return true;
            }
            else return false;

            //DataBaseManager db = new DataBaseManager();
            //List<Filettl> f = db.GetUFiles(uid);
            //if (f != null)
            //{
            //    foreach (Filettl t in f)
            //    {      
            //        DateTime now = DateTime.Now;
            //        TimeSpan diff = now.Subtract(t.FVal);
            //        if (diff > new TimeSpan(0, 0, 0))
            //        {
            //            DeleteUserFile(t.Uid, t.FName);
            //            db.RemoveFilettl(t.Uid,t.FName);
            //            return true;
            //        }                    
            //    }
            //}
            //return false;
        }

        /// <summary>
        /// Apaga um ficheiro de um utilizador
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        private static void DeleteUserFile(int id, string filename)
        {
            DataBaseManager db = new DataBaseManager();
            var filepath = AppDomain.CurrentDomain.BaseDirectory +"Users\\"+ db.GetUsername(id) + "\\" + filename;
            if (File.Exists(filepath))
            {
                db.RemoveFilettl(id,filename);
                File.Delete(filepath);
            }
        }

        /// <summary>
        /// Cria um pedido de registo
        /// </summary>
        /// <param name="usrnm"></param>
        /// <param name="psswd"></param>
        /// <returns></returns>
        public static int RegistrationRequest(string usrnm,string psswd)
        {
            
            DataBaseManager db = new DataBaseManager();
            if (db.UserID(usrnm) == -1 && !db.RegRequestExists(usrnm,psswd))
            {
                db.AddRegistrationRequest(usrnm, psswd);
                return 1;
            }
            else return -1;
        }

        /// <summary>
        /// Retorna a lista de pedidos de registo
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static List<string> RequestList(int token)
        {
            DataBaseManager db = new DataBaseManager();
            if (TokenIsValid(token) && db.GetAccessLevel(GetUserId(token)) == 2)
            {
                return db.RegRequestList();
            }
            return null;
        }

        /// <summary>
        /// Aceita pedidos de registo
        /// </summary>
        /// <param name="token"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static int RequestAcception(int token, string username)
        {
            DataBaseManager db = new DataBaseManager();
            if (TokenIsValid(token))
            {
                if (db.GetAccessLevel(GetUserId(token)) == 2)
                {
                    Registration r = db.GetRegRequest(username);
                    if (r != null)
                    {                     
                        User u = new User(db.UserCount() + 1, r.UserName, r.PassWord, 1);
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Users\\" + u.UserName);
                        db.RemoveRequest(username);
                        db.AddUser(u);
                        return 1;
                    }
                    else return -3;
                }
                else return -2;
            }
            return -1;
        }

        /// <summary>
        /// Login do utilizador
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Token se login com sucesso, -1 se o user não existe, -2 se já esta loged in</returns>
        public static int LoginUser(string username, string password)
        {
            DataBaseManager db = new DataBaseManager();

            int uid = db.UserID(username, password);
            if (uid != -1)
            {
                if (!IsLoggedIn(uid))
                {
                    int token = rand.Next(1000, 9999);
                    if (AddToken(token, uid))
                    {
                        return token;
                    }
                    else return -3;      
                }
                else return -2;
            }
            else return -1;

        }
        
        /// <summary>
        /// Logout do user
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool LogoutUser(int token)
        {
            if (TokenIsValid(token))
            {
                Token[] t = tokens.ToArray();
                foreach (Token a in t)
                {
                    if (a.Idtoken == token)
                    {
                        tokens.Remove(a);
                        return true;
                    }
                }
                return false;
            }
            else return false;
        }

        /// <summary>
        /// Verificaçao se o token é valido
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool TokenIsValid(int token)
        {
            return tokens.Exists(x=>x.Idtoken == token);

            //Token[] t = tokens.ToArray();
            //foreach(Token a in t)
            //{
            //    if(a.Idtoken == token)
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }

        /// <summary>
        /// Verifica se um utilizador está logged
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        private static bool IsLoggedIn(int idUser)
        {
            Token[] t = tokens.ToArray();
            foreach (Token a in t)
            {
                if (a.IdUser == idUser)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adiciona um token á lista de tokens
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="id">Id do user a que pertence o token</param>
        /// <returns>Retorna true se adicionou, caso contrário false</returns>
        private static bool AddToken(int token, int id)
        {
            Token t = new Token(token, id);
            if (!tokens.Contains(t))
            {
                tokens.Add(t);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Vai buscar o nome de utilizador através do token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetUsername(int token)
        {
            if(TokenIsValid(token))
            {
                DataBaseManager db = new DataBaseManager();
                return db.GetUsername(GetUserId(token));
            }
            return null;
        }

        /// <summary>
        /// Vai buscar o id do utilizador através do token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int GetUserId(int token)
        {
            if (TokenIsValid(token))
            {
                Token[] t = tokens.ToArray();

                foreach (Token a in t)
                {
                    if (a.Idtoken == token) return a.IdUser;
                }
            }
            return -1;
        }

        /// <summary>
        /// Verifica se um utilizador está subscrito a um determinado canal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool IsUserSub(int id, string channel)
        {
            DataBaseManager db = new DataBaseManager();
            int r = db.IsUserSub(id, channel);
            if (r == -1) return false;
            return true;
        }

        /// <summary>
        /// Verifica de um canal existe
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool ChannelExists(string channel)
        {
            DataBaseManager db = new DataBaseManager();

            int r = db.GetChannelId(channel);
            if (r == -1) return false;
            else return true;

        }


        #endregion




    }
}