using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NeSHOP.Models
{
    public class TranModels
    {
        [DisplayName("Order/Tran #")]
        public string TranNo { get; set; }

        [Required(ErrorMessage = "Date Required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Tran Date")]
        public DateTime TranDate { get; set; }
        
        public string EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public bool IsActive { get; set; }

        public List<ChallanModels> TranIteams { get; set; }
        
        public String SearchKey { get; set; }

        public SelectList OrderList { get; set; }

        public string Btn { get; set; }
       
    }

    public class InvoiceModels
    {
        [DisplayName("Scan Barcode")]
        public string Barcode { get; set; }
        public string ModelCodeTxt { get; set; }

        [DisplayName("Invoice #")]
        public string InvoiceNo { get; set; }

        [DisplayName("Customer")]
        public string CustomerCode { get; set; }
        public SelectList CustomerList { get; set; }

        [DisplayName("Chalan #")]
        public string ChalanNo { get; set; }

        [DisplayName("Brancd")]
        public String BrandCode { get; set; }
        public SelectList BrandList { get; set; }

        [DisplayName("Product Model")]
        public string ModelCode { get; set; }
        public SelectList ModelList { get; set; }
        //public string ModelCodeTest { get; set; }

        [DisplayName("Product Details")]
        public String ModelDesc { get; set; }

        [DisplayName("Item SL#")]
        public string ItemSlNo { get; set; }

        [DisplayName("Quantity")]
        public int Quantity { get; set; }
         
        [DisplayName("Price/Unit")]
        public decimal UnitPrice { get; set; }
         
        [DisplayName("Tran Date")]
        public DateTime TranDate { get; set; }
        
        public string EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public bool IsActive { get; set; }

        //public bool IsDiscount { get; set; }
        [DisplayName("Comm Amount")]
        public decimal DisPerc { get; set; }

        [DisplayName("Comm (%)")]
        public decimal CommPerc { get; set; }

        [DisplayName("Tax (%)")]
        public decimal TaxPerc { get; set; }

        [DisplayName("Vat(%)")]
        public decimal VatPerc { get; set; }

        //--------------------------------------------
        public List<IteamModels> ChallanList { get; set; }

        public string Btn { get; set; }

        [DisplayName("Total Quantity")]
        public int TQuantity { get; set; }

        [DisplayName("Total Price Amount")]
        public decimal TUnitPtice { get; set; }

        [DisplayName("Total Discount")]
        public decimal TDisAmount { get; set; }

        [DisplayName("Total Vat")]
        public decimal TVatAmount { get; set; }

        [DisplayName("Total Tax")]
        public decimal TTaxAmount { get; set; }

        [DisplayName("Net Amount")]
        public decimal NetAmount { get; set; }

        [DisplayName("Transport Cost")]
        public decimal TransportAmount { get; set; }
        //-------------------------------------------------

        public String MsgCode { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal DueAmount { get; set; }

        public String SearchType { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date From")]
         public DateTime TranDateFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date To")]
        public DateTime TranDateTo { get; set; }

        public List<InvoiceModel> InvoiceList { get; set; }

        public string Customer { get; set; }

        public decimal TAmount { get; set; }

        public string DisplayInvoiceDate { get; set; }

        public SelectList CustChallanList { get; set; }

        public decimal CostBuyPrice { get; set; }

        [DisplayName("Iteam Reord")]
        public string InvoiceItem { get; set; }
        public SelectList InvoiceItemList { get; set; }



        public String PONo { get; set; }

        public String ChallanRemarks { get; set; }

        public string[] pDataList { get; set; }

        
    }

    public class InvoiceModel
    {
        [DisplayName("Invoice #")]
        public string InvoiceNo { get; set; }

        [DisplayName("Customer")]
        public string CustomerCode { get; set; }
        public SelectList CustomerList { get; set; }

        [DisplayName("Chalan #")]
        public string ChalanNo { get; set; }

        [DisplayName("Product")]
        public string ModelCode { get; set; }
        public SelectList ModelList { get; set; }

        [DisplayName("Quantity")]
        public int Quantity { get; set; }

        [DisplayName("Price/Unit")]
        public decimal UnitPrice { get; set; }

        [DisplayName("Vat (%)")]
        public decimal TaxPerc { get; set; }

        [DisplayName("Tran Date")]
        public DateTime TranDate { get; set; }

        public string EntryBy { get; set; }

        public DateTime EntryDate { get; set; }

        public bool IsActive { get; set; }
        //--------------------------------------------
        public List<IteamModels> ChallanList { get; set; }

        public string Btn { get; set; }

        [DisplayName("Total Quantity")]
        public String TQuantity { get; set; }

        [DisplayName("Total Price Amount")]
        public decimal TUnitPtice { get; set; }

        [DisplayName("Vat Amount")]
        public decimal TVatAmount { get; set; }

        [DisplayName("Tax Amount")]
        public decimal TTaxAmount { get; set; }
        

        [DisplayName("Net Amount")]
        public decimal NetAmount { get; set; }

        [DisplayName("Advance Pay Amount")]
        public decimal AdvanceAmount { get; set; }
        //-------------------------------------------------

        public String MsgCode { get; set; }

        public string IsPaird { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal DueAmount { get; set; }

        public String SearchType { get; set; }


        public string Customer { get; set; }

        public string SLNo { get; set; }

        public string IteamName { get; set; }

        public string ChalanPO { get; set; }

        public string vQuantity { get; set; }


        public decimal VatPerc { get; set; }
        public string TaxDesc { get; set; }



        public DateTime ChallanDate { get; set; }

        public decimal NCostPrice { get; set; }

        public decimal DisPerc { get; set; }

        public decimal CommPerc { get; set; }

        public decimal TDisCost { get; set; }

        public decimal TCommCost { get; set; }

        public decimal TotalCost { get; set; }

        public decimal NUnitPrice { get; set; }

        public decimal TNUnitPrice { get; set; }
    }

    public class ChallanModels
    {
        public string ChallanNo { get; set; }
        public string Barcode { get; set; }
        public string ModelCodeTxt { get; set; }
        


        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Challan Date")]
        public DateTime ChallanDate { get; set; }

        [DisplayName("Customer")]
        public string CustomerCode { get; set; }
        public SelectList CustomerList { get; set; }

        [DisplayName("Purchas Order #")]
        public string PONo { get; set; }

        [DisplayName("Remarks About Challan")]
        public string ChallanRemarks { get; set; }

        [DisplayName("Brand")]
        public string BrandCode { get; set; }
        public SelectList BrandList { get; set; }

        [DisplayName("Product Item")]
        public string ModelCode { get; set; }
        public SelectList ModelList { get; set; }

        [DisplayName("Desription")]
        public String ModelDesc { get; set; }

        [DisplayName("Item SL#")]
        public string ItemSlNo { get; set; }


        [DisplayName("Quantity")]
        public int Quantity { get; set; }

        public List<IteamModels> IteamList { get; set; }

        public String MsgCode { get; set; }

        public string Btn { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date From")]
        public DateTime TranDateFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date To")]
        public DateTime TranDateTo { get; set; }

        public List<ChallanModel> ChallanList { get; set; }

        public String SearchType { get; set; }

        [DisplayName("Iteam List")]
        public String ChallanIteam { get; set; }
        public SelectList ChallanIteamList { get; set; }

        public bool  CheckSelect { get; set; }

        public int SlNo { get; set; }


        public String OrdTranNo { get; set; }

        public String IteamName { get; set; }
        public String QuantityPerIteam { get; set; }
        public String UnitPricePerIteam { get; set; }

        public String TotalAmount { get; set; }
        public String PayAmount { get; set; }

        public String PayType { get; set; }

        public String TranNo { get; set; }

    }

    public class ChallanModel
    {

        public string ChallanNo { get; set; }

        [DisplayName("Challan Date")]
        public DateTime ChallanDate { get; set; }

        [DisplayName("Customer")]
        public string CustomerCode { get; set; }
        public SelectList CustomerList { get; set; }

        [DisplayName("PO Number")]
        public string PONo { get; set; }

        [DisplayName("Remarks")]
        public string ChallanRemarks { get; set; }

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

        [DisplayName("Desription")]
        public Decimal ModelDesc { get; set; }

        [DisplayName("Serial #")]
        public string IteamSLNo { get; set; }

        [DisplayName("Is Paired")]
        public bool IsPaired { get; set; }


        [DisplayName("Catagory")]
        public string CatagoryCode2 { get; set; }
        public SelectList CatagoryList2 { get; set; }

        [DisplayName("Product")]
        public string ProductCode2 { get; set; }
        public SelectList ProductList2 { get; set; }

        [DisplayName("Brand")]
        public string VenCode2 { get; set; }
        public SelectList VenList2 { get; set; }

        [DisplayName("Model")]
        public string ModelCode2 { get; set; }
        public SelectList ModelList2 { get; set; }

        [DisplayName("Desription")]
        public Decimal ModelDesc2 { get; set; }

        [DisplayName("Serial #")]
        public string IteamSLNo2 { get; set; }

        public string Customer { get; set; }

        public string Quantity { get; set; }

        public string IteamName { get; set; }

        public string SLNo { get; set; }

        public string IsPair { get; set; }
    }

    public class TarmsModels
    {
        [DisplayName("Tarms Code")]
        public string TarmsCode { get; set; }

        public DateTime EntryDate { get; set; }

        [DisplayName("Tarms Desc")]
        public String TarmsName { get; set; }
        
        public String SearchKey { get; set; }

        public SelectList TarmsList { get; set; }

        public string Btn { get; set; }


        public object MsgCode { get; set; }
    }

    public class InvoiceReturnModels
    {
        [DisplayName("Invoice #")]
        public string InvoiceNo { get; set; }

        [DisplayName("Customer")]
        public string CustomerCode { get; set; }
        public SelectList CustomerList { get; set; }

        [DisplayName("Chalan #")]
        public string ChalanNo { get; set; }

        [DisplayName("Product")]
        public string ModelCode { get; set; }
        public SelectList ModelList { get; set; }

        [DisplayName("Return Quantity")]
        public int ReturnQuantity { get; set; }

        [DisplayName("Issue Quantity")]
        public int IssueQuantity { get; set; }

        public string EntryBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Tran Date")]
        public DateTime TranDate { get; set; }

        public bool IsActive { get; set; }
        //--------------------------------------------
        public List<IteamModels> ChallanList { get; set; }

        public string Btn { get; set; }



        public string Customer { get; set; }

        public string MsgCode { get; set; }
        
        public string NewChalanNo { get; set; }
    }
}