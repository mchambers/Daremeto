using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DareyaAPI.Controllers
{
    public interface IBillingProcessor
    {
        // These are the money we're charged to take this amount via this billing processor.
        decimal GetProcessingFeesForAmount(decimal Amount);

    }
}
