using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PnaAPI.Models.paramModel
{
    public class NewsModel : BaseModel
    {

        public int themeId { get; set; }

        public int newsCategoryId { get; set; }

        public int newsId { get; set; }

        public string topicContents { get; set; }

        public string creater { get; set; }
    }
}