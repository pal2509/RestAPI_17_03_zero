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

namespace RestAPI_17_03_zero.Services
{
    static class UserRepository
    {

        #region Attributes

        static string FILEUSERS = AppDomain.CurrentDomain.BaseDirectory + "Users.bin"; //Ficheiro dos Users
        static List<User> users = LoadUsers(); //Lista de utilizadores
        static Dictionary<int, int> tokens = new Dictionary<int, int>(); //Lista para guradar os tokens de cada id que está loged in
        #endregion

        #region Properties

        /// <summary>
        /// Retorna os users
        /// </summary>
        public static List<User> GetUsers
        {
            get { return users; }
        }

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

            User u = UserExists(username, password);

            if (u != null)
            {
                int token = u.Id + u.PassWord.Length * 2;
                if (AddToken(token, u.Id) == true) return token;
                else return -2;
            }
            else return -1;


        }
        
        public static bool LogoutUser(int token)
        {
            if (TokenIsValid(token))
            {
                return tokens.Remove(token);
            }
            else return false;
        }


        private static bool TokenIsValid(int token)
        {
            return tokens.ContainsKey(token);
        }

        private static User TokenBelongings(int token)
        {
            if(TokenIsValid(token))
            {
                int id;
                if(tokens.TryGetValue(token,out id))
                {
                    return GetUser(id);
                }
            }
            return null;
        }

        private static User GetUser(int id)
        {
            User[] u = users.ToArray();
            User user = (User)from User in u
                        where User.Id == id
                        select User;
            return user;
        }



        /// <summary>
        /// Carrega os Users para memória
        /// </summary>
        /// <returns>Lista de Users</returns>
        private static List<User> LoadUsers()
        {
            if (File.Exists(FILEUSERS))
            {
                List<User> u;
                using (Stream str = File.OpenRead(FILEUSERS))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    u = (List<User>)bf.Deserialize(str);
                }
                return u;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Encontra um User através do username e password
        /// </summary>
        /// <param name="username">string username</param>
        /// <param name="password">string password</param>
        /// <returns>Retorna o User ou null caso não exista</returns>
        public static User UserExists(string username, string password)
        {
            User[] u = users.ToArray();
            for (int i = 0; i < u.Length; i++)
            {
                if (u[i].UserName.CompareTo(username) == 0) return u[i];
            }
            return null;
        }

        /// <summary>
        /// Adicona um User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool AddUser(string username, string password)
        {
            if (UserExists(username, password) == null)
            {
                User u = new User(users.Count + 1, username, password);
                users.Add(u);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Adiciona um token á lista de tokens
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="id">Id do user a que pertence o token</param>
        /// <returns>Retorna true se adicionou, caso contrário false</returns>
        public static bool AddToken(int token, int id)
        {
            if (tokens.ContainsKey(token) == false)
            {
                tokens.Add(token, id);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Guarda os Users para um ficheiro binário
        /// </summary>
        private static void SaveUsers()
        {
            using (Stream str = File.Open(FILEUSERS, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(str, users);
                str.Close();
            }
        }

        public static void CreateSomeUsers()
        {


            AddUser("admin", "admin");
            AddUser("admin", "admin");
            AddUser("user", "user");
            AddUser("user", "admin");
            AddUser("aluno", "aluno");
            AddUser("a17611", "admin");
            AddUser("Paulo", "123");
            SaveUsers();
        }

        #endregion




    }
}