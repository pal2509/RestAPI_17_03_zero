using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace RestAPI_17_03_zero.Models
{
    public class Canal
    {
        int id;
        string name;
        string CHATFILE;
        List<int> u;

        public int Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }
        public int[] users
        {
            get { return u.ToArray(); }
        }
        public string ChatFile
        {
            get { return CHATFILE; }
        }

        public Canal(int id, string nm , string file, List<int> u)
        {
            this.id = id;
            this.name = nm;
            this.CHATFILE = file;
            this.u = u;
        }

    }
}