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
        //Configurações de acesso á base de dados
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

        public List<Filettl> GetFiles()
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT * FROM filettl");
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            List<Filettl> result = new List<Filettl>();
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    Filettl values = new Filettl(dataReader.GetInt32(0),dataReader.GetString(1), DateTime.Parse(dataReader.GetString(2)));

                    result.Add(values);
                }
            }
            conn.Close();
            return result;
        }

        public Filettl GetFile(string filename)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT * FROM filettl WHERE f_name = '{0}'",filename);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            Filettl result = null;
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                { 
                    result = new Filettl(dataReader.GetInt32(0), dataReader.GetString(1), DateTime.Parse(dataReader.GetString(2)));
                }
            }
            conn.Close();
            return result;
        }


        public int RemoveFilettl(string filename)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("DELETE FROM filettl WHERE f_name = '{0}'", filename);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            sqlCommand.ExecuteNonQuery();
            conn.Close();
            return 1;
        }



        public int RemoveRequest(string username)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("DELETE FROM pedidoresg WHERE u_name = '{0}'",username);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            sqlCommand.ExecuteNonQuery();
            conn.Close();
            return 1;
        }

        /// <summary>
        /// Verifica se um User existe
        /// </summary>
        /// <param name="username"></param>
        /// <param name="psswd"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Procura o Id do user através do nome de utilizador e da palavra-passe
        /// </summary>
        /// <param name="username">Nome de utilizador</param>
        /// <param name="psswd">Palavra-pase</param>
        /// <returns>O id do utilizador</returns>
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

        /// <summary>
        /// Metodo para encontrar os Id do User através do username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public int UserID(string username)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT id_user FROM users WHERE username = '{0}'", username);
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

        public int UserCount()
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT COUNT(id_user) FROM users ");
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


        public string GetUsername(int id)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT username FROM users WHERE id_user = {0}", id);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            string r = null;
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    r = dataReader.GetString(0);
                }
            }

            conn.Close();
            return r;
        }

        public int GetAccessLevel(int id)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT acclevel FROM users WHERE id_user = {0}", id);
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

        public int AddUser(User u)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("INSERT INTO users ( id_user, username, u_psswd, acclevel ) values ( {0},'{1}','{2}',{3} );", u.Id,u.UserName, u.PassWord,u.Acclevel);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            sqlCommand.ExecuteNonQuery();
            conn.Close();
            return 1;
        }


        public int AddFileTime(int uid, string filename, TimeSpan time )
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("INSERT INTO filettl ( u_id, f_name, f_time ) values ( {0},'{1}','{2}' );",uid, filename, DateTime.Now.Add(time).ToString());
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            sqlCommand.ExecuteNonQuery();
            conn.Close();
            return 1;
        }

        public int AddRegistrationRequest(string username, string psswd)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("INSERT INTO pedidoresg ( u_name, u_psswd ) values ( '{0}','{1}' );", username, psswd);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            sqlCommand.ExecuteNonQuery();
            conn.Close();
            return 1;
        }

        public Registration GetRegRequest(string usrnm)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT * FROM pedidoresg WHERE u_name = '{0}'", usrnm);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            Registration r = null;
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    r = new Registration(dataReader.GetString(0),dataReader.GetString(1));
                }
            }
            conn.Close();
            return r;
        }


        public bool RegRequestExists(string usrnm, string psswd)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT u_name FROM pedidoresg WHERE u_name = '{0}'", usrnm);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            string r = null;
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    r = dataReader.GetString(0);
                }
            }
            conn.Close();
            if (r == null) return false;
            else return true;
        }

        public List<string> RegRequestList()
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT u_name FROM pedidoresg");
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            List<string> result = new List<string>() ;
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    result.Add(dataReader.GetString(0)); 
                }
            }
            conn.Close();
            return result;
        }


    }
    
}