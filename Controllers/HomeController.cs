using Pna.Service.Interface;
using PnaAPI.Models;
using PnaAPI.Models.paramModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace PnaAPI.Controllers
{
    public class HomeController : ApiController
    {
        [ActionName("test")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };

        }
    }
}
