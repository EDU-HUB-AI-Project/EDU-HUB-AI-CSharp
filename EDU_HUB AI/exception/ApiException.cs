using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.exception
{
    public class ApiException : Exception
    {
        public int Status { get; set; }

        public ApiException(int status, string message) : base(message)  
        {
            Status = status;
        }
    }
}
