using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using RestAPI_17_03_zero.Models;
using System.IO;

namespace RestAPI_17_03_zero.Services
{
    public class Tasks
    {
        static int timeint = 60000;
        static Timer timer = new System.Threading.Timer(new TimerCallback(CheckFiles), null, timeint, Timeout.Infinite);

        private static void CheckFiles(object state)
        {
            DataBaseManager db = new DataBaseManager();
            List<Filettl> f = db.GetFiles();
            if (f != null)
            {
                DateTime now = DateTime.Now;
                foreach (Filettl t in f)
                {
                    TimeSpan diff = now.Subtract(t.FVal);
                    if (diff > new TimeSpan(0, 0, 0))
                    {
                        DeleteUserFile(t.Uid, t.FName);
                        db.RemoveFilettl(t.FName);                      
                    }
                }
            }

            timer.Change(timeint, Timeout.Infinite);
        }

        private static void DeleteUserFile(int id, string filename)
        {
            DataBaseManager db = new DataBaseManager();
            var filepath = AppDomain.CurrentDomain.BaseDirectory + db.GetUsername(id) + "\\" + filename;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);

            }
        }

    }
}