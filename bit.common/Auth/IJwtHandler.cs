using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using bit.common.Auth;

namespace bit.common.Auth
{
    public interface IJwtHandler
    {
        JsonWebToken Create(string userId);
    }
}
