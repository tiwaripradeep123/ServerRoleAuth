using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFeed.Core.Funnel
{
    public interface IRegistration
    {
        TResolve Resolve<TResolve>();

        TResolve Resolve<TResolve>(string name);
    }
}
