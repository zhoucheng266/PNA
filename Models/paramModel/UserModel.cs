using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PnaAPI.Models.paramModel
{
    public class UserModel:BaseModel
    {

        public string userName { get; set; }

        public string phone { get; set; }

        public string email { get; set; }

        public string passWord { get; set; }


        public string oldpwd { get; set; }

        public string newpwd { get; set; }

        public string smskey { get; set; }

        public int refUserId { get; set; }

        public string Longitude { get; set; }

        public string Dimension { get; set; }

        public int metre { get; set; }
    }
}