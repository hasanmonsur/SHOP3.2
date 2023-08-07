using System;

namespace NESHOP.Models
{
    public class DaoOrder
    {
        public string OrdTranNo { get; set; }

        public char OrdType { get; set; }

        public int OrdQuantity { get; set; }

        public string OrdMediaCode { get; set; }

        public DateTime OrdExecutioDate { get; set; }

        public string OrdComment { get; set; }

        public decimal OrdAmount { get; set; }

        public decimal OrdAdvAmount { get; set; }

        public string IteamCode { get; set; }

        public string Remarks { get; set; }

        public decimal TaxPerc { get; set; }

        public decimal VatPerc { get; set; }

        public string EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public bool IsActive { get; set; }

        public string VenCode { get; set; }

        public string CounterCode { get; set; }

        public string DelverMediaCode { get; set; }

        public string TranType { get; set; }
    }
}
