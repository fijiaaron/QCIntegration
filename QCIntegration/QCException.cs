using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace oneshore.QCIntegration
{
    public class QCException : ApplicationException
    {
        public QCException(string message) : base(message) 
        {}
    }
}
