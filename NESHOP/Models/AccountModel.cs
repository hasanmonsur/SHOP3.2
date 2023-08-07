using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NeSHOP.Models
{
    public class AcTranModel
    {
        public AcTranModel()
        {

        }

        [DisplayName("Customer")]
        public string CustomerCode { get; set; }
        public SelectList CustomerList { get; set; }

        [DisplayName("Invoice #")]
        public string InvoiceNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        [DisplayName("Total Amount")]
        public decimal TAmount { get; set; }

        [DisplayName("Paid Amount")]
        public decimal TPAmount { get; set; }

        [DisplayName("Due Amount")]
        public decimal TDAmount { get; set; }

        [DisplayName("Money Receipt #")]
        public string MoneyReceptNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Collection Date")]
        public DateTime CollectionDate { get; set; }

        [DisplayName("Collected By")]
        public string CollectedBy { get; set; }
        public string EmployeeList { get; set; }

        [DisplayName("Collected Amount")]
        public string CollectedAmount { get; set; }

        [DisplayName("Remaining Amount")]
        public string RemainingAmount { get; set; }

        [DisplayName("Payment Type")]
        public string PaymentType { get; set; }
        public SelectList PaymentTypeList { get; set; }

        [DisplayName("Status")]
        public String TranStatus { get; set; }
        public SelectList TranStatusList { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date From")]
        public DateTime TranDateFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date To")]
        public DateTime TranDateTo { get; set; }

        //---------------
        
        public string MsgCode { get; set; }

        public List<AcTranModel> AcTranList { get; set; }

        public string TranNo { get; set; }

        public string Customer { get; set; }

        public string Btn { get; set; }

        public DateTime TranDate { get; set; }

        public String TranType { get; set; }

        public SelectList InvoiceList { get; set; }

        public String Payment { get; set; }

        public String Status { get; set; }

        public String TranDatetemp { get; set; }

        public String Invoice { get; set; }

        public String SearchType { get; set; }

        [DisplayName("Only Posted")]
        public bool IsActive { get; set; }

        public List<AccountSumModel> AccountList { get; set; }

        public List<AcTranModel> AcList { get; set; }

        [DisplayName("Party Type")]
        public String PType { get; set; }

        public SelectList PTypeList { get; set; }

        public string PTypeName { get; set; }

        [DisplayName("Bank Name")]
        public String ChequeDetails { get; set; }
        public SelectList BankList { get; set; }

        [DisplayName("Cheque Number")]
        public String ChequeNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
         [DisplayName("Cheque Date")]
        public String ChequeDate { get; set; }

        public SelectList EmpList { get; set; }

        public string EmployeeName { get; set; }

        public string BankName { get; set; }

        public decimal CrAmount { get; set; }

        public decimal DrAmount { get; set; }
    }

    public class AcTranPaymentModel
    {
        public string BankName { get; set; }

        [DisplayName("Supplier")]
        public string VendorCode { get; set; }
        public SelectList VenList { get; set; }

        [DisplayName("Challan #")]
        public string ChallanNo { get; set; }

        [DisplayName("Invoice #")]
        public string InvoiceNo { get; set; }

        [DisplayName("Money Receipt #")]
        public string MoneyReceptNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Payment Date")]
        public DateTime PaymentDate { get; set; }

        [DisplayName("Payment By")]
        public string PaymentBy { get; set; }
        public string EmployeeList { get; set; }

        [DisplayName("Amount")]
        public decimal Amount { get; set; }

        [DisplayName("Payment Type")]
        public string PaymentType { get; set; }
        public SelectList PaymentTypeList { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date From")]
        public DateTime TranDateFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date To")]
        public DateTime TranDateTo { get; set; }

        [DisplayName("Only Posted")]
        public bool IsActive { get; set; }

        [DisplayName("Cheque Details")]
        public String  ChequeDetails { get; set; }
        //---------------

        public string MsgCode { get; set; }

        public List<AcTranPaymentModel> AcTranList { get; set; }

        public string TranNo { get; set; }

        public string Vendor { get; set; }

        public string Btn { get; set; }

        public DateTime TranDate { get; set; }

        public String TranType { get; set; }

        public SelectList InvoiceList { get; set; }

        public String Payment { get; set; }

        public String Status { get; set; }

        public String TranDatetemp { get; set; }

        public String Invoice { get; set; }

        public String SearchType { get; set; }



        public string PaymentAmount { get; set; }

        [DisplayName("Cheque Number")]
        public String ChequeNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Cheque Date")]
        public String ChequeDate { get; set; }

        public SelectList EmpList { get; set; }

        public SelectList BankList { get; set; }
    }


    public class AccountSumModel
    {
        [DisplayName("Code")]
        public string PartyCode { get; set; }

        [DisplayName("Party")]
        public string PartyName { get; set; }

        [DisplayName("# of Tran")]
        public int NoTran { get; set; }

        [DisplayName("Total Amount")]
        public decimal TAmount { get; set; }

        [DisplayName("Cr Amount")]
        public decimal CrAmount { get; set; }

        [DisplayName("Dr Amount")]
        public decimal DrAmount { get; set; }

        public string PType { get; set; }

        public string SlNo { get; set; }

        public string ChequeDetails { get; set; }

        public string TranDate { get; set; }

        public decimal TVatAmount { get; set; }

        public string ChallanNo { get; set; }

        public string InvoiceNo { get; set; }

        public decimal Amount { get; set; }

        public string TranType { get; set; }
    }

    public class JsonData
    {
        public string Message { get; set; }

        public string Time { get; set; }

        public String OrdTranNo { get; set; }

        public SelectList OrderList { get; set; }

        public String SearchKey { get; set; }
    }


    public class Product
    {
        public int Id { set; get; }
        public string Name { set; get; }
    }
}