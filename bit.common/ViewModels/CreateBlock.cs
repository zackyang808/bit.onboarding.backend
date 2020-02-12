namespace bit.common.Models
{
    public class CreateBlock
    {
        public string BlockOwner { get; set; }
        public int Axis { get; set; }
        public int YAxis { get; set; }
        public int BlockNumber { get; set; }
        public string Status { get; set; }
        public string TxTran { get; set; }
        public string CountryId { get; set; }
    }
}