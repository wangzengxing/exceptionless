using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exceptionless.Demo
{
    public class ExceptionlessLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ExceptionlessLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }
}
