﻿using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.Mongo
{
    public class MongoOption
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public bool Seed { get; set; }
    }
}
