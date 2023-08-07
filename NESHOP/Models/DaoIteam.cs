using System;

namespace NESHOP.Models
{
    public class DaoIteam
    {
        public DateTime EntryDate;
        public string IteamCode { get; set; }
        public string IteamDesc { get; set; }
        public string CatagoryCode { get; set; }
        public byte IteamPicture { get; set; }
        public string IteamType { get; set; }
        public DateTime EffictiveDate { get; set; }
        public string TaxCode { get; set; }

        public string EntryBy { get; set; }
    }
}
