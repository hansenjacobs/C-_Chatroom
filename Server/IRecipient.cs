using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    interface IRecipient
    {
        void DeliverMessage(Message message);
    }
}
