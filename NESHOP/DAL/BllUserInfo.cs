using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Contacts;

namespace NeSHOP.DAL
{
    public class BllUserInfo : IBllUserInfo
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<BllUserInfo> _logger;
        private readonly IBllDbConnection _dbConn;
        public BllUserInfo(IConfiguration configuration, ILogger<BllUserInfo> logger, IBllDbConnection dbConn)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
        }

        public DaoUserInfo FunValidateUser(DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            var daoUserInfo = new DaoUserInfo();

            SqlConnection sqlConn = new SqlConnection();

            var SqlString = "SELECT u.[UserId],u.[IsActive],d.BusinessDate,e.INSTCODE,e.FullName,e.EmpPhone " +
                               " FROM [Tbl_UserInfo] u, [Tbl_DayEnd] d, [Tbl_Employee] e " +
                               " where u.UserId=@User_ID AND u.UserPassword=@User_Password " +
                               " and u.EmpId=e.EMPID and d.InstCode=e.INSTCODE and d.SysStatus='O';";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("User_ID", Convert.ToString(objDaoUserInfo.UserId));
            sqlCmd.Parameters.AddWithValue("User_Password", Convert.ToString(objDaoUserInfo.Password));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        daoUserInfo.UserId = Convert.ToString(sqlReader["UserId"]);
                        daoUserInfo.IsActive = Convert.ToBoolean(sqlReader["IsActive"]);
                        daoUserInfo.BusinessDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        daoUserInfo.InstCode = Convert.ToString(sqlReader["INSTCODE"]);
                        daoUserInfo.FullName = Convert.ToString(sqlReader["FullName"]);
                        daoUserInfo.EmpPhone = Convert.ToString(sqlReader["EmpPhone"]);
                        daoUserInfo.EmpPhoto = "avatar-male.png";
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Exception Error: "+ e.Message);
            }
            finally
            {
                sqlConn.Close();
            }
            

            return daoUserInfo;
        }

        public bool FunSaveUserInfo(Models.RegisterModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " INSERT INTO [TBL_USERINFO]([USERID],[USERPASSWORD],[EMPID],[ENTRYBY],[ENTRYDATE],[ISACTIVE]) " +
                               " VALUES(@USERID,@USERPASSWORD,@EMPID,@ENTRYBY,@ENTRYDATE,@ISACTIVE)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("USERID", Convert.ToString(viewModel.UserId));
            sqlCmd.Parameters.AddWithValue("USERPASSWORD", Convert.ToString(viewModel.UserPassword));
            sqlCmd.Parameters.AddWithValue("EMPID", Convert.ToString(viewModel.EmpId));
            sqlCmd.Parameters.AddWithValue("ENTRYBY", Convert.ToString(objDaoUserInfo.UserId));
            sqlCmd.Parameters.AddWithValue("ENTRYDATE", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            sqlCmd.Parameters.AddWithValue("ISACTIVE", Convert.ToBoolean(viewModel.IsActive));
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

        public RegisterModel FunSearchUserInfo(string UserId)
        {
            String conString = _dbConn.FunReturnConString();
            RegisterModel objRegisterModel = new RegisterModel();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [USERID],[USERPASSWORD],[EMPID],[ISACTIVE] FROM [TBL_USERINFO] WHERE [USERID]=@USERID";
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("USERID", Convert.ToString(UserId));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    while (sqlReader.Read())
                    {
                        objRegisterModel.UserId = Convert.ToString(sqlReader["USERID"]);
                        objRegisterModel.UserPassword = Convert.ToString(sqlReader["USERPASSWORD"]);
                        objRegisterModel.EmpId = Convert.ToString(sqlReader["EMPID"]);
                        objRegisterModel.IsActive = Convert.ToBoolean(sqlReader["ISACTIVE"]);
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objRegisterModel;
        }

        public bool FunUpdateUserInfo(RegisterModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " UPDATE [TBL_USERINFO] SET [EMPID]=@EMPID,[ISACTIVE]=@ISACTIVE WHERE [USERID]=@USERID ";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("USERID", Convert.ToString(viewModel.UserId));
            sqlCmd.Parameters.AddWithValue("EMPID", Convert.ToString(viewModel.EmpId));
            sqlCmd.Parameters.AddWithValue("ISACTIVE", Convert.ToBoolean(viewModel.IsActive));
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

        public bool FunLoginPasswordChange(ChangePasswordModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "UPDATE [TBL_USERINFO] SET [USERPASSWORD]=@NEWUSERPASSWORD WHERE [USERID]=@USERID AND [USERPASSWORD]=@OLDUSERPASSWORD";
            sqlConn.ConnectionString = conString;
            bool opStatus = false;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("USERID", Convert.ToString(objDaoUserInfo.UserId));
            sqlCmd.Parameters.AddWithValue("NEWUSERPASSWORD", Convert.ToString(viewModel.NewPassword));
            sqlCmd.Parameters.AddWithValue("OLDUSERPASSWORD", Convert.ToString(viewModel.OldPassword));
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

        public SelectList FunReturnRoleList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ROLECODE],[ROLEDESC] FROM [TBL_ROLE]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Text = "--Select--", Value = "" });
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["ROLEDESC"].ToString(), Value = sqlReader["ROLECODE"].ToString() });
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

        public bool FunSaveRoleInfo(TblRoleModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " INSERT INTO [TBL_ROLE]([ROLECODE],[ROLEDESC],[ISACTIVE])VALUES(@ROLECODE,@ROLEDESC,@ISACTIVE) ";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ROLECODE", Convert.ToString(viewModel.Rolecode));
            sqlCmd.Parameters.AddWithValue("ROLEDESC", Convert.ToString(viewModel.Roledesc));
            sqlCmd.Parameters.AddWithValue("ISACTIVE", Convert.ToBoolean(viewModel.IsActive));

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

        public string FuncReturnMaxRoleCode()
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT isnull(MAX([ROLECODE]),'00')as maxCode FROM [TBL_ROLE]";

            sqlConn.ConnectionString = conString;
            String MaxCode = "00";
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        MaxCode = (Convert.ToInt32(sqlReader["maxCode"]) + 1).ToString().PadLeft(2, '0');
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return MaxCode;
        }

        public TblRoleModel FunSearchRoleInfo(string RoleCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ROLECODE],[ROLEDESC],[ISACTIVE] FROM [TBL_ROLE] WHERE [ROLECODE]=@ROLECODE";
            TblRoleModel objTblRoleModel = new TblRoleModel();
            sqlConn.ConnectionString = conString;
            String MaxCode = "00";
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ROLECODE", Convert.ToString(RoleCode));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objTblRoleModel.Rolecode = sqlReader["ROLECODE"].ToString();
                        objTblRoleModel.Roledesc = sqlReader["ROLEDESC"].ToString();
                        objTblRoleModel.IsActive = Convert.ToBoolean(sqlReader["ISACTIVE"]);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objTblRoleModel;
        }

        public bool FunUpdateRoleInfo(TblRoleModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " UPDATE [TBL_ROLE] SET [ROLEDESC]=@ROLEDESC,[ISACTIVE]=@ISACTIVE WHERE [ROLECODE]=@ROLECODE";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ROLECODE", Convert.ToString(viewModel.Rolecode));
            sqlCmd.Parameters.AddWithValue("ROLEDESC", Convert.ToString(viewModel.Roledesc));
            sqlCmd.Parameters.AddWithValue("ISACTIVE", Convert.ToBoolean(viewModel.IsActive));

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

        public bool FunLoginPasswordChange(NESHOP.Models.ChangePasswordModel viewModel, NESHOP.Models.DaoUserInfo objDaoUserInfo)
        {
            throw new NotImplementedException();
        }

        public bool FunLoginPasswordChange(NESHOP.Models.ChangePasswordModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            throw new NotImplementedException();
        }

        public bool FunSaveUserInfo(RegisterModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            throw new NotImplementedException();
        }

        RegisterModel IBllUserInfo.FunSearchUserInfo(string UserId)
        {
            throw new NotImplementedException();
        }

        public bool FunUpdateUserInfo(NESHOP.Models.RegisterModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            throw new NotImplementedException();
        }

        public bool FunLoginPasswordChange(NESHOP.Models.ChangePasswordModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            throw new NotImplementedException();
        }

        public bool FunSaveUserInfo(NESHOP.Models.RegisterModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            throw new NotImplementedException();
        }

        RegisterModel FunSearchUserInfo(string UserId)
        {
            throw new NotImplementedException();
        }

        public bool FunUpdateUserInfo(NESHOP.Models.RegisterModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            throw new NotImplementedException();
        }
    }
}
