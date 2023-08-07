using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Contacts;
using NESHOP.Models;

namespace NeSHOP.DAL
{
    public class BllOrder : IBllOrder
    {
        //BllDbConnection objBllDbConnection = new BllDbConnection();

        private readonly IConfiguration _configuration;
        private readonly ILogger<BllOrder> _logger;
        private readonly IBllDbConnection _dbConn;
        public BllOrder(IConfiguration configuration, ILogger<BllOrder> logger, IBllDbConnection dbConn)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
        }


        public SelectList FuncIteamCodeList(string IteamCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new();
            String SqlString =
                "SELECT i.IteamCode,i.IteamDesc,c.CatagoryDesc,i.IteamType from [Tbl_Iteam] i,dbo.Tbl_IteamCatagory c where i.CatagoryCode=c.CatagoryCode";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem
            { Selected = true, Text = "--Select--", Value = "0000000000".ToString() });

            var sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("IteamCode", Convert.ToString(IteamCode));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem
                        {
                            Text =
                                                          sqlReader["IteamDesc"].ToString() + "-" +
                                                          sqlReader["CatagoryDesc"].ToString() + "Type-" +
                                                          sqlReader["IteamType"].ToString(),
                            Value = sqlReader["IteamCode"].ToString()
                        });
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                //_logger
                _logger.LogInformation("FuncIteamCodeList Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", IteamCode);

            return objSelectList;
        }

        public bool FuncOrderEntry(OrderModels objOrder, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                " INSERT INTO [dbo].[Tbl_Order]([OrderRefNo],[CustomerCode],[OrderSub],[IteamCode],[OrderDesc],[Quantity],[Paired],[CostPrice],[IsVat],[IsTax],[TaxTex],VatPerc,[TaxPerc],[EntryBy],[EntryDate]) " +
                " VALUES(@OrderRefNo,@CustomerCode,@OrderSub,@IteamCode,@OrderDesc,@Quantity,'N',@CostPrice,@IsVat,@IsTax,@TaxTex,@VatPerc,@TaxPerc,@EntryBy,@EntryDate)";

            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = null;
                var TaxTex = "";

                foreach (IteamModels objOrd in objOrder.IteamlList)
                {
                    sqlCmd = new SqlCommand(SqlString, sqlConn);

                    sqlCmd.Parameters.AddWithValue("OrderRefNo", Convert.ToString(objOrder.OrderRefNo));
                    sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(objOrder.CustomerCode));
                    sqlCmd.Parameters.AddWithValue("OrderSub", Convert.ToString(objOrder.OrderSub));
                    //--------------------------
                    sqlCmd.Parameters.AddWithValue("IteamCode", Convert.ToString(objOrd.IteamCode + objOrd.SLNo));
                    sqlCmd.Parameters.AddWithValue("OrderDesc", Convert.ToString(objOrd.IteamDesc));
                    sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToDecimal(objOrd.Quantity));
                    sqlCmd.Parameters.AddWithValue("CostPrice", Convert.ToDecimal(objOrd.CostPrice));
                    if (objOrd.IsVat > 0)
                    {
                        sqlCmd.Parameters.AddWithValue("IsVat", Convert.ToDecimal(objOrd.IsVat));
                        sqlCmd.Parameters.AddWithValue("VatPerc", Convert.ToDecimal(objOrd.VatPerc));
                    }
                    else {
                        sqlCmd.Parameters.AddWithValue("IsVat", Convert.ToDecimal(0));
                        sqlCmd.Parameters.AddWithValue("VatPerc", Convert.ToDecimal(0));
                    }

                    if (objOrd.IsTax > 0)
                    {
                        sqlCmd.Parameters.AddWithValue("IsTax", Convert.ToDecimal(objOrd.IsTax));
                        sqlCmd.Parameters.AddWithValue("TaxPerc", Convert.ToDecimal(objOrd.TaxPerc));
                    }
                    else
                    {
                        sqlCmd.Parameters.AddWithValue("IsTax", Convert.ToDecimal(0));
                        sqlCmd.Parameters.AddWithValue("TaxPerc", Convert.ToDecimal(0));
                    }

                    if (objOrd.IsTax > 0) TaxTex = TaxTex + " Tax"; 
                    if (objOrd.IsVat > 0) TaxTex = TaxTex + " Vat";
                    //--------------------------
                    sqlCmd.Parameters.AddWithValue("TaxTex", Convert.ToString(TaxTex));
                    sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                    sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));

                    int sqlResult = sqlCmd.ExecuteNonQuery();
                    if (sqlResult > 0) opStatus = true;
                    //objOrder.OrderRefNo = objOrder.OrderRefNo.Substring(0, 7) + Convert.ToString(Convert.ToInt32(objOrder.OrderRefNo.Substring(7, 6))+1).PadLeft(6,'0');
                }
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncOrderEntry Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }


            return opStatus;
        }

        public string FuncReturnOrdTranNo(DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT NEXT VALUE FOR SEQ_ORDER AS MaxNo";
            //"SELECT MAX(substring(OrderRefNo,11,4)) as MaxOrdTranNo from[Tbl_Order] where datepart(year,EntryDate)=datepart(year,@BusinessDate)";

            sqlConn.ConnectionString = conString;
            String OrdTranNo = "0000";
            String RefCode = FuncReturnOrdRefCode(objDaoUserInfo.InstCode);
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("BusinessDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        OrdTranNo = RefCode + "/" + objDaoUserInfo.BusinessDate.ToString("yy") +
                                    Convert.ToString(Convert.ToInt32(sqlReader["MaxNo"].ToString())).PadLeft(
                                        4, '0');
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncReturnOrdTranNo Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }

            if (OrdTranNo == "0000")
                OrdTranNo = RefCode + "/" + objDaoUserInfo.BusinessDate.ToString("yy") +
                            Convert.ToString(1).PadLeft(4, '0');

            return OrdTranNo;
        }

        public string FuncReturnOrdRefCode(string InstCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT RefCode from [Tbl_AppInfo] where InstCode=@InstCode";

            sqlConn.ConnectionString = conString;
            String RefCode = "";
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("InstCode", Convert.ToString(InstCode));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        RefCode = sqlReader["RefCode"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncReturnOrdRefCode Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }


            return RefCode;
        }

        public List<IteamModels> FuncReturnDeleteOrderData(List<IteamModels> iteamList, string ids)
        {
            bool opStatus = false;
            String[] InStr = ids.Split('-');
            String SlNo = InStr[1].ToString();
            List<IteamModels> newIteamList = new List<IteamModels>();

            try
            {
                foreach (IteamModels objIteam in iteamList)
                {
                    if (objIteam.SLNo != SlNo)
                        newIteamList.Add(objIteam);
                }
            }
            catch (Exception e)
            {
                //opStatus = false;//throw;
                _logger.LogInformation("FuncReturnDeleteOrderData Exception: " + e.Message);
            }

            return newIteamList;
        }

        public IteamModels FuncReturnParseOrderData(string ids)
        {
            var objSelectList = new IteamModels();
            try
            {
                String[] pString = ids.Split('+');

                objSelectList.SLNo = pString[11].ToString();
                objSelectList.IteamCode = pString[3].ToString() + pString[11].ToString();
                objSelectList.CatagoryName = FuncReturnCatagoryName(pString[0].ToString());
                objSelectList.ProductName = FuncReturnProductName(pString[1].ToString());
                objSelectList.VendorName = FuncReturnVenName(pString[2].ToString());
                objSelectList.ModelName = FuncReturnModelName(pString[3].ToString());
                objSelectList.CostPrice = Convert.ToDecimal(pString[4]);
                objSelectList.IteamDesc = pString[5].ToString();
                objSelectList.Quantity = Convert.ToInt32(pString[6]);

                if(!string.IsNullOrEmpty(pString[7]))
                    objSelectList.IsVat = Convert.ToInt32(1);
                else
                    objSelectList.IsVat = Convert.ToInt32(0);

                if (!string.IsNullOrEmpty(pString[8]))
                    objSelectList.VatPerc = Convert.ToDecimal(pString[8]);
                else
                    objSelectList.VatPerc = Convert.ToDecimal(0);

                if (!string.IsNullOrEmpty(pString[9]))
                    objSelectList.IsTax = Convert.ToInt32(1);
                else
                    objSelectList.IsTax = Convert.ToInt32(0);
                if (!string.IsNullOrEmpty(pString[10]))
                    objSelectList.TaxPerc = Convert.ToDecimal(pString[10]);
                else
                    objSelectList.TaxPerc = Convert.ToDecimal(0);                

                objSelectList.DisPerc = (objSelectList.TaxPerc + objSelectList.VatPerc).ToString();

            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncReturnParseOrderData Exception: " + e.Message);
            }


            return objSelectList;
        }

        public string FuncReturnModelName(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ModelName] FROM [dbo].[Tbl_IteamModel] where ModelCode=@ModelCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(Code));
            String rName = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName = Convert.ToString(sqlReader["ModelName"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncReturnModelName Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }


            return rName;
        }

        public string FuncReturnProductName(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new ();
            String SqlString = "SELECT [BrandName] FROM [dbo].[Tbl_BrandInfo] where BrandCode=substring(@ModelCode,1,3)";

            sqlConn.ConnectionString = conString;
            var sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(Code));
            String rName = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName = Convert.ToString(sqlReader["BrandName"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncReturnProductName Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }

            return rName;
        }

        public string FuncReturnVenName(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [BrandName] FROM [dbo].[Tbl_BrandInfo] where [BrandCode]=@BrandCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(Code));
            String rName = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName = Convert.ToString(sqlReader["BrandName"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return rName;
        }

        public string FuncReturnCatagoryName(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [CatagoryName] FROM [dbo].[Tbl_IteamCatagory] where [CatagoryCode]=@CatagoryCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(Code));
            String rName = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName = Convert.ToString(sqlReader["CatagoryName"]);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("FuncIteamCodeList Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }

            return rName;
        }

        public string[] FuncReturnModelDesc(string ids)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT i.[ModelDesc],i.ModelPrice FROM [dbo].[Tbl_IteamModel] i  where i.ModelCode=@ModelCode;";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(ids));
            String[] rName = new string[2];
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName[0] = Convert.ToString(sqlReader["ModelDesc"]);
                        rName[1] = Convert.ToString(sqlReader["ModelPrice"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncIteamCodeList Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }

            return rName;
        }

        public void FuncOrderTermsEntry(OrderModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "INSERT INTO [dbo].[Tbl_QutationCondition]([OrdRefNo],[QtationDate],[ConditionNo])VALUES(@OrdRefNo,@EntryDate,@ConditionNo)";

            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = null;
                foreach (var objOrd in viewModel.Tarms)
                {
                    sqlCmd = new SqlCommand(SqlString, sqlConn);

                    sqlCmd.Parameters.AddWithValue("OrdRefNo", Convert.ToString(viewModel.OrderRefNo));
                    //--------------------------
                    sqlCmd.Parameters.AddWithValue("ConditionNo", Convert.ToString(objOrd));
                    //--------------------------
                    sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));

                    int sqlResult = sqlCmd.ExecuteNonQuery();
                    if (sqlResult > 0) opStatus = true;
                    //objOrder.OrderRefNo = objOrder.OrderRefNo.Substring(0, 7) + Convert.ToString(Convert.ToInt32(objOrder.OrderRefNo.Substring(7, 6))+1).PadLeft(6,'0');
                }
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncOrderTermsEntry Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public IteamModels FuncReturnParseChallanData(string ids)
        {
            var objSelectList = new IteamModels();
            try
            {
                String[] pString = ids.Split('+');

                objSelectList.SLNo = pString[5].ToString();
                objSelectList.IteamCode = pString[1].ToString();
                objSelectList.BrandCode = pString[0].ToString();
                objSelectList.VendorName = FuncReturnVenName(pString[0].ToString());
                objSelectList.ModelName = FuncReturnModelName(pString[1].ToString());
                objSelectList.IteamDesc = pString[2].ToString();
                objSelectList.Quantity = Convert.ToInt32(pString[3].ToString());
                objSelectList.IteamSLNo = pString[4].ToString();
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncReturnParseChallanData Exception: " + e.Message);
            }
           

            return objSelectList;
        }

        public List<IteamModels> FuncReturnDeleteChalaData(List<IteamModels> objIteamlList, string ids)
        {
            bool opStatus = false;
            String[] InStr = ids.Split('-');
            String SlNo = InStr[1].ToString();
            List<IteamModels> newIteamList = new List<IteamModels>();

            try
            {
                foreach (IteamModels objIteam in objIteamlList)
                {
                    if (objIteam.SLNo != SlNo)
                        newIteamList.Add(objIteam);
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("FuncReturnDeleteChalaData Exception: " + e.Message);
            }
       

            return newIteamList;
        }

        public string FuncReturnModelSlNo(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT TotalQuantity FROM [dbo].[Tbl_Inventory] where ModelCode =@ModelCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            String[] qString = Sel_StateName.Split('+');
            //sqlCmd.Parameters.AddWithValue("IteamSLNo", Convert.ToString(qString[0]));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(qString[1]));
            String rName = "0";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName = Convert.ToString(sqlReader["TotalQuantity"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncReturnDeleteChalaData Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }

            if (Convert.ToInt32(rName) - Convert.ToInt32(qString[0]) > 0)
                return qString[0];
            else return "0";
        }

        public string FuncReturnChallanTranNo(DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT NEXT VALUE FOR SEQ_CHALLAN AS MaxNo";
                //"SELECT max(substring([ChalanNo],7,6)) MaxNo  FROM [dbo].[Tbl_Chalan] where DATEPART(year,ChalanDate)=DATEPART(year,@BusinessDate)";
            sqlConn.ConnectionString = conString;
            String OrdTranNo = "000000";

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("BusinessDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        OrdTranNo = "CHL-" + objDaoUserInfo.BusinessDate.ToString("yy") +
                                    Convert.ToString(Convert.ToInt32(sqlReader["MaxNo"])).PadLeft(6, '0');
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                _logger.LogInformation("FuncReturnChallanTranNo Exception: " + e.Message);
            }
            finally
            {
                sqlConn.Close();
            }
            //sqlConn.Close();

            if (OrdTranNo == "000000")
                OrdTranNo = "CHL-" + objDaoUserInfo.BusinessDate.ToString("yy") + Convert.ToString(1).PadLeft(6, '0');

            return OrdTranNo;
        }

        public bool FuncChallanEntry(ChallanModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                " INSERT INTO [dbo].[Tbl_Chalan](SlNo,[ChalanNo] ,[ChalanDate] ,[CustomerCode] ,[ChalanRemk] ,[ChalanPoNo] ,[ModelCode] ,[ModelDesc],[ItemSlNo],[Quantity],[TranStatus],[EntryBy],[EntryDate])" +
                " VALUES(@SlNo,@ChalanNo,@ChalanDate,@CustomerCode,@ChalanRemk,@ChalanPoNo,@ModelCode,@ModelDesc,@ItemSlNo,@Quantity,@TranStatus, @EntryBy,@EntryDate)";

            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                int slNo = 1;
                foreach (IteamModels objOrd in viewModel.IteamList)
                {
                    if (viewModel.PONo == null) viewModel.PONo = "";
                    if (viewModel.ChallanRemarks == null) viewModel.ChallanRemarks = "";
                    SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

                    sqlCmd.Parameters.AddWithValue("SlNo", Convert.ToString(slNo));
                    sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(viewModel.ChallanNo));
                    sqlCmd.Parameters.AddWithValue("ChalanDate", Convert.ToDateTime(viewModel.ChallanDate));
                    sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
                    sqlCmd.Parameters.AddWithValue("ChalanPoNo", Convert.ToString(viewModel.PONo));
                    sqlCmd.Parameters.AddWithValue("ChalanRemk", Convert.ToString(viewModel.ChallanRemarks));
                    sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                    sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));

                    sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(objOrd.IteamCode));
                    sqlCmd.Parameters.AddWithValue("ModelDesc", Convert.ToString(objOrd.IteamDesc));
                    sqlCmd.Parameters.AddWithValue("ItemSlNo", Convert.ToString(objOrd.IteamSLNo));
                    sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(objOrd.Quantity));
                    sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString("N"));

                    slNo++;

                    int sqlResult = sqlCmd.ExecuteNonQuery();
                    if (sqlResult > 0)
                    {
                        FuncUpdateInventoryIteam(objOrd);
                    }
                }
                opStatus = true;
            }
            catch (Exception se)
            {
                //sqlConn.Close;
                _logger.LogInformation("FuncChallanEntry Exception: " + se.Message);
            }
            finally {
                sqlConn.Close();
            }
           

            return opStatus;
        }

        public bool FuncChallanEntryFromInvoice(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new ();
            sqlConn.ConnectionString = conString;
            sqlConn.Open();
            SqlTransaction sqlTran = sqlConn.BeginTransaction();

            SqlCommand sqlCmd;

            String SqlString =
                " INSERT INTO [dbo].[Tbl_Chalan]([SlNo],[ChalanNo] ,[ChalanDate] ,[CustomerCode] ,[ChalanRemk] ,[ChalanPoNo] ,[ModelCode] ,[ModelDesc],[ItemSlNo],[Quantity],[TranStatus],[EntryBy],[EntryDate])" +
                " VALUES(@SlNo,@ChalanNo,@ChalanDate,@CustomerCode,@ChalanRemk,@ChalanPoNo,@ModelCode,@ModelDesc,@ItemSlNo,@Quantity,@TranStatus, @EntryBy,@EntryDate)";


            SqlCommand sqlCmd1;
            String SqlString1 =" UPDATE [dbo].[Tbl_Chalan] set TranStatus='Y' where [ChalanNo]=@ChalanNo ";


            try
            {
               
                foreach (var objOrd in viewModel.ChallanList)
                {
                    
                    if (viewModel.PONo == null) viewModel.PONo = "";
                    if (viewModel.ChallanRemarks == null) viewModel.ChallanRemarks = "";

                    sqlCmd = new SqlCommand(SqlString, sqlConn, sqlTran);

                    String vsr = viewModel.InvoiceNo.Substring(4);
                    sqlCmd.Parameters.AddWithValue("SlNo", Convert.ToInt32(objOrd.SLNo));

                    if (string.IsNullOrEmpty(objOrd.IteamSLNo)) 
                        objOrd.IteamSLNo = "";

                    if (!string.IsNullOrEmpty(objOrd.ChallanNo))
                        sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(objOrd.ChallanNo));
                    else
                        sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString("CHL-" + viewModel.InvoiceNo.Substring(4)));

                    sqlCmd.Parameters.AddWithValue("ChalanDate", Convert.ToDateTime(viewModel.InvoiceDate));
                    sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
                    sqlCmd.Parameters.AddWithValue("ChalanPoNo", Convert.ToString(viewModel.PONo));
                    sqlCmd.Parameters.AddWithValue("ChalanRemk", Convert.ToString(viewModel.ChallanRemarks));
                    sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                    sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
                    sqlCmd.Parameters.AddWithValue("ItemSlNo", Convert.ToString(objOrd.IteamSLNo));
                    sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(objOrd.IteamCode));
                    sqlCmd.Parameters.AddWithValue("ModelDesc", Convert.ToString(objOrd.IteamDesc));
                    sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(objOrd.Quantity));
                    sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString("Y"));

                    if (objOrd.TranType == "0" && string.IsNullOrEmpty(objOrd.ChallanNo))
                    {
                        int sqlResult = sqlCmd.ExecuteNonQuery();
                        if (sqlResult > 0)
                        {
                            FuncUpdateInventoryIteam(objOrd);
                        }
                    }
                    else
                    {
                        sqlCmd1 = new SqlCommand(SqlString1, sqlConn, sqlTran);
                        sqlCmd1.Parameters.AddWithValue("ChalanNo", Convert.ToString(objOrd.ChallanNo));

                        sqlCmd1.ExecuteNonQuery();
                    }
                }


                sqlTran.Commit();
                opStatus = true;
            }
            catch (Exception se)
            {
                sqlTran.Rollback();

                _logger.LogInformation("FuncChallanEntryFromInvoice Exception: " + se.Message);
            }
            finally
            {
                sqlConn.Close();
            }


            return opStatus;
        }

        public void FuncUpdateInventoryIteam(IteamModels objOrd)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();

            String SqlString = "UPDATE [dbo].[Tbl_Inventory]  SET  TotalQuantity=(TotalQuantity-@Quantity) WHERE ModelCode= @ModelCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(objOrd.IteamCode));
            sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(objOrd.Quantity));
            try
            {
                sqlConn.Open();
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
        }

        public SelectList FuncReturnSelectChallanModelList(string Sel_StateName)
        {

            String conString = _dbConn.FunReturnConString();

            SqlConnection sqlConn = new SqlConnection();

            String SqlString = "SELECT c.[ModelCode1st],(select p.ProductDesc+'-'+m.[ModelName] from dbo.Tbl_IteamModel m,Tbl_IteamProduct p where substring(m.ModelCode,1,5)=p.ProductCode and m.ModelCode=c.ModelCode1st) as pName " +
                ",c.[IsPair] FROM [dbo].[Tbl_Chalan] c where (@ChalanNo='' OR c.ChalanNo=@ChalanNo) and c.TranStatus='N' group by c.[ModelCode1st],c.[IsPair]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add
                (
                    new
                        SelectListItem
                    {
                        Selected = true,
                        Text = "",
                        Value = "".ToString()
                    }
                )
                ;

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(Sel_StateName));
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (
                    sqlReader
                    != null)
                {
                    while (sqlReader.Read())
                    {
                        String isPaired = sqlReader["IsPair"].ToString();
                        String TextValue = "";
                        if (isPaired == "Y") TextValue = sqlReader["pName"].ToString() + "-(P)";
                        else TextValue = sqlReader["pName"].ToString();

                        objSelectListItem.Add(new SelectListItem
                        { Text = TextValue, Value = sqlReader["ModelCode1st"].ToString() });
                    }
                }
            }
            catch (
                Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList =
                new
                    SelectList(objSelectListItem, "Value", "Text", "000");

            return objSelectList;
        }

        public SelectList FuncReturnSelectChallanModelList(string Sel_StateName, String ModelCode, String PairCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT c.[ModelCode1st],(select p.ProductDesc+'-'+m.[ModelName] from dbo.Tbl_IteamModel m,Tbl_IteamProduct p where substring(m.ModelCode,1,5)=p.ProductCode and m.ModelCode=c.ModelCode1st) as pName "
                +
                ",c.[IsPair] FROM [dbo].[Tbl_Chalan] c where (@ChalanNo='' OR c.ChalanNo=@ChalanNo) and c.[ModelCode1st]=@ModelCode and c.TranStatus='N' and c.[IsPair]=@IsPair group by c.[ModelCode1st],c.[IsPair]"
                ;

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(Sel_StateName));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(ModelCode));
            if (PairCode == "Pair")
                sqlCmd.Parameters.AddWithValue("IsPair", Convert.ToString("Y"));
            else sqlCmd.Parameters.AddWithValue("IsPair", Convert.ToString("N"));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        String isPaired = sqlReader["IsPair"].ToString();
                        String TextValue = "";
                        if (isPaired == "Y") TextValue = sqlReader["pName"].ToString() + "-(P)";
                        else TextValue = sqlReader["pName"].ToString();

                        objSelectListItem.Add(new SelectListItem
                        { Text = TextValue, Value = sqlReader["ModelCode1st"].ToString() });
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", "000");

            return objSelectList;
        }

        public SelectList FuncReturnSelectChallanModelSerialList(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT c.[ModelCode],(select m.[ModelName]+' ( '+(select b.BrandName from dbo.Tbl_BrandInfo b where b.BrandCode=m.BrandCode)+' ) ' from dbo.Tbl_IteamModel m where m.ModelCode=c.ModelCode) as pName " +
                " FROM [dbo].[Tbl_Chalan] c where (@ChalanNo='' OR c.ChalanNo=@ChalanNo) and c.TranStatus='N' group by c.[ModelCode]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(Sel_StateName));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        //String isPaired = sqlReader["IsPair"].ToString();
                        String TextValue = "";
                        String VlValue = "";
                        TextValue = sqlReader["pName"].ToString();
                        VlValue = sqlReader["ModelCode"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = VlValue });
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", "000");

            return objSelectList;
        }

        public string[] FuncReturnProductQuantity(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT c.ModelCode,sum(c.Quantity) as mCount,c.ModelDesc,c.ItemSlNo FROM [dbo].[Tbl_Chalan] c where c.[ModelCode]=@ModelCode and c.ChalanNo=@ChalanNo group by c.[ModelCode],c.ModelDesc,c.ItemSlNo";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            String[] qString = Sel_StateName.Split('-');
            //String verIsPair = "";
            //if (Convert.ToString(qString[2]).Substring(11, 1) == "1") verIsPair = "Y";
            //else verIsPair = "N";
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(qString[0] + "-" + qString[1]));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(qString[2]));
            
            String[] rName = new string[4];
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName[0] = Convert.ToString(sqlReader["mCount"]);
                        rName[2] = Convert.ToString(sqlReader["ModelDesc"]);
                        rName[3] = Convert.ToString(sqlReader["ItemSlNo"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }

            rName[1] = FuncRetunUnitPrice(qString[2]);

            return rName;
        }

        public string FuncRetunUnitPrice(string qString)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "select m.ModelPrice from dbo.Tbl_IteamModel m where m.ModelCode=@ModelCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(qString));
            String rName = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName = Convert.ToString(sqlReader["ModelPrice"]);

                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return rName;
        }

        public IteamModels FuncReturnParseChallanInvoiceData(string ids)
        {
            var objSelectList = new IteamModels();
            try
            {
                String[] pString = ids.Split('+');

                objSelectList.SLNo = pString[9].ToString();
                objSelectList.IteamCode = pString[0].ToString();
                objSelectList.ChallanNo = pString[7].ToString();
                objSelectList.ProductName = FuncReturnProductName(objSelectList.IteamCode); // +"," + piardIteam[2];
                objSelectList.ModelName = FuncReturnModelName(objSelectList.IteamCode); // +"," + piardIteam[1];
                objSelectList.Quantity = Convert.ToInt32(pString[1]);
                objSelectList.CostPrice = Convert.ToDecimal(pString[2].ToString());
                objSelectList.IteamDesc = Convert.ToString(pString[3]);
                objSelectList.CommPerc = Convert.ToDecimal(pString[4].ToString());
                objSelectList.VatPerc = Convert.ToDecimal(pString[5].ToString());
                objSelectList.TaxPerc = Convert.ToDecimal(pString[6].ToString());
                objSelectList.TranType = "0";
                objSelectList.IteamSLNo = pString[8].ToString();


                //-------------------------------
            }
            catch (Exception e)
            {
                //throw;
            }


            return objSelectList;
        }


        public string[] FuncReturnSumData(List<IteamModels> IteamModelList)
        {
            //var objSelectList = new IteamModels();
            string[] pString = new string[6];
            try
            {
                decimal TQuantity = 0, TAmount = 0, TDisAmount = 0, TCommAmount = 0, NetAmount = 0, TVatAmount=0, TTaxAmount=0;

                foreach (IteamModels iteam in IteamModelList)
                {
                    TQuantity = TQuantity + Convert.ToDecimal(iteam.Quantity);
                    decimal varTamount = (Convert.ToDecimal(iteam.CostPrice) * Convert.ToDecimal(iteam.Quantity));
                    TAmount = TAmount + varTamount;
                    decimal varDiscount = ((Convert.ToDecimal(iteam.CostPrice) * Convert.ToDecimal(iteam.Quantity)) * Convert.ToDecimal(iteam.CommPerc) / 100);
                    TDisAmount = TDisAmount + varDiscount;
                    //TCommAmount = TCommAmount + ((varTamount-varDiscount) * Convert.ToDecimal(iteam.CommPerc) / 100);

                    TVatAmount = TVatAmount+((Convert.ToDecimal(iteam.CostPrice) * Convert.ToDecimal(iteam.Quantity)) * Convert.ToDecimal(iteam.VatPerc) / 100);

                    TTaxAmount = TTaxAmount+((Convert.ToDecimal(iteam.CostPrice) * Convert.ToDecimal(iteam.Quantity)) * Convert.ToDecimal(iteam.TaxPerc) / 100);
                }

                //NetAmount = TAmount ;
                pString[0] = TQuantity.ToString();
                pString[1] = TAmount.ToString("F");
                pString[3] = TDisAmount.ToString("F");
                pString[4] = TVatAmount.ToString("F");
                pString[5] = TTaxAmount.ToString("F");


                pString[2] = (TAmount - TDisAmount+ TVatAmount+ TTaxAmount).ToString("F");

                //-------------------------------
            }
            catch (Exception e)
            {
                //throw;
            }


            return pString;
        }

        public string FuncReturnInvoiceTranNo(DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT NEXT VALUE FOR SEQ_INVOICE AS MaxNo";
            //"SELECT max(substring([InvoiceNo],7,6)) MaxNo  FROM [dbo].[Tbl_InvoiceMaster] where DATEPART(year,EntryDate)=DATEPART(year,@BusinessDate)"
            // ;
            sqlConn.ConnectionString = conString;
            String OrdTranNo = "000000";

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("BusinessDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        OrdTranNo = "INV-" + objDaoUserInfo.BusinessDate.ToString("yy") +
                                    Convert.ToString(Convert.ToInt32(sqlReader["MaxNo"
                                                                         ])).PadLeft(6, '0');
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            if (OrdTranNo == "000000")
                OrdTranNo = "INV-" + objDaoUserInfo.BusinessDate.ToString("yy") + Convert.ToString(1).
                                                                                      PadLeft(6, '0');

            return OrdTranNo;
        }

        public string FuncReturnInvoiceStatusUpdate(String InvoiceNo, String TranStatus)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "UPDATE [dbo].[Tbl_InvoiceMaster]  set TranType=@TranType where InvoiceNo=@InvoiceNo";
            sqlConn.ConnectionString = conString;
            String OrdTranNo = "000000";

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TranType", Convert.ToString(TranStatus));
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(InvoiceNo));
            try
            {
                sqlConn.Open();
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();


            return OrdTranNo;
        }

        public bool FuncInvoiceMasterEntry(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection(); 
            sqlConn.ConnectionString = conString;
            sqlConn.Open();
            SqlTransaction sqlTran = sqlConn.BeginTransaction();

            String SqlString =
                " INSERT INTO [dbo].[Tbl_InvoiceMaster]([InvoiceNo],[CustomerCode],[TQuantity],[TAmount],[TCommAmount],[TVatAmount],[TTaxAmount],[NetAmount],[TransportAmount] " +
                ",[IsActive],[EntryBy],[EntryDate],[TranType]) VALUES(@InvoiceNo,@CustomerCode,@TQuantity,@TAmount,@TDisAmount,@TVatAmount,@TTaxAmount,@NetAmount,@TransportAmount,'Y',@EntryBy,@EntryDate,'N')";

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn,sqlTran);


            String SqlString1 =
                " INSERT INTO [dbo].[Tbl_Invoice]([SlNo],[InvoiceNo],[CustomerCode],[ModelCode],[Quantity],[UnitPrice],[CommPerc],VatPerc,TaxPerc,[IsActive],[EntryBy],[EntryDate]) " +
                " VALUES (@SlNo,@InvoiceNo,@CustomerCode,@ModelCode,@Quantity,@UnitPrice,@CommPerc,@VatPerc,@TaxPerc,'Y',@EntryBy,@EntryDate)";

            

            try
            {              
                
                sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
                sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
                sqlCmd.Parameters.AddWithValue("TQuantity", Convert.ToDecimal(viewModel.TQuantity));
                sqlCmd.Parameters.AddWithValue("TAmount", Convert.ToDecimal(viewModel.TAmount));
                //Decimal DisAmount = viewModel.NetAmount - (viewModel.TAmount + viewModel.TransportAmount + viewModel.TVatAmount + viewModel.TTaxAmount);
                sqlCmd.Parameters.AddWithValue("TDisAmount", Convert.ToDecimal(Math.Abs(viewModel.DisPerc)));
                sqlCmd.Parameters.AddWithValue("TVatAmount", Convert.ToDecimal(Math.Abs(viewModel.TVatAmount)));
                sqlCmd.Parameters.AddWithValue("TTaxAmount", Convert.ToDecimal(Math.Abs(viewModel.TTaxAmount)));
                sqlCmd.Parameters.AddWithValue("NetAmount", Convert.ToDecimal(viewModel.NetAmount));
                sqlCmd.Parameters.AddWithValue("TransportAmount", Convert.ToDecimal(viewModel.TransportAmount));
                sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(viewModel.InvoiceDate));


                int sqlResult = sqlCmd.ExecuteNonQuery();
                //if (sqlResult > 0) //opStatus = true;

                //-------------------------------------------Details Entry--------

                SqlCommand sqlCmd1;

                foreach (IteamModels objOrd in viewModel.ChallanList)
                {
                    sqlCmd1 = new SqlCommand(SqlString1, sqlConn, sqlTran);

                    sqlCmd1.Parameters.AddWithValue("SlNo", Convert.ToString(objOrd.SLNo));
                    sqlCmd1.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
                    sqlCmd1.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
                    sqlCmd1.Parameters.AddWithValue("TranDate", Convert.ToDateTime(viewModel.InvoiceDate));
                    sqlCmd1.Parameters.AddWithValue("ModelCode", Convert.ToString(objOrd.IteamCode));
                    sqlCmd1.Parameters.AddWithValue("Quantity", Convert.ToInt32(objOrd.Quantity));
                    sqlCmd1.Parameters.AddWithValue("UnitPrice", Convert.ToDecimal(objOrd.CostPrice));
                    sqlCmd1.Parameters.AddWithValue("CommPerc", Convert.ToDecimal(objOrd.CommPerc));
                    sqlCmd1.Parameters.AddWithValue("VatPerc", Convert.ToDecimal(objOrd.VatPerc));
                    sqlCmd1.Parameters.AddWithValue("TaxPerc", Convert.ToDecimal(objOrd.TaxPerc));
                    sqlCmd1.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                    sqlCmd1.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(viewModel.InvoiceDate));
                    //--------------------------

                    //FucUpdateChalanStatusForInvoice("Y", objOrd.ChallanNo, objOrd.IteamCode);
                    if (objOrd.TranType == "0")
                    {
                        sqlCmd1.ExecuteNonQuery();
                    }
                }

                sqlTran.Commit();

                opStatus = true;
            }
            catch (Exception e)
            {
                sqlTran.Rollback();
                opStatus = false;
            }
            finally
            {
                sqlConn.Close();
            }

            return opStatus;
        }

        public void FuncInvoiceChieldEntry(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                " INSERT INTO [dbo].[Tbl_Invoice]([SlNo],[InvoiceNo],[CustomerCode],[ModelCode],[Quantity],[UnitPrice],[CommPerc],[IsActive],[EntryBy],[EntryDate]) " +
                " VALUES (@SlNo,@InvoiceNo,@CustomerCode,@ModelCode,@Quantity,@UnitPrice,@CommPerc,'Y',@EntryBy,@EntryDate)";

            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = null;
                foreach (IteamModels objOrd in viewModel.ChallanList)
                {
                    sqlCmd = new SqlCommand(SqlString, sqlConn);

                    sqlCmd.Parameters.AddWithValue("SlNo", Convert.ToString(objOrd.SLNo));
                    sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
                    sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
                    sqlCmd.Parameters.AddWithValue("TranDate", Convert.ToDateTime(viewModel.InvoiceDate));
                    sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(objOrd.IteamCode));
                    sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToInt32(objOrd.Quantity));
                    sqlCmd.Parameters.AddWithValue("UnitPrice", Convert.ToDecimal(objOrd.CostPrice));
                    sqlCmd.Parameters.AddWithValue("CommPerc", Convert.ToDecimal(objOrd.CommPerc));
                    sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                    sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(viewModel.InvoiceDate));
                    //--------------------------

                    //FucUpdateChalanStatusForInvoice("Y", objOrd.ChallanNo, objOrd.IteamCode);
                    if (objOrd.TranType == "0")
                    {
                        sqlCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            finally
            {
                sqlConn.Close();
            }

            //return opStatus;
        }

        public List<OrderModel> FuncReturnQutationList(OrderModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT o.[OrderRefNo],(c.CompanyName) Customer,o.[OrderSub],o.[EntryDate],count(*)" +
                               " FROM [dbo].[Tbl_Order] o, dbo.Tbl_CustomerInfo c,dbo.Tbl_IteamModel i  " +
                               " where o.[CustomerCode]=c.CustomerCode and i.ModelCode=substring(o.[IteamCode],1,6) and (@OrderRefNo='' OR ltrim(rtrim(@OrderRefNo))=ltrim(rtrim(o.OrderRefNo))) "
                               +
                               " and ((@OrderRefNo!='') OR (@OrderRefNo='' and convert(int,o.EntryDate) between  convert(int,@TranDateFrom) and convert(int,@TranDateTo))) "
                               +
                               " group by o.[OrderRefNo],o.[OrderSub],o.[EntryDate],c.CompanyName";
            sqlConn.ConnectionString = conString;

            List<OrderModel> objQutationList = new List<OrderModel>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (viewModel.OrderRefNo == null || viewModel.OrderRefNo == "SKY/SGL/")
                sqlCmd.Parameters.AddWithValue("OrderRefNo", Convert.ToString(""));
            else sqlCmd.Parameters.AddWithValue("OrderRefNo", Convert.ToString(viewModel.OrderRefNo));
            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    OrderModel objOrderIteamModel = new OrderModel();
                    while (sqlReader.Read())
                    {
                        objOrderIteamModel = new OrderModel();
                        objOrderIteamModel.OrderRefNo = sqlReader["OrderRefNo"].ToString();
                        objOrderIteamModel.Customer = sqlReader["Customer"].ToString();
                        objOrderIteamModel.OrderSub = sqlReader["OrderSub"].ToString();
                        //objOrderIteamModel.IteamName = sqlReader["IteamName"].ToString();
                        //objOrderIteamModel.OrderDesc = sqlReader["OrderDesc"].ToString();
                        // objOrderIteamModel.CostPrice = sqlReader["CostPrice"].ToString();
                        objOrderIteamModel.OrderDate = Convert.ToDateTime(sqlReader["EntryDate"]);
                        objQutationList.Add(objOrderIteamModel);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }

            return objQutationList;
        }

        public List<OrderModel> FuncReturnQutationDetailsList(OrderModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT o.[OrderRefNo],c.CompanyName,c.CompanyAddress,o.[OrderSub], " +
                               " (select  dbo.fncReturnIteamDetail(substring(o.[IteamCode],1,6))) IteamName,o.[OrderDesc],(Convert(varchar(10),sum(o.Quantity))+' '+o.[Paired])as tQuantity, "
                               +
                               " (Convert(varchar(50),o.[CostPrice])) as unitCost, " +
                               " sum(o.Quantity)*o.[CostPrice] as tCostPrice,(convert(varchar(20),sum(o.Quantity)*o.[CostPrice]))AS  tCost, " +
                               " ROUND((((sum(o.Quantity)*o.[CostPrice])*o.TaxPerc/100)+((((sum(o.Quantity)*o.[CostPrice])*o.TaxPerc/100)+(sum(o.Quantity)*o.[CostPrice]))*o.VatPerc/100)),0) as tVatPrice,o.[EntryBy],o.[EntryDate] FROM [dbo].[Tbl_Order] o, dbo.Tbl_CustomerInfo c,dbo.Tbl_IteamModel i   "
                               +
                               " where o.[CustomerCode]=c.CustomerCode and i.ModelCode=substring(o.[IteamCode],1,6) and ltrim(rtrim(@OrderRefNo))=ltrim(rtrim(o.OrderRefNo))  "
                               +
                               " group by o.[OrderRefNo],c.CompanyName,c.CompanyAddress,o.[OrderSub],o.[IteamCode],o.[OrderDesc],o.[CostPrice] ,o.[EntryBy],o.[EntryDate],o.[Paired],o.TaxTex,o.VatPerc,o.TaxPerc"
                ;

            sqlConn.ConnectionString = conString;

            List<OrderModel> objQutationList = new List<OrderModel>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (viewModel.OrderRefNo == null)
                sqlCmd.Parameters.AddWithValue("OrderRefNo", Convert.ToString(""));
            else sqlCmd.Parameters.AddWithValue("OrderRefNo", Convert.ToString(viewModel.OrderRefNo));
            decimal tAmount = 0;
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    OrderModel objOrderIteamModel = new OrderModel();
                    int count = 1;
                    while (sqlReader.Read())
                    {
                        objOrderIteamModel = new OrderModel();
                        objOrderIteamModel.SlNo = count.ToString();
                        objOrderIteamModel.OrderRefNo = sqlReader["OrderRefNo"].ToString();
                        objOrderIteamModel.Customer = sqlReader["CompanyName"].ToString();
                        objOrderIteamModel.CustomerAddress = sqlReader["CompanyAddress"].ToString();
                        objOrderIteamModel.OrderSub = sqlReader["OrderSub"].ToString();
                        objOrderIteamModel.Model = Convert.ToString(sqlReader["IteamName"]);
                        objOrderIteamModel.OrderDesc = sqlReader["OrderDesc"].ToString();
                        objOrderIteamModel.CostPrice = Convert.ToDecimal(sqlReader["tCostPrice"]) +
                                                       Convert.ToDecimal(sqlReader["tVatPrice"])
                            ;
                        objOrderIteamModel.VatPrice = Convert.ToDecimal(sqlReader["tVatPrice"]);
                        tAmount = tAmount + objOrderIteamModel.CostPrice;
                        objOrderIteamModel.showPrice = sqlReader["tCost"].ToString();
                        objOrderIteamModel.showUnitCost = sqlReader["unitCost"].ToString();
                        objOrderIteamModel.showQuantity = sqlReader["tQuantity"].ToString();

                        objOrderIteamModel.OrderDate = Convert.ToDateTime(sqlReader["EntryDate"]);

                        objQutationList.Add(objOrderIteamModel);
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }

            if(objQutationList.Count()>0)
                    objQutationList[0].tCostAmount = tAmount;


            return objQutationList;
        }

        public List<TermsModel> FuncReturnTermsList(string QutationNo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT (select TarmsName from Tbl_TarmsCondition where TarmsCode=[ConditionNo])termDesc FROM [dbo].[Tbl_QutationCondition] where OrdRefNo=@OrdRefNo"
                ;

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("OrdRefNo", Convert.ToString(QutationNo));
            List<TermsModel> objTermsList = new List<TermsModel>();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    TermsModel objTermsModel = new TermsModel();
                    int count = 1;
                    while (sqlReader.Read())
                    {
                        objTermsModel = new TermsModel();
                        objTermsModel.SlNo = Convert.ToString(count);
                        objTermsModel.TermsDesc = Convert.ToString(sqlReader["termDesc"]);
                        objTermsList.Add(objTermsModel);
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objTermsList;
        }

        public List<ChallanModel> FuncReturnChallanList(ChallanModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT o.ChalanNo,o.ChalanDate,(select c.CompanyName from dbo.Tbl_CustomerInfo c where o.CustomerCode=c.CustomerCode ) Customer, "
                +
                " o.ChalanPoNo,COUNT(*) as Quantity FROM [dbo].Tbl_Chalan o where (@ChalanNo='' OR ltrim(rtrim(@ChalanNo))=ltrim(rtrim(o.ChalanNo)))  "
                +
                " and ((@ChalanNo!='') OR (@ChalanNo='' and convert(int,o.ChalanDate) between  convert(int,@TranDateFrom) and convert(int,@TranDateTo)))  "
                +
                " group by o.ChalanNo,o.ChalanDate,o.ChalanPoNo,o.CustomerCode";
            sqlConn.ConnectionString = conString;

            List<ChallanModel> objQutationList = new List<ChallanModel>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (string.IsNullOrEmpty(viewModel.ChallanNo))
                sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(""));
            else sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString("CHL-" + viewModel.ChallanNo));

            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    ChallanModel objOrderIteamModel = new ChallanModel();
                    while (sqlReader.Read())
                    {
                        objOrderIteamModel = new ChallanModel();
                        objOrderIteamModel.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objOrderIteamModel.Customer = sqlReader["Customer"].ToString();
                        objOrderIteamModel.ChallanDate = Convert.ToDateTime(sqlReader["ChalanDate"]);
                        objOrderIteamModel.PONo = sqlReader["ChalanPoNo"].ToString();
                        objOrderIteamModel.Quantity = sqlReader["Quantity"].ToString();
                        objQutationList.Add(objOrderIteamModel);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }

            return objQutationList;
        }

        public SelectList FuncReturnSelectChallanList(String CustomerCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString ="SELECT o.ChalanNo FROM [dbo].Tbl_Chalan o where o.CustomerCode=@CustomerCode and o.TranStatus !='Y' group by o.ChalanNo";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(CustomerCode));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    //ChallanModel objOrderIteamModel = new ChallanModel();
                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem
                        {
                            Text = sqlReader["ChalanNo"].ToString(),
                            Value = sqlReader["ChalanNo"].ToString()
                        });
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", CustomerCode);

            return objSelectList;
        }

        public SelectList FuncReturnSelectChallanList(String CustomerCode, String ChallanNo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT o.ChalanNo FROM [dbo].Tbl_Chalan o where o.CustomerCode=@CustomerCode and o.TranStatus !='Y' and o.ChalanNo=@ChallanNo group by o.ChalanNo"
                ;

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(CustomerCode));
            sqlCmd.Parameters.AddWithValue("ChallanNo", Convert.ToString(ChallanNo));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    //ChallanModel objOrderIteamModel = new ChallanModel();
                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem
                        {
                            Text = sqlReader["ChalanNo"].ToString(),
                            Value = sqlReader["ChalanNo"].
                                                          ToString()
                        });
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", CustomerCode);

            return objSelectList;
        }

        public List<ChallanModel> FuncReturnChallanReport(ChallanModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT o.ChalanNo,o.ChalanDate,(select '<b>'+c.CompanyName+'</b>,<br/>'+c.CompanyAddress from dbo.Tbl_CustomerInfo c where o.CustomerCode=c.CustomerCode ) Customer, " +
                " o.ChalanPoNo,((select  dbo.fncReturnIteamDetail(o.ModelCode))+' Sl#:'+o.ItemSlNo) IteamName,sum(Quantity) as Quantity " +
                // " ,(select  dbo.fncReturnIteamSlNo(o.ChalanNo,o.ModelCode1st,o.IsPair)) as IteamSLNo " +
                " FROM [dbo].Tbl_Chalan o where ltrim(rtrim(@ChalanNo))=ltrim(rtrim(o.ChalanNo)) " +
                " group by o.ChalanNo,o.ChalanDate,o.ChalanPoNo,o.CustomerCode,o.ModelCode,o.ItemSlNo;";
            sqlConn.ConnectionString = conString;

            List<ChallanModel> objQutationList = new List<ChallanModel>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (viewModel.ChallanNo == null)
                sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(""));
            else sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(viewModel.ChallanNo));

            //sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            //sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    ChallanModel objOrderIteamModel = new ChallanModel();
                    int count = 1;
                    while (sqlReader.Read())
                    {
                        objOrderIteamModel = new ChallanModel();
                        objOrderIteamModel.SLNo = count.ToString();
                        objOrderIteamModel.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objOrderIteamModel.Customer = sqlReader["Customer"].ToString();
                        objOrderIteamModel.ChallanDate = Convert.ToDateTime(sqlReader["ChalanDate"]);
                        objOrderIteamModel.PONo = sqlReader["ChalanPoNo"].ToString();
                        objOrderIteamModel.IteamName = sqlReader["IteamName"].ToString();
                        //objOrderIteamModel.IsPair = sqlReader["IsPair"].ToString();
                        //if (objOrderIteamModel.IsPair == "Y")
                        objOrderIteamModel.Quantity = sqlReader["Quantity"].ToString();
                        //else objOrderIteamModel.Quantity = sqlReader["Quantity"].ToString() + " Pcs";
                        //objOrderIteamModel.IteamSLNo = sqlReader["IteamSLNo"].ToString();
                        //objOrderIteamModel.PONo = sqlReader["ChalanPoNo"].ToString();
                        objQutationList.Add(objOrderIteamModel);
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }

            return objQutationList;
        }

        public List<ChallanModel> FuncReturnSalesReturnChallanData(ChallanModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT o.ChalanNo,o.ChalanDate,(select '<b>'+c.CompanyName+'</b>,<br/>'+c.CompanyAddress from dbo.Tbl_CustomerInfo c where o.CustomerCode=c.CustomerCode ) Customer, " +
                " o.ChalanPoNo,((select  dbo.fncReturnIteamDetail(o.ModelCode))) IteamName,sum(Quantity) as Quantity " +
                " FROM [dbo].Tbl_Chalan_Return o where ltrim(rtrim(@ChalanNo))=ltrim(rtrim(o.ChalanNo)) " +
                " group by o.ChalanNo,o.ChalanDate,o.ChalanPoNo,o.CustomerCode,o.ModelCode;";
            sqlConn.ConnectionString = conString;

            List<ChallanModel> objQutationList = new List<ChallanModel>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (viewModel.ChallanNo == null)
                sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(""));
            else sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(viewModel.ChallanNo));

            //sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            //sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    ChallanModel objOrderIteamModel = new ChallanModel();
                    int count = 1;
                    while (sqlReader.Read())
                    {
                        objOrderIteamModel = new ChallanModel();
                        objOrderIteamModel.SLNo = count.ToString();
                        objOrderIteamModel.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objOrderIteamModel.Customer = sqlReader["Customer"].ToString();
                        objOrderIteamModel.ChallanDate = Convert.ToDateTime(sqlReader["ChalanDate"]);
                        objOrderIteamModel.PONo = sqlReader["ChalanPoNo"].ToString();
                        objOrderIteamModel.IteamName = sqlReader["IteamName"].ToString();
                        //objOrderIteamModel.IsPair = sqlReader["IsPair"].ToString();
                        //if (objOrderIteamModel.IsPair == "Y")
                        objOrderIteamModel.Quantity = sqlReader["Quantity"].ToString();
                        //else objOrderIteamModel.Quantity = sqlReader["Quantity"].ToString() + " Pcs";
                        //objOrderIteamModel.IteamSLNo = sqlReader["IteamSLNo"].ToString();
                        //objOrderIteamModel.PONo = sqlReader["ChalanPoNo"].ToString();
                        objQutationList.Add(objOrderIteamModel);
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objQutationList;
        }

        public List<InvoiceModel> FuncReturnInvoiceList(InvoiceModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT o.InvoiceNo,o.EntryDate,(select c.CompanyName from dbo.Tbl_CustomerInfo c where o.CustomerCode=c.CustomerCode ) Customer,"
                +" o.TQuantity  FROM [dbo].Tbl_InvoiceMaster o where (ltrim(rtrim(@InvoiceNo)) ='' OR ltrim(rtrim(@InvoiceNo))=ltrim(rtrim(o.InvoiceNo))) "
                +" and ((@InvoiceNo!='') OR (@InvoiceNo='' and cast(o.EntryDate as Date) between  cast(@TranDateFrom as Date) and cast(@TranDateTo as Date))) "
                +" group by o.InvoiceNo,o.EntryDate,o.CustomerCode,TQuantity;";

            sqlConn.ConnectionString = conString;

            List<InvoiceModel> objQutationList = new List<InvoiceModel>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (viewModel.InvoiceNo == null)
                sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(""));
            else sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + viewModel.InvoiceNo));

            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    InvoiceModel objOrderIteamModel = new InvoiceModel();
                    while (sqlReader.Read())
                    {
                        objOrderIteamModel = new InvoiceModel();
                        objOrderIteamModel.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objOrderIteamModel.Customer = sqlReader["Customer"].ToString();
                        objOrderIteamModel.InvoiceDate = Convert.ToDateTime(sqlReader["EntryDate"]);
                        objOrderIteamModel.ChalanNo = "CHL-" + sqlReader["InvoiceNo"].ToString().Substring(4);
                        objOrderIteamModel.Quantity = Convert.ToInt32(sqlReader["TQuantity"]);
                        objQutationList.Add(objOrderIteamModel);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }

            return objQutationList;
        }

        public List<InvoiceModel> FuncReturnInvoiceReport(InvoiceModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT o.InvoiceNo,o.EntryDate,(select c.CompanyName+',<br/>'+c.CompanyAddress from dbo.Tbl_CustomerInfo c where o.CustomerCode=c.CustomerCode ) Customer, " +
                            " ('CHL-'+substring(o.InvoiceNo,5,8)) ChalanNo,o.UnitPrice,(select  dbo.fncReturnIteamNameForInvoice(('CHL-'+substring(o.InvoiceNo,5,8)),o.ModelCode)) IteamName ,o.Quantity,o.CommPerc,o.VatPerc,o.TaxPerc " +
                            " FROM [dbo].Tbl_Invoice o where ltrim(rtrim(@InvoiceNo))=ltrim(rtrim(o.InvoiceNo)) group by o.InvoiceNo,o.EntryDate,o.CustomerCode,o.ModelCode,o.UnitPrice,o.Quantity,o.CommPerc,o.VatPerc,o.TaxPerc ";

            sqlConn.ConnectionString = conString;

            List<InvoiceModel> objQutationList = new List<InvoiceModel>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (viewModel.InvoiceNo == null)
                sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(""));
            else sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + viewModel.InvoiceNo));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    InvoiceModel objOrderIteamModel = new InvoiceModel();
                    int count = 1;
                    while (sqlReader.Read())
                    {
                        objOrderIteamModel = new InvoiceModel();
                        objOrderIteamModel.SLNo = count.ToString();
                        objOrderIteamModel.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objOrderIteamModel.Customer = sqlReader["Customer"].ToString();
                        objOrderIteamModel.InvoiceDate = Convert.ToDateTime(sqlReader["EntryDate"]);
                        objOrderIteamModel.ChalanNo = sqlReader["ChalanNo"].ToString();
                        objOrderIteamModel.IteamName = sqlReader["IteamName"].ToString();
                        objOrderIteamModel.UnitPrice = Convert.ToDecimal(sqlReader["UnitPrice"]);
                        objOrderIteamModel.Quantity = Convert.ToInt32(sqlReader["Quantity"]);
                        objOrderIteamModel.CommPerc = Convert.ToDecimal(sqlReader["CommPerc"]);
                        objOrderIteamModel.VatPerc = Convert.ToDecimal(sqlReader["VatPerc"]);
                        objOrderIteamModel.TaxPerc = Convert.ToDecimal(sqlReader["TaxPerc"]);

                        objOrderIteamModel.ChalanPO = "";
                        //objOrderIteamModel.DisPerc = Convert.ToDecimal(sqlReader["DisPerc"].ToString());
                        //objOrderIteamModel.CommPerc = Convert.ToDecimal(sqlReader["CommPerc"].ToString());

                        //objOrderIteamModel.NUnitPrice = objOrderIteamModel.UnitPrice - Convert.ToDecimal(sqlReader["TDisCost"].ToString());
                        //objOrderIteamModel.TDisCost = objOrderIteamModel.Quantity *Convert.ToDecimal(sqlReader["TDisCost"].ToString());
                        //objOrderIteamModel.TCommCost =objOrderIteamModel.Quantity * Convert.ToDecimal(sqlReader["TCommCost"].ToString());

                        Decimal commUAmount = objOrderIteamModel.UnitPrice * objOrderIteamModel.CommPerc / 100;
                        Decimal vatAmount = objOrderIteamModel.UnitPrice * objOrderIteamModel.VatPerc / 100;
                        Decimal taxAmount = objOrderIteamModel.UnitPrice * objOrderIteamModel.TaxPerc / 100;

                        objOrderIteamModel.TUnitPtice = (objOrderIteamModel.Quantity * objOrderIteamModel.UnitPrice);
                        objOrderIteamModel.TotalCost = objOrderIteamModel.TUnitPtice;
                        objOrderIteamModel.TVatAmount = vatAmount;
                        objOrderIteamModel.TTaxAmount = taxAmount;
                        objOrderIteamModel.TCommCost = commUAmount;
                        objOrderIteamModel.NetAmount = objOrderIteamModel.TUnitPtice + vatAmount+ taxAmount- commUAmount;
                        objOrderIteamModel.ChallanDate = Convert.ToDateTime(sqlReader["EntryDate"]);
                        objOrderIteamModel.vQuantity = objOrderIteamModel.Quantity.ToString();

                        objQutationList.Add(objOrderIteamModel);
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }
            

            return objQutationList;
        }

        public InvoiceModels FuncReturnInvoiceReportMaster(string InvoiceNo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT o.InvoiceNo,o.EntryDate,(select '<b>'+c.CompanyName+'</b>,<br/>'+c.CompanyAddress from dbo.Tbl_CustomerInfo c where o.CustomerCode=c.CustomerCode ) Customer," +
                " o.TQuantity ,o.TVatAmount,o.TTaxAmount,o.TCommAmount,o.TAmount,o.TransportAmount,o.NetAmount FROM [dbo].Tbl_InvoiceMaster o where (ltrim(rtrim(@InvoiceNo)) ='' OR ltrim(rtrim(@InvoiceNo))=ltrim(rtrim(o.InvoiceNo)));";
            sqlConn.ConnectionString = conString;

            InvoiceModels objOrderIteamModel = new InvoiceModels();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (InvoiceNo == null)
                sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(""));
            else sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + InvoiceNo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        //objOrderIteamModel = new InvoiceModel();
                        objOrderIteamModel.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objOrderIteamModel.Customer = sqlReader["Customer"].ToString();
                        objOrderIteamModel.InvoiceDate = Convert.ToDateTime(sqlReader["EntryDate"]);
                        

                        objOrderIteamModel.TAmount = Convert.ToDecimal(sqlReader["TAmount"]);
                        objOrderIteamModel.TDisAmount = Convert.ToDecimal(sqlReader["TCommAmount"]);
                        objOrderIteamModel.TVatAmount = Convert.ToDecimal(sqlReader["TVatAmount"]);
                        objOrderIteamModel.TTaxAmount = Convert.ToDecimal(sqlReader["TTaxAmount"]);
                        objOrderIteamModel.TransportAmount = Convert.ToDecimal(sqlReader["TransportAmount"]);

                        objOrderIteamModel.NetAmount = objOrderIteamModel.TAmount+ objOrderIteamModel.TVatAmount+ objOrderIteamModel.TTaxAmount + objOrderIteamModel.TransportAmount - objOrderIteamModel.TDisAmount ;
                        //objOrderIteamModel.TQuantity = Convert.ToInt32(sqlReader["TQuantity"]);
                        //objQutationList.Add(objOrderIteamModel);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objOrderIteamModel;
        }

        public bool FuncDeleteChallanInfo(string ChalanNo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete from  [Tbl_Chalan] where [ChalanNo]=@ChalanNo";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString("CHL-" + ChalanNo));
            try
            {
                sqlConn.Open();
                bool iteamStatus = FucUpdateInventoryIteam(ChalanNo);
                if (iteamStatus)
                {
                    int sqlResult = sqlCmd.ExecuteNonQuery();
                    if (sqlResult > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public bool FucUpdateInventoryIteam(string ChalanNo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ModelCode],[Quantity] FROM [dbo].[Tbl_Chalan] where ChalanNo=@ChalanNo;";
            sqlConn.ConnectionString = conString;

            bool opStatus = false;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString("CHL-" + ChalanNo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    var objData = new IteamModels();
                    while (sqlReader.Read())
                    {
                        objData.ModelCode = sqlReader["ModelCode1st"].ToString();
                        objData.Quantity = Convert.ToInt32(sqlReader["Quantity"].ToString()) * (-1);
                    }

                    FuncUpdateInventoryIteam(objData);

                }
                opStatus = true;
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }


        public bool FucUpdateChalanStatusForInvoice(string TranStatus, String ChallanNo, String ModelCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "update Tbl_Chalan set TranStatus=@TranStatus where ChalanNo=@ChalanNo and ModelCode=@IteamCode;";
            sqlConn.ConnectionString = conString;

            bool opStatus = false;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(ChallanNo));
            sqlCmd.Parameters.AddWithValue("IteamCode", Convert.ToString(ModelCode));
            sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString(TranStatus));
            try
            {
                sqlConn.Open();
                sqlCmd.ExecuteNonQuery();
                opStatus = true;
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return opStatus;
        }

        public SelectList FuncReturnSelectInvoiceModelList(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT c.ModelCode,(select [dbo].[fncReturnIteamDetail](c.ModelCode)) as pName " +
                " FROM [dbo].Tbl_Invoice c where (@InvoiceNo='' OR c.InvoiceNo=@InvoiceNo);";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + Sel_StateName));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        //String isPaired = sqlReader["IsPair"].ToString();
                        String TextValue = "";
                        //if (isPaired == "Y") 
                        TextValue = sqlReader["pName"].ToString(); // +"-(P)";
                        //else TextValue = sqlReader["pName"].ToString();

                        objSelectListItem.Add(new SelectListItem
                        { Text = TextValue, Value = sqlReader["ModelCode"].ToString() });
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

        public string[] FuncReturnItemInformation(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "select i.InvoiceNo,i.ChalanNo,(select c.CompanyName from dbo.Tbl_CustomerInfo c where i.CustomerCode=c.CustomerCode ) Customer ,i.EntryDate from dbo.Tbl_Invoice i where i.ModelCode=@ModelCode "
                +
                " and i.InvoiceNo=@InvoiceNo and i.ChalanNo in(select ChalanNo from dbo.Tbl_Chalan where ModelCode1st=@ModelCode and (IteamSlNo1st=@IteamSlNo OR IteamSlNo2nd=@IteamSlNo)) "
                +
                " group by i.InvoiceNo,i.ChalanNo,i.CustomerCode ,i.EntryDate;";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            String[] qString = Sel_StateName.Split('+');
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + qString[0].ToString()));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(qString[1]));
            sqlCmd.Parameters.AddWithValue("IteamSlNo", Convert.ToString(qString[2]));
            String[] rName = new string[4];
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName[0] = Convert.ToString(sqlReader["InvoiceNo"]);
                        rName[1] = Convert.ToString(sqlReader["ChalanNo"]);
                        rName[2] = Convert.ToString(sqlReader["Customer"]);
                        rName[3] = Convert.ToString(sqlReader["EntryDate"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return rName;
        }

        public bool FuncWarrentyInfoEntry(InvoiceReturnModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                " INSERT INTO [dbo].[Tbl_Chalan_Return]([ChalanNo] ,[ChalanDate] ,[CustomerCode] ,[ChalanRemk] ,[ChalanPoNo] ,[ModelCode] ,[ModelDesc],[Quantity],[TranStatus],[EntryBy],[EntryDate])" +
                " VALUES(@ChalanNo,@ChalanDate,@CustomerCode,@ChalanRemk,@ChalanPoNo,@ModelCode,@ModelDesc,@Quantity,@TranStatus, @EntryBy,@EntryDate)";

            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

                sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(viewModel.NewChalanNo));
                sqlCmd.Parameters.AddWithValue("ChalanDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
                sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
                sqlCmd.Parameters.AddWithValue("ChalanPoNo", Convert.ToString("100000001"));
                sqlCmd.Parameters.AddWithValue("ChalanRemk", Convert.ToString("Return Challan Entry"));
                sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));

                sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
                sqlCmd.Parameters.AddWithValue("ModelDesc", Convert.ToString(""));
                sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(viewModel.ReturnQuantity));
                sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString("N"));

                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0)
                {
                    var objOrd = new IteamModels();
                    objOrd.IteamCode = viewModel.ModelCode;
                    objOrd.Quantity = viewModel.ReturnQuantity * (-1);
                    FuncUpdateInventoryIteam(objOrd);
                }

                opStatus = true;
            }
            catch (Exception se)
            {
                //sqlConn.Close;
            }
            sqlConn.Close();

            return opStatus;
        }

        public bool FuncWarrantyNewInfoEntry(InvoiceReturnModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                " INSERT INTO [dbo].[Tbl_Chalan]([ChalanNo] ,[ChalanDate] ,[CustomerCode] ,[ChalanRemk] ,[ChalanPoNo] ,[ModelCode] ,[ModelDesc],[Quantity],[TranStatus],[EntryBy],[EntryDate])" +
                " VALUES(@ChalanNo,@ChalanDate,@CustomerCode,@ChalanRemk,@ChalanPoNo,@ModelCode,@ModelDesc,@Quantity,@TranStatus, @EntryBy,@EntryDate)";

            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

                sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(viewModel.NewChalanNo));
                sqlCmd.Parameters.AddWithValue("ChalanDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
                sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
                sqlCmd.Parameters.AddWithValue("ChalanPoNo", Convert.ToString("100000001"));
                sqlCmd.Parameters.AddWithValue("ChalanRemk", Convert.ToString("Return Challan Entry"));
                sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));

                sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
                sqlCmd.Parameters.AddWithValue("ModelDesc", Convert.ToString(""));
                sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(viewModel.IssueQuantity));
                sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString("N"));

                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0)
                {
                    var objOrd = new IteamModels();
                    objOrd.IteamCode = viewModel.ModelCode;
                    objOrd.Quantity = viewModel.IssueQuantity;
                    FuncUpdateInventoryIteam(objOrd);
                }

                opStatus = true;
            }
            catch (Exception se)
            {
                //sqlConn.Close;
            }
            sqlConn.Close();

            return opStatus;
        }

        public SelectList FuncReturnChallanItemList(String ChallanNo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ChalanNo]+'+'+c.[ModelCode]+'+'+convert(varchar(10),[Quantity]) code, " +
                               " ((select ModelName from Tbl_IteamModel m where m.ModelCode=c.[ModelCode])+' Quantity :'+Convert(varchar(10),[Quantity])+' )') name " +
                               " FROM [dbo].[Tbl_Chalan] c  where [ChalanNo]=@ChalanNo";
            //String conString = _dbConn.FunReturnConString();
            //SqlConnection sqlConn = new SqlConnection();
            //String SqlString = "SELECT c.ModelCode,(select p.ProductDesc+'-'+m.[ModelName] from dbo.Tbl_IteamModel m,Tbl_IteamProduct p where substring(m.ModelCode,1,5)=p.ProductCode and m.ModelCode=c.ModelCode) as pName " +
            //                   " FROM [dbo].Tbl_Invoice c where (@InvoiceNo='' OR c.InvoiceNo=@InvoiceNo);";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (ChallanNo == null) ChallanNo = "";
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString("CHL-" + ChallanNo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        //String isPaired = sqlReader["IsPair"].ToString();
                        String TextValue = "";
                        //if (isPaired == "Y") 
                        TextValue = sqlReader["name"].ToString(); // +"-(P)";
                        //else TextValue = sqlReader["pName"].ToString();

                        objSelectListItem.Add(new SelectListItem
                        { Text = TextValue, Value = sqlReader["code"].ToString() });
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

        public ChallanModels FuncSearchChallanItemInfo(ChallanModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT [ChalanNo],[ChalanDate],[CustomerCode] ,[ChalanRemk],[ChalanPoNo],[ModelCode],[Quantity] " +
                " FROM [dbo].[Tbl_Chalan] where [ChalanNo]=@ChallanNo and [ModelCode]=@ModelCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("ChallanNo", Convert.ToString(viewModel.ChallanNo));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
            //sqlCmd.Parameters.AddWithValue("IteamSlNo", Convert.ToString(viewModel.IteamSLNo));
            String[] rName = new string[4];
            ChallanModels objChallanModels = new ChallanModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        objChallanModels.ChallanNo = Convert.ToString(sqlReader["ChalanNo"]);
                        objChallanModels.ChallanDate = Convert.ToDateTime(sqlReader["ChalanDate"]);
                        objChallanModels.CustomerCode = Convert.ToString(sqlReader["CustomerCode"]);
                        objChallanModels.ChallanRemarks = Convert.ToString(sqlReader["ChalanRemk"]);
                        objChallanModels.PONo = Convert.ToString(sqlReader["ChalanPoNo"]);
                        objChallanModels.ModelCode = Convert.ToString(sqlReader["ModelCode"]);
                        objChallanModels.Quantity = Convert.ToInt32(sqlReader["Quantity"]);
                        //objChallanModels.IteamSLNo2 = Convert.ToString(sqlReader["IteamSlNo2nd"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objChallanModels;
        }

        public bool FuncChallanIteamEntry(ChallanModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                " INSERT INTO [dbo].[Tbl_Chalan]([ChalanNo] ,[ChalanDate] ,[CustomerCode] ,[ChalanRemk] ,[ChalanPoNo] ,[ModelCode],ModelDesc,[Quantity]" +
                " ,[EntryBy],[EntryDate],TranStatus) VALUES(@ChalanNo,@ChalanDate,@CustomerCode,@ChalanRemk,@ChalanPoNo,@ModelCode,@ModelDesc,@Quantity,@EntryBy,@EntryDate,@TranStatus)";

            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = null;
                sqlCmd = new SqlCommand(SqlString, sqlConn);
                if (viewModel.PONo == null) viewModel.PONo = "";
                if (viewModel.ChallanRemarks == null) viewModel.ChallanRemarks = "";

                sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString("CHL-" + viewModel.ChallanNo));
                sqlCmd.Parameters.AddWithValue("ChalanDate", Convert.ToDateTime(viewModel.ChallanDate));
                sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
                sqlCmd.Parameters.AddWithValue("ChalanPoNo", Convert.ToString(viewModel.PONo));
                sqlCmd.Parameters.AddWithValue("ChalanRemk", Convert.ToString(viewModel.ChallanRemarks));
                sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
                sqlCmd.Parameters.AddWithValue("ModelDesc", Convert.ToString(viewModel.ModelDesc));
                sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(viewModel.Quantity));
                sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
                sqlCmd.Parameters.AddWithValue("TranStatus", Convert.ToString("N"));


                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0)
                {
                    opStatus = true;
                    var objOrd = new IteamModels();
                    objOrd.IteamCode = viewModel.ModelCode;
                    objOrd.Quantity = viewModel.Quantity;
                    FuncUpdateInventoryIteam(objOrd);
                }

            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public bool FuncDeleteChallanItemInfo(ChallanModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete FROM [dbo].[Tbl_Chalan] where [ChalanNo]=@ChalanNo and [Quantity]=@Quantity and [ModelCode]=@ModelCode"
                ;

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString("CHL-" + viewModel.ChallanNo));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
            sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(viewModel.Quantity));
            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0)
                {
                    var objOrd = new IteamModels();
                    objOrd.IteamCode = viewModel.ModelCode;
                    objOrd.Quantity = viewModel.Quantity * (-1);
                    FuncUpdateInventoryIteam(objOrd);
                    opStatus = true;
                }

            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public SelectList FuncReturnInvoiceItemList(string InvoiceNo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT (i.InvoiceNo+'+'+i.ModelCode+'+'+CAST(i.Quantity as varchar(10))) code " +
                              " ,((select m.ModelName+' ( '+(select b.BrandName from dbo.Tbl_BrandInfo b where b.BrandCode=m.BrandCode)+' ) ' from Tbl_IteamModel m where m.ModelCode=i.ModelCode)+',  ('+CAST(i.Quantity as varchar(10))+')') name   " +
                              " FROM [dbo].Tbl_Invoice i  where i.InvoiceNo=@InvoiceNo";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (InvoiceNo == null) InvoiceNo = "";
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + InvoiceNo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        String TextValue = "";
                        TextValue = sqlReader["name"].ToString();
                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["code"].ToString() });
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

        public InvoiceModels FuncSearchInvoiceItemInfo(InvoiceModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT InvoiceNo,EntryDate,[CustomerCode] ,ChalanNo,ModelCode,Quantity,UnitPrice,DisPerc" +
                " FROM [dbo].Tbl_Invoice where InvoiceNo=@InvoiceNo and ModelCode=@ModelCode and Quantity=@Quantity";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
            sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(viewModel.Quantity));
            //sqlCmd.Parameters.AddWithValue("PairCode", Convert.ToString(viewModel.PairCode));
            // String[] rName = new string[4];
            InvoiceModels objChallanModels = new InvoiceModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        objChallanModels.InvoiceNo = Convert.ToString(sqlReader["InvoiceNo"]);
                        objChallanModels.EntryDate = Convert.ToDateTime(sqlReader["EntryDate"]);
                        objChallanModels.InvoiceDate = Convert.ToDateTime(sqlReader["EntryDate"]);
                        objChallanModels.CustomerCode = Convert.ToString(sqlReader["CustomerCode"]);
                        objChallanModels.ChalanNo = Convert.ToString(sqlReader["ChalanNo"]);
                        objChallanModels.ModelCode = Convert.ToString(sqlReader["ModelCode"]);
                        objChallanModels.Quantity = Convert.ToInt32(sqlReader["Quantity"]);
                        objChallanModels.UnitPrice = Convert.ToDecimal(sqlReader["UnitPrice"]);
                        objChallanModels.DisPerc = Convert.ToDecimal(sqlReader["DisPerc"]);
                        //objChallanModels.PairCode = Convert.ToString(sqlReader["PairCode"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objChallanModels;
        }

        public InvoiceModels FuncSearchInvoiceMasterInfo(InvoiceModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT InvoiceNo,TQuantity,TAmount ,TVatAmount,NetAmount,AdvanceAmount  FROM [dbo].Tbl_InvoiceMaster where InvoiceNo=@InvoiceNo "
                ;

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
            // String[] rName = new string[4];
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        viewModel.TQuantity = Convert.ToInt32(sqlReader["TQuantity"]);
                        viewModel.TUnitPtice = Convert.ToDecimal(sqlReader["TAmount"]);
                        //viewModel.DisAmount = Convert.ToDecimal(sqlReader["TVatAmount"]);
                        viewModel.NetAmount = Convert.ToDecimal(sqlReader["NetAmount"]);
                        viewModel.TransportAmount = Convert.ToDecimal(sqlReader["TransportAmount"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return viewModel;
        }

        public bool FuncInvoiceMasterUpdate(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "update [Tbl_InvoiceMaster] set [TQuantity]=@TQuantity,[TAmount]=@TAmount,[TVatAmount]=@TVatAmount " +
                               ",[NetAmount]=@NetAmount,[TransportAmount]=@TransportAmount where [InvoiceNo]=@InvoiceNo";
            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
                sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
                sqlCmd.Parameters.AddWithValue("TQuantity", Convert.ToInt32(viewModel.TQuantity));
                sqlCmd.Parameters.AddWithValue("TAmount", Convert.ToDecimal(viewModel.TAmount));
                //viewModel.TDisAmount = Math.Round(viewModel.TAmount*viewModel.CommPerc/100);
                sqlCmd.Parameters.AddWithValue("TVatAmount", Convert.ToDecimal(viewModel.DisPerc));
                sqlCmd.Parameters.AddWithValue("TransportAmount", Convert.ToDecimal(viewModel.TransportAmount));
                sqlCmd.Parameters.AddWithValue("NetAmount", Convert.ToDecimal(viewModel.NetAmount));

                //sqlCmd.CommandType = CommandType.StoredProcedure;
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0) opStatus = true;
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return opStatus;
        }

        public bool FuncInvoiceChieldItemEntry(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " INSERT INTO [dbo].[Tbl_Invoice]([InvoiceNo],[CustomerCode],[ChalanNo],[ModelCode],[Quantity],[UnitPrice],[DisPerc] ,[IsActive],[EntryBy],[EntryDate])" +
                " VALUES (@InvoiceNo,@CustomerCode,@ChalanNo,@ModelCode,@Quantity,@UnitPrice,@DisPerc,'Y',@EntryBy,@EntryDate)"
                ;

            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

                sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + viewModel.InvoiceNo));
                sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
                sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(viewModel.ChalanNo));
                sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
                sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToDecimal(viewModel.Quantity));
                sqlCmd.Parameters.AddWithValue("UnitPrice", Convert.ToDecimal(viewModel.UnitPrice));
                sqlCmd.Parameters.AddWithValue("DisPerc", Convert.ToDecimal(viewModel.DisPerc));
                //sqlCmd.Parameters.AddWithValue("CommPerc", Convert.ToDecimal(viewModel.CommPerc));

                sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(viewModel.InvoiceDate));

                bool opStatus1 = FucUpdateChalanStatusForInvoice("Y", viewModel.ChalanNo, viewModel.ModelCode);
                if (opStatus1)
                {
                    sqlCmd.ExecuteNonQuery();
                    opStatus = true;
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return opStatus;
        }

        public SelectList FuncReturnSelectChallanSearchList(String CustomerCode, String ChallanNo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT o.ChalanNo FROM [dbo].Tbl_Chalan o where o.CustomerCode=@CustomerCode and o.TranStatus ='Y' and o.ChalanNo=@ChallanNo group by o.ChalanNo";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(CustomerCode));
            sqlCmd.Parameters.AddWithValue("ChallanNo", Convert.ToString(ChallanNo));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    //ChallanModel objOrderIteamModel = new ChallanModel();
                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem
                        {
                            Text = sqlReader["ChalanNo"].ToString(),
                            Value = sqlReader["ChalanNo"].
                                                          ToString()
                        });
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", ChallanNo);

            return objSelectList;
        }

        public SelectList FuncReturnSelectChallanModelSearchList(string Sel_StateName, String ModelCode,
                                                                   String Quantity)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT c.[ModelCode],(select m.[ModelName] from dbo.Tbl_IteamModel m where m.ModelCode=c.ModelCode) as pName " +
                " FROM [dbo].[Tbl_Chalan] c where (@ChalanNo='' OR c.ChalanNo=@ChalanNo) and c.[ModelCode]=@ModelCode and c.TranStatus='Y' and Quantity=@Quantity group by c.[ModelCode]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(Sel_StateName));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(ModelCode));
            sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(Quantity));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        String TextValue = "";
                        TextValue = sqlReader["pName"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["ModelCode"].ToString() });
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", ModelCode);

            return objSelectList;
        }

        public bool FuncDeleteInvoiceItemInfo(InvoiceModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete from  [Tbl_Invoice] where [InvoiceNo]=@InvoiceNo and ModelCode=@ModelCode and Quantity=@Quantity and Quantity=@Quantity;";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + viewModel.InvoiceNo));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
            sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(viewModel.Quantity));
            //sqlCmd.Parameters.AddWithValue("PairCode", Convert.ToString(viewModel.IsPaird));
            try
            {
                sqlConn.Open();
                bool iteamStatus = FucUpdateChalanStatusForInvoice("N", viewModel.ChalanNo, viewModel.ModelCode);
                if (iteamStatus)
                {
                    int sqlResult = sqlCmd.ExecuteNonQuery();
                    if (sqlResult > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public bool FuncInvoiceMasterDelete(InvoiceModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete from dbo.Tbl_InvoiceMaster i where i.InvoiceNo=@InvoiceNo and i.InvoiceNo not in( select m.InvoiceNo from Tbl_Invoice m where  m.InvoiceNo=i.InvoiceNo)";
            sqlConn.ConnectionString = conString;
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
                sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
                //sqlCmd.CommandType = CommandType.StoredProcedure;
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0) opStatus = true;
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            finally
            {
                sqlConn.Close();
            }

            return opStatus;
        }

        public List<string> FuncReturnChallanQuantity(string Sel_StateName)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT c.ModelCode,c.Quantity as mCount,c.ChalanNo," +
                            " c.CustomerCode,(select s.CompanyName FROM Tbl_CustomerInfo  s where c.CustomerCode=s.CustomerCode ) CompanyName from [dbo].[Tbl_Chalan] c ,Tbl_Invoice i " +
                            " where c.ChalanNo=i.ChalanNo and  c.[ModelCode]=@ModelCode and i.InvoiceNo=@InvoiceNo";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            String[] qString = Sel_StateName.Split('-');
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV" + "-" + qString[0]));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(qString[1]));

            var rName = new List<String>();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        rName.Add(Convert.ToString(sqlReader["mCount"]));
                        rName.Add(Convert.ToString(sqlReader["ChalanNo"]));
                        rName.Add(Convert.ToString(sqlReader["CompanyName"]));
                        rName.Add(Convert.ToString(sqlReader["CustomerCode"]));
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return rName;
        }

        public List<InvoiceModel> FuncReturnInvoiceListReport(InvoiceModels objOrder)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT o.InvoiceNo,o.EntryDate,(select c.CompanyName+',<br/>'+c.CompanyAddress from dbo.Tbl_CustomerInfo c where o.CustomerCode=c.CustomerCode ) Customer," +
                               " o.TQuantity,NetAmount FROM [dbo].Tbl_InvoiceMaster o where (@InvoiceNo ='' OR @InvoiceNo=o.InvoiceNo)";

            sqlConn.ConnectionString = conString;

            List<InvoiceModel> objQutationList = new List<InvoiceModel>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (objOrder.InvoiceNo == "")
                sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(""));
            else sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + objOrder.InvoiceNo));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    InvoiceModel objOrderIteamModel = new InvoiceModel();
                    int count = 1;
                    while (sqlReader.Read())
                    {
                        objOrderIteamModel = new InvoiceModel();
                        objOrderIteamModel.SLNo = count.ToString();
                        objOrderIteamModel.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objOrderIteamModel.Customer = sqlReader["Customer"].ToString();
                        objOrderIteamModel.InvoiceDate = Convert.ToDateTime(sqlReader["EntryDate"]);
                        objOrderIteamModel.NetAmount = Convert.ToDecimal(sqlReader["NetAmount"]);
                        objOrderIteamModel.TQuantity = Convert.ToString(sqlReader["TQuantity"]);
                        objQutationList.Add(objOrderIteamModel);
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objQutationList;
        }

        public List<IteamModels> FuncReturnAllInvoic(string ids)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT SLNo,InvoiceNo,EntryDate,[CustomerCode] ,ModelCode,Quantity,UnitPrice,CommPerc" +
                " FROM [dbo].Tbl_Invoice where InvoiceNo=@InvoiceNo";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + ids));

            List<IteamModels> objChallanList = new List<IteamModels>();
            IteamModels objChallanModels = new IteamModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        objChallanModels = new IteamModels();

                        objChallanModels.SLNo = Convert.ToString(sqlReader["SLNo"]);
                        objChallanModels.InvoiceNo = Convert.ToString(sqlReader["InvoiceNo"]);
                        objChallanModels.TranDate = Convert.ToDateTime(sqlReader["EntryDate"]);
                        objChallanModels.CutomerCode = Convert.ToString(sqlReader["CustomerCode"]);
                        objChallanModels.ModelCode = Convert.ToString(sqlReader["ModelCode"]);
                        objChallanModels.ProductName = FuncReturnProductName(objChallanModels.ModelCode); // +"," + piardIteam[2];
                        objChallanModels.ModelName = FuncReturnModelName(objChallanModels.ModelCode); // +"," + piardIteam[1];
                        objChallanModels.Quantity = Convert.ToInt32(sqlReader["Quantity"]);
                        objChallanModels.CostPrice = Convert.ToDecimal(sqlReader["UnitPrice"]);
                        objChallanModels.IteamCode = Convert.ToString(sqlReader["ModelCode"]);
                        objChallanModels.IteamDesc = "";
                        objChallanModels.CommPerc = Convert.ToDecimal(sqlReader["CommPerc"]);
                        objChallanModels.TranType = "1";
                        objChallanList.Add(objChallanModels);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objChallanList;
        }

        public List<IteamModels> FuncReturnDeleteInvoiceIteam(List<IteamModels> objIteamlList, string ids)
        {
            //bool opStatus = false;
            String[] InStr = ids.Split('-');
            String SlNo = InStr[1].ToString();
            List<IteamModels> newIteamList = new List<IteamModels>();

            try
            {
                foreach (IteamModels objIteam in objIteamlList)
                {
                    if (objIteam.SLNo != SlNo)
                        newIteamList.Add(objIteam);
                    else
                    {
                        FuncDeleteInvoiceItem(objIteam);
                    }
                }
            }
            catch (Exception e)
            {
                //opStatus = false;//throw;
            }
            //sqlConn.Close();

            return newIteamList;
        }

        public bool FuncDeleteInvoiceItem(IteamModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete FROM [dbo].[Tbl_Chalan] where [ChalanNo]=@ChalanNo and [SlNo]=@SlNo;" +
                               "delete FROM [dbo].[Tbl_Invoice] where [InvoiceNo]=@InvoiceNo and [SlNo]=@SlNo;";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString("CHL-" + viewModel.InvoiceNo.Substring(4)));
            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString(viewModel.InvoiceNo));
            sqlCmd.Parameters.AddWithValue("SlNo", Convert.ToString(viewModel.SLNo));
            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public string FuncReturnCustomerCode(string ids)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "SELECT [CustomerCode] FROM [dbo].Tbl_Invoice where InvoiceNo=@InvoiceNo";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("InvoiceNo", Convert.ToString("INV-" + ids));
            String CutomerCode = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {

                        CutomerCode = Convert.ToString(sqlReader["CustomerCode"]);

                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return CutomerCode;
        }
    }
}