using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Contacts;
using NESHOP.Models;

namespace NeSHOP.DAL
{
    public class BllCommon : IBllCommon
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BllCommon> _logger;
        private readonly IBllDbConnection _dbConn;
        public BllCommon(IConfiguration configuration, ILogger<BllCommon> logger, IBllDbConnection dbConn)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
        }

        public List<DaoRoleInfo> FuncReturnRoleList(DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            List<DaoRoleInfo> objDaoRoleInfoList = new List<DaoRoleInfo>();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "select *from Tbl_UserPrev where UserId=@User_ID order by RoleCode";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("User_ID", Convert.ToString(objDaoUserInfo.UserId));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        DaoRoleInfo objDaoRoleInfo = new DaoRoleInfo();
                        objDaoRoleInfo.RoleCode = Convert.ToString(sqlReader["RoleCode"]);
                        objDaoRoleInfoList.Add(objDaoRoleInfo);

                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDaoRoleInfoList;
        }
        public List<DaoModuleInfo> FuncReturnModuleList(List<DaoRoleInfo> objDaoRoleList)
        {
            String conString = _dbConn.FunReturnConString();
            List<DaoModuleInfo> objDaoModuleInfoList = new List<DaoModuleInfo>();
            String RoleList = "'99'";
            for (int i = 0; i < objDaoRoleList.Count; i++)
            {
                RoleList = RoleList + ",'" + objDaoRoleList[i].RoleCode + "'";
            }
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " select mc.ModuleCode,m.ModuleDesc from dbo.Tbl_RoleConfig r, dbo.Tbl_ModuleConfig mc,Tbl_Module m " +
                               " where r.RoleCode in(" + RoleList + ") and r.FormCode=mc.FormCode and m.ModuleCode=mc.ModuleCode AND mc.IsActive=1 group by mc.ModuleCode,m.ModuleDesc order by mc.ModuleCode";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("RoleList", Convert.ToString(RoleList));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        DaoModuleInfo objDaoModuleInfo = new DaoModuleInfo();
                        objDaoModuleInfo.ModuleCode = Convert.ToString(sqlReader["ModuleCode"]);
                        objDaoModuleInfo.ModuleDesc = Convert.ToString(sqlReader["ModuleDesc"]);
                        objDaoModuleInfoList.Add(objDaoModuleInfo);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDaoModuleInfoList;
        }

        public string FunctionReturnFileLoc(string varAction, String ModuleCode)
        {
            String conString = _dbConn.FunReturnConString();
            //List<DaoModuleInfo> objDaoModuleInfoList = new List<DaoModuleInfo>();
            String varLocation = "";

            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "select f.FORMLOC from TBL_FORMS f,TBL_MODULECONFIG c where f.ACTIONLINK=@ACTIONLINK AND c.FORMCODE=f.FORMCODE and  c.MODULECODE=@MODULECODE";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ACTIONLINK", Convert.ToString(varAction));
            sqlCmd.Parameters.AddWithValue("MODULECODE", Convert.ToString(ModuleCode));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        varLocation = Convert.ToString(sqlReader["FORMLOC"]);
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


            return varLocation;
        }
        public List<DaoModuleInfo> FuncReturnMenuList(List<DaoRoleInfo> objDaoRoleList, List<DaoModuleInfo> objDaoModuleList)
        {
            String conString = _dbConn.FunReturnConString();
            List<DaoMenuInfo> objDaoMenuInfoList = new List<DaoMenuInfo>();
            List<DaoModuleInfo> objModuleList = new List<DaoModuleInfo>();
            String RoleList = "'99'";
            for (int i = 0; i < objDaoRoleList.Count; i++)
            {
                RoleList = RoleList + ",'" + objDaoRoleList[i].RoleCode + "'";
            }
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " select mc.ModuleCode,m.ModuleDesc,f.FormCode,f.FormName,f.ActionLink,f.FormLoc from dbo.Tbl_RoleConfig r, dbo.Tbl_ModuleConfig mc ,dbo.Tbl_Forms f, dbo.Tbl_Module m " +
                               " where r.RoleCode in(" + RoleList + ") and r.FormCode=mc.FormCode  and mc.FormCode=f.FormCode and m.ModuleCode=mc.ModuleCode AND mc.IsActive=1 and r.IsActive=1 " +
                               " group by mc.ModuleCode,m.ModuleDesc,f.FormCode,f.FormName,f.ActionLink,f.FormLoc order by mc.ModuleCode asc,f.FormCode asc";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("RoleList", Convert.ToString(RoleList));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        DaoMenuInfo objDaoMenuInfo = new DaoMenuInfo();
                        objDaoMenuInfo.ModuleCode = Convert.ToString(sqlReader["ModuleCode"]);
                        objDaoMenuInfo.ModuleDesc = Convert.ToString(sqlReader["ModuleDesc"]);
                        objDaoMenuInfo.FormCode = Convert.ToString(sqlReader["FormCode"]);
                        objDaoMenuInfo.FormName = Convert.ToString(sqlReader["FormName"]);
                        objDaoMenuInfo.ActionLink = Convert.ToString(sqlReader["ActionLink"]);
                        objDaoMenuInfo.FormLoc = Convert.ToString(sqlReader["FormLoc"]);
                        objDaoMenuInfoList.Add(objDaoMenuInfo);
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


            foreach (DaoModuleInfo objDaoModule in objDaoModuleList)
            {
                List<DaoMenuInfo> objDaoMenuInfoTemp = new List<DaoMenuInfo>();
                DaoModuleInfo nobjDaoModule = new DaoModuleInfo();

                nobjDaoModule = objDaoModule;

                foreach (var objDaoMenuIn in objDaoMenuInfoList)
                {
                    if (objDaoModule.ModuleCode == objDaoMenuIn.ModuleCode)
                        nobjDaoModule.MenuInfo.Add(objDaoMenuIn);
                }

                //nobjDaoModule.MenuInfo = objDaoMenuInfoTemp;

                objModuleList.Add(nobjDaoModule);
            }

            return objModuleList;
        }
        public TblAppinfoModel FuncReturnAppInformation(String Instcode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " SELECT [INSTCODE],[INSTNAME],[INSTADDRESS],[INSTEMAIL],[INSTFAX],[INTPHONE],[WEBSITE],[INSTCATAGORYCODE],[RefCode]" +
                               " FROM [TBL_APPINFO] WHERE INSTCODE=@INSTCODE";
            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("INSTCODE", Convert.ToString(Instcode));
            TblAppinfoModel objTblAppinfoModel = new TblAppinfoModel();
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        objTblAppinfoModel.Instcode = sqlReader["INSTCODE"].ToString();
                        objTblAppinfoModel.Instname = sqlReader["INSTNAME"].ToString();
                        objTblAppinfoModel.Instaddress = sqlReader["INSTADDRESS"].ToString();
                        objTblAppinfoModel.Instemail = sqlReader["INSTEMAIL"].ToString();
                        objTblAppinfoModel.Instfax = sqlReader["INSTFAX"].ToString();
                        objTblAppinfoModel.Intphone = sqlReader["INTPHONE"].ToString();
                        objTblAppinfoModel.Website = sqlReader["WEBSITE"].ToString();
                        objTblAppinfoModel.Instcatagorycode = sqlReader["INSTCATAGORYCODE"].ToString();
                        objTblAppinfoModel.RefCode = sqlReader["RefCode"].ToString();

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();


            return objTblAppinfoModel;
        }

        public SelectList FuncReturnIteamCatagoryList(String CatagoryCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_IteamCatagory";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["CatagoryDesc"].ToString(), Value = sqlReader["CatagoryCode"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", CatagoryCode);

            return objSelectList;
        }

        public SelectList FuncReturnIteamVendorList(String VenCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_VendorInfo";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["CompanyName"].ToString() + "-" + sqlReader["ContactPerson"].ToString(), Value = sqlReader["VenCode"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", VenCode);

            return objSelectList;
        }

        public List<SelectListItem> GetCompanyList()
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_Company";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new SelectListItem[1000];
            objSelectListItem[0] = new SelectListItem
            {
                Selected = true,
                Text = "--Select--",
                Value = "000".ToString()
            };
            int counter = 1;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem[counter] = new SelectListItem
                        {
                            Text = sqlReader["CompanyName"].ToString(),
                            Value = sqlReader["CompanyCode"].ToString()
                        };

                        counter++;
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            var objSelectList = new SelectListItem[counter];
            for (int i = 0; i < counter; i++)
            {
                objSelectList[i] = objSelectListItem[i];
            }
            return objSelectList.ToList();
        }

        public SelectList FuncIteamOrdTypeList(String OrdType)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Buy Order", Value = "1".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Sale Order", Value = "2".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Production In Order", Value = "3".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Production Out Order", Value = "3".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Others", Value = "3".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", OrdType);

            return objSelectList;
        }

        public SelectList FuncReturnIteamTypeList(String IteamType)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "--Select--", Value = "0".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Sole Product", Value = "1".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Raw Matarials", Value = "2".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Others", Value = "3".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", IteamType);

            return objSelectList;
        }

        public SelectList FuncReturnTaxCodeList(String TaxCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_TaxInfo";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["TaxDesc"].ToString(), Value = sqlReader["TaxCode"].ToString() });

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", TaxCode);

            return objSelectList;
        }

        public SelectList FuncReturnProductList(String TaxCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ProductCode] ,[ProductDesc] FROM [dbo].[Tbl_IteamProduct]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["TaxDesc"].ToString(), Value = sqlReader["TaxCode"].ToString() });

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", TaxCode);

            return objSelectList;
        }

        public SelectList GetCountryList(String CountryCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_Country order by CountryName";
            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "000" });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Selected = true, Text = sqlReader["CountryName"].ToString(), Value = sqlReader["CountryCode"].ToString() });
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", CountryCode);

            return objSelectList;
        }

        public SelectList GetDistrictList(String DistCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_District";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "00" });
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Selected = true, Text = sqlReader["DistName"].ToString(), Value = sqlReader["DistCode"].ToString() });

                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", DistCode);

            return objSelectList;
        }

        public SelectList FuncReturnVenTypeList(String VenType)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "--Select--", Value = "N" });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Class-A", Value = "A" });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Class-B", Value = "B" });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Class-C", Value = "C" });
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", VenType);

            return objSelectList;
        }

        public SelectList FuncCounterCodeList(string CounterCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from [Tbl_Counter]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0000000000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["CenterName"].ToString(), Value = sqlReader["CounterCode"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", CounterCode);

            return objSelectList;
        }

        public SelectList FuncDelverMediaCodeList(String DelverMediaCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_DelverMedia";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["DelverMediaName"].ToString(), Value = sqlReader["DelverMediaCode"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", DelverMediaCode);

            return objSelectList;
        }

        public SelectList FuncPaymentTypeList(String PaymentType)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_PaymentType";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "-Select--", Value = "0000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["PaymentTypeName"].ToString(), Value = sqlReader["PaymentType"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", PaymentType);

            return objSelectList;
        }

        public SelectList FuncTranTypeList(String TranType)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "0".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Buy Tran", Value = "1".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Sale Sale", Value = "2".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Production In Tran", Value = "3".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Production Out Tran", Value = "4".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Others", Value = "5".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", TranType);

            return objSelectList;
        }

        public SelectList FuncInvTranTypeList(String TranType)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "None Paid", Value = "N".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Partial Paid", Value = "P".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Full Paid", Value = "Y".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", TranType);

            return objSelectList;
        }

        public List<IteamModels> FuncReturnIteamList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT TOP 1000 i.[IteamCode],v.BrandName, i.[IteamDesc],m.ModelName,m.ModelBName,i.Quantity,i.CostPrice ,i.ChalanNo,i.EntryDate,i.EntryBy " +
                               " FROM [dbo].[Tbl_Iteam] i,dbo.Tbl_BrandInfo v,dbo.Tbl_IteamModel m " +
                               " where substring(i.[IteamCode],1,6)=m.ModelCode and substring(i.[IteamCode],1,3)=v.BrandCode order by EntryDate desc";
            sqlConn.ConnectionString = conString;
            List<IteamModels> objIteamList = new List<IteamModels>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
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
                        //objIteam.CatagoryName = sqlReader["CatagoryName"].ToString();
                        //objIteam.ProductName = sqlReader["ProductDesc"].ToString();
                        objIteam.VendorName = sqlReader["BrandName"].ToString();
                        objIteam.ModelName = sqlReader["ModelName"].ToString() + "( " + sqlReader["ModelBName"].ToString() + " )";
                        objIteam.Quantity = Convert.ToInt32(sqlReader["Quantity"]);
                        objIteam.CostPrice = Convert.ToDecimal(sqlReader["CostPrice"]);
                        objIteam.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objIteam.EntryDate = Convert.ToDateTime(sqlReader["EntryDate"]).ToString("yyyy-MM-dd");
                        objIteam.EntryBy = sqlReader["EntryBy"].ToString();
                        objIteamList.Add(objIteam);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objIteamList;
        }

        public SelectList FuncReturnVenList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [BrandCode],[BrandName] FROM [dbo].[Tbl_BrandInfo] order by [BrandName]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {

                        // String[] strValue = sqlReader["VenName"].ToString().Split('-');
                        String TextValue = sqlReader["BrandName"].ToString();
                        //sqlReader["OriginDesc"].ToString().PadRight(20, Convert.ToChar(160)) +
                        //sqlReader["ProductDesc"].ToString().PadRight(30, Convert.ToChar(160))+
                        //sqlReader["CatagoryName"].ToString().PadRight(30, Convert.ToChar(160));

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["BrandCode"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Code);

            return objSelectList;
        }

        public SelectList FuncReturnDeptList(string Deptcode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [DEPTID],[DEPTNAME],[DIVID] FROM [TBL_DEPARTMENTS]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["DEPTID"].ToString() + "-" + sqlReader["DEPTNAME"].ToString() + "-" + sqlReader["DIVID"].ToString(), Value = sqlReader["DEPTID"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Deptcode);

            return objSelectList;
        }

        public SelectList FuncDesigcodeList(string Desigcode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [DESIGCODE],[DESIGDESC] FROM [TBL_DESIGNATION]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["DESIGDESC"].ToString(), Value = sqlReader["DESIGCODE"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Desigcode);

            return objSelectList;
        }

        public SelectList FuncEmptypeList(string EmpType)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Permanent", Value = "P".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Temporary", Value = "T".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", EmpType);

            return objSelectList;
        }

        public SelectList FuncReturnGenderList(string Gender)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Male", Value = "M".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Female", Value = "F".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Gender);

            return objSelectList;
        }

        public SelectList FuncReturnReligiouscodeList(string Code)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Islam", Value = "1".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Hidus", Value = "2".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Buddist", Value = "3".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Code);

            return objSelectList;
        }

        public SelectList FuncReturnReportTypeList(string Code)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Report Viewer", Value = "1".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Report Pdf", Value = "2".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Code);

            return objSelectList;
        }

        public SelectList FuncReturnReportViewList(string Code)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Form View", Value = "1".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "List View", Value = "2".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Code);

            return objSelectList;
        }

        public SelectList FuncReturnShistNoList(string Code)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "One Shift", Value = "1".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Two Shift", Value = "2".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Three Shift", Value = "3".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Code);

            return objSelectList;
        }

        public SelectList FuncReturnInstList(string code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [INSTCODE] ,[INSTNAME] FROM [TBL_APPINFO]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["INSTNAME"].ToString(), Value = sqlReader["INSTCODE"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", code);

            return objSelectList;
        }

        public string FuncReturnInstCode(DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT isnull(MAX([INSTCODE]),'1001') as maxCode  FROM [TBL_APPINFO]";

            sqlConn.ConnectionString = conString;
            String maxCode = "0000";
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        maxCode = (Convert.ToInt32(sqlReader["maxCode"]) + 1).ToString();
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return maxCode;
        }

        public SelectList FuncIteamOrdStatusList(string OrdStatus)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Buy Order", Value = "1".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Sale Order", Value = "2".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Production In Order", Value = "3".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Production Out Order", Value = "3".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Others", Value = "3".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", OrdStatus);

            return objSelectList;
        }

        public List<Product> FuncIteamOrdList(String OrdType)
        {
            var objSelectListItem = new List<Product>();

            var obListItem = new Product();
            obListItem.Name = "Buy Order" + "--" + OrdType;
            obListItem.Id = 1;
            objSelectListItem.Add(obListItem);

            obListItem = new Product();
            obListItem.Name = "Sale Order";
            obListItem.Id = 2;
            objSelectListItem.Add(obListItem);

            return objSelectListItem;
        }

        public SelectList FuncVendorList(string VenCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_VendorInfo";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["CompanyName"].ToString(), Value = sqlReader["VenCode"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", VenCode);

            return objSelectList;
        }

        public SelectList FuncReturnSelectVenList(string BrandCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT *from Tbl_BrandInfo where (@BrandCode='' OR BrandCode=@BrandCode) order by BrandName";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(BrandCode));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["BrandName"].ToString(), Value = sqlReader["BrandCode"].ToString() });
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

        public SelectList FuncReturnPTypeList(string Code)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Vendor", Value = "V".ToString() });
            objSelectListItem.Add(new SelectListItem { Selected = false, Text = "Customer", Value = "C".ToString() });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Code);

            return objSelectList;
        }

        public string funcAmountToWord(decimal amount)
        {
            String wordAmount = "";
            String[] stramount = Convert.ToString(amount).Split('.');
            wordAmount = AmountToWord(Convert.ToInt32(stramount[0]));
            wordAmount = wordAmount + " Taka Only ";// " and " + AmountToWord(Convert.ToInt32(stramount[1])) + " Paisa only.";

            return wordAmount;
        }

        public static string AmountToWord(int number)
        {
            if (number == 0) return "Zero";

            //if (number == -2147483648)
            // return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";

            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }

            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };

            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };

            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };

            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs

            //You can increase as per your need.

            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }


            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;

                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens

                if (h > 0) sb.Append(words0[h] + "Hundred ");

                if (u > 0 || t > 0)
                {
                    if (h == 0)
                        sb.Append("");
                    else
                        if (h > 0 || i == 0) sb.Append("and ");


                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }

                if (i != 0) sb.Append(words3[i - 1]);

            }
            return sb.ToString().TrimEnd();
        }

        public List<IteamModels> FuncReturnFaultIteamList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT TOP 1000 i.[IteamCode],i.[IteamDesc],i.fStatus,c.CatagoryName,v.VenName,p.ProductDesc,m.ModelName,i.IteamSLNo,i.CostPrice ,i.ChalanNo,i.EntryDate,i.EntryBy " +
                               " FROM [dbo].[Tbl_Iteam] i, dbo.Tbl_IteamCatagory c ,dbo.Tbl_VendorInfo v, dbo.Tbl_IteamProduct p,dbo.Tbl_IteamModel m " +
                               " where substring(i.[IteamCode],1,2)=c.[CatagoryCode] and substring(i.[IteamCode],1,5)=p.[ProductCode] " +
                               " and substring(i.[IteamCode],1,11)=m.ModelCode and substring(i.[IteamCode],1,8)=v.VenCode and fStatus='Y' order by EntryDate desc";
            sqlConn.ConnectionString = conString;
            List<IteamModels> objIteamList = new List<IteamModels>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
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
                        objIteam.CatagoryName = sqlReader["CatagoryName"].ToString();
                        objIteam.ProductName = sqlReader["ProductDesc"].ToString();
                        objIteam.VendorName = sqlReader["VenName"].ToString();
                        objIteam.ModelName = sqlReader["ModelName"].ToString();
                        objIteam.Quantity = Convert.ToInt32(sqlReader["Quantity"]);
                        objIteam.CostPrice = Convert.ToDecimal(sqlReader["CostPrice"]);
                        objIteam.ChallanNo = sqlReader["ChalanNo"].ToString();
                        if (sqlReader["fStatus"].ToString() == "Y")
                            objIteam.fStatus = true;
                        else objIteam.fStatus = false;
                        objIteam.EntryDate = Convert.ToDateTime(sqlReader["EntryDate"]).ToString("yyyy-MM-dd");
                        objIteam.EntryBy = sqlReader["EntryBy"].ToString();
                        objIteamList.Add(objIteam);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objIteamList;
        }
    }
}