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
    public class BllSettings : IBllSettings
    {
        //BllDbConnection objBllDbConnection = new BllDbConnection();

        private readonly IConfiguration _configuration;
        private readonly ILogger<BllSettings> _logger;
        private readonly IBllDbConnection _dbConn;
        public BllSettings(IConfiguration configuration, ILogger<BllSettings> logger, IBllDbConnection dbConn)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
        }

        //------------Catagory Info-------------------

        public string FuncReturnCatagoryCode()
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT MAX([CatagoryCode]) maxValue FROM [dbo].[Tbl_IteamCatagory]";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            String LastVenCode = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        LastVenCode = Convert.ToString(Convert.ToInt32(sqlReader["maxValue"]) + 1).PadLeft(2, '0');
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            if (LastVenCode == "")
                LastVenCode = Convert.ToString(1).PadLeft(2, '0');
            return LastVenCode;
        }

        public SelectList FuncReturnCatagoryList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [CatagoryCode],[CatagoryName] FROM [dbo].[Tbl_IteamCatagory] order by [CatagoryName]";

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
                        String TextValue = sqlReader["CatagoryName"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["CatagoryCode"].ToString() });
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

        public bool FuncCatagoryEntry(CatagoryModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [dbo].[Tbl_IteamCatagory]([CatagoryCode],[CatagoryName]) " +
                               " VALUES (@CatagoryCode,@CatagoryName)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(viewModel.CatagoryCode));
            sqlCmd.Parameters.AddWithValue("CatagoryName", Convert.ToString(viewModel.CatagoryName));
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

        public CatagoryModels FuncReturnCatagoryInfo(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "Select * from dbo.Tbl_IteamCatagory where CatagoryCode=@CatagoryCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(Code));
            var objDao = new CatagoryModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objDao.CatagoryCode = Convert.ToString(sqlReader["CatagoryCode"]);
                        objDao.CatagoryName = Convert.ToString(sqlReader["CatagoryName"]);

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDao;
        }

        public bool FuncCatagoryUpdate(CatagoryModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "UPDATE [Tbl_IteamCatagory] set [CatagoryName]=@CatagoryName where [CatagoryCode]=@CatagoryCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(viewModel.CatagoryCode));
            sqlCmd.Parameters.AddWithValue("CatagoryName", Convert.ToString(viewModel.CatagoryName));

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

        public bool FuncCatagoryDelete(CatagoryModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete from  [Tbl_IteamCatagory] where [CatagoryCode]=@CatagoryCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(viewModel.CatagoryCode));
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

        //-------------Product Info-------------------

        public string FuncReturnProductCode(string catagoryCode)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT MAX(substring(ProductCode,3,3)) maxValue FROM dbo.Tbl_IteamProduct where substring(ProductCode,1,2)=@CatagoryCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(catagoryCode));
            String LastVenCode = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        LastVenCode = Convert.ToString(Convert.ToInt32(sqlReader["maxValue"]) + 1).PadLeft(3, '0');
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            if (LastVenCode == "")
                LastVenCode = Convert.ToString(1).PadLeft(3, '0');

            return catagoryCode + LastVenCode;
        }

        public SelectList FuncReturnProductList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ProductCode],[ProductDesc],(select CatagoryName from Tbl_IteamCatagory where CatagoryCode=substring([ProductCode],1,2)) CatagoryName FROM [dbo].[Tbl_IteamProduct] order by ProductDesc";

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
                        String TextValue = sqlReader["ProductDesc"].ToString().PadRight(30, Convert.ToChar(160)) +
                                           sqlReader["CatagoryName"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["ProductCode"].ToString() });
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

        public bool FuncProductEntry(ProductModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [dbo].[Tbl_IteamProduct]([ProductCode],[ProductDesc],CatagoryCode)VALUES(@ProductCode,@ProductDesc,@CatagoryCode)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(viewModel.ProductCode));
            sqlCmd.Parameters.AddWithValue("ProductDesc", Convert.ToString(viewModel.ProductName));
            sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(viewModel.ProductCode.Substring(0, 2)));
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

        public ProductModels FuncReturnProductInfo(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT substring([ProductCode],1,2) CatagoryCode,[ProductCode],[ProductDesc] FROM [dbo].[Tbl_IteamProduct] where ProductCode=@ProductCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(Code));
            var objDao = new ProductModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objDao.CatagoryCode = Convert.ToString(sqlReader["CatagoryCode"]);
                        objDao.ProductCode = Convert.ToString(sqlReader["ProductCode"]);
                        objDao.ProductName = Convert.ToString(sqlReader["ProductDesc"]);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDao;
        }

        public bool FuncProductUpdate(ProductModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "UPDATE [dbo].[Tbl_IteamProduct] SET [ProductDesc] = @ProductDesc WHERE [ProductCode] = @ProductCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(viewModel.ProductCode));
            sqlCmd.Parameters.AddWithValue("ProductDesc", Convert.ToString(viewModel.ProductName));

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

        public bool FuncProductDelete(ProductModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete from  [Tbl_IteamProduct] where [ProductCode] = @ProductCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(viewModel.ProductCode));
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

        //-------------------------Product Serach for vendor catagory wise-----
        public SelectList FuncReturnSelectProductList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ProductCode],[ProductDesc] FROM [dbo].[Tbl_IteamProduct] where substring([ProductCode],1,2)=@CatagoryCode order by ProductDesc";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(Code));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        //String TextValue = sqlReader["ProductDesc"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["ProductDesc"].ToString(), Value = sqlReader["ProductCode"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            finally { sqlConn.Close(); }

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", "000");

            return objSelectList;
        }

        //-------------------------Model Serach for vendor catagory wise-----
        public SelectList FuncReturnSelectModelList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ModelCode],[ModelName],[ModelDesc] FROM [dbo].[Tbl_IteamModel] where substring([ModelCode],1,3)=@BrandCode order by ModelName";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(Code));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        //String TextValue = sqlReader["ProductDesc"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["ModelName"].ToString(), Value = sqlReader["ModelCode"].ToString() });
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

        public SelectList FuncReturnModelList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ModelCode],[ModelName]+'( '+[ModelBName]+' )' as ModelName,(select BrandName from dbo.Tbl_BrandInfo where BrandCode=substring([ModelCode],1,3)) as BrandName" +
                               ",ModelPrice FROM [dbo].[Tbl_IteamModel] order by BrandName,ModelName";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(Code));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        String TextValue = sqlReader["ModelName"].ToString().PadRight(30, Convert.ToChar(160)) +
                                           sqlReader["BrandName"].ToString().PadRight(30, Convert.ToChar(160)) + "-" + sqlReader["ModelPrice"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["ModelCode"].ToString() });
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

        //-------------------Model Data-------------------------------------

        public string FuncReturnModelCode(string BrandCode)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT MAX(substring(ModelCode,4,3)) maxValue FROM dbo.[Tbl_IteamModel] where substring(ModelCode,1,3)=@BrandCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(BrandCode));
            String LastVenCode = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        LastVenCode = Convert.ToString(Convert.ToInt32(sqlReader["maxValue"]) + 1).PadLeft(3, '0');
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            if (LastVenCode == "")
                LastVenCode = Convert.ToString(1).PadLeft(3, '0');

            return BrandCode + LastVenCode;
        }

        public bool FuncModelEntry(IteamModelModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [dbo].[Tbl_IteamModel]([ModelCode],[ModelName],ModelBName,[ModelDesc],BrandCode,ModelPrice) VALUES(@ModelCode,@ModelName,@ModelBName,@ModelDesc,@BrandCode,@ModelPrice)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
            sqlCmd.Parameters.AddWithValue("ModelName", Convert.ToString(viewModel.ModelName));
            sqlCmd.Parameters.AddWithValue("ModelBName", Convert.ToString(viewModel.ModelBName));
            sqlCmd.Parameters.AddWithValue("ModelDesc", Convert.ToString(viewModel.ModelDesc));
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(viewModel.ModelCode.Substring(0, 3)));
            if (viewModel.ModelPrice == null) viewModel.ModelPrice = 0;

            sqlCmd.Parameters.AddWithValue("ModelPrice", Convert.ToDecimal(viewModel.ModelPrice));

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

        public IteamModelModels FuncReturnModelInfo(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "select BrandCode,ModelCode,[ModelName],[ModelBName],[ModelDesc],ModelPrice FROM [dbo].[Tbl_IteamModel] where ModelCode=@ModelCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(Code));
            var objDao = new IteamModelModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objDao.BrandCode = Convert.ToString(sqlReader["BrandCode"]);
                        objDao.ModelCode = Convert.ToString(sqlReader["ModelCode"]);
                        objDao.ModelName = Convert.ToString(sqlReader["ModelName"]);
                        objDao.ModelBName = Convert.ToString(sqlReader["ModelBName"]);
                        objDao.ModelDesc = Convert.ToString(sqlReader["ModelDesc"]);
                        objDao.ModelPrice = Convert.ToDecimal(sqlReader["ModelPrice"]);

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDao;
        }

        public bool FuncModelUpdate(IteamModelModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "UPDATE [dbo].[Tbl_IteamModel] SET [ModelName] = @ModelName,ModelBName=@ModelBName,[ModelDesc] = @ModelDesc,ModelPrice=@ModelPrice WHERE [ModelCode] = @ModelCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
            sqlCmd.Parameters.AddWithValue("ModelName", Convert.ToString(viewModel.ModelName));
            sqlCmd.Parameters.AddWithValue("ModelBName", Convert.ToString(viewModel.ModelBName));
            sqlCmd.Parameters.AddWithValue("ModelDesc", Convert.ToString(viewModel.ModelDesc));
            sqlCmd.Parameters.AddWithValue("ModelPrice", Convert.ToDecimal(viewModel.ModelPrice));


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

        public bool FuncModelDelete(IteamModelModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete from  [Tbl_IteamModel] where [ModelCode] = @ModelCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
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

        //----------------Customerinfo-------------------------

        public SelectList FuncReturnCustomerList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [CustomerCode],[CompanyName],[ContactPerson],[CompanyAddress],[ContactTPhone],[ContactCPhone],[ContactEmail] FROM [dbo].[Tbl_CustomerInfo] order by CompanyName";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(Code));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        String TextValue = sqlReader["CompanyName"].ToString();// +"-" + sqlReader["CompanyAddress"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["CustomerCode"].ToString() });
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

        public bool FuncCustomerEntry(CustomerModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [dbo].[Tbl_CustomerInfo]([CustomerCode],[CompanyName],[ContactPerson],[CompanyAddress],[ContactTPhone] ,[ContactCPhone],[ContactEmail]) " +
                               " VALUES(@CustomerCode,@CompanyName,@ContactPerson,@CompanyAddress,@ContactTPhone,@ContactCPhone,@ContactEmail)";

            sqlConn.ConnectionString = conString;
            if (viewModel.ContactTPhone == null) viewModel.ContactTPhone = "";
            if (viewModel.ContactCPhone == null) viewModel.ContactCPhone = "";
            if (viewModel.ContactEmail == null) viewModel.ContactEmail = "";
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
            sqlCmd.Parameters.AddWithValue("CompanyName", Convert.ToString(viewModel.CompanyName));
            sqlCmd.Parameters.AddWithValue("ContactPerson", Convert.ToString(viewModel.ContactPerson));
            sqlCmd.Parameters.AddWithValue("CompanyAddress", Convert.ToString(viewModel.CompanyAddress));
            sqlCmd.Parameters.AddWithValue("ContactTPhone", Convert.ToString(viewModel.ContactTPhone));
            sqlCmd.Parameters.AddWithValue("ContactCPhone", Convert.ToString(viewModel.ContactCPhone));
            sqlCmd.Parameters.AddWithValue("ContactEmail", Convert.ToString(viewModel.ContactEmail));
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

        public CustomerModels FuncReturnCustomerInfo(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [CustomerCode],[CompanyName],[ContactPerson],[CompanyAddress] ,[ContactTPhone] " +
                               ",[ContactCPhone] ,[ContactEmail] FROM [dbo].[Tbl_CustomerInfo] where CustomerCode=@CustomerCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(Code));
            var objDao = new CustomerModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objDao.CustomerCode = Convert.ToString(sqlReader["CustomerCode"]);
                        objDao.CompanyName = Convert.ToString(sqlReader["CompanyName"]);
                        objDao.ContactPerson = Convert.ToString(sqlReader["ContactPerson"]);
                        objDao.CompanyAddress = Convert.ToString(sqlReader["CompanyAddress"]);
                        objDao.ContactTPhone = Convert.ToString(sqlReader["ContactTPhone"]);
                        objDao.ContactCPhone = Convert.ToString(sqlReader["ContactCPhone"]);
                        objDao.ContactEmail = Convert.ToString(sqlReader["ContactEmail"]);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDao;
        }

        public bool FuncCustomerUpdate(CustomerModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "UPDATE [dbo].[Tbl_CustomerInfo] SET [CompanyName] = @CompanyName,[ContactPerson] = @ContactPerson,[CompanyAddress] = @CompanyAddress," +
                "[ContactTPhone] = @ContactTPhone,[ContactCPhone] = @ContactCPhone,[ContactEmail] = @ContactEmail WHERE [CustomerCode] = @CustomerCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
            sqlCmd.Parameters.AddWithValue("CompanyName", Convert.ToString(viewModel.CompanyName));
            sqlCmd.Parameters.AddWithValue("ContactPerson", Convert.ToString(viewModel.ContactPerson));
            sqlCmd.Parameters.AddWithValue("CompanyAddress", Convert.ToString(viewModel.CompanyAddress));
            sqlCmd.Parameters.AddWithValue("ContactTPhone", Convert.ToString(viewModel.ContactTPhone));
            sqlCmd.Parameters.AddWithValue("ContactCPhone", Convert.ToString(viewModel.ContactCPhone));
            sqlCmd.Parameters.AddWithValue("ContactEmail", Convert.ToString(viewModel.ContactEmail));

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

        public bool FuncCustomerDelete(CustomerModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete from  [Tbl_CustomerInfo] where [CustomerCode] = @CustomerCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CustomerCode", Convert.ToString(viewModel.CustomerCode));
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

        public string FuncReturnCustomerCode()
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT MAX(substring(CustomerCode,5,4)) maxValue FROM dbo.Tbl_CustomerInfo where substring(CustomerCode,1,4)=@MonthYear";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("MonthYear", DateTime.Now.ToString("yyMM"));
            String LastVenCode = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        LastVenCode = Convert.ToString(Convert.ToInt32(sqlReader["maxValue"]) + 1).PadLeft(4, '0');
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            if (LastVenCode == "")
                LastVenCode = Convert.ToString(1).PadLeft(4, '0');

            return DateTime.Now.ToString("yyMM") + LastVenCode;
        }

        //--------------------------------------------------------------
        public List<IteamModels> FuncReturnSearchIteamList(IteamModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "";
            if (viewModel.SearchType == "1")
            {

                SqlString =
                    "SELECT TOP 1000 i.[IteamCode],i.[IteamDesc],v.BrandName,m.ModelName,i.Quantity,i.CostPrice ,i.ChalanNo,i.EntryDate,i.EntryBy " +
                    " FROM [dbo].[Tbl_Iteam] i, dbo.Tbl_BrandInfo v, dbo.Tbl_IteamModel m  where substring(i.[IteamCode],1,6)=m.ModelCode and substring(i.[IteamCode],1,3)=v.BrandCode " +
                    " and (@ChalanNo='' OR i.[ChalanNo]=@ChalanNo) and (@ModelCode='' OR substring(i.[IteamCode],1,6)=@ModelCode) and (@BrandCode='' OR substring(i.[IteamCode],1,3)=@BrandCode) " +
                    " and convert(int,i.EntryDate) >=convert(int,@TranDateFrom) and convert(int,i.EntryDate) <=convert(int,@TranDateTo) order by EntryDate desc,m.ModelName asc";
            }
            else
            {
                SqlString =
                    "SELECT TOP 1000 i.ModelCode IteamCode,'' IteamDesc,v.BrandName,m.ModelName,i.Quantity,i.UnitPrice CostPrice ,i.InvoiceNo ChalanNo,i.EntryDate,i.EntryBy " +
                    " FROM [dbo].Tbl_Invoice i, dbo.Tbl_BrandInfo v, dbo.Tbl_IteamModel m  where substring(i.ModelCode,1,6)=m.ModelCode and substring(i.ModelCode,1,3)=v.BrandCode " +
                    " and (@ChalanNo='' OR i.[ChalanNo]=@ChalanNo) and (@ModelCode='' OR substring(i.ModelCode,1,6)=@ModelCode) and (@BrandCode='' OR substring(i.ModelCode,1,3)=@BrandCode)" +
                    " and convert(int,i.EntryDate) >=convert(int,@TranDateFrom) and convert(int,i.EntryDate) <=convert(int,@TranDateTo) order by EntryDate desc,m.ModelName asc";
            }

            sqlConn.ConnectionString = conString;
            List<IteamModels> objIteamList = new List<IteamModels>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (viewModel.ChallanNo == null) viewModel.ChallanNo = "";
            //if (viewModel.IteamSLNo == null) viewModel.IteamSLNo = "";
            if (viewModel.ModelCode == null) viewModel.ModelCode = "";
            if (viewModel.BrandCode == null) viewModel.BrandCode = "";
            //if (viewModel.ProductCode == null) viewModel.ProductCode = "";
            //if (viewModel.CatagoryCode == null) viewModel.CatagoryCode = "";


            sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(viewModel.ChallanNo));
            //sqlCmd.Parameters.AddWithValue("IteamSLNo", Convert.ToString(viewModel.IteamSLNo));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(viewModel.BrandCode));
            //sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(viewModel.ProductCode));
            //sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(viewModel.CatagoryCode));
            //sqlCmd.Parameters.AddWithValue("TranType", Convert.ToString("1"));
            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));

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
                        objIteam.ModelName = sqlReader["ModelName"].ToString();
                        objIteam.Quantity = Convert.ToInt32(sqlReader["Quantity"].ToString());
                        objIteam.CostPrice = Convert.ToDecimal(sqlReader["CostPrice"]);
                        objIteam.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objIteam.EntryDate = Convert.ToDateTime(sqlReader["EntryDate"]).ToString("dd/MM/yyyy");
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

        public List<IteamModels> FuncReturnSearchStockList(IteamModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "";
            if (viewModel.SearchType == "1")
            {
                SqlString = "SELECT v.BrandName,m.ModelName,sum(i.Quantity) as TotalStock" +
                               " FROM [dbo].[Tbl_Iteam] i, dbo.Tbl_BrandInfo v, dbo.Tbl_IteamModel m " +
                               " where substring(i.[IteamCode],1,6)=m.ModelCode and substring(i.[IteamCode],1,3)=v.BrandCode " +
                               " and (@ModelCode='' OR substring(i.[IteamCode],1,6)=@ModelCode) and (@BrandCode='' OR substring(i.[IteamCode],1,3)=@BrandCode) " +
                               " and convert(int,i.EntryDate) >=convert(int,@TranDateFrom) and convert(int,i.EntryDate) <=convert(int,@TranDateTo) " +
                               " group by v.BrandName,m.ModelName order by m.ModelName";

            }
            else if (viewModel.SearchType == "2")
            {
                SqlString = "SELECT v.BrandName,m.ModelName,sum(i.Quantity) as TotalStock" +
                              " FROM [dbo].[Tbl_Invoice] i, dbo.Tbl_BrandInfo v, dbo.Tbl_IteamModel m " +
                              " where substring(i.[ModelCode],1,6)=m.ModelCode and substring(i.[ModelCode],1,3)=v.BrandCode " +
                              " and (@ModelCode='' OR substring(i.[ModelCode],1,6)=@ModelCode) and (@BrandCode='' OR substring(i.[ModelCode],1,3)=@BrandCode) " +
                              " and convert(int,i.EntryDate) >=convert(int,@TranDateFrom) and convert(int,i.EntryDate) <=convert(int,@TranDateTo) " +
                              " group by v.BrandName,m.ModelName order by m.ModelName";
            }

            else if (viewModel.SearchType == "3")
            {
                SqlString = "SELECT v.BrandName,m.ModelName,sum(TotalQuantity) as TotalStock" +
                              " FROM [dbo].[Tbl_Inventory] i, dbo.Tbl_BrandInfo v, dbo.Tbl_IteamModel m " +
                              " where substring(i.[ModelCode],1,6)=m.ModelCode and substring(i.[ModelCode],1,3)=v.BrandCode " +
                              " and (@ModelCode='' OR substring(i.[ModelCode],1,6)=@ModelCode) and (@BrandCode='' OR substring(i.[ModelCode],1,3)=@BrandCode) " +
                              " and convert(int,@TranDateFrom) <=convert(int,@TranDateTo) " +
                              " group by v.BrandName,m.ModelName order by m.ModelName";
            }
            sqlConn.ConnectionString = conString;
            List<IteamModels> objIteamList = new List<IteamModels>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //if (viewModel.ChallanNo == null) viewModel.ChallanNo = "";
            //if (viewModel.IteamSLNo == null) viewModel.IteamSLNo = "";
            if (viewModel.ModelCode == null) viewModel.ModelCode = "";
            if (viewModel.BrandCode == null) viewModel.BrandCode = "";
            //if (viewModel.ProductCode == null) viewModel.ProductCode = "";
            //if (viewModel.CatagoryCode == null) viewModel.CatagoryCode = "";
            //if (viewModel.SearchType == "1") viewModel.TranType = "";
            //else if (viewModel.SearchType == "2") viewModel.TranType = "2";
            //else viewModel.TranType = "1";

            //sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(viewModel.ChallanNo));
            //sqlCmd.Parameters.AddWithValue("IteamSLNo", Convert.ToString(viewModel.IteamSLNo));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(viewModel.BrandCode));
            //sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(viewModel.ProductCode));
            //sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(viewModel.CatagoryCode));
            //sqlCmd.Parameters.AddWithValue("TranType", Convert.ToString(""));
            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    IteamModels objIteam = new IteamModels();
                    int count = 1;
                    while (sqlReader.Read())
                    {
                        objIteam = new IteamModels();
                        objIteam.SLNo = count.ToString();
                        //objIteam.CatagoryName = sqlReader["CatagoryName"].ToString();
                        //objIteam.ProductName = sqlReader["ProductDesc"].ToString();
                        objIteam.VendorName = sqlReader["BrandName"].ToString();
                        objIteam.ModelName = sqlReader["ModelName"].ToString();
                        //objIteam.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objIteam.TotalStock = Convert.ToInt32(sqlReader["TotalStock"]);
                        objIteamList.Add(objIteam);
                        count++;
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

        public List<IteamModels> FuncReturnSearchStockReport(IteamModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "";
            if (viewModel.SearchType == "1")
            {
                SqlString = "SELECT v.BrandName,m.ModelName,sum(i.Quantity) as TotalStock" +
                               " FROM [dbo].[Tbl_Iteam] i, dbo.Tbl_BrandInfo v, dbo.Tbl_IteamModel m " +
                               " where substring(i.[IteamCode],1,6)=m.ModelCode and substring(i.[IteamCode],1,3)=v.BrandCode " +
                               " and (@ModelCode='' OR substring(i.[IteamCode],1,6)=@ModelCode) and (@BrandCode='' OR substring(i.[IteamCode],1,3)=@BrandCode) " +
                               " and convert(int,i.EntryDate) >=convert(int,@TranDateFrom) and convert(int,i.EntryDate) <=convert(int,@TranDateTo) " +
                               " group by v.BrandName,m.ModelName order by m.ModelName";

            }
            else if (viewModel.SearchType == "2")
            {
                SqlString = "SELECT v.BrandName,m.ModelName,sum(i.Quantity) as TotalStock" +
                              " FROM [dbo].[Tbl_Invoice] i, dbo.Tbl_BrandInfo v, dbo.Tbl_IteamModel m " +
                              " where substring(i.[ModelCode],1,6)=m.ModelCode and substring(i.[ModelCode],1,3)=v.BrandCode " +
                              " and (@ModelCode='' OR substring(i.[ModelCode],1,6)=@ModelCode) and (@BrandCode='' OR substring(i.[ModelCode],1,3)=@BrandCode) " +
                              " and convert(int,i.EntryDate) >=convert(int,@TranDateFrom) and convert(int,i.EntryDate) <=convert(int,@TranDateTo) " +
                              " group by v.BrandName,m.ModelName order by m.ModelName";
            }

            else if (viewModel.SearchType == "3")
            {
                SqlString = "SELECT v.BrandName,m.ModelName,sum(TotalQuantity) as TotalStock" +
                              " FROM [dbo].[Tbl_Inventory] i, dbo.Tbl_BrandInfo v, dbo.Tbl_IteamModel m " +
                              " where substring(i.[ModelCode],1,6)=m.ModelCode and substring(i.[ModelCode],1,3)=v.BrandCode " +
                              " and (@ModelCode='' OR substring(i.[ModelCode],1,6)=@ModelCode) and (@BrandCode='' OR substring(i.[ModelCode],1,3)=@BrandCode) " +
                              " and convert(int,@TranDateFrom) <=convert(int,@TranDateTo) " +
                              " group by v.BrandName,m.ModelName order by m.ModelName";
            }
            sqlConn.ConnectionString = conString;
            List<IteamModels> objIteamList = new List<IteamModels>();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //if (viewModel.ChallanNo == null) viewModel.ChallanNo = "";
            //if (viewModel.IteamSLNo == null) viewModel.IteamSLNo = "";
            if (viewModel.ModelCode == null) viewModel.ModelCode = "";
            if (viewModel.BrandCode == null) viewModel.BrandCode = "";
            //if (viewModel.ProductCode == null) viewModel.ProductCode = "";
            //if (viewModel.CatagoryCode == null) viewModel.CatagoryCode = "";
            //if (viewModel.SearchType == "1") viewModel.TranType = "";
            //else if (viewModel.SearchType == "2") viewModel.TranType = "2";
            //else viewModel.TranType = "1";

            //sqlCmd.Parameters.AddWithValue("ChalanNo", Convert.ToString(viewModel.ChallanNo));
            //sqlCmd.Parameters.AddWithValue("IteamSLNo", Convert.ToString(viewModel.IteamSLNo));
            sqlCmd.Parameters.AddWithValue("ModelCode", Convert.ToString(viewModel.ModelCode));
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(viewModel.BrandCode));
            //sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(viewModel.ProductCode));
            //sqlCmd.Parameters.AddWithValue("CatagoryCode", Convert.ToString(viewModel.CatagoryCode));
            //sqlCmd.Parameters.AddWithValue("TranType", Convert.ToString(""));
            sqlCmd.Parameters.AddWithValue("TranDateFrom", Convert.ToDateTime(viewModel.TranDateFrom));
            sqlCmd.Parameters.AddWithValue("TranDateTo", Convert.ToDateTime(viewModel.TranDateTo));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    IteamModels objIteam = new IteamModels();
                    int count = 1;
                    while (sqlReader.Read())
                    {
                        objIteam = new IteamModels();
                        objIteam.SLNo = count.ToString();
                        //objIteam.CatagoryName = sqlReader["CatagoryName"].ToString();
                        //objIteam.ProductName = sqlReader["ProductDesc"].ToString();
                        objIteam.VendorName = sqlReader["BrandName"].ToString();
                        objIteam.ModelName = sqlReader["ModelName"].ToString();
                        //objIteam.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objIteam.TotalStock = Convert.ToInt32(sqlReader["TotalStock"]);
                        objIteamList.Add(objIteam);
                        count++;
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

        public List<IteamModels> FuncReturnSearchFultStockReport(IteamModels viewModel)
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
                            objIteam.FaultStatus = "Yes";
                        else objIteam.FaultStatus = "No";
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

        public SelectList FuncReturnTarmsList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [TarmsCode],[TarmsName] FROM [dbo].[Tbl_TarmsCondition]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(Code));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        String TextValue = sqlReader["TarmsCode"].ToString() + " .  " + sqlReader["TarmsName"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["TarmsCode"].ToString() });
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

        public SelectList FuncReturnPartyList(string PType)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "";
            if (PType == "")
                SqlString = "SELECT [CustomerCode] code,[CompanyName]+', '+[ContactPerson] name FROM [dbo].[Tbl_CustomerInfo]  order by [CompanyName]" +
                               " union SELECT [SupplierCode] code,([CompanyName]+'-, '+[ContactPerson]) name FROM [dbo].[Tbl_SupplierInfo] order by [CompanyName] ;";

            else if (PType == "V")
                SqlString = "SELECT [SupplierCode] code,([CompanyName]+'-, '+[ContactPerson]) name FROM [dbo].[Tbl_SupplierInfo] order by [CompanyName] ;";

            else if (PType == "C")
                SqlString = "SELECT [CustomerCode] code,[CompanyName]+', '+[ContactPerson] name FROM [dbo].[Tbl_CustomerInfo] order by [CompanyName];";

            else
                SqlString = "SELECT [CustomerCode] code,[CompanyName]+', '+[ContactPerson] name FROM [dbo].[Tbl_CustomerInfo] " +
                               " union SELECT [SupplierCode] code,([CompanyName]+'-, '+[ContactPerson]) name FROM [dbo].[Tbl_SupplierInfo] order by [CompanyName];";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("PType", Convert.ToString(Code));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {

                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["name"].ToString(), Value = sqlReader["code"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", PType);

            return objSelectList;
        }

        public string FuncReturnTarmsCode()
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT MAX([TarmsCode]) maxValue FROM [dbo].[Tbl_TarmsCondition]";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            String LastVenCode = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        LastVenCode = Convert.ToString(Convert.ToInt32(sqlReader["maxValue"]) + 1);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            if (LastVenCode == "")
                LastVenCode = Convert.ToString(1);
            return LastVenCode;
        }

        public bool FuncTarmsEntry(TarmsModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [dbo].[Tbl_TarmsCondition]([TarmsCode],[TarmsName],EntryDate) " +
                               " VALUES (@TarmsCode,@TarmsName,@EntryDate)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TarmsCode", Convert.ToString(viewModel.TarmsCode));
            sqlCmd.Parameters.AddWithValue("TarmsName", Convert.ToString(viewModel.TarmsName));
            sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.BusinessDate));

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

        public TarmsModels FuncReturnTarmsInfo(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [TarmsCode],TarmsName FROM [dbo].[Tbl_TarmsCondition] where TarmsCode=@TarmsCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TarmsCode", Convert.ToString(Code));
            TarmsModels objDao = new TarmsModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objDao.TarmsCode = Convert.ToString(sqlReader["TarmsCode"]);
                        objDao.TarmsName = Convert.ToString(sqlReader["TarmsName"]);

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDao;
        }

        public bool FuncTarmsUpdate(TarmsModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "UPDATE [Tbl_TarmsCondition] set [TarmsName]=@TarmsName where [TarmsCode]=@TarmsCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TarmsCode", Convert.ToString(viewModel.TarmsCode));
            sqlCmd.Parameters.AddWithValue("TarmsName", Convert.ToString(viewModel.TarmsName));

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

        public bool FuncTarmsDelete(TarmsModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete from  [Tbl_TarmsCondition] where [TarmsCode]=@TarmsCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("TarmsCode", Convert.ToString(viewModel.TarmsCode));
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


        //----------------Supplier Info-------------------------

        public SelectList FuncReturnSupplierList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [SupplierCode],[CompanyName],[ContactPerson],[CompanyAddress],[ContactTPhone],[ContactCPhone],[ContactEmail] FROM [dbo].[Tbl_SupplierInfo]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(Code));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        String TextValue = sqlReader["CompanyName"].ToString();// +"-" + sqlReader["CompanyAddress"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["SupplierCode"].ToString() });
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

        public bool FuncSupplierEntry(SupplierModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [dbo].[Tbl_SupplierInfo]([SupplierCode],[CompanyName],[ContactPerson],[CompanyAddress],[ContactTPhone] ,[ContactCPhone],[ContactEmail]) " +
                               " VALUES(@SupplierCode,@CompanyName,@ContactPerson,@CompanyAddress,@ContactTPhone,@ContactCPhone,@ContactEmail)";

            sqlConn.ConnectionString = conString;
            if (viewModel.ContactTPhone == null) viewModel.ContactTPhone = "";
            if (viewModel.ContactCPhone == null) viewModel.ContactCPhone = "";
            if (viewModel.ContactEmail == null) viewModel.ContactEmail = "";
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("SupplierCode", Convert.ToString(viewModel.SupplierCode));
            sqlCmd.Parameters.AddWithValue("CompanyName", Convert.ToString(viewModel.CompanyName));
            sqlCmd.Parameters.AddWithValue("ContactPerson", Convert.ToString(viewModel.ContactPerson));
            sqlCmd.Parameters.AddWithValue("CompanyAddress", Convert.ToString(viewModel.CompanyAddress));
            sqlCmd.Parameters.AddWithValue("ContactTPhone", Convert.ToString(viewModel.ContactTPhone));
            sqlCmd.Parameters.AddWithValue("ContactCPhone", Convert.ToString(viewModel.ContactCPhone));
            sqlCmd.Parameters.AddWithValue("ContactEmail", Convert.ToString(viewModel.ContactEmail));
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

        public SupplierModels FuncReturnSupplierInfo(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [SupplierCode],[CompanyName],[ContactPerson],[CompanyAddress] ,[ContactTPhone] " +
                               ",[ContactCPhone] ,[ContactEmail] FROM [dbo].[Tbl_SupplierInfo] where SupplierCode=@SupplierCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("SupplierCode", Convert.ToString(Code));
            var objDao = new SupplierModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objDao.SupplierCode = Convert.ToString(sqlReader["SupplierCode"]);
                        objDao.CompanyName = Convert.ToString(sqlReader["CompanyName"]);
                        objDao.ContactPerson = Convert.ToString(sqlReader["ContactPerson"]);
                        objDao.CompanyAddress = Convert.ToString(sqlReader["CompanyAddress"]);
                        objDao.ContactTPhone = Convert.ToString(sqlReader["ContactTPhone"]);
                        objDao.ContactCPhone = Convert.ToString(sqlReader["ContactCPhone"]);
                        objDao.ContactEmail = Convert.ToString(sqlReader["ContactEmail"]);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDao;
        }

        public bool FuncSupplierUpdate(SupplierModels viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "UPDATE [dbo].[Tbl_SupplierInfo] SET [CompanyName] = @CompanyName,[ContactPerson] = @ContactPerson,[CompanyAddress] = @CompanyAddress," +
                "[ContactTPhone] = @ContactTPhone,[ContactCPhone] = @ContactCPhone,[ContactEmail] = @ContactEmail WHERE [SupplierCode] = @SupplierCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (viewModel.ContactTPhone == null) viewModel.ContactTPhone = "";
            if (viewModel.ContactCPhone == null) viewModel.ContactCPhone = "";
            if (viewModel.ContactEmail == null) viewModel.ContactEmail = "";
            sqlCmd.Parameters.AddWithValue("SupplierCode", Convert.ToString(viewModel.SupplierCode));
            sqlCmd.Parameters.AddWithValue("CompanyName", Convert.ToString(viewModel.CompanyName));
            sqlCmd.Parameters.AddWithValue("ContactPerson", Convert.ToString(viewModel.ContactPerson));
            sqlCmd.Parameters.AddWithValue("CompanyAddress", Convert.ToString(viewModel.CompanyAddress));
            sqlCmd.Parameters.AddWithValue("ContactTPhone", Convert.ToString(viewModel.ContactTPhone));
            sqlCmd.Parameters.AddWithValue("ContactCPhone", Convert.ToString(viewModel.ContactCPhone));
            sqlCmd.Parameters.AddWithValue("ContactEmail", Convert.ToString(viewModel.ContactEmail));

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

        public bool FuncSupplierDelete(SupplierModels viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "delete from  [Tbl_SupplierInfo] where [SupplierCode] = @SupplierCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("SupplierCode", Convert.ToString(viewModel.SupplierCode));
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

        public string FuncReturnSupplierCode()
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT MAX(substring(SupplierCode,3,4)) maxValue FROM dbo.Tbl_SupplierInfo where substring(SupplierCode,1,2)=@MonthYear";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("MonthYear", DateTime.Now.ToString("yy"));
            String LastVenCode = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        LastVenCode = Convert.ToString(Convert.ToInt32(sqlReader["maxValue"]) + 1).PadLeft(4, '0');
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            if (LastVenCode == "")
                LastVenCode = Convert.ToString(1).PadLeft(4, '0');

            return DateTime.Now.ToString("yy") + LastVenCode;
        }

        //--------------------------------------------------------------

        public SelectList FuncReturnBankList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [BankCode],[BankName] ,[BankAddress] FROM [dbo].[Tbl_BankIno]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("ProductCode", Convert.ToString(Code));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        String TextValue = sqlReader["BankName"].ToString() + "-" + sqlReader["BankAddress"].ToString();

                        objSelectListItem.Add(new SelectListItem { Text = TextValue, Value = sqlReader["BankCode"].ToString() });
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

        public List<IteamModels> FuncReturnReplaceIteamList()
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT r.[InvoiceNo],r.[ChalanNo],(select m.ModelName from dbo.Tbl_IteamModel m where m.[ModelCode]=r.[ModelCode]) ModelName " +
                               " ,r.[OriIteamSlNo],r.[DupIteamSlNo],r.[EntryBy],r.[EntryDate] FROM [dbo].[Tbl_Chalan_Return] r order by r.[EntryBy] desc,ModelName";
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
                        objIteam.InvoiceNo = sqlReader["InvoiceNo"].ToString();
                        objIteam.ChallanNo = sqlReader["ChalanNo"].ToString();
                        objIteam.ModelName = sqlReader["ModelName"].ToString();
                        objIteam.Quantity = Convert.ToInt32(sqlReader["Quantity"]);
                        objIteam.EntryBy = sqlReader["EntryBy"].ToString();
                        objIteam.EntryDate = Convert.ToDateTime(sqlReader["EntryDate"]).ToString("dd/MM/yyyy");
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