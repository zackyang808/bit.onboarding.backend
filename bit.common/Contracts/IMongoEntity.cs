using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.Contracts
{
    public interface IMongoEntity<TId>
    {
        TId Id { get; set; }
    }
}
