using RestAPI_17_03_zero.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace RestAPI_17_03_zero.Services
{
    public class UserRepository
    {
        
        public int AuthUser(string username, string password)
        {
            try
            {
                Users users = new Users();

                User u = users.UserExists(username, password);

                if (u != null) return u.Id;
                else return -1;
            }
            catch(Exception e)
            {
                throw new HttpResponseException(e);
            }
            
        }




    }
}