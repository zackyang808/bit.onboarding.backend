using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.Contracts
{
    public interface IMongoCommon : IMongoEntity<string>
    {
        bool IsDeleted { get; set; }
    }
}
