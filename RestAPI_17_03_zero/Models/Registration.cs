using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPI_17_03_zero.Models
{
    public class Registration
    {
        #region Attributes
       
        string username;
        string psswd;

        #endregion

        /// <summary>
        /// Retorna o nome de utilizador
        /// </summary>
        public string UserName
        {
            get { return username; }
        }

        /// <summary>
        /// Retorna a password do utilizador
        /// </summary>
        public string PassWord
        {
            get { return psswd; }
        }

        public Registration(string username,string password)
        {
            this.username = username;
            this.psswd = password;
        }

    }
}