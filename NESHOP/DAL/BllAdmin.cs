using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NeSHOP.Models;
using NESHOP.Contacts;
using NESHOP.Controllers;
using NESHOP.Models;

namespace NeSHOP.DAL
{
    public class BllAdmin : IBllAdmin
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BllAdmin> _logger;
        private readonly IBllDbConnection _dbConn;
        public BllAdmin(IConfiguration configuration, ILogger<BllAdmin> logger, IBllDbConnection dbConn)
        {
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
        }

        public List<DepartmentsModel> FuncReturnDeptList(String DeptId)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [DEPTID],(CAST([DEPTID] AS VARCHAR(10))+'-'+[DEPTNAME]) AS DEPTNAME,[DIVID] FROM [DEPARTMENTS]";

            sqlConn.ConnectionString = conString;
            var deptList = new List<DepartmentsModel>();

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {


                        var dept = new DepartmentsModel();
                        dept.Deptid = sqlReader["DEPTID"].ToString();
                        dept.Deptname = sqlReader["DEPTNAME"].ToString();
                        if (sqlReader["DIVID"].ToString() != "")
                            dept.Supdeptid = sqlReader["DIVID"].ToString();

                        deptList.Add(dept);

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            //SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", DeptId);

            return deptList;
        }

        public SelectList FuncDeptList(string DeptId)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT dt.[DEPTID],(dt.[DEPTNAME]+'-'+ds.[DIVNAME]) as DeptName FROM [TBL_DEPARTMENTS] dt, [TBL_DIVISION] ds WHERE dt.[DIVID]=ds.[DIVID]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            //objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["DeptName"].ToString(), Value = sqlReader["DEPTID"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", DeptId);

