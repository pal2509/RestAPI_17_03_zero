using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPI_17_03_zero.Models
{
    public class Filettl
    {
        int id;
        string name;
        DateTime val;

        public int Uid
        {
            get { return id; }
        }

        public string FName
        {
            get { return name; }
        }

        public DateTime FVal
        {
            get { return val; }
        }

        public Filettl(int id,string name, DateTime val)
        {
            this.id = id;
            this.name = name;
            this.val = val;
        }
    }
}