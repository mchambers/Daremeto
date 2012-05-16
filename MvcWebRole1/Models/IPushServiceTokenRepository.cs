using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface IPushServiceTokenRepository
    {
        void Add(PushServiceToken t);
        List<PushServiceToken> TokensForCustomer(long CustomerID);
        void Delete(PushServiceToken t);
    }
}