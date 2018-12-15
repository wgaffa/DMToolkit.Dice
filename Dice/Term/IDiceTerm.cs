using System.Collections.Generic;

namespace DMTools.Die.Term
{
    public interface IDiceTerm
    {
        IEnumerable<int> GetResults();
    }
}
