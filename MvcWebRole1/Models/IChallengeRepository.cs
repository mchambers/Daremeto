using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DareyaAPI.Models
{
    interface IChallengeRepository
    {
        IQueryable<Challenge> GetAll();
        Challenge Get(int id);
        Challenge Add(Challenge item);
        void Remove(int id);
        bool Update(Challenge item);
    }
}
