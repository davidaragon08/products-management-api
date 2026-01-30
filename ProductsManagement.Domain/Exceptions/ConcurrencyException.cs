using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsManagement.Domain.Exceptions
{
    public sealed class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message) : base(message) { }
    }
}
