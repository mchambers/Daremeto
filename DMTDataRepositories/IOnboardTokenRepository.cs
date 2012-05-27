using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DareyaAPI.Models
{
    public interface IOnboardTokenRepository
    {
        OnboardToken Get(string VerificationString);
        void Add(OnboardToken token);
        void Update(OnboardToken token);
        void Remove(OnboardToken token);
    }
}
