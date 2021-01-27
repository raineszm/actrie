using System;
using System.Collections.Generic;

namespace AcTrie
{
    public interface ITrie<TToken, TValue> : IDictionary<IEnumerable<TToken>, TValue> 
    {
    }
    
}