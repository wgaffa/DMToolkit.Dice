using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Core.DiceTerm
{
    public interface IDiceTerm
    {
        IEnumerable<int> GetResults();
    }
}
