using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPI_17_03_zero.Models
{
    /// <summary>
    /// Classe Utilizador
    /// </summary>
    [Serializable]
    public class User
    {

        #region Attributes

        int id;
        string username;
        string psswd;
        int acclevel;

        #endregion

        #region Properties

        /// <summary>
        /// Retorna o Id do utilizador
        /// </summary>
        public int Id
        {
            get { return id; }
        }


        public int Acclevel
        {
            get { return id; }
        }


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

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor de Utilizador
        /// </summary>
        /// <param name="id">Id do utilizador</param>
        /// <param name="name">Nome de utilizador</param>
        /// <param name="psswd">Palavra-passe do utilizador</param>
        public User(int id,string name,string psswd,int acc)
        {
            this.id = id;
            this.username = name;
            this.psswd = psswd;
            this.acclevel = acc;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return string.Format("Id: {0}, User: {1}, PassWord: {2}", id, username, psswd);
        }

        public override bool Equals(object obj)
        {
            User aux = (User)obj;
            return (this.id == aux.id && this.username == aux.username && this.psswd == aux.psswd);
        }

        #endregion

    }
}