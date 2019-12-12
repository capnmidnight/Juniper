using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public interface IUpdatable
    {
        void Update(object sender, EventArgs args);
    }
}
