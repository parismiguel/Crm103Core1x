using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbpCompanyName.AbpProjectName.Web.Models
{
    public class Response
    {
        public Response()
        {
            Status = false;
        }

        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string Trace { get; set; }
    }
}
