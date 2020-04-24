using Npgsql;
using RestAPI_17_03_zero.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace RestAPI_17_03_zero.Services
{
    public class DataBaseManager
    {
        private const string Host = "127.0.0.1";
        private const string User = "apilog";
        private const string DBname = "APIDB";
        private const string Password = "pc50";
        private const string Port = "5432";
        //String com os parametros para a conecção é base de dados
        private string connString = String.Format(
                            "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                            Host,
                           User,
                           DBname,
                           Port,
                           Password);

        /// <summary>
        /// Recebe todos os users da base de dados
        /// </summary>
        /// <returns>List<User> com todos os users</returns>
        public List<User> GetAllUsers()
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT * FROM users");
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            List<User> result = new List<User>();
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    User values = new User(dataReader.GetInt32(0),dataReader.GetString(1),dataReader.GetString(2),1);
                    
                    result.Add(values);
                }
            }
            conn.Close();
            return result;
        }

        public bool UserExists(string username,string psswd)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT id_user FROM users WHERE username = '{0}' AND u_psswd = '{1}'",username,psswd);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            int r = -1;
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {

                    r = dataReader.GetInt32(0);
                    
                }
            }

            conn.Close();
            if (r == -1) return false;
            else return true;
        }

        public int UserID(string username, string psswd)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT id_user FROM users WHERE username = '{0}' AND u_psswd = '{1}'", username, psswd);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            int r = -1;
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {

                    r = dataReader.GetInt32(0);

                }
            }

            conn.Close();
            return r;
        }
    }
    
}