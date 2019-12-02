using Support.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Support.Controllers
{
    public class BaseWebAPIController : ApiController
    {
        private DB _db;
        public DB db
        {
            get
            {
                if (_db == null)
                {
                    _db = DB.Create();
                }

                return _db;
            }
        }
    }
}
