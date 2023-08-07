namespace NESHOP.Models
{
    public class DaoSuplier
    {
        public string SupplierContractPerson { set; get; }
        public string CompanyName { set; get; }
        public string ContractPhone { set; get; }
        public string ContractEmail { set; get; }
        public string SupplierCode { set; get; }
        public string CompanyAddress { get; set; }
        public string EntryBy { get; set; }
        public decimal EntryDate { get; set; }
        public bool IsActive { set; get; }
    }
}
