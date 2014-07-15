using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PnaAPI.Models.paramModel
{
    public class BaseModel
    {

        public string keyWord { get; set; }

        public int userId { get; set; }

        public int pId { get; set; }

        public long KeyId { get; set; }

        //  public int firstPage { get; set; }
        // public int endPage { get; set; }

        private int _fristPage = 1;
        private int _endPage = 20;
        public int firstPage
        {
            get { return _fristPage; }
            set { _fristPage = value; }
        }

        public int endPage
        {
            get { return _endPage; }
            set { _endPage = value; }
        }

    }
}