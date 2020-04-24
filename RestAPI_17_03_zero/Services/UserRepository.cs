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

namespace RestAPI_17_03_zero.Services
{
    static class UserRepository
    {

        #region Attributes

        //static string FILEUSERS = AppDomain.CurrentDomain.BaseDirectory + "Users.bin"; //Ficheiro dos Users
        //static string LOGFILES = AppDomain.CurrentDomain.BaseDirectory + "logFiles.bin"; //Ficheiro dos Users
        static List<Token> tokens = new List<Token>(); //Lista para guradar os tokens de cada id que está loged in
        static Random rand = new Random();
        #endregion
        #region Methods

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
            Token[] t = tokens.ToArray();
            foreach(Token a in t)
            {
                if(a.Idtoken == token)
                {
                    return true;
                }
            }
            return false;
        }

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

        ///// <summary>
        ///// Metodo para procurara e retornar um user 
        ///// </summary>
        ///// <param name="id">Id do user</param>
        ///// <returns>User</returns>
        //private static User GetUser(int id)
        //{
        //    List<User> users = LoadUsers(FILEUSERS);
        //    User[] u = users.ToArray();
        //    User user = (User)from User in u
        //                where User.Id == id
        //                select User;
        //    return user;
        //}



        ///// <summary>
        ///// Carrega os Users para memória
        ///// </summary>
        ///// <returns>Lista de Users</returns>
        //private static List<User> LoadUsers(string file)
        //{
        //    if (File.Exists(file))
        //    {
        //        List<User> u;
        //        using (Stream str = File.OpenRead(file))
        //        {
        //            BinaryFormatter bf = new BinaryFormatter();
        //            u = (List<User>)bf.Deserialize(str);
        //        }
        //        return u;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //private static Dictionary<string,DateTime> LoadFiles(string file)
        //{
        //    if (File.Exists(file))
        //    {
        //        Dictionary<string,DateTime> u;
        //        using (Stream str = File.OpenRead(file))
        //        {
        //            BinaryFormatter bf = new BinaryFormatter();
        //            u = (Dictionary<string, DateTime>)bf.Deserialize(str);
        //        }
        //        return u;
        //    }
        //    else
        //    {
        //        return new Dictionary<string, DateTime>();
        //    }
        //}


        ///// <summary>
        ///// Encontra um User através do username e password
        ///// </summary>
        ///// <param name="username">string username</param>
        ///// <param name="password">string password</param>
        ///// <returns>Retorna o User ou null caso não exista</returns>
        //public static User UserExists(string username, string password)
        //{
        //    List<User> users = LoadUsers(FILEUSERS);
        //    User[] u = users.ToArray();
        //    for (int i = 0; i < u.Length; i++)
        //    {
        //        if (u[i].UserName.CompareTo(username) == 0 && u[i].PassWord.CompareTo(password) == 0) return u[i];
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Adicona um User
        ///// </summary>
        ///// <param name="username"></param>
        ///// <param name="password"></param>
        ///// <returns></returns>
        //public static bool AddUser(string username, string password)
        //{
        //    if (UserExists(username, password) == null)
        //    {
        //        List<User> users = LoadUsers(FILEUSERS);
        //        User u = new User(users.Count + 1, username, password, 1);
        //        users.Add(u);
        //        SaveUsers(users, FILEUSERS);
        //        return true;
        //    }
        //    else return false;
        //}

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

        ///// <summary>
        ///// Guarda os Users para um ficheiro binário
        ///// </summary>
        //private static void SaveUsers(List<User> users, string FILENAME)
        //{
        //    using (Stream str = File.Open(FILEUSERS, FileMode.Create))
        //    {
        //        BinaryFormatter bf = new BinaryFormatter();
        //        bf.Serialize(str, users);
        //        str.Close();
        //    }
        //}

        //public static void CreateSomeUsers()
        //{


        //    AddUser("admin", "admin");
        //    AddUser("admin", "admin");
        //    AddUser("user", "user");
        //    AddUser("user", "admin");
        //    AddUser("aluno", "aluno");
        //    AddUser("a17611", "admin");
        //    AddUser("Paulo", "123");
        //    SaveUsers();
        //}

        #endregion




    }
}