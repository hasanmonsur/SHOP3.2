using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using NeSHOP.Models;
using NESHOP.Contacts;
using NESHOP.Models;

namespace NeSHOP.DAL
{
    public class BllIteam : IBllIteam
    {
        //BllDbConnection objBllDbConnection = new BllDbConnection();

        private readonly IConfiguration _configuration;
        private readonly ILogger<BllIteam> _logger;
        private readonly IBllDbConnection _dbConn;
        public BllIteam(IConfiguration configuration, ILogger<BllIteam> logger, IBllDbConnection dbConn)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
        }

        public bool FuncIteamStockEntry(IteamModels objDaoIteam, DaoUserInfo objDaoUserInfo)
        {
           // BllDbConnection objBllDbConnection = new BllDbConnection();
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [dbo].[Tbl_Iteam]([IteamCode],[IteamDesc]" +
                            " ,[Quantity],[CostPrice],[ChalanNo],[TranType],[EntryBy],[EntryDate],[ModelCode])" +
                            " VALUES(@IteamCode,@IteamDesc,@Quantity,@CostPrice,@ChalanNo," +
                            " @TranType,@EntryBy,@EntryDate,@ModelCode)";

            sqlConn.ConnectionString = conString;

            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
                sqlCmd.Parameters.AddWithValue("IteamCode", Convert.ToString(objDaoIteam.IteamCode));
                sqlCmd.Parameters.AddWithValue("IteamDesc", Convert.ToString(objDaoIteam.IteamDesc));
                sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(objDaoIteam.ModelCode));
                sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToInt32(objDaoIteam.Quantity));
                sqlCmd.Parameters.AddWithValue("CostPrice", Convert.ToDecimal(objDaoIteam.CostPrice));
                sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(objDaoIteam.ChallanNo));
                sqlCmd.Parameters.AddWithValue("TranType", Convert.ToString(objDaoIteam.TranType));
                sqlCmd.Parameters.AddWithValue("EntryBy", Convert.ToString(objDaoUserInfo.UserId));
                sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0)
                {
                    opStatus = true;

                    FuncInventoryUpdate(objDaoIteam);
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
            

            return opStatus;
        }

        public void FuncInventoryUpdate(IteamModels objDaoIteam)
        {
            //var objBllDbConnection = new BllDbConnection();
            String conString = _dbConn.FunReturnConString();
            var InvData = new IteamModels();
            InvData = FuncReturnInventoryInfo(objDaoIteam.ModelCode);

            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "";
            if (InvData.ModelCode != null)
            {
                SqlString =
                    "Update [Tbl_Inventory] set [TotalQuantity]=@Quantity,[LastAvgCostPrice]=@CostPrice where [ModelCode]=@ModelCode";
            }
            else
            {
                SqlString =
                    "INSERT INTO [dbo].[Tbl_Inventory]([ModelCode],[TotalQuantity],[LastAvgCostPrice])VALUES (@ModelCode,@Quantity,@CostPrice)";
            }

            sqlConn.ConnectionString = conString;


            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            if (InvData.ModelCode != "")
            {
                sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(objDaoIteam.ModelCode));
                sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToInt32(objDaoIteam.Quantity + InvData.Quantity));
                sqlCmd.Parameters.AddWithValue("CostPrice", Convert.ToDecimal(((objDaoIteam.CostPrice * objDaoIteam.Quantity) + (InvData.CostPrice * InvData.Quantity)) / (objDaoIteam.Quantity + InvData.Quantity)));
            }
            else
            {
                sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(objDaoIteam.ModelCode));
                sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToInt32(objDaoIteam.Quantity));
                sqlCmd.Parameters.AddWithValue("CostPrice", Convert.ToDecimal(objDaoIteam.CostPrice));
            }

            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
                //if (sqlResult > 0) //opStatus = true;
            }
            catch (Exception e)
            {
                //throw;
            }
            finally
            {
                sqlConn.Close();
            }
            
        }

        public IteamModels FuncReturnInventoryInfo(string ModelCode)
        {
            String conString = _dbConn.FunReturnConString();
            var InvData = new IteamModels();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT ModelCode,[TotalQuantity] ,[LastAvgCostPrice] FROM [dbo].[Tbl_Inventory] where [ModelCode]=@ModelCode;";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(ModelCode));
            //sqlCmd.Parameters.AddWithValue("slNo", Convert.ToString(slNo));

            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    int tcount = 0;
                    while (sqlReader.Read())
                    {
                        InvData.ModelCode = Convert.ToString(sqlReader["ModelCode"]);
                        InvData.Quantity = Convert.ToInt32(sqlReader["TotalQuantity"]);
                        InvData.CostPrice = Convert.ToDecimal(sqlReader["LastAvgCostPrice"]);
                    }

                }
            }
            catch (Exception e)
            {
                //opStatus = false;
            }
            sqlConn.Close();

            return InvData;
        }

        public string FuncReturnIteamCode(String ModelCode)
        {
            String conString = _dbConn.FunReturnConString();
            // bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT max(substring(IteamCode,11,4))as MaxIteamCode from Tbl_Iteam where substring(IteamCode,1,10)=@ModelCode ";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(CatagoryCode));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(ModelCode + DateTime.Now.ToString("yyMM")));
            String MaxIteamCode = "";
            string IteamCode = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        MaxIteamCode = Convert.ToString(sqlReader["MaxIteamCode"]);
                    }

                }
                IteamCode = ModelCode + DateTime.Now.ToString("yyMM") + Convert.ToString(Convert.ToInt32(MaxIteamCode) + 1).PadLeft(4, '0');
            }
            catch (Exception e)
            {

            }
            sqlConn.Close();
            if (IteamCode == "") IteamCode = ModelCode + DateTime.Now.ToString("yyMM") + Convert.ToString(1).PadLeft(4, '0');
            return IteamCode;
        }

        public IteamModels FuncReturnIteamInfo(string IteamCode)
        {
            String conString = _dbConn.FunReturnConString();
            IteamModels objIteam = new IteamModels();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT TOP 1000 i.[IteamCode],i.[IteamDesc],v.BrandName,m.ModelName,i.Quantity,i.CostPrice ,i.ChalanNo,i.EntryDate,i.EntryBy " +
                               " FROM [dbo].[Tbl_Iteam] i,dbo.Tbl_BrandInfo v,dbo.Tbl_IteamModel m " +
                               " where substring(i.[IteamCode],1,6)=m.ModelCode and substring(i.[IteamCode],1,3)=v.BrandCode and i.[IteamCode]=@IteamCode ";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("IteamCode", Convert.ToString(IteamCode));

            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objIteam = new IteamModels();
                        objIteam.IteamCode = sqlReader["IteamCode"].ToString();
                        objIteam.IteamDesc = sqlReader["IteamDesc"].ToString();
                        //objIteam.CatagoryCode = sqlReader["IteamCode"].ToString().Substring(0,2);
                        //objIteam.ProductCode = sqlReader["IteamCode"].ToString().Substring(0, 5);
                        objIteam.BrandCode = sqlReader["IteamCode"].ToString().Substring(0, 3);
                        objIteam.ModelCode = sqlReader["IteamCode"].ToString().Substring(0, 6);
                        objIteam.Quantity = Convert.ToInt32(sqlReader["Quantity"].ToString());
                        objIteam.CostPrice = Convert.ToDecimal(sqlReader["CostPrice"]);
                        objIteam.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objIteam.EntryBy = sqlReader["EntryBy"].ToString();
                        objIteam.EntryDate = Convert.ToDateTime(sqlReader["EntryDate"]).ToString("yyyy-MM-dd");
                    }

                }
            }
            catch (Exception e)
            {

            }
            sqlConn.Close();

            return objIteam;
        }

        public IteamModels FuncReturnFaultIteamInfo(string ModelCode, string iteamSlNo)
        {
            String conString = _dbConn.FunReturnConString();
            IteamModels objIteam = new IteamModels();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT TOP 1000 i.[IteamCode],i.[IteamDesc],c.CatagoryName,v.VenName,p.ProductDesc,m.ModelName,i.IteamSLNo,i.CostPrice ,i.ChalanNo,i.EntryDate,i.EntryBy " +
                               " FROM [dbo].[Tbl_Iteam] i, dbo.Tbl_IteamCatagory c ,dbo.Tbl_VendorInfo v, dbo.Tbl_IteamProduct p,dbo.Tbl_IteamModel m " +
                               " where substring(i.[IteamCode],1,2)=c.[CatagoryCode] and substring(i.[IteamCode],1,5)=p.[ProductCode] " +
                               " and m.ModelCode=i.ModelCode and substring(i.[IteamCode],1,8)=v.VenCode and i.ModelCode=@ModelCode and i.IteamSLNo=@IteamSLNo ";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(ModelCode));
            sqlCmd.Parameters.AddWithValue("IteamSLNo", Convert.ToString(iteamSlNo));

            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objIteam = new IteamModels();
                        objIteam.IteamCode = sqlReader["IteamCode"].ToString();
                        objIteam.IteamDesc = sqlReader["IteamDesc"].ToString();
                        objIteam.CatagoryCode = sqlReader["IteamCode"].ToString().Substring(0, 2);
                        objIteam.ProductCode = sqlReader["IteamCode"].ToString().Substring(0, 5);
                        objIteam.BrandCode = sqlReader["IteamCode"].ToString().Substring(0, 8);
                        objIteam.ModelCode = sqlReader["IteamCode"].ToString().Substring(0, 11);
                        objIteam.Quantity = Convert.ToInt32(sqlReader["Quantity"]);
                        objIteam.CostPrice = Convert.ToDecimal(sqlReader["CostPrice"]);
                        objIteam.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objIteam.EntryBy = sqlReader["EntryBy"].ToString();
                        objIteam.EntryDate = Convert.ToDateTime(sqlReader["EntryDate"]).ToString("yyyy-MM-dd");
                    }

                }
            }
            catch (Exception e)
            {

            }
            sqlConn.Close();

            return objIteam;
        }

        public bool FuncIteamFaultUpdate(IteamModels objDaoIteam, DaoUserInfo objDaoUserInfo)
        {
            //BllDbConnection objBllDbConnection = new BllDbConnection();
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "Update [Tbl_Iteam] set [fStatus]=@fStatus where [IteamSLNo]=@IteamSLNo and [ModelCode]=@ModelCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(objDaoIteam.ModelCode));
            //sqlCmd.Parameters.AddWithValue("IteamDesc", Convert.ToString(objDaoIteam.IteamDesc));
            //sqlCmd.Parameters.AddWithValue("ChallanNo", Convert.ToString(objDaoIteam.ChallanNo));
            sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(objDaoIteam.Quantity));
            if (objDaoIteam.fStatus)
                sqlCmd.Parameters.AddWithValue("fStatus", Convert.ToString("Y"));
            else sqlCmd.Parameters.AddWithValue("fStatus", Convert.ToString("N"));

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

        public bool FuncIteamDetailsUpdate(IteamModels objDaoIteam, DaoUserInfo objDaoUserInfo)
        {
            //BllDbConnection objBllDbConnection = new BllDbConnection();
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "Update [Tbl_Iteam] set [IteamDesc]=@IteamDesc,[ChalanNo]=@ChallanNo," +
                            " [Quantity]=@Quantity,[CostPrice]=@CostPrice where [IteamCode]=@IteamCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("IteamCode", Convert.ToString(objDaoIteam.IteamCode));
            sqlCmd.Parameters.AddWithValue("IteamDesc", Convert.ToString(objDaoIteam.IteamDesc));
            sqlCmd.Parameters.AddWithValue("ChallanNo", Convert.ToString(objDaoIteam.ChallanNo));
            sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToString(objDaoIteam.Quantity));
            sqlCmd.Parameters.AddWithValue("CostPrice", Convert.ToDecimal(objDaoIteam.CostPrice));

            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0)
                {
                    opStatus = true;
                    FuncUpdateInventoryInfo(objDaoIteam);
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public void FuncUpdateInventoryInfo(IteamModels objDaoIteam)
        {
            //BllDbConnection objBllDbConnection = new BllDbConnection();
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "sp_UpdateInventory";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(objDaoIteam.ModelCode));
            //sqlCmd.Parameters.AddWithValue("Quantity", Convert.ToInt32(objDaoIteam.Quantity));
            //sqlCmd.Parameters.AddWithValue("CostPrice", Convert.ToDecimal(objDaoIteam.CostPrice));

            sqlCmd.CommandType = CommandType.StoredProcedure;

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

            // return opStatus;
        }

        public bool FuncIteamDelete(IteamModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            //BllDbConnection objBllDbConnection = new BllDbConnection();
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "Delete From [Tbl_Iteam]  where [IteamCode]=@IteamCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("IteamCode", Convert.ToString(viewModel.IteamCode));

            try
            {
                sqlConn.Open();
                int sqlResult = sqlCmd.ExecuteNonQuery();
                if (sqlResult > 0)
                {
                    opStatus = true;
                    FuncUpdateInventoryInfo(viewModel);
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return opStatus;
        }

        public List<IteamModels> FuncReturnDuplicateIteamInfo(string ModelCode, string iteamSlNo)
        {
            String conString = _dbConn.FunReturnConString();
            List<IteamModels> objIteamList = new List<IteamModels>();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " SELECT TOP 1000 i.[IteamCode],i.[IteamDesc],c.CatagoryName,v.VenName,p.ProductDesc,m.ModelName,i.IteamSLNo,i.CostPrice ,i.ChalanNo,i.EntryDate,i.EntryBy " +
                               " FROM [dbo].[Tbl_Iteam] i, dbo.Tbl_IteamCatagory c ,dbo.Tbl_VendorInfo v, dbo.Tbl_IteamProduct p,dbo.Tbl_IteamModel m " +
                                " where substring(i.[IteamCode],1,2)=c.[CatagoryCode] and substring(i.[IteamCode],1,5)=p.[ProductCode] " +
                                " and m.ModelCode=i.ModelCode and substring(i.[IteamCode],1,8)=v.VenCode and (@ModelCode='' OR i.ModelCode=@ModelCode) " +
                                " and (@IteamSLNo='' OR i.IteamSLNo=@IteamSLNo) and i.IteamSLNo in(select x.IteamSLNo from( " +
                                " select d.IteamSLNo,COUNT(*) from [dbo].[Tbl_Iteam] d group by d.IteamSLNo having COUNT(*)>1)x) ";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(ModelCode));
            sqlCmd.Parameters.AddWithValue("IteamSLNo", Convert.ToString(iteamSlNo));

            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    IteamModels objIteam = new IteamModels();
                    while (sqlReader.Read())
                    {
                        objIteam = new IteamModels();
                        objIteam.IteamCode = sqlReader["IteamCode"].ToString();
                        objIteam.IteamDesc = sqlReader["IteamDesc"].ToString();
                        objIteam.CatagoryCode = sqlReader["IteamCode"].ToString().Substring(0, 2);
                        objIteam.ProductCode = sqlReader["IteamCode"].ToString().Substring(0, 5);
                        objIteam.BrandCode = sqlReader["IteamCode"].ToString().Substring(0, 8);
                        objIteam.ModelCode = sqlReader["IteamCode"].ToString().Substring(0, 11);
                        objIteam.Quantity = Convert.ToInt32(sqlReader["Quantity"]);
                        objIteam.CostPrice = Convert.ToDecimal(sqlReader["CostPrice"]);
                        objIteam.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objIteam.EntryBy = sqlReader["EntryBy"].ToString();
                        objIteam.EntryDate = Convert.ToDateTime(sqlReader["EntryDate"]).ToString("yyyy-MM-dd");
                        objIteamList.Add(objIteam);
                    }

                }
            }
            catch (Exception e)
            {

            }
            sqlConn.Close();

            return objIteamList;
        }
    }
}
