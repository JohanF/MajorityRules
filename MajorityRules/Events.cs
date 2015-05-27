using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMessenger;

namespace SurfaceApplication1
{
    public class NewIdeaEvent : ITinyMessage
    {
        /// <summary>
        /// The sender of the message, or null if not supported by the message implementation.
        /// </summary>
        public object Sender { get; private set; }
    }

    public class DismissTextboxEvent : ITinyMessage
    {
        /// <summary>
        /// The sender of the message, or null if not supported by the message implementation.
        /// </summary>
        public object Sender { get; private set; }
    }
}
