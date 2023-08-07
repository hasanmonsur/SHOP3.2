using System;

namespace NESHOP.Models
{
    public class DaoVendor
    {
        public string VenCode { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string DistCode { get; set; }
        public string CountryCode { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsActive { get; set; }
        public string VenType { get; set; }


        public string EntryBy { get; set; }

        public DateTime EntryDate { get; set; }
    }
}
