using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace RestAPI_17_03_zero.Models
{
    [Serializable]
    public class Users
    {
        #region Attributes
        const string FILEUSERS = "Users.bin";
        static List<User> users;

        #endregion

        #region Constructor

        public Users()
        {
            try
            {
                LoadUsers();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Retorna os users
        /// </summary>
        public List<User> GetUsers
        {
            get { return users; }
        }

        #endregion
        #region Methods
        private void LoadUsers()
        {
            try
            {
                if (File.Exists(FILEUSERS))
                {
                    
                    using (Stream str = File.OpenRead(FILEUSERS))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        users = (List<User>)bf.Deserialize(str);
                    }

                }
                else
                {
                    throw new Exception("Ficheiro não existe!!!");
                }
            }
            catch (Exception e)
            {
                throw new Exception("ERRO:Nao foi possivel ler o ficheiro!!! - ", e);
            }
        }

        
        public User UserExists(string username, string password)
        {
            try
            {
                User[] u = users.ToArray();
                for (int i = 0; i < u.Length; i++)
                {
                    if (u[i].PassWord.CompareTo(password) == 0 && u[i].UserName.CompareTo(username) == 0) return u[i];
                }
                return null;
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
        }


        #endregion

    }
}