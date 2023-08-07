using System;
using System.Data.SqlClient;
using NeSHOP.Contacts;
using NeSHOP.Models;
using NESHOP.Contacts;
using NESHOP.Models;

namespace NeSHOP.DAL
{
    public class BllSuplier : IBllSuplier
    {
        //BllDbConnection objBllDbConnection=new BllDbConnection();
        private readonly IConfiguration _configuration;
        private readonly ILogger<BllSuplier> _logger;
        private readonly IBllDbConnection _dbConn;
        public BllSuplier(IConfiguration configuration, ILogger<BllSuplier> logger, IBllDbConnection dbConn)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
        }

        public string FuncIteamSuplierCode()
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "Select COUNT([SupplierCode]) as maxCount from [Tbl_Suplaier] ";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            int counter = 0;
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        counter = Convert.ToInt32(sqlReader["maxCount"]);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            string customerCode = Convert.ToString(counter + 1).PadLeft(3, '0');
            return customerCode;
        }

        public bool FuncSuplierEntry(DaoSuplier objDaoSuplier)
        {
           // BllDbConnection objBllDbConnection = new BllDbConnection();
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [Tbl_Suplaier] " +
                               " ([SupplierCode],[CompanyName],[SupplierContractPerson],[ContactPhone],[ContactEmail]" +
                               ",[CompanyAddress],[IsActive],[UserId]],[EntryDate])VALUES " +
                               " (@SupplierCode,@CompanyName,@SupplierContractPerson,@ContactPhone,@ContactEmail,@CompanyAddress,@IsActive,@UserId,@EntryDate)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("SupplierCode", Convert.ToString(objDaoSuplier.SupplierCode));
            sqlCmd.Parameters.AddWithValue("CompanyName", Convert.ToString(objDaoSuplier.CompanyName));
            sqlCmd.Parameters.AddWithValue("SupplierContractPerson", Convert.ToString(objDaoSuplier.SupplierContractPerson));
            sqlCmd.Parameters.AddWithValue("ContactPhone", Convert.ToString(objDaoSuplier.ContractPhone));
            sqlCmd.Parameters.AddWithValue("ContactEmail", Convert.ToString(objDaoSuplier.ContractEmail));
            sqlCmd.Parameters.AddWithValue("CompanyAddress", Convert.ToString(objDaoSuplier.CompanyAddress));
            sqlCmd.Parameters.AddWithValue("IsActive", Convert.ToBoolean(objDaoSuplier.IsActive));
            sqlCmd.Parameters.AddWithValue("UserId", Convert.ToString(objDaoSuplier.EntryBy));
            sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoSuplier.EntryDate));
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

        public bool FuncVendorEntry(VendorModels objDaoVendor, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [dbo].[Tbl_BrandInfo]([BrandCode],[BrandName],[SupplierCode]) " +
                               " VALUES (@BrandCode,@BrandName,@SupplierCode)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(objDaoVendor.BrandCode));
            sqlCmd.Parameters.AddWithValue("BrandName", Convert.ToString(objDaoVendor.BrandName));
            sqlCmd.Parameters.AddWithValue("SupplierCode", Convert.ToString(objDaoVendor.SupplierCode));
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


        public string FuncReturnVendorCode(string productCode)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "Select MAX(Convert(int,BrandCode)) as maxVen from dbo.Tbl_BrandInfo";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("productCode", Convert.ToString(productCode));
            String LastVenCode = "";
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        LastVenCode = Convert.ToString(Convert.ToInt32(sqlReader["maxVen"]) + 1).PadLeft(3, '0');
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

            return LastVenCode;
        }


        public VendorModels FuncReturnVendorInfo(string BrandCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "Select * from dbo.Tbl_BrandInfo where BrandCode=@BrandCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(BrandCode));
            var objDaoVendor = new VendorModels();
            try
            {
                sqlConn.Open();

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objDaoVendor.BrandCode = Convert.ToString(sqlReader["BrandCode"]);
                        objDaoVendor.BrandName = Convert.ToString(sqlReader["BrandName"]);
                        objDaoVendor.SupplierCode = Convert.ToString(sqlReader["SupplierCode"]);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDaoVendor;
        }

        public bool FuncVendorUpdate(VendorModels objDaoVendor, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "UPDATE [Tbl_BrandInfo] set [BrandName]=@BrandName,[SupplierCode]=@SupplierCode where [BrandCode]=@BrandCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("BrandCode", Convert.ToString(objDaoVendor.BrandCode));
            sqlCmd.Parameters.AddWithValue("BrandName", Convert.ToString(objDaoVendor.BrandName));
            sqlCmd.Parameters.AddWithValue("SupplierCode", Convert.ToString(objDaoVendor.SupplierCode));

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

        public bool FuncVendorDelete(VendorModels objDaoVendor)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "delete from [Tbl_BrandCodeInfo] where [BrandCodeCode]=@BrandCodeCode";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("BrandCodeCode", Convert.ToString(objDaoVendor.BrandCode));
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


    }
}
