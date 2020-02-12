using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.ViewModels
{
    public class BlockViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public UserViewModel Owner { get; set; }
        public int BlockAxis { get; set; }
        public int BlockYxis { get; set; }
        public int TotalResidents { get; set; }
    }
}
