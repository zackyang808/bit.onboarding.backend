using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bit.common.Mongo
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }
}
