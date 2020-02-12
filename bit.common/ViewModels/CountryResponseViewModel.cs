using bit.common.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.ViewModels
{
    public class CountryResponseViewModel
    {
        public CountryViewModel Country { get; set; }
        public List<BlockViewModel> Blocks { get; set; }
    }
}
