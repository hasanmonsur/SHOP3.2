using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NeSHOP.Models
{
    public class ProductModels
    {

        [DisplayName("Catagory")]
        public string CatagoryCode { get; set; }
        public SelectList CatagoryList { get; set; }

        [DisplayName("Product Code ")]
        public string ProductCode { get; set; }

        [DisplayName("Product Name ")]
        public string ProductName { get; set; }

        //---------------Button----------------------
        public string Btn { get; set; }

        public String MsgCode { get; set; }

        public String SearchKey { get; set; }

        public SelectList ProductList { get; set; }
        
    }
    //----------------------------------------
    public class CatagoryModels
    {

        [DisplayName("Catagory Code")]
        public string CatagoryCode { get; set; }

        [DisplayName("Catagory Name ")]
        public string CatagoryName { get; set; }

        //---------------Button----------------------
        public string Btn { get; set; }

        public String MsgCode { get; set; }

        public String SearchKey { get; set; }

        public SelectList CatagoryList { get; set; }
    }
    //----------------------------------------
    public class IteamModelModels
    {

        [DisplayName("Product Code ")]
        public string ModelCode { get; set; }

        [DisplayName("Product Name (Eng.) ")]
        public string ModelName { get; set; }

        [DisplayName("Product Name (Bang.) ")]
        public string ModelBName { get; set; }

        [DisplayName("Brand")]
        public String BrandCode { get; set; }
        public SelectList BrandList { get; set; }

        [DisplayName("Class & Addition")]
        public String ModelDesc { get; set; }

        [DisplayName("Unit Price")]
        public Decimal ModelPrice { get; set; }

        //---------------Button----------------------
        public string Btn { get; set; }
        
        public String MsgCode { get; set; }

        public String SearchKey { get; set; }

        public SelectList IteamModelList { get; set; }

        public string Barcodeurl { get; set; }


    }

    //---------------------------------------------------------------------------------
    public class VendorModels
    {

        [DisplayName("Brand Code ")]
        public string BrandCode { get; set; }

        [DisplayName("Brand Name ")]
        public string BrandName { get; set; }

        [DisplayName("Supplier ")]
        public string SupplierCode { get; set; }
        public SelectList SupplierList { get; set; }
        
        //---------------Button----------------------
        public string Btn { get; set; }

        public String MsgCode { get; set; }

        public String SearchKey { get; set; }

        public SelectList BrandList { get; set; }



       
    }

    //--------------------------------------------
    public class OrderModels
    {
        [Required(ErrorMessage = "OrdTranNo Required")]
        [DisplayName("Qutation #")]
        [StringLength(16, ErrorMessage = "Must be less than 16 characters")]
        public string OrderRefNo { get; set; }

        [DisplayName("Iteam")]
        public string IteamCode { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date")]
        public DateTime EntryDate { get; set; }

        [DisplayName("By")]
        public DateTime EntryBy { get; set; }

        [DisplayName("Cost/Unit")]
        public Decimal CostPrice { get; set; }

        [DisplayName("Decsription")]
        public string OrderDesc { get; set; }

        [DisplayName("Subject")]
        public string OrderSub { get; set; }

        [DisplayName("Customer")]
        public string CustomerCode { get; set; }
        public SelectList CustomerList { get; set; }
        
        //-------------------------------------
        [DisplayName("Catagory")]
        public string CatagoryCode { get; set; }
        public SelectList CatagoryList { get; set; }

        [DisplayName("Product")]
        public string ProductCode { get; set; }
        public SelectList ProductList { get; set; }

        [DisplayName("Brand")]
        public string VenCode { get; set; }
        public SelectList VenList { get; set; }

        [DisplayName("Model")]
        public string ModelCode { get; set; }
        public SelectList ModelList { get; set; }
        public bool IsActive { get; set; }


        public string Btn { get; set; }

        public String SearchKey { get; set; }

        public List<OrderModel> QutationList { get; set; }

        public string MsgCode { get; set; }


        public String[] Tarms { get; set; }

        public MultiSelectList TarmsList { get; set; }

        public List<IteamModels> IteamlList { get; set; }

        public String SearchType { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date From")]
        public DateTime TranDateFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date To")]
        public DateTime TranDateTo { get; set; }

        public String CostBuyPrice { get; set; }

        [DisplayName("Item Quantity")]
        public String Quantity { get; set; }

        public bool IsVat { get; set; }

        public bool IsTax { get; set; }

        public decimal TaxPerc { get; set; }

        public decimal VatPerc { get; set; }

        public bool IsPaird { get; set; }
    }
    //----------------------------------------

    public class OrderIteamModel
    {
        [DisplayName("Sl#")]
        public string SlNo { get; set; }

        [Required(ErrorMessage = "IteamCode Required")]
        [DisplayName("Iteam Code")]
        public string IteamCode { get; set; }
        public string IteamDec { get; set; }
        public SelectList IteamCodeList { get; set; }

        [Required(ErrorMessage = "Ord. Quantity Required")]
        [DisplayName("Ord. Quantity")]
        public int OrdQuantity { get; set; }

        [DisplayName("Unit Cost Price")]
        public Decimal UnitCostPrice { get; set; }

        [DisplayName("Vat Percentage")]
        public Decimal VatPercentage { get; set; }

        [DisplayName("Discount Percentage")]
        public Decimal Discount { get; set; }

        [Required(ErrorMessage = "Vendor Code Required")]
        [DisplayName("Vendor Code")]
        public string VenCode { get; set; }
        public string VenDec { get; set; }
        public SelectList VenCodeList { get; set; }


        public int Status { get; set; }
    }

    //--------------------------------------------------------------------------------
    public class IteamModels
    {
        [Required(ErrorMessage = "Iteam Code Required")]
        [DisplayName("Tran #")]
        public string IteamCode { get; set; }
       
        

        [Required(ErrorMessage = "Iteam Desc Required")]
        [DisplayName("Iteam Description")]
        public string IteamDesc { get; set; }

        [Required(ErrorMessage = "Catagory Code Required")]
        [DisplayName("Catagory")]
        [StringLength(3, ErrorMessage = "Must be less than 11 characters")]
        public string CatagoryCode { get; set; }
        public SelectList CatagoryList { get; set; }

        

        [Required(ErrorMessage = "Catagory Code Required")]
        [DisplayName("Brand")]
        [StringLength(3, ErrorMessage = "Must be less than 11 characters")]
        public string BrandCode { get; set; }
        public SelectList BrandList { get; set; }

        [DisplayName("Product")]
        public string ProductCode { get; set; }
        public SelectList ProductList { get; set; }

        [DisplayName("Product Item ")]
        public string IteamSLNo { get; set; }
        public SelectList ModelList { get; set; }

        [DisplayName("Photo")]
        public byte IteamPicture { get; set; }

        [DisplayName("Quantity ")]
        public int Quantity { get; set; }
        //public SelectList IteamTypeList { get; set; }

        [DisplayName("Price/Unit")]
        public decimal CostPrice { get; set; }

        [DisplayName("Challan #")]
        public String ChallanNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Tran Date")]
        public DateTime TranDate { get; set; }

        public string TranType { get; set; }

        public string VendorCode { get; set; }
        public SelectList VendorList { get; set; }



        //-------Button Feild--------------------------
        public string Btn { get; set; }

        public List<IteamModels> IteamList { get; set; }

        public String SearchKey { get; set; }

        public String MsgCode { get; set; }

        //----------Report Extra Field--------

        public string CatagoryName { get; set; }

        public string VendorName { get; set; }

        public string ProductName { get; set; }

        public string ModelName { get; set; }

        public string EntryDate { get; set; }


        public String  SearchType { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date From")]
        public DateTime TranDateFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date To")]
        public DateTime TranDateTo { get; set; }

        public int TotalStock { get; set; }

        public string SLNo { get; set; }

        
        public string EntryBy { get; set; }

        public string InvoiceNo { get; set; }

        [DisplayName("Commission (%)")]
        public string DisPerc { get; set; }

        [DisplayName("Is Item Falut")]
        public bool fStatus { get; set; }

        public string FaultStatus { get; set; }

        public string[] pDataList { get; set; }


        public string CutomerCode { get; set; }

        public decimal CommPerc { get; set; }
        public string ModelCode { get; set; }
        public int IsVat { get;  set; }
        public decimal VatPerc { get;  set; }
        public int IsTax { get;  set; }
        public decimal TaxPerc { get;  set; }
    }

    public class OrderModel
    {

        public string OrderRefNo { get; set; }

        public string Customer { get; set; }

        public string OrderSub { get; set; }

        public string IteamName { get; set; }

        public string OrderDesc { get; set; }

        public decimal CostPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public string Catagory { get; set; }

        public string Model { get; set; }

        public string Product { get; set; }

        public string Vendor { get; set; }

        public string CustomerAddress { get; set; }

        public string SlNo { get; set; }

        public decimal VatPrice { get; set; }

        public string showPrice { get; set; }

        public string showUnitCost { get; set; }

        public string showQuantity { get; set; }

        public decimal tCostAmount { get; set; }
    }

    public class TermsModel
    {

        public string SlNo { get; set; }

        public string TermsDesc { get; set; }
        
    }

    public class ItemModel
    {

        public string ItemCode1 { get; set; }

        public string ItemSl1 { get; set; }

        public string ItemCode2 { get; set; }

        public string ItemSl2 { get; set; }

        public string TranType { get; set; }

    }
   

}