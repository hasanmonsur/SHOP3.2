namespace NESHOP.Models
{
    public class DaoTran
    {
        public string OrdTranNo { get; set; }

        public string IteamCode { get; set; }

        public string TranNo { get; set; }

        public string TranType { get; set; }

        public string VenCode { get; set; }

        public DateTime TranDate { get; set; }

        public string TranComment { get; set; }

        public decimal TranAmount { get; set; }

        public decimal OtherChargAmount { get; set; }

        public string PaymentType { get; set; }

        public string DelverMediaCode { get; set; }

        public string EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public bool IsActive { get; set; }

        public int QuantityPerIteam { get; set; }

        public decimal UnitPricePerIteam { get; set; }

        public decimal VatPercentage { get; set; }

        public decimal DiscountPercentage { get; set; }

        public string CounterCode { get; set; }

        public string OrdMediaCode { get; set; }

        public string ChargType { get; set; }
    }
}
