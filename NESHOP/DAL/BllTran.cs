using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Contacts;
using NESHOP.Models;

namespace NeSHOP.DAL
{
    public class BllTran : IBllTran
    {
        //BllDbConnection objBllDbConnection = new BllDbConnection();
        //BllCommon objBllCommon=new BllCommon();

        private readonly IConfiguration _configuration;
        private readonly ILogger<BllTran> _logger;
        private readonly IBllDbConnection _dbConn;
        public BllTran(IConfiguration configuration, ILogger<BllTran> logger, IBllDbConnection dbConn)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
        }

        public SelectList FuncReturnIteamCodeList(string IteamCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT i.IteamCode,i.IteamDesc,c.CatagoryDesc,i.IteamType from [Tbl_Iteam] i,dbo.Tbl_IteamCatagory c where i.CatagoryCode=c.CatagoryCode";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0000000000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("IteamCode", Convert.ToString(IteamCode));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["IteamDesc"].ToString() + "-" + sqlReader["CatagoryDesc"].ToString() + "Type-" + sqlReader["IteamType"].ToString(), Value = sqlReader["IteamCode"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", IteamCode);

            return objSelectList;
        }
        public InvoiceModels FuncInvoiceInfo(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT EntryDate InvoiceDate,NetAmount TAmount," +
                " isnull((select sum(isnull(a.TranAmount,0)) from Tbl_AcTran a where a.InvoiceNo=@InvoiceNo and a.IsActive='Y' group by a.InvoiceNo  ),0)TPAmount,0 TDAmount " +
                " FROM [dbo].[Tbl_InvoiceMaster]  where InvoiceNo=@InvoiceNo and TranType!='Y' and IsActive='Y';";
            sqlConn.ConnectionString = conString;
            InvoiceModels Invoice = new InvoiceModels();

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(Sel_StateName));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        Invoice.DisplayInvoiceDate = Convert.ToDateTime(sqlReader["InvoiceDate"]).ToString("yyyy-MM-dd");
                        Invoice.NetAmount = Convert.ToDecimal(sqlReader["TAmount"]);
                        Invoice.PaidAmount = Convert.ToDecimal(sqlReader["TPAmount"]);
                        Invoice.DueAmount = Invoice.NetAmount - Invoice.PaidAmount;
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return Invoice;
        }
        public string FuncReturnTranNo(DateTime BusinessDate)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT isnull(MAX(substring(TranNo,7,6)),0 )as MaxTranNo from Tbl_AcTran where Convert(int,TranDate)=Convert(int,@BusinessDate)";

            sqlConn.ConnectionString = conString;
            String OrdTranNo = "";

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("BusinessDate", Convert.ToDateTime(BusinessDate));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        OrdTranNo = BusinessDate.ToString("ddMMyy") + Convert.ToString(Convert.ToInt32(sqlReader["MaxTranNo"]) + 1).PadLeft(6, '0');

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            if (OrdTranNo == "") OrdTranNo = BusinessDate.ToString("ddMMyy") + Convert.ToString(1).PadLeft(6, '0');

            return OrdTranNo;
        }

        public bool FuncTransactionEntry(AcTranModel objTran, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " INSERT INTO [dbo].[Tbl_AcTran]([TranNo],[TranType],[CustomerCode],[InvoiceNo],[MoneyReceiptNo],[TranDate],[TranRemarks] ,[TranAmount] ,[TranStatus],[PaymentType] ,[EntryBy],[EntryDate],[IsActive],[ChequeNumber],[Chequedate],[ChequeDetails],[CollectedBy])" +
                               " VALUES(@TranNo,@TranType,@CustomerCode,@InvoiceNo,@MoneyReceiptNo,@TranDate,@TranRemarks,@TranAmount,@TranStatus,@PaymentType,@EntryBy,@EntryDate,'Y',@ChequeNumber,@Chequedate,@ChequeDetails,@CollectedBy)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(objTran.TranNo));
            sqlCmd.Parameters.AddWithValue("TranType", Convert.ToString(objTran.TranType));
            sqlCmd.Parameters.AddWithValue("TranDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(objTran.CustomerCode));
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(objTran.InvoiceNo));
            sqlCmd.Parameters.AddWithValue("MoneyReceiptNo", Convert.ToString(objTran.MoneyReceptNo));
            sqlCmd.Parameters.AddWithValue("TranRemarks", Convert.ToString("Received voucher"));
            sqlCmd.Parameters.AddWithValue("TranAmount", Convert.ToDecimal(objTran.CollectedAmount));
            if (Convert.ToInt32(objTran.RemainingAmount) <= 0)
                sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString("Y"));
            else sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString(objTran.TranStatus));

            sqlCmd.Parameters.AddWithValue("PaymentType", Convert.ToString(objTran.PaymentType));
            sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
            sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            if (objTran.PaymentType == "1001")
            {
                sqlCmd.Parameters.AddWithValue("Chequedate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
                sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString("0"));
                sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString("None"));
            }
            else if (objTran.PaymentType == "1002")
            {
                sqlCmd.Parameters.AddWithValue("Chequedate", Convert.ToDateTime(objTran.ChequeDate));
                sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString(objTran.ChequeNumber));
                sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString(objTran.ChequeDetails));
            }
            else
            {
                sqlCmd.Parameters.AddWithValue("Chequedate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
                sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString("0"));
                sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString(objTran.ChequeDetails));
            }
            sqlCmd.Parameters.AddWithValue("CollectedBy", Convert.ToString(objTran.CollectedBy));


            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0) opStatus = true;
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public List<AcTranModel> FuncReturnTranList(string TranNo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " Select a.[TranNo],(select (c.CompanyName+','+c.CompanyAddress) from Tbl_CustomerInfo c where c.CustomerCode=a.[CustomerCode]) Customer " +
                            " ,a.[InvoiceNo],a.[MoneyReceiptNo],a.[TranDate],a.[TranRemarks],a.[TranAmount],a.[TranType],a.[PaymentType],a.[IsActive],[ChequeNumber],[Chequedate]," +
                            " (select b.BankName from Tbl_BankIno b where b.BankCode=a.ChequeDetails) ChequeDetails from Tbl_AcTran a where (@TranNo='0' OR [TranNo]=@TranNo) and CONVERT(varchar,a.[TranDate],103)=convert(varchar,GETDATE(),103)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(TranNo));

            List<AcTranModel> objTranIteam = new List<AcTranModel>();

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    AcTranModel objDaoTran = new AcTranModel();
                    while (sqlReader.Read())
                    {

                        objDaoTran = new AcTranModel();
                        objDaoTran.TranNo = Convert.ToString(sqlReader["TranNo"]);
                        objDaoTran.TranDate = Convert.ToDateTime(sqlReader["TranDate"]);
                        objDaoTran.Customer = sqlReader["Customer"].ToString();
                        objDaoTran.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objDaoTran.MoneyReceptNo = sqlReader["MoneyReceiptNo"].ToString();
                        objDaoTran.TAmount = Convert.ToDecimal(sqlReader["TranAmount"]);
                        if (sqlReader["IsActive"].ToString() == "Y")
                            objDaoTran.Status = "Posted";
                        else
                            objDaoTran.Status = "Cancel";
                        objDaoTran.ChequeNumber = sqlReader["ChequeNumber"].ToString();
                        objDaoTran.ChequeDate = sqlReader["Chequedate"].ToString();
                        objDaoTran.ChequeDetails = sqlReader["ChequeDetails"].ToString();
                        objTranIteam.Add(objDaoTran);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }

            sqlConn.Close();

            return objTranIteam;
        }

        public bool FuncTransactionUpdate(AcTranModel objTran)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " UPDATE [dbo].[Tbl_AcTran] SET [InvoiceNo] = @InvoiceNo,[MoneyReceiptNo] = @MoneyReceiptNo,[TranDate] = @TranDate, " +
                               " [TranAmount] = @TranAmount,[TranStatus] = @TranStatus,[PaymentType] = @PaymentType" +
                               " ,[CollectedBy] = @CollectedBy,[ChequeNumber] = @ChequeNumber,[ChequeDate] = @ChequeDate,[ChequeDetails] = @ChequeDetails WHERE [TranNo] = @TranNo;";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(objTran.TranNo));
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(objTran.InvoiceNo));
            sqlCmd.Parameters.AddWithValue("TranDate", Convert.ToDateTime(objTran.CollectionDate));
            sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString(objTran.TranStatus));
            sqlCmd.Parameters.AddWithValue("MoneyReceiptNo", Convert.ToString(objTran.MoneyReceptNo));
            sqlCmd.Parameters.AddWithValue("TranAmount", Convert.ToDecimal(objTran.CollectedAmount));
            sqlCmd.Parameters.AddWithValue("PaymentType", Convert.ToString(objTran.PaymentType));
            sqlCmd.Parameters.AddWithValue("CollectedBy", Convert.ToString(objTran.CollectedBy));
            if (objTran.PaymentType == "1001")
            {
                sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString("0"));
                sqlCmd.Parameters.AddWithValue("ChequeDate", Convert.ToString(objTran.CollectionDate));
                sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString("None"));
            }
            else if (objTran.PaymentType == "1002")
            {
                sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString(objTran.ChequeNumber));
                sqlCmd.Parameters.AddWithValue("ChequeDate", Convert.ToString(objTran.ChequeDate));
                sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString(objTran.ChequeDetails));
            }
            else
            {
                sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString("0"));
                sqlCmd.Parameters.AddWithValue("ChequeDate", Convert.ToString(objTran.CollectionDate));
                sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString(objTran.ChequeDetails));
            }

            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0) opStatus = true;
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public bool FuncReturnTranDelete(string TranNo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " update [Tbl_AcTran]  set IsActive='N' where [TranNo]=@TranNo";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(TranNo));


            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0) opStatus = true;
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public SelectList FuncReturnSelectInvoiceList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT InvoiceNo FROM [dbo].[Tbl_InvoiceMaster] where CustomerCode=@CustomerCode and IsActive='Y' and TranType!='Y';";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(Code));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        //String TextValue = sqlReader["ProductDesc"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["InvoiceNo"].ToString(), Value = sqlReader["InvoiceNo"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", "000");

            return objSelectList;
        }

        public List<String> FuncBuyTranDataSum(List<ChallanModels> TranIteamList)
        {
            List<String> objString = new List<string>();

            Decimal TAmount = 0;

            foreach (ChallanModels TranItem in TranIteamList)
            {
                // TAmount = TAmount + TranItem.ChallanNo;
            }
            objString.Add(TAmount.ToString());

            return objString;

        }

        public AcTranModel FuncTransactionInfo(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            AcTranModel objAcTran = new AcTranModel();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " SELECT [TranNo],a.[CustomerCode],(SELECT (c.[CompanyName]+'-'+c.[ContactPerson])  FROM [dbo].[Tbl_CustomerInfo] c where c.[CustomerCode]=a.[CustomerCode]) as Customer " +
                               " ,TranType,[InvoiceNo],[MoneyReceiptNo],[TranDate],[TranRemarks],[TranAmount],TranStatus, (SELECT p.PaymentTypeName from  Tbl_PaymentType p where p.PaymentType=a.[PaymentType]) as Payment,a.[PaymentType] " +
                               " ,CollectedBy ,(SELECT e.[FullName]From [dbo].[Tbl_Employee] e where e.EmpId=CollectedBy) EmployeeName " +
                               ",ChequeNumber,ChequeDate,ChequeDetails,(SELECT b.[BankName] FROM [dbo].[Tbl_BankIno] b where b.[BankCode]=ChequeDetails) BankName FROM [dbo].[Tbl_AcTran] a where TranNo=@TranNo";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(Sel_StateName));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objAcTran.TranNo = sqlReader["TranNo"].ToString();
                        objAcTran.CustomerCode = sqlReader["CustomerCode"].ToString();
                        objAcTran.Customer = sqlReader["Customer"].ToString();
                        objAcTran.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objAcTran.MoneyReceptNo = sqlReader["MoneyReceiptNo"].ToString();
                        objAcTran.TranDatetemp = Convert.ToDateTime(sqlReader["TranDate"]).ToString("yyyy-MM-dd");
                        objAcTran.CollectedBy = sqlReader["CollectedBy"].ToString();
                        objAcTran.EmployeeName = sqlReader["EmployeeName"].ToString();
                        objAcTran.CollectedAmount = sqlReader["TranAmount"].ToString();
                        objAcTran.TranStatus = sqlReader["TranStatus"].ToString();
                        objAcTran.TranType = sqlReader["TranType"].ToString();
                        if (objAcTran.TranStatus == "N") objAcTran.Status = "UnPaid";
                        else if (objAcTran.TranStatus == "P") objAcTran.Status = "Partial Paid";
                        else if (objAcTran.TranStatus == "Y") objAcTran.Status = "Full Paid";
                        objAcTran.Payment = sqlReader["Payment"].ToString();
                        objAcTran.PaymentType = sqlReader["PaymentType"].ToString();
                        objAcTran.ChequeNumber = sqlReader["ChequeNumber"].ToString();
                        objAcTran.ChequeDate = Convert.ToDateTime(sqlReader["ChequeDate"]).ToString("yyyy-MM-dd");
                        objAcTran.ChequeDetails = sqlReader["ChequeDetails"].ToString();
                        objAcTran.BankName = sqlReader["BankName"].ToString();
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objAcTran;
        }

        public object FuncReturnSelectModifyInvoiceList(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT InvoiceNo FROM [dbo].[Tbl_InvoiceMaster] where CustomerCode=@CustomerCode and IsActive='Y';";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(Sel_StateName));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        //String TextValue = sqlReader["ProductDesc"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["InvoiceNo"].ToString(), Value = sqlReader["InvoiceNo"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", "000");

            return objSelectList;
        }

        public List<AcTranModel> FuncReturnSearchTranList(AcTranModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " Select a.[TranNo],(select (c.ContactPerson+'-'+c.CompanyName) from Tbl_CustomerInfo c where c.CustomerCode=a.[CustomerCode]) Customer " +
                            " ,a.[InvoiceNo],a.[MoneyReceiptNo],a.[TranDate],a.[TranRemarks],a.[TranAmount],a.[TranType],a.[PaymentType],a.[IsActive] from Tbl_AcTran a where (@TranNo='' OR [TranNo]=@TranNo) and (@TranType='' OR TranType=@TranType) " +
                            " and CONVERT(int,a.[TranDate]) between convert(int,@TranDateFrom) and convert(int,@TranDateTo) and (@InvoiceNo='' OR a.[InvoiceNo]=@InvoiceNo) and (@CustomerCode='' OR a.[CustomerCode]=@CustomerCode) and (@IsActive='' OR a.[IsActive]=@IsActive)";

            sqlConn.ConnectionString = conString;
            if (viewModel.TranNo == null) viewModel.TranNo = "";
            if (viewModel.CustomerCode == null) viewModel.CustomerCode = "";
            if (viewModel.InvoiceNo == null) viewModel.InvoiceNo = "";
            if (viewModel.IsActive) viewModel.TranStatus = "Y";
            else viewModel.TranStatus = "";

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(viewModel.TranNo));
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            sqlCmd.Parameters.AddWithValue("IsActive", Convert.ToString(viewModel.TranStatus));
            sqlCmd.Parameters.AddWithValue("TranType", Convert.ToString(viewModel.TranType));
            List<AcTranModel> objTranIteam = new List<AcTranModel>();

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    AcTranModel objDaoTran = new AcTranModel();
                    while (sqlReader.Read())
                    {

                        objDaoTran = new AcTranModel();
                        objDaoTran.TranNo = Convert.ToString(sqlReader["TranNo"]);
                        objDaoTran.TranDate = Convert.ToDateTime(sqlReader["TranDate"]);
                        objDaoTran.Customer = sqlReader["Customer"].ToString();
                        objDaoTran.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objDaoTran.MoneyReceptNo = sqlReader["MoneyReceiptNo"].ToString();
                        objDaoTran.TAmount = Convert.ToDecimal(sqlReader["TranAmount"]);
                        if (sqlReader["IsActive"].ToString() == "Y")
                            objDaoTran.Status = "Posted";
                        else
                            objDaoTran.Status = "Cancel";
                        objTranIteam.Add(objDaoTran);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }

            sqlConn.Close();

            return objTranIteam;
        }

        public List<AcTranPaymentModel> FuncReturnTranPaymentList(string TranNo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " Select a.[TranNo],(SELECT c.[BrandName] FROM [dbo].[Tbl_BrandInfo] c where c.[BrandCode]=a.[CustomerCode]) Vendor " +
                            " ,a.[InvoiceNo],a.[MoneyReceiptNo],a.[TranDate],a.[TranRemarks],a.[TranAmount],a.[TranType],a.[PaymentType],a.[IsActive]" +
                            ",ChequeNumber, Chequedate,(select b.BankName from Tbl_BankIno b where b.BankCode=a.ChequeDetails) ChequeDetails from Tbl_AcTran a where (@TranNo='0' OR [TranNo]=@TranNo) and CONVERT(VARCHAR(10),a.[TranDate],110)=CONVERT(VARCHAR(10),GETDATE(),110)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(TranNo));

            List<AcTranPaymentModel> objTranIteam = new List<AcTranPaymentModel>();

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    AcTranPaymentModel objDaoTran = new AcTranPaymentModel();
                    while (sqlReader.Read())
                    {

                        objDaoTran = new AcTranPaymentModel();
                        objDaoTran.TranNo = Convert.ToString(sqlReader["TranNo"]);
                        objDaoTran.TranDate = Convert.ToDateTime(sqlReader["TranDate"]);
                        objDaoTran.Vendor = sqlReader["Vendor"].ToString();
                        objDaoTran.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objDaoTran.MoneyReceptNo = sqlReader["MoneyReceiptNo"].ToString();
                        objDaoTran.Amount = Convert.ToDecimal(sqlReader["TranAmount"]);
                        if (sqlReader["IsActive"].ToString() == "Y")
                            objDaoTran.Status = "Posted";
                        else
                            objDaoTran.Status = "Cancel";
                        objDaoTran.ChequeNumber = sqlReader["ChequeNumber"].ToString();
                        objDaoTran.ChequeDate = sqlReader["Chequedate"].ToString();
                        objDaoTran.ChequeDetails = sqlReader["ChequeDetails"].ToString();

                        objTranIteam.Add(objDaoTran);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }

            sqlConn.Close();

            return objTranIteam;
        }

        public bool FuncTransactionPaymentEntry(AcTranPaymentModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " INSERT INTO [dbo].[Tbl_AcTran]([TranNo],[TranType],[CustomerCode],[InvoiceNo],[MoneyReceiptNo],[TranDate],[TranRemarks] ,[TranAmount] ,[TranStatus],[PaymentType] ,[EntryBy],[EntryDate],[IsActive],[ChequeDetails],[ChequeNumber],[Chequedate])" +
                               " VALUES(@TranNo,@TranType,@CustomerCode,@InvoiceNo,@MoneyReceiptNo,@TranDate,@TranRemarks,@TranAmount,@TranStatus,@PaymentType,@EntryBy,@EntryDate,'Y',@ChequeDetails,@ChequeNumber,@Chequedate)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(viewModel.TranNo));
            sqlCmd.Parameters.AddWithValue("TranType", Convert.ToString(viewModel.TranType));
            sqlCmd.Parameters.AddWithValue("TranDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.VendorCode));
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
            sqlCmd.Parameters.AddWithValue("MoneyReceiptNo", Convert.ToString(viewModel.MoneyReceptNo));
            sqlCmd.Parameters.AddWithValue("TranRemarks", Convert.ToString("Payment voucher"));
            sqlCmd.Parameters.AddWithValue("TranAmount", Convert.ToDecimal(viewModel.PaymentAmount));
            sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString("Y"));
            sqlCmd.Parameters.AddWithValue("PaymentType", Convert.ToString(viewModel.PaymentType));
            sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
            sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            if (viewModel.PaymentType == "1001")
            {
                sqlCmd.Parameters.AddWithValue("Chequedate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
                sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString("0"));
                sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString("None"));
            }
            else if (viewModel.PaymentType == "1002")
            {
                sqlCmd.Parameters.AddWithValue("Chequedate", Convert.ToDateTime(viewModel.ChequeDate));
                sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString(viewModel.ChequeNumber));
                sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString(viewModel.ChequeDetails));
            }
            else
            {
                sqlCmd.Parameters.AddWithValue("Chequedate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
                sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString("0"));
                sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString(viewModel.ChequeDetails));
            }
            sqlCmd.Parameters.AddWithValue("CollectedBy", Convert.ToString(viewModel.PaymentBy));

            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0) opStatus = true;
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public bool FuncTransactionPaymentUpdate(AcTranPaymentModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " UPDATE [dbo].[Tbl_AcTran] SET [InvoiceNo] = @InvoiceNo,[MoneyReceiptNo] = @MoneyReceiptNo,[TranDate] = @TranDate, " +
                               " [ChequeDetails] = @ChequeDetails,[ChequeNumber]=@ChequeNumber,[TranRemarks]=@TranRemarks,[ChequeDate] = @ChequeDate,[TranAmount] = @TranAmount,[TranStatus] = @TranStatus,[PaymentType] = @PaymentType WHERE [TranNo] = @TranNo;";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(viewModel.TranNo));
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
            sqlCmd.Parameters.AddWithValue("TranDate", Convert.ToDateTime(viewModel.PaymentDate));
            sqlCmd.Parameters.AddWithValue("MoneyReceiptNo", Convert.ToString(viewModel.MoneyReceptNo));
            sqlCmd.Parameters.AddWithValue("TranAmount", Convert.ToDecimal(viewModel.Amount));
            sqlCmd.Parameters.AddWithValue("PaymentType", Convert.ToString(viewModel.PaymentType));
            sqlCmd.Parameters.AddWithValue("TranRemarks", Convert.ToString(viewModel.PaymentBy));
            sqlCmd.Parameters.AddWithValue("ChequeNumber", Convert.ToString(viewModel.ChequeNumber));
            sqlCmd.Parameters.AddWithValue("ChequeDate", Convert.ToString(viewModel.ChequeDate));
            sqlCmd.Parameters.AddWithValue("ChequeDetails", Convert.ToString(viewModel.ChequeDetails));

            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0) opStatus = true;
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public AcTranPaymentModel FuncTransactionPaymentInfo(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            AcTranPaymentModel objAcTran = new AcTranPaymentModel();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " SELECT [TranNo],a.[CustomerCode],(SELECT c.[BrandName] FROM [dbo].[Tbl_BrandInfo] c where c.[BrandCode]=a.[CustomerCode]) as Customer  " +
                               " ,[InvoiceNo],[MoneyReceiptNo],[TranDate],[TranRemarks],[TranAmount],TranStatus, (SELECT p.PaymentTypeName from  Tbl_PaymentType p where p.PaymentType=a.[PaymentType]) as Payment,a.[PaymentType] " +
                               " ,ChequeNumber, ChequeDate,ChequeDetails, (SELECT b.[BankName] FROM [dbo].[Tbl_BankIno] b where b.[BankCode]=ChequeDetails) BankName FROM [dbo].[Tbl_AcTran] a where TranNo=@TranNo and TranType='D'";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranNo", Convert.ToString(Sel_StateName));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objAcTran.VendorCode = sqlReader["CustomerCode"].ToString();
                        objAcTran.Vendor = sqlReader["Customer"].ToString();
                        objAcTran.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objAcTran.MoneyReceptNo = sqlReader["MoneyReceiptNo"].ToString();
                        objAcTran.TranDatetemp = Convert.ToDateTime(sqlReader["TranDate"]).ToString("yyyy-MM-dd");
                        objAcTran.PaymentBy = sqlReader["TranRemarks"].ToString();
                        objAcTran.PaymentAmount = sqlReader["TranAmount"].ToString();
                        objAcTran.Payment = sqlReader["Payment"].ToString();
                        objAcTran.PaymentType = sqlReader["PaymentType"].ToString();

                        objAcTran.ChequeNumber = sqlReader["ChequeNumber"].ToString();
                        objAcTran.ChequeDate = Convert.ToDateTime(sqlReader["ChequeDate"]).ToString("yyyy-MM-dd");
                        objAcTran.ChequeDetails = sqlReader["ChequeDetails"].ToString();
                        objAcTran.BankName = sqlReader["BankName"].ToString();

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objAcTran;
        }

        public List<AccountSumModel> FuncReturnAccountDataList(AcTranModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "spAccountSumData";
            if (viewModel.CustomerCode == null) viewModel.CustomerCode = "";
            if (viewModel.PType == null) viewModel.PType = "";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("PartyCode", Convert.ToString(viewModel.CustomerCode));
            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime("2014-01-01"));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            sqlCmd.Parameters.AddWithValue("PType", Convert.ToString(viewModel.PType));
            sqlCmd.CommandType = CommandType.StoredProcedure;
            List<AccountSumModel> objTranIteam = new List<AccountSumModel>();

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    AccountSumModel objDaoTran = new AccountSumModel();
                    int Count = 0;
                    while (sqlReader.Read())
                    {

                        objDaoTran = new AccountSumModel();
                        objDaoTran.SlNo = (Count + 1).ToString();
                        objDaoTran.PartyName = Convert.ToString(sqlReader["PartyName"]);
                        objDaoTran.NoTran = Convert.ToInt32(sqlReader["NoTran"]);
                        objDaoTran.CrAmount = Convert.ToDecimal(sqlReader["CrAmount"].ToString());
                        objDaoTran.DrAmount = Convert.ToDecimal(sqlReader["DrAmount"].ToString());
                        objDaoTran.TAmount = Convert.ToDecimal(sqlReader["NetAmount"].ToString());

                        objDaoTran.PType = Convert.ToString(sqlReader["PType"]);
                        objTranIteam.Add(objDaoTran);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }

            sqlConn.Close();

            return objTranIteam;
        }

        public List<AcTranModel> FuncReturnDailyTranList(AcTranModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [TranNo],(case [TranType] when 'C' then 'Credit' when 'D' then 'Debit' end) TnType " +
                               " ,(select c.CompanyName from Tbl_CustomerInfo c where c.CustomerCode=a.[CustomerCode]) PartyName, [CustomerCode],(case [TranStatus] when 'P' then 'Partial ' else 'Full ' end)+[TranRemarks]+', Invoice No: '+[InvoiceNo]+', Mony Receipt No : '+[MoneyReceiptNo] as TranDetails" +
                               " ,[TranDate],(case [TranType] when 'C' then [TranAmount] else '0' end) CrAmount,(case [TranType] when 'D' then [TranAmount] else '0' end) DrAmount " +
                               " ,[PaymentType],[EntryBy],[EntryDate],(case [IsActive] when 'Y' then 'Posted' else 'Cancel' end) vStatus " +
                               " ,[ChequeDetails] ,[ChequeNumber] ,[Chequedate],[CollectedBy] FROM [dbo].[Tbl_AcTran] a" +
                               "  where (@PartyCode='' OR CustomerCode=@PartyCode) and convert(int,TranDate) between convert(int,@TranDateFrom)  and convert(int,@TranDateTo) and IsActive='Y';";
            if (viewModel.CustomerCode == null) viewModel.CustomerCode = "";
            if (viewModel.PType == null) viewModel.PType = "";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("PartyCode", Convert.ToString(viewModel.CustomerCode));
            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            //sqlCmd.Parameters.AddWithValue("PType", Convert.ToString(viewModel.PType));
            List<AcTranModel> objTranIteam = new List<AcTranModel>();

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    var objDaoTran = new AcTranModel();
                    int Count = 0;
                    while (sqlReader.Read())
                    {

                        objDaoTran = new AcTranModel();
                        objDaoTran.TranNo = Convert.ToString(sqlReader["TranNo"]);
                        objDaoTran.Customer = Convert.ToString(sqlReader["PartyName"]);
                        objDaoTran.TranType = Convert.ToString(sqlReader["TnType"]);
                        objDaoTran.ChequeDetails = Convert.ToString(sqlReader["TranDetails"].ToString());
                        objDaoTran.CrAmount = Convert.ToDecimal(sqlReader["CrAmount"].ToString());
                        objDaoTran.DrAmount = Convert.ToDecimal(sqlReader["DrAmount"].ToString());
                        objDaoTran.Status = Convert.ToString(sqlReader["vStatus"].ToString());

                        objTranIteam.Add(objDaoTran);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }

            sqlConn.Close();

            return objTranIteam;
        }

        public List<AccountSumModel> FuncReturnAccountDetailsData(AcTranModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "spAccountDetailsData";
            if (viewModel.CustomerCode == null) viewModel.CustomerCode = "";
            if (viewModel.PType == null) viewModel.PType = "";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("PartyCode", Convert.ToString(viewModel.CustomerCode));
            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            sqlCmd.Parameters.AddWithValue("PType", Convert.ToString(viewModel.PType));
            sqlCmd.CommandType = CommandType.StoredProcedure;
            List<AccountSumModel> objTranIteam = new List<AccountSumModel>();

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    AccountSumModel objDaoTran = new AccountSumModel();
                    int Count = 0;
                    decimal balance = 0;
                    while (sqlReader.Read())
                    {

                        objDaoTran = new AccountSumModel();
                        objDaoTran.SlNo = (Count + 1).ToString();
                        objDaoTran.ChallanNo = Convert.ToString(sqlReader["ChallanNo"]);
                        objDaoTran.TranDate = Convert.ToDateTime(sqlReader["TranDate"]).ToString("dd-MM-yyyy");
                        objDaoTran.InvoiceNo = Convert.ToString(sqlReader["InvoiceNo"]);
                        objDaoTran.PartyName = Convert.ToString(sqlReader["PartyName"]);
                        //objDaoTran.Amount = Convert.ToDecimal(sqlReader["Amount"].ToString());
                        //objDaoTran.TVatAmount = Convert.ToDecimal(sqlReader["VatAmount"].ToString());
                        objDaoTran.DrAmount = Convert.ToDecimal(sqlReader["DrAmount"].ToString());
                        objDaoTran.CrAmount = Convert.ToDecimal(sqlReader["CrAmount"].ToString());
                        balance = balance + objDaoTran.CrAmount - objDaoTran.DrAmount;
                        objDaoTran.TAmount = balance;
                        objTranIteam.Add(objDaoTran);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }

            sqlConn.Close();

            return objTranIteam;
        }
    }
}
