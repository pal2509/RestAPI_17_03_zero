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
        static string FILEUSERS = AppDomain.CurrentDomain.BaseDirectory + "Users.bin";
        static List<User> users = LoadUsers();
        static Dictionary<int, int> tokens = new Dictionary<int, int>();
        #endregion

        #region Constructor
        /*
        public static UserRepository()
        {
            FILEUSERS = AppDomain.CurrentDomain.BaseDirectory + "Users.bin";

            //CreateSomeUsers();
            //LoadUsers();
            //LoadUsers();
        }
        */
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
        private static List<User> LoadUsers()
        {
            if (File.Exists(FILEUSERS))
            {

                using (Stream str = File.OpenRead(FILEUSERS))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    users = (List<User>)bf.Deserialize(str);
                }
                return users;
            }
            else
            {
                return null;
            }
        }

        public static User UserExists(string username, string password)
        {
            User[] u = users.ToArray();
            for (int i = 0; i < u.Length; i++)
            {
                if (u[i].UserName.CompareTo(username) == 0) return u[i];
            }
            return null;
        }

        public static bool AddUser(string username, string password)
        {
            if (UserExists(username, password) == null)
            {
                var rand = new Random((int)new DateTime().Ticks);
                User u = new User(rand.Next(0, 50000), username, password);
                users.Add(u);
                return true;
            }
            else return false;
        }

        public static bool AddToken(int token, int id)
        {
            if (tokens.ContainsKey(token) == false)
            {
                tokens.Add(token, id);
                return true;
            }
            else return false;
        }

        public static bool SaveUsers()
        {
            using (Stream str = File.Open(FILEUSERS, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(str, users);
                str.Close();
                return true;
            }
        }

        public static void CreateSomeUsers()
        {


            AddUser("admin", "admin");
            AddUser("admin", "admin");
            AddUser("user", "user");
            AddUser("user", "admin");
            SaveUsers();
        }

        #endregion

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
                int token = u.Id + u.PassWord.Length;
                if (AddToken(token, u.Id) == true) return token;
                else return -2;
            }
            else return -1;
      
            
        }




    }
}