using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface IAuthorizationRepository
    {
        Authorization GetWithToken(String Token);
        void Add(Authorization a);
        void Remove(String Token);
    }
}