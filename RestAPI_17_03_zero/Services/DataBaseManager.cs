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
        /// Retorna os tempos de vida dos ficheiros todos de um utilizador
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<Filettl> GetUFiles(int uid)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT * FROM filettl WHERE u_id = {0}", uid);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            List<Filettl> result = new List<Filettl>();
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    Filettl values = new Filettl(dataReader.GetInt32(0), dataReader.GetString(1), DateTime.Parse(dataReader.GetString(2)));

                    result.Add(values);
                }
            }
            conn.Close();
            return result;
        }

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

        /// <summary>
        /// Retorna todos os ficheiros que têm tempo de vida da base de dados para uma lista
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Procura um determinado ficheiro na base dados se para ver o seu tempo de vida
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Remove o tempo de vida de um ficheiro da base de dados
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public int RemoveFilettl(int uid, string filename)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("DELETE FROM filettl WHERE f_name = '{0}' AND u_id = {1}", filename, uid);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            sqlCommand.ExecuteNonQuery();
            conn.Close();
            return 1;
        }

        /// <summary>
        /// Retorna todos os canais existentes
        /// </summary>
        /// <returns></returns>
        public string[] GetChannels()
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT c_name FROM canais");
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            List<string> r = new List<string>();
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    r.Add(dataReader.GetString(0));
                }
            }
            conn.Close();
            return r.ToArray();
        }

        /// <summary>
        /// Retorna os canais que um utilizador está subscrito
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string[] GetSubedChannels(int uid)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT c_name FROM canais INNER JOIN (SELECT c_id FROM canaisrelação WHERE u_id = {0}) ids ON ids.c_id = canais.c_id",uid);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            List<string> r = new List<string>();
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    r.Add(dataReader.GetString(0));
                }
            }
            conn.Close();
            return r.ToArray();
        }

        /// <summary>
        /// Remove um pedido de registo da base de dados
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Faz a contagem de utilizadores
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Reotorna o nome do ficheiro em que o chat desse canal está
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string GetChannelFile(string channel)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT c_file FROM canais WHERE c_name = '{0}'",channel);
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

        /// <summary>
        /// Verifica se o utilizador está subscrito a esse canal
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="channel"></param>
        /// <returns>Retorna -1 se não está subscrito, se está subscrito retorna o id do canal</returns>
        public int IsUserSub(int uid, string channel)
        {
            int cid = GetChannelId(channel);
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT c_id FROM canaisRelação WHERE u_id = {0} AND c_id = {1}", uid, cid);
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
        /// Retorna o nome de utilizador através do seu id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Retorna o nivel de acesso do utilizador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Retorna o id do canal através do seu nome
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public int GetChannelId(string channel)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("SELECT c_id FROM canais WHERE c_name = '{0}'", channel);
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
        /// Adiciona um utilizador á base de dados
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adiciona um tempo de vida a um ficheiro
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="filename"></param>
        /// <param name="time"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adiciona um pedido de registo á base de dados
        /// </summary>
        /// <param name="username"></param>
        /// <param name="psswd"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Subscreve um utilizador a um canal
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int SubToChannel(string channel, int uid)
        {
            int c = GetChannelId(channel);
            NpgsqlConnection conn = new NpgsqlConnection(connString);
            var sqlStatement = string.Format("INSERT INTO canaisrelação values ( {0},{1} );", c, uid);
            var sqlCommand = new NpgsqlCommand(sqlStatement, conn);
            conn.Open();
            sqlCommand.ExecuteNonQuery();
            conn.Close();
            return 1;
        }

        /// <summary>
        /// Retorna um pedido de registo através do seu nome de utilizador
        /// </summary>
        /// <param name="usrnm"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Verifica se um pedido de registo existe através do nome de utilizador no pedido de registo
        /// </summary>
        /// <param name="usrnm"></param>
        /// <param name="psswd"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Retorna todos os pedidos de registo
        /// </summary>
        /// <returns></returns>
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