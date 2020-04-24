using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPI_17_03_zero.Models
{
    public class Token
    {
        int idToken;
        int idUser; 

        public int Idtoken
        {
            get { return idToken; }
            set { idToken = value; }
        }

        public int IdUser
        {
            get { return idUser; }
            set { idUser = value; }
        }

        public Token(int idToken, int idUser)
        {
            this.idToken = idToken;
            this.idUser = idUser;
        }

        public override bool Equals(object obj)
        {
            Token aux = (Token)obj;
            return (this.idUser == aux.idUser && this.idToken == aux.idToken );
        }
    }
}