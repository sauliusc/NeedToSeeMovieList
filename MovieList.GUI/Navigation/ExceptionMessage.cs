using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.GUI.Navigation
{
    public class ExceptionMessage
    {
        public Exception Exception {get;set;}
        public string Message
        {
            get { return Exception.Message; }
        }

        public ExceptionMessage(Exception exception)
        {
            Exception = exception;
        }

    }
}
