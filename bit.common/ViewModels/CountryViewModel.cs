using bit.common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.ViewModels
{
    public class CountryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string President { get; set; }
        public decimal BankBalance { get; set; }
        public int Population { get; set; }
        public string Theme { get; set; }
        public string ThemeImage { get; set; }
        public List<int> DigitalAssests { get; set; }
        public decimal EstimatedValue { get; set; }
        public decimal OriginalValue { get; set; }
        public string OwnerId { get; set; }
        public string OwnerAddress { get; set; }
        public int CountryIndex { get; set; }
        public string TxTran { get; set; }
        public int CountryContractUniqueId { get; set; }
        public string Status { get; set; }
        public List<Block> Blocks { get; set; }
        public int TotalBlocks { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