            return objSelectList;
        }

        public List<TblRoleModel> FuncRoleModel()
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ROLECODE],[ROLEDESC]FROM [dbo].[TBL_ROLE]";

            sqlConn.ConnectionString = conString;
            List<TblRoleModel> tblRoleModelList = new List<TblRoleModel>();

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {


                        var roleModel = new TblRoleModel();
                        roleModel.Rolecode = sqlReader.GetString(sqlReader.GetOrdinal("ROLECODE"));
                        roleModel.Roledesc = sqlReader.GetString(sqlReader.GetOrdinal("ROLEDESC"));

                        tblRoleModelList.Add(roleModel);

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            //SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", DeptId);

            return tblRoleModelList;
        }

        public SelectList FuncRoleModelList(string RoleCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [ROLECODE],[ROLEDESC] FROM [dbo].[TBL_ROLE]";

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
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["ROLEDESC"].ToString(), Value = sqlReader["ROLECODE"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", RoleCode);

            return objSelectList;
        }

        public Collection<TblFormsModel> FuncReturnFormList(string roleCode)
        {

            Collection<TblFormsModel> TblFormsModelList = new Collection<TblFormsModel>();

            if (roleCode != "")
            {

                String conString = _dbConn.FunReturnConString();
                SqlConnection sqlConn = new SqlConnection();
                String SqlString = "SELECT f.[FORMCODE],f.[FORMNAME],f.[ACTIONLINK],f.[FORMLOC] " +
                                   " ,ISNULL((select r.ISACTIVE from TBL_ROLECONFIG r where r.FORMCODE=f.FORMCODE AND r.ROLECODE=@roleCode),0) as  IsActive FROM [TBL_FORMS] f";

                sqlConn.ConnectionString = conString;
                SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
                sqlCmd.Parameters.AddWithValue("roleCode", Convert.ToString(roleCode));
                try
                {
                    sqlConn.Open();
                    SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                    if (sqlReader != null)
                    {

                        while (sqlReader.Read())
                        {

                            var objTblFormsModel = new TblFormsModel();

                            objTblFormsModel.Formcode = sqlReader["FORMCODE"].ToString();
                            objTblFormsModel.Formname = sqlReader["FORMNAME"].ToString();
                            objTblFormsModel.IsActive = Convert.ToBoolean(sqlReader["IsActive"]);
                            TblFormsModelList.Add(objTblFormsModel);
                        }

                    }
                }
                catch (Exception e)
                {
                    //throw;
                }
            }

            return TblFormsModelList;
        }

        public Collection<TblRoleModel> FuncRoleModelArray(String UserId)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT r.ROLECODE,r.ROLEDESC,isnull((select p.ISACTIVE from TBL_USERPREV p where p.ROLECODE=r.ROLECODE AND p.USERID=@USERID ),'FALSE')as IsActive FROM [TBL_ROLE] r";
            sqlConn.ConnectionString = conString;
            Collection<TblRoleModel> objTblRoleModelList = new Collection<TblRoleModel>();
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
                        var objTblRoleModel = new TblRoleModel();

                        objTblRoleModel.Rolecode = sqlReader["ROLECODE"].ToString();
                        objTblRoleModel.Roledesc = sqlReader["ROLEDESC"].ToString();
                        objTblRoleModel.IsActive = Convert.ToBoolean(sqlReader["IsActive"]);
                        objTblRoleModelList.Add(objTblRoleModel);
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objTblRoleModelList;
        }

        public SelectList FuncReturnCatagoryList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [INSTCATACODE],[INSTCATANAME] FROM [TBL_INSTCATAGORY]";

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
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["INSTCATANAME"].ToString(), Value = sqlReader["INSTCATACODE"].ToString() });
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

        public bool FuncSaveInstInformation(TblAppinfoModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " INSERT INTO [TBL_APPINFO]([INSTCODE],[INSTNAME],[INSTADDRESS],[INSTEMAIL],[INSTFAX],[INTPHONE],[WEBSITE],[INSTCATAGORYCODE],[RefCode],[INSTALLDATE],[INSTALLBY]) " +
                               " VALUES(@INSTCODE,@INSTNAME,@INSTADDRESS,@INSTEMAIL,@INSTFAX,@INTPHONE,@WEBSITE,@INSTCATAGORYCODE,@RefCode,@INSTALLDATE,@INSTALLBY)";
            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("INSTCODE", Convert.ToString(viewModel.Instcode));
            sqlCmd.Parameters.AddWithValue("INSTNAME", Convert.ToString(viewModel.Instname));
            sqlCmd.Parameters.AddWithValue("INSTADDRESS", Convert.ToString(viewModel.Instaddress));
            sqlCmd.Parameters.AddWithValue("INSTEMAIL", Convert.ToString(viewModel.Instemail));
            sqlCmd.Parameters.AddWithValue("INSTFAX", Convert.ToString(viewModel.Instfax));
            sqlCmd.Parameters.AddWithValue("INTPHONE", Convert.ToString(viewModel.Intphone));
            sqlCmd.Parameters.AddWithValue("WEBSITE", Convert.ToString(viewModel.Website));
            sqlCmd.Parameters.AddWithValue("INSTCATAGORYCODE", Convert.ToString(viewModel.Instcatagorycode));
            sqlCmd.Parameters.AddWithValue("RefCode", Convert.ToString(viewModel.RefCode));
            sqlCmd.Parameters.AddWithValue("INSTALLDATE", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            sqlCmd.Parameters.AddWithValue("INSTALLBY", Convert.ToString(objDaoUserInfo.UserId));
            try
            {
                sqlConn.Open();
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

        public TblAppinfoModel FuncSearchInstInformation(String Instcode, DaoUserInfo objDaoUserInfo)
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

        public bool FuncUpdateInstInformation(TblAppinfoModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " UPDATE [TBL_APPINFO] SET [INSTNAME]=@INSTNAME,[INSTADDRESS]=@INSTADDRESS,[INSTEMAIL]=@INSTEMAIL,[INSTFAX]=@INSTFAX,[INTPHONE]=@INTPHONE " +
                               ",[WEBSITE]=@WEBSITE,[INSTCATAGORYCODE]=@INSTCATAGORYCODE,[RefCode]=@RefCode WHERE [INSTCODE]=@INSTCODE ";
            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("INSTCODE", Convert.ToString(viewModel.Instcode));
            sqlCmd.Parameters.AddWithValue("INSTNAME", Convert.ToString(viewModel.Instname));
            sqlCmd.Parameters.AddWithValue("INSTADDRESS", Convert.ToString(viewModel.Instaddress));
            sqlCmd.Parameters.AddWithValue("INSTEMAIL", Convert.ToString(viewModel.Instemail));
            sqlCmd.Parameters.AddWithValue("INSTFAX", Convert.ToString(viewModel.Instfax));
            sqlCmd.Parameters.AddWithValue("INTPHONE", Convert.ToString(viewModel.Intphone));
            sqlCmd.Parameters.AddWithValue("WEBSITE", Convert.ToString(viewModel.Website));
            sqlCmd.Parameters.AddWithValue("INSTCATAGORYCODE", Convert.ToString(viewModel.Instcatagorycode));
            sqlCmd.Parameters.AddWithValue("RefCode", Convert.ToString(viewModel.RefCode));
            sqlCmd.Parameters.AddWithValue("INSTALLDATE", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            sqlCmd.Parameters.AddWithValue("INSTALLBY", Convert.ToString(objDaoUserInfo.UserId));
            try
            {
                sqlConn.Open();
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

        public SelectList FuncReturnEmpList(string EmpId)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [EmpId],([FullName]+'('+[EmpId]+')') as EMPNAME  FROM [Tbl_Employee]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["EMPNAME"].ToString(), Value = sqlReader["EmpId"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", EmpId);

            return objSelectList;
        }

        public bool FuncSaveUserPermission(TblUserprevModel viewModel)
        {
            int rCount = 0;
            foreach (var tblRoleModel in viewModel.RoleModelList)
            {
                bool opStatus = FuncSaveUserPermission(tblRoleModel, viewModel.Userid);

                if (opStatus) rCount++;
            }
            if (rCount > 0) return true;
            else return false;
        }

        public bool FuncSaveUserPermission(TblRoleModel tblRoleModel, string Userid)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String InsertSqlString = "INSERT INTO [TBL_USERPREV] ([ROLECODE],[USERID],[ISACTIVE])VALUES(@ROLECODE,@USERID,@ISACTIVE) ";
            String UpdateSqlString = "UPDATE [TBL_USERPREV] SET [ISACTIVE]=@ISACTIVE WHERE [ROLECODE]=@ROLECODE AND [USERID]=@USERID";

            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            bool velidCheck = FuncReturnValidPermission(Userid, tblRoleModel.Rolecode);
            SqlCommand sqlCmd = null;
            if (velidCheck)
                sqlCmd = new SqlCommand(UpdateSqlString, sqlConn);
            else
                sqlCmd = new SqlCommand(InsertSqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("USERID", Convert.ToString(Userid));
            sqlCmd.Parameters.AddWithValue("ROLECODE", Convert.ToString(tblRoleModel.Rolecode));
            sqlCmd.Parameters.AddWithValue("ISACTIVE", Convert.ToString(tblRoleModel.IsActive));

            try
            {
                sqlConn.Open();
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

        public bool FuncReturnValidPermission(string Userid, string Rolecode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT COUNT(*) as  rCount FROM [TBL_USERPREV]  WHERE [ROLECODE]=@ROLECODE AND [USERID]=@USERID";

            sqlConn.ConnectionString = conString;
            bool opStatus = false;

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("USERID", Convert.ToString(Userid));
            sqlCmd.Parameters.AddWithValue("ROLECODE", Convert.ToString(Rolecode));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    int rCount = 0;
                    while (sqlReader.Read())
                    {
                        rCount = Convert.ToInt32(sqlReader["rCount"].ToString());
                    }

                    if (rCount > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return opStatus;
        }

        public bool FuncSaveRoleConfig(TblRoleconfigModel viewModel)
        {
            int rCount = 0;
            foreach (var tblFormModel in viewModel.FormsModelList)
            {
                //if (tblFormModel.IsActive !=false)
                //{
                bool opStatus = FuncSaveRoleConfig(tblFormModel, viewModel.Rolecode);
                if (opStatus) rCount++;
                //}
            }
            if (rCount > 0) return true;
            else return false;
        }

        public bool FuncSaveRoleConfig(TblFormsModel tblFormModel, string Rolecode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String InsertSqlString = "INSERT INTO [TBL_ROLECONFIG]([ROLECODE],[FORMCODE],[ISACTIVE])VALUES(@ROLECODE, @FORMCODE,@ISACTIVE) ";
            String UpdateSqlString = "UPDATE [TBL_ROLECONFIG] SET [ISACTIVE]=@ISACTIVE WHERE [ROLECODE]=@ROLECODE AND [FORMCODE]= @FORMCODE";

            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            bool velidCheck = FuncReturnValidRoleConfig(Rolecode, tblFormModel.Formcode);
            SqlCommand sqlCmd = null;
            if (velidCheck)
                sqlCmd = new SqlCommand(UpdateSqlString, sqlConn);
            else
                sqlCmd = new SqlCommand(InsertSqlString, sqlConn);
            if (tblFormModel.Formcode == null) tblFormModel.Formcode = "";
            sqlCmd.Parameters.AddWithValue("ROLECODE", Convert.ToString(Rolecode));
            sqlCmd.Parameters.AddWithValue("FORMCODE", Convert.ToString(tblFormModel.Formcode));
            sqlCmd.Parameters.AddWithValue("ISACTIVE", Convert.ToString(tblFormModel.IsActive));

            try
            {
                sqlConn.Open();
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

        public bool FuncReturnValidRoleConfig(string Rolecode, string FormCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT COUNT(*) as  rCount FROM [TBL_ROLECONFIG]  WHERE [ROLECODE]=@ROLECODE AND [FORMCODE]= @FORMCODE";

            sqlConn.ConnectionString = conString;
            bool opStatus = false;

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("FORMCODE", Convert.ToString(FormCode));
            sqlCmd.Parameters.AddWithValue("ROLECODE", Convert.ToString(Rolecode));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    int rCount = 0;
                    while (sqlReader.Read())
                    {
                        rCount = Convert.ToInt32(sqlReader["rCount"].ToString());
                    }

                    if (rCount > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return opStatus;
        }

        public DepartmentsModel FuncDeptSearchData(string DeptId)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [DEPTID],[DEPTNAME],[DIVID] FROM [TBL_DEPARTMENTS] WHERE [DEPTID]=@DEPTID ";

            sqlConn.ConnectionString = conString;
            DepartmentsModel objDepartmentsModel = new DepartmentsModel();

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("DEPTID", Convert.ToString(DeptId));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objDepartmentsModel.Deptname = sqlReader["DeptName"].ToString();
                        objDepartmentsModel.Deptid = Convert.ToString(sqlReader["DEPTID"]);
                        objDepartmentsModel.Supdeptid = Convert.ToString(sqlReader["DIVID"]);
                    }
                    objDepartmentsModel.DeptList = FuncDeptList(DeptId);
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDepartmentsModel;
        }

        public bool FuncSaveDeptInfo(DepartmentsModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String InsertSqlString = "INSERT INTO [TBL_DEPARTMENTS]([DEPTID],[DEPTNAME],[DIVID])VALUES(@DEPTID,@DEPTNAME,@DIVID) ";
            String UpdateSqlString = "UPDATE [TBL_DEPARTMENTS] SET [DEPTNAME]=@DEPTNAME,[DIVID]=@DIVID WHERE [DEPTID]=@DEPTID";

            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            if (viewModel.DeptSearchKey == null) viewModel.DeptSearchKey = "";
            bool velidCheck = FuncReturnValidDept(viewModel.DeptSearchKey);
            SqlCommand sqlCmd = null;
            if (velidCheck)
                sqlCmd = new SqlCommand(UpdateSqlString, sqlConn);
            else
                sqlCmd = new SqlCommand(InsertSqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("DEPTID", Convert.ToString(viewModel.Deptid));
            sqlCmd.Parameters.AddWithValue("DEPTNAME", Convert.ToString(viewModel.Deptname));
            sqlCmd.Parameters.AddWithValue("DIVID", Convert.ToString(viewModel.Supdeptid));

            try
            {
                sqlConn.Open();
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

        public bool FuncReturnValidDept(String DeptId)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT COUNT(*) as  rCount FROM [TBL_DEPARTMENTS] WHERE [DEPTID]=@DEPTID";

            sqlConn.ConnectionString = conString;
            bool opStatus = false;

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("DEPTID", Convert.ToString(DeptId));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    int rCount = 0;
                    while (sqlReader.Read())
                    {
                        rCount = Convert.ToInt32(sqlReader["rCount"].ToString());
                    }

                    if (rCount > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return opStatus;
        }

        public string FuncReturnLastDeptId()
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT isnull(MAX(DEPTID),'0') as  MAXDEPTID FROM [TBL_DEPARTMENTS] ";

            sqlConn.ConnectionString = conString;
            bool opStatus = false;

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("DEPTID", Convert.ToString(DeptId));
            int rCount = 0;
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();

                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        rCount = Convert.ToInt32(sqlReader["MAXDEPTID"].ToString());
                    }

                    if (rCount > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return (rCount + 1).ToString();
        }

        public SelectList FuncReturnDesigDataList(string Code)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [DESIGCODE],[DESIGDESC],[DESIGTYPE] FROM [TBL_DESIGNATION]";

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
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["DESIGDESC"].ToString(), Value = sqlReader["DESIGCODE"].ToString() });
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

        public SelectList FuncReturnDesigTypeList(string Code)
        {
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0".ToString() });

            objSelectListItem.Add(new SelectListItem { Text = "Officer", Value = "1" });
            objSelectListItem.Add(new SelectListItem { Text = "Worker", Value = "2" });

            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", Code);

            return objSelectList;
        }

        public TblDesigModel FuncDesigSearchData(string DesigCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [DESIGCODE],[DESIGDESC],[DESIGTYPE] FROM [TBL_DESIGNATION] WHERE DESIGCODE=@DESIGCODE";

            sqlConn.ConnectionString = conString;
            TblDesigModel objTblDesigModel = new TblDesigModel();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("DESIGCODE", Convert.ToString(DesigCode));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objTblDesigModel.Desigcode = sqlReader["DESIGCODE"].ToString();
                        objTblDesigModel.Desigdesc = sqlReader["DESIGDESC"].ToString();
                        objTblDesigModel.Desigtype = sqlReader["DESIGTYPE"].ToString();

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objTblDesigModel;
        }

        public bool FuncSaveDesigInfo(TblDesigModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String InsertSqlString = "INSERT INTO [TBL_DESIGNATION]([DESIGCODE],[DESIGDESC],[DESIGTYPE])VALUES(@DESIGCODE,@DESIGDESC,@DESIGTYPE) ";
            String UpdateSqlString = "UPDATE [TBL_DESIGNATION] SET [DESIGDESC]=@DESIGDESC,[DESIGTYPE]=@DESIGTYPE  WHERE [DESIGCODE]=@DESIGCODE";

            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            bool velidCheck = FuncReturnValidDesigCode(viewModel.Desigcode);
            SqlCommand sqlCmd = null;
            if (velidCheck)
                sqlCmd = new SqlCommand(UpdateSqlString, sqlConn);
            else
                sqlCmd = new SqlCommand(InsertSqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("DESIGCODE", Convert.ToString(viewModel.Desigcode));
            sqlCmd.Parameters.AddWithValue("DESIGDESC", Convert.ToString(viewModel.Desigdesc));
            sqlCmd.Parameters.AddWithValue("DESIGTYPE", Convert.ToString(viewModel.Desigtype));

            try
            {
                sqlConn.Open();
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

        public bool FuncReturnValidDesigCode(string DesigCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT COUNT([DESIGCODE]) as rCount FROM [TBL_DESIGNATION] WHERE DESIGCODE=@DESIGCODE";
            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            TblDesigModel objTblDesigModel = new TblDesigModel();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("DESIGCODE", Convert.ToString(DesigCode));
            int rCount = 0;
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();

                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        rCount = Convert.ToInt32(sqlReader["rCount"]);

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            if (rCount > 0) opStatus = true;
            return opStatus;
        }

        public bool FuncSaveDayEndInfo(DayEndModel viewModel, DaoUserInfo objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String InsertSqlString = "INSERT INTO [TBL_DAYEND]([INSTCODE],[SYSSTATUS],[BUSINESSDATE],[ENTRYDATE] ,[ENTRYBY])VALUES(@INSTCODE,'O',@BUSINESSDATE,@ENTRYDATE,@ENTRYBY);" +
                                     "UPDATE [TBL_DAYEND] SET [SYSSTATUS] ='C'  WHERE [INSTCODE]=@INSTCODE AND [SYSSTATUS]='O' AND [BUSINESSDATE]=@BUSINESSDATE; ";

            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(InsertSqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("INSTCODE", Convert.ToString(viewModel.InstCode));
            sqlCmd.Parameters.AddWithValue("BUSINESSDATE", Convert.ToDateTime(viewModel.NewBusinessDate));
            sqlCmd.Parameters.AddWithValue("ENTRYBY", Convert.ToString(objDaoUserInfo.UserId));
            sqlCmd.Parameters.AddWithValue("ENTRYDATE", Convert.ToDateTime(objDaoUserInfo.BusinessDate));
            try
            {
                sqlConn.Open();
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

        public TblAppinfoModel FuncReturnInstInformation(string Instcode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = " SELECT [INSTCODE],[INSTNAME],[INSTADDRESS],[INSTEMAIL],[INSTFAX],[INTPHONE],[WEBSITE],[INSTCATAGORYCODE] " +
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

        public SelectList FuncDivsionList(string DivId)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [DIVID],[DIVNAME] FROM [TBL_DIVISION]";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            //objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "000".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["DIVNAME"].ToString(), Value = sqlReader["DIVID"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", DivId);

            return objSelectList;
        }

        public string FuncReturnLastDivId()
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT isnull(MAX(DIVID),'0') as  MAXDEPTID FROM [TBL_DIVISION] ";

            sqlConn.ConnectionString = conString;
            bool opStatus = false;

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            //sqlCmd.Parameters.AddWithValue("DEPTID", Convert.ToString(DeptId));
            int rCount = 0;
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();

                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        rCount = Convert.ToInt32(sqlReader["MAXDEPTID"].ToString());
                    }

                    if (rCount > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return (rCount + 1).ToString();
        }

        public bool FuncSaveDivInfo(DivisionModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String InsertSqlString = "INSERT INTO [TBL_DIVISION]([DIVID],[DIVNAME])VALUES(@DIVID,@DIVNAME) ";
            String UpdateSqlString = "UPDATE [TBL_DIVISION] SET [DIVNAME] =@DIVNAME WHERE [DIVID] =@DIVID";

            bool opStatus = false;
            sqlConn.ConnectionString = conString;
            if (viewModel.DivSearchKey == null) viewModel.DivSearchKey = "";
            bool velidCheck = FuncReturnValidDiv(viewModel.DivSearchKey);
            SqlCommand sqlCmd = null;
            if (velidCheck)
                sqlCmd = new SqlCommand(UpdateSqlString, sqlConn);
            else
                sqlCmd = new SqlCommand(InsertSqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("DIVID", Convert.ToString(viewModel.Divid));
            sqlCmd.Parameters.AddWithValue("DIVNAME", Convert.ToString(viewModel.Divname));

            try
            {
                sqlConn.Open();
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

        public bool FuncReturnValidDiv(string DivId)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT COUNT(*) as  rCount FROM [TBL_DIVISION] WHERE [DIVID]=@DIVID";

            sqlConn.ConnectionString = conString;
            bool opStatus = false;

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("DIVID", Convert.ToString(DivId));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    int rCount = 0;
                    while (sqlReader.Read())
                    {
                        rCount = Convert.ToInt32(sqlReader["rCount"].ToString());
                    }

                    if (rCount > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return opStatus;
        }

        public DivisionModel FuncDivSearchData(string DivId)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [DIVID],[DIVNAME] FROM [TBL_DIVISION] WHERE [DIVID]=@DIVID ";

            sqlConn.ConnectionString = conString;
            DivisionModel objDivisionModel = new DivisionModel();

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("DIVID", Convert.ToString(DivId));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objDivisionModel.Divname = sqlReader["DIVNAME"].ToString();
                        objDivisionModel.Divid = Convert.ToString(sqlReader["DIVID"]);

                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objDivisionModel;
        }

        public string FuncReturnMaxEmpId()
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT isnull(max([EmpId]),0) maxCode FROM [dbo].[Tbl_Employee]";

            sqlConn.ConnectionString = conString;
            String MaxCode = "0";
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        MaxCode = (Convert.ToInt32(sqlReader["maxCode"]) + 1).ToString();
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

        public bool FuncEntryEmpInfo(UserInfoModel objDaoUserInfo)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "INSERT INTO [Tbl_Employee]([EmpId],[NickName],[FullName],[EmpPhone]" +
                               ",[EmpEmail],[EmpAddress],[EmpComments],[EntryBy],[EntryDate],[IsActive],[InstCode],[DesigCode])" +
                               " VALUES(@EmpId,@NickName,@FullName,@EmpPhone,@EmpEmail,@EmpAddress," +
                                "@EmpComments,@UserId,@EntryDate,@IsActive,@InstCode,@DesigCode)";

            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            if (objDaoUserInfo.EmpPhone == null) objDaoUserInfo.EmpPhone = "";
            if (objDaoUserInfo.EmpEmail == null) objDaoUserInfo.EmpEmail = "";
            if (objDaoUserInfo.EmpAddress == null) objDaoUserInfo.EmpAddress = "";
            if (objDaoUserInfo.EmpComments == null) objDaoUserInfo.EmpComments = "";
            sqlCmd.Parameters.AddWithValue("EmpId", Convert.ToString(objDaoUserInfo.EmpId));
            sqlCmd.Parameters.AddWithValue("NickName", Convert.ToString(objDaoUserInfo.NickName));
            sqlCmd.Parameters.AddWithValue("FullName", Convert.ToString(objDaoUserInfo.FullName));
            sqlCmd.Parameters.AddWithValue("EmpPhone", Convert.ToString(objDaoUserInfo.EmpPhone));
            sqlCmd.Parameters.AddWithValue("EmpEmail", Convert.ToString(objDaoUserInfo.EmpEmail));
            sqlCmd.Parameters.AddWithValue("EmpAddress", Convert.ToString(objDaoUserInfo.EmpAddress));
            sqlCmd.Parameters.AddWithValue("EmpComments", Convert.ToString(objDaoUserInfo.EmpComments));
            sqlCmd.Parameters.AddWithValue("InstCode", Convert.ToString(objDaoUserInfo.InstCode));
            sqlCmd.Parameters.AddWithValue("UserId", Convert.ToString(objDaoUserInfo.UserId));
            sqlCmd.Parameters.AddWithValue("DesigCode", Convert.ToString("1001"));
            sqlCmd.Parameters.AddWithValue("EntryDate", Convert.ToDateTime(objDaoUserInfo.EntryDate));
            sqlCmd.Parameters.AddWithValue("IsActive", Convert.ToBoolean(objDaoUserInfo.IsActive));
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

        public Models.UserInfoModel FunSearchEmpInfo(string EmpCode)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT [EmpId],[NickName],[FullName] ,[EmpPhone],[EmpEmail] ,[EmpAddress] ,[EmpComments] ,[DesigCode],[IsActive] " +
                               " FROM [dbo].[Tbl_Employee] where [EmpId]=@EmpId";

            sqlConn.ConnectionString = conString;
            UserInfoModel objUserInfoModel = new UserInfoModel();
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("EmpId", Convert.ToString(EmpCode));
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objUserInfoModel.EmpId = sqlReader["EmpId"].ToString();
                        objUserInfoModel.NickName = sqlReader["NickName"].ToString();
                        objUserInfoModel.FullName = sqlReader["FullName"].ToString();
                        objUserInfoModel.EmpPhone = sqlReader["EmpPhone"].ToString();
                        objUserInfoModel.EmpEmail = sqlReader["EmpEmail"].ToString();
                        objUserInfoModel.EmpAddress = sqlReader["EmpAddress"].ToString();
                        objUserInfoModel.EmpComments = sqlReader["EmpComments"].ToString();
                        if (sqlReader["IsActive"].ToString() == "1")
                            objUserInfoModel.IsActive = true;
                        else objUserInfoModel.IsActive = false;


                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();

            return objUserInfoModel;
        }

        public bool FunUpdateEmpInfo(UserInfoModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            bool opStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            String SqlString =
                "UPDATE [Tbl_Employee] set [NickName]=@NickName,[FullName]=@FullName,[EmpPhone]=@EmpPhone" +
                ",[EmpEmail]=@EmpEmail,[EmpAddress]=@EmpAddress,[EmpComments]=@EmpComments,[IsActive]=@IsActive where [EmpId]=@EmpId";


            sqlConn.ConnectionString = conString;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("EmpId", Convert.ToString(viewModel.EmpId));
            sqlCmd.Parameters.AddWithValue("NickName", Convert.ToString(viewModel.NickName));
            sqlCmd.Parameters.AddWithValue("FullName", Convert.ToString(viewModel.FullName));
            sqlCmd.Parameters.AddWithValue("EmpPhone", Convert.ToString(viewModel.EmpPhone));
            sqlCmd.Parameters.AddWithValue("EmpEmail", Convert.ToString(viewModel.EmpEmail));
            sqlCmd.Parameters.AddWithValue("EmpAddress", Convert.ToString(viewModel.EmpAddress));
            sqlCmd.Parameters.AddWithValue("EmpComments", Convert.ToString(viewModel.EmpComments));
            sqlCmd.Parameters.AddWithValue("IsActive", Convert.ToBoolean(viewModel.IsActive));
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

        public bool FuncSaveLicanceInfo(LicanceModel viewModel)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String InsertSqlString = "INSERT INTO [dbo].[TBL_APPAUTHENTICATION]([INSTCODE],[CODE],[VALIDDAY],[EFFICTIVEDATE]) " +
                                    " VALUES(@INSTCODE, @CODE,@VALIDDAY,@EFFICTIVEDATE)";

            bool opStatus = false;
            sqlConn.ConnectionString = conString;

            SqlCommand sqlCmd = new SqlCommand(InsertSqlString, sqlConn);

            sqlCmd.Parameters.AddWithValue("INSTCODE", Convert.ToString(viewModel.Licance).Substring(0, 4));
            sqlCmd.Parameters.AddWithValue("CODE", Convert.ToString(viewModel.Licance).Substring(16, 8));
            sqlCmd.Parameters.AddWithValue("VALIDDAY", Convert.ToString(viewModel.Licance).Substring(12, 4));
            sqlCmd.Parameters.AddWithValue("EFFICTIVEDATE", DateTime.Now);

            try
            {
                sqlConn.Open();
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

        public bool FuncReturnValidLicance(string Licance)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT COUNT(*) as  rCount FROM TBL_APPAUTHENTICATION WHERE [CODE]=@CODE";

            sqlConn.ConnectionString = conString;
            bool opStatus = false;
            String OldCode = Licance.Substring(4, 8);
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("CODE", Convert.ToString(OldCode));

            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    int rCount = 0;
                    while (sqlReader.Read())
                    {
                        rCount = Convert.ToInt32(sqlReader["rCount"].ToString());
                    }

                    if (rCount > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return opStatus;
        }

        public int FuncValidLicanceAuthentication()
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "select sum(datediff(day,@ONDATE,dateadd(day,convert(int,VALIDDAY),EFFICTIVEDATE))) tDays from  dbo.TBL_APPAUTHENTICATION  " +
                               " where EFFICTIVEDATE<@ONDATE AND dateadd(day,convert(int,VALIDDAY),EFFICTIVEDATE)>0 group by INSTCODE";

            sqlConn.ConnectionString = conString;
            bool opStatus = false;
            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            sqlCmd.Parameters.AddWithValue("ONDATE", Convert.ToDateTime(DateTime.Now));
            int rCount = 0;
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {
                    //int rCount = 0;
                    while (sqlReader.Read())
                    {
                        rCount = Convert.ToInt32(sqlReader["tDays"].ToString());
                    }

                    if (rCount > 0) opStatus = true;
                }
            }
            catch (Exception e)
            {
                opStatus = false;
            }
            sqlConn.Close();

            return rCount;
        }

        public SelectList FuncReturnUserList(string userid)
        {
            String conString = _dbConn.FunReturnConString();
            SqlConnection sqlConn = new SqlConnection();
            String SqlString = "SELECT u.[UserId],(select e.FullName from dbo.Tbl_Employee e where e.EmpId=u.[EmpId] ) UserName FROM [dbo].[Tbl_UserInfo] u;";

            sqlConn.ConnectionString = conString;
            var objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "".ToString() });

            SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
            try
            {
                sqlConn.Open();
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader != null)
                {

                    while (sqlReader.Read())
                    {
                        objSelectListItem.Add(new SelectListItem { Text = sqlReader["UserName"].ToString(), Value = sqlReader["UserId"].ToString() });
                    }

                }
            }
            catch (Exception e)
            {
                //throw;
            }
            sqlConn.Close();
            SelectList objSelectList = new SelectList(objSelectListItem, "Value", "Text", userid);

            return objSelectList;
        }

        public bool FuncEntryEmpInfo(Models.UserInfoModel objDaoUserInfo)
        {
            throw new NotImplementedException();
        }

        public bool FunUpdateEmpInfo(Models.UserInfoModel viewModel)
        {
            throw new NotImplementedException();
        }
    }


}
