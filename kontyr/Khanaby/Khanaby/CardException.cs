using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Khanaby
{
    public class CardException: ApplicationException
    {
        public CardException() { }

        public CardException(string message) : base(message) { }

        public CardException(string message, Exception inner) : base(message, inner) { }

        protected CardException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
