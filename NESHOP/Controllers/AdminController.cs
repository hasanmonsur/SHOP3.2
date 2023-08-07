using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NeSHOP.Controllers;
using NeSHOP.DAL;
using NeSHOP.Models;
using NESHOP.Auth;
using NESHOP.Contacts;
using NESHOP.Models;
using Newtonsoft.Json;

namespace NESHOP.Controllers
{
    [MyAuth]
    // [HandleError]
    public class AdminController : Controller
    {
        DaoUserInfo objDaoUserInfo = new DaoUserInfo();

        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminController> _logger;

        private IBllDbConnection _dbConn;

        private IBllUserInfo _bllUserInfo;
        private IBllAdmin _bllAdmin;
        private IBllCommon _bllCommon;
        

        public AdminController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<AdminController> logger,IBllUserInfo bllUserInfo, IBllAdmin bllAdmin, IBllCommon bllCommon, IBllDbConnection dbConn)
        {
            _environment = environment;
            _configuration = configuration;
            _logger = logger;
            _bllUserInfo = bllUserInfo;
            _bllAdmin = bllAdmin;
            _bllCommon = bllCommon;
            _dbConn = dbConn;
        }

        #region Employee Manage
        // **************************************
        // URL: /Account/UserInfo
        // **************************************

        public ActionResult EmpInfo()
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                UserInfoModel viewModel = new UserInfoModel();
                viewModel.IsActive = true;
                viewModel.EmpList = _bllAdmin.FuncReturnEmpList("");
                ModelState.Clear();
                ViewData.Model = viewModel;
                //ViewData["NoRows"] = objTblRoleconfigModel.FormsModelList.Count;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        [HttpPost]
        public ActionResult EmpInfo(UserInfoModel viewModel, string returnUrl)
        {

            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    viewModel.EntryDate = DateTime.Now;
                    viewModel.UserId = objDaoUserInfo.UserId;
                    viewModel.InstCode = objDaoUserInfo.InstCode;
                    viewModel.EmpId = _bllAdmin.FuncReturnMaxEmpId();
                    bool createStatus = _bllAdmin.FuncEntryEmpInfo(viewModel);

                    if (createStatus)
                    {
                        return EmpInfo();
                    }
                    else
                    {
                        return EmpInfo();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    viewModel = _bllAdmin.FunSearchEmpInfo(viewModel.EmpId);
                    if (viewModel.EmpId != "")
                    {
                        viewModel.EmpList = _bllAdmin.FuncReturnEmpList(viewModel.EmpId);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();

                    }
                    else
                    {
                        return EmpInfo();
                    }
                }
                else if (viewModel.Btn == "Update")
                {
                    bool opStatus = _bllAdmin.FunUpdateEmpInfo(viewModel);
                    if (opStatus)
                    {
                        return EmpInfo();
                    }
                    else
                    {
                        return EmpInfo();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return EmpInfo();

                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    return EmpInfo();
                }

            }

            else
            {
                return RedirectToAction("Index", "Home");

            }

        }

        #endregion

        
        #region logoff
        // **************************************
        // URL: /Account/LogOff
        // **************************************
        public ActionResult Logout()
        {
            //Session["loginStatus"] = null;
            HttpContext.Session.Clear();

            return RedirectToAction("Logon", "Home");
        }

        #endregion

        #region Create New user
        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult CreateNewUser()
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);
                RegisterModel objRegisterModel = new RegisterModel();
                objRegisterModel.EmpIdList = _bllAdmin.FuncReturnEmpList("");
                objRegisterModel.IsActive = true;
                ModelState.Clear();
                ViewData.Model = objRegisterModel;
                //ViewData["NoRows"] = objTblRoleconfigModel.FormsModelList.Count;
                return View();
            }
            else {
                return RedirectToAction("Index", "Home");

            }
        }

        [HttpPost]
        public ActionResult CreateNewUser(RegisterModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);
                if (viewModel.Btn == "Save")
                {
                    bool opStatus = _bllUserInfo.FunSaveUserInfo(viewModel, objDaoUserInfo);
                    if (opStatus)
                    {
                        return CreateNewUser();
                    }
                    else
                    {
                        viewModel.EmpIdList = _bllAdmin.FuncReturnEmpList(viewModel.EmpId);
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    viewModel = _bllUserInfo.FunSearchUserInfo(viewModel.UserId);
                    if (viewModel.UserPassword != "")
                    {
                        viewModel.EmpIdList = _bllAdmin.FuncReturnEmpList(viewModel.EmpId);
                        viewModel.UserPassword = "";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();

                    }
                    else
                    {
                        return CreateNewUser();
                    }
                }
                else if (viewModel.Btn == "Update")
                {
                    bool opStatus = _bllUserInfo.FunUpdateUserInfo(viewModel, objDaoUserInfo);
                    if (opStatus)
                    {
                        return CreateNewUser();
                    }
                    else
                    {
                        viewModel.EmpIdList = _bllAdmin.FuncReturnEmpList(viewModel.EmpId);
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return CreateNewUser();

                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    return CreateNewUser();
                }

            }
            else
            {
                return CreateNewUser();
            }
        }
        #endregion

        #region change password
        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        public ActionResult ChangePass()
        {

            ChangePasswordModel objChangePasswordModel = new ChangePasswordModel();
            ModelState.Clear();
            ViewData.Model = objChangePasswordModel;
            return View();
        }

        [HttpPost]
        public ActionResult ChangePass(ChangePasswordModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    bool opStatus = _bllUserInfo.FunLoginPasswordChange(viewModel, objDaoUserInfo);
                    if (opStatus)
                    {
                        return ChangePass();
                    }
                    else
                    {
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return ChangePass();

                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    return ChangePass();
                }

            }
            else
            {
                return ChangePass();
            }
        }
        #endregion

        #region Role Info
        public ActionResult RoleInfo()
        {
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                TblRoleModel objTblRoleModel = new TblRoleModel();
                objTblRoleModel.RolecodeList = _bllUserInfo.FunReturnRoleList("");
                ModelState.Clear();
                ViewData.Model = objTblRoleModel;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }


        [HttpPost]
        public ActionResult RoleInfo(TblRoleModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                if (viewModel.Btn == "Save")
                {
                    viewModel.Rolecode = _bllUserInfo.FuncReturnMaxRoleCode();
                    bool opStatus = _bllUserInfo.FunSaveRoleInfo(viewModel, objDaoUserInfo);
                    if (opStatus)
                    {
                        return RoleInfo();
                    }
                    else
                    {
                        viewModel.RolecodeList = _bllUserInfo.FunReturnRoleList(viewModel.Rolecode);
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    viewModel = _bllUserInfo.FunSearchRoleInfo(viewModel.Rolecode);
                    if (viewModel.Roledesc != "")
                    {
                        viewModel.RolecodeList = _bllUserInfo.FunReturnRoleList(viewModel.Rolecode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();

                    }
                    else
                    {
                        return RoleInfo();
                    }
                }
                else if (viewModel.Btn == "Update")
                {
                    bool opStatus = _bllUserInfo.FunUpdateRoleInfo(viewModel);
                    if (opStatus)
                    {
                        return RoleInfo();
                    }
                    else
                    {
                        viewModel.RolecodeList = _bllUserInfo.FunReturnRoleList(viewModel.Rolecode);
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return RoleInfo();

                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    return RoleInfo();
                }

            }
            else
            {
                return RoleInfo();
            }
        }
        #endregion

        #region Role Config
        
        public ActionResult RoleConfig()
        {
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                TblRoleconfigModel viewModel = new TblRoleconfigModel();
                viewModel.RolecodeList = _bllAdmin.FuncRoleModelList("");
                viewModel.FormsModelList = _bllAdmin.FuncReturnFormList("");

                ModelState.Clear();
                ViewData.Model = viewModel;
                ViewData["NoRows"] = viewModel.FormsModelList.Count;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }

        }

        [HttpPost]
        public ActionResult RoleConfig(TblRoleconfigModel viewModel, string returnUrl)
        {
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {

                    bool opStatus = _bllAdmin.FuncSaveRoleConfig(viewModel);
                    if (opStatus)
                    {
                        return RoleConfig();
                    }
                    else
                    {
                        viewModel.RolecodeList = _bllAdmin.FuncRoleModelList(viewModel.Rolecode);
                        viewModel.FormsModelList = _bllAdmin.FuncReturnFormList(viewModel.Rolecode);

                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    viewModel.RolecodeList = _bllAdmin.FuncRoleModelList(viewModel.Rolecode);
                    viewModel.FormsModelList = _bllAdmin.FuncReturnFormList(viewModel.Rolecode);

                    ModelState.Clear();
                    ViewData.Model = viewModel;
                    return View();
                }
                else if (viewModel.Btn == "Clear")
                {
                    return RoleConfig();
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RoleConfig();
                }
            }
            else
            {
                return RoleConfig();
            }

        }
        #endregion

        #region Ints Info
        public ActionResult InstInfo()
        {
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                TblAppinfoModel objTblAppinfoModel = new TblAppinfoModel();
                objTblAppinfoModel.InstcatagorycodeList = _bllAdmin.FuncReturnCatagoryList("");
                objTblAppinfoModel.ShiftNoList = _bllCommon.FuncReturnShistNoList("");
                objTblAppinfoModel.InstCodeList = _bllCommon.FuncReturnInstList("");
                ModelState.Clear();
                ViewData.Model = objTblAppinfoModel;
                //ViewData["NoRows"] = objTblRoleconfigModel.FormsModelList.Count;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        [HttpPost]
        public ActionResult InstInfo(TblAppinfoModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    viewModel.Instcode = _bllCommon.FuncReturnInstCode(objDaoUserInfo);
                    bool opStatus = _bllAdmin.FuncSaveInstInformation(viewModel, objDaoUserInfo);
                    if (opStatus)
                    {
                        return InstInfo();
                    }
                    else
                    {
                        viewModel.InstcatagorycodeList = _bllAdmin.FuncReturnCatagoryList(viewModel.Instcatagorycode);
                        viewModel.ShiftNoList = _bllCommon.FuncReturnShistNoList(viewModel.RefCode);
                        viewModel.InstCodeList = _bllCommon.FuncReturnInstList(viewModel.Instcode);
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Update")
                {
                    bool opStatus = _bllAdmin.FuncUpdateInstInformation(viewModel, objDaoUserInfo);
                    if (opStatus)
                    {
                        return InstInfo();
                    }
                    else
                    {
                        viewModel.InstcatagorycodeList = _bllAdmin.FuncReturnCatagoryList(viewModel.Instcatagorycode);
                        viewModel.ShiftNoList = _bllCommon.FuncReturnShistNoList(viewModel.RefCode);
                        viewModel.InstCodeList = _bllCommon.FuncReturnInstList(viewModel.Instcode);
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    viewModel = _bllAdmin   .FuncSearchInstInformation(viewModel.Instcode, objDaoUserInfo);
                    if (viewModel.Instname != "")
                    {
                        viewModel.InstcatagorycodeList = _bllAdmin.FuncReturnCatagoryList(viewModel.Instcatagorycode);
                        viewModel.ShiftNoList = _bllCommon.FuncReturnShistNoList(viewModel.RefCode);
                        viewModel.InstCodeList = _bllCommon.FuncReturnInstList(viewModel.Instcode);
                        viewModel.Btn = "Search";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return InstInfo();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return InstInfo();

                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    return InstInfo();
                }
            }
            else
            {
                return InstInfo();
            }

        }
        #endregion

        #region User Permission
        
        public ActionResult UserPermission()
        {
            TblUserprevModel viewModel = new TblUserprevModel();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.RoleModelList = _bllAdmin.FuncRoleModelArray("");
                viewModel.UserList = _bllAdmin.FuncReturnUserList("");
                ModelState.Clear();
                ViewData.Model = viewModel;
                ViewData["NoRows"] = viewModel.RoleModelList.Count;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        [HttpPost]
        public ActionResult UserPermission(TblUserprevModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    bool opStatus = _bllAdmin.FuncSaveUserPermission(viewModel);
                    if (opStatus)
                    {
                        return UserPermission();
                    }
                    else
                    {
                        viewModel.RoleModelList = _bllAdmin.FuncRoleModelArray(viewModel.Userid);
                        viewModel.UserList = _bllAdmin.FuncReturnUserList("");
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        ViewData["NoRows"] = viewModel.RoleModelList.Count;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Search")
                {

                    viewModel.RoleModelList = _bllAdmin.FuncRoleModelArray(viewModel.Userid);
                    viewModel.UserList = _bllAdmin.FuncReturnUserList(viewModel.Userid);
                    viewModel.Userid = viewModel.Userid;
                    ModelState.Clear();
                    ViewData.Model = viewModel;
                    ViewData["NoRows"] = viewModel.RoleModelList.Count;
                    return View();
                }
                else if (viewModel.Btn == "Clear")
                {
                    return UserPermission();
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return UserPermission();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }

        }
        #endregion
    
        #region Day End Information

        public ActionResult DayEnd()
        {
            DayEndModel objDayEndModel = new DayEndModel();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                objDayEndModel.CurrentBusinessDate = objDaoUserInfo.BusinessDate;
                objDayEndModel.NewBusinessDate = DateTime.Now;
                ModelState.Clear();
                ViewData.Model = objDayEndModel;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult DayEnd(DayEndModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    bool opStatus = _bllAdmin.FuncSaveDayEndInfo(viewModel, objDaoUserInfo);
                    if (opStatus)
                    {
                        return DayEnd();
                    }
                    else
                    {
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }

                else if (viewModel.Btn == "Clear")
                {
                    return DayEnd();
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return DayEnd();
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion

        #region Database Backup

        public ActionResult DailyBackup()
        {
            DatabaseBackModel objDayEndModel = new DatabaseBackModel();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                objDayEndModel.BackupDate = objDaoUserInfo.BusinessDate;
                ModelState.Clear();
                ViewData.Model = objDayEndModel;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult DailyBackup(DatabaseBackModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Backup")
                {
                    bool opStatus = FuncDailyDatabaseBackup(viewModel);
                    if (opStatus)
                    {
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.MsgCode = "2";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }

                else if (viewModel.Btn == "Clear")
                {
                    return DailyBackup();
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return DailyBackup();
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        internal bool FuncDailyDatabaseBackup(DatabaseBackModel viewModel)
        {
            bool execStatus = false;
            SqlConnection sqlConn = new SqlConnection();
            //var objBllDbConnection = new BllDbConnection(_configuration);
            sqlConn.ConnectionString = _dbConn.FunReturnDbConnectionStringDb();
            //String bkPath = Server.MapPath("~/DataBackups/");
            String bkPath=Path.Combine(_environment.WebRootPath, "/DataBackups/");

            String SqlString = "";
            SqlString = "DECLARE @BackupFile varchar(255), @DB varchar(30), @Description varchar(255), @LogFile varchar(50) " +
                " DECLARE @Name varchar(30), @MediaName varchar(30), @BackupDirectory nvarchar(200)  " +
                " SET @BackupDirectory = '" + bkPath + "' " +
                " DECLARE Database_CURSOR CURSOR FOR SELECT name FROM sysdatabases WHERE name='DBSHOP' " +
                " OPEN Database_Cursor " +
                " FETCH next FROM Database_CURSOR INTO @DB " +
                 " WHILE @@fetch_status = 0 " +
                 " BEGIN " +
                        " SET @Name = @DB + '( Daily BACKUP )'" +
                        " SET @MediaName = @DB + '_Dump' + CONVERT(varchar, CURRENT_TIMESTAMP , 112)" +
                        " SET @BackupFile = @BackupDirectory + + @DB + '_' + 'Full' + '_' + " +
                            " CONVERT(varchar, CURRENT_TIMESTAMP , 112)+convert (varchar(10),datepart (hh, (CURRENT_TIMESTAMP)))+convert (varchar(10),datepart (mm, (CURRENT_TIMESTAMP)))+convert (varchar(10),datepart (ss, (CURRENT_TIMESTAMP))) + '.bak' " +
                        " SET @Description = 'Normal' + ' BACKUP at ' + CONVERT(varchar, CURRENT_TIMESTAMP) + '.' " +
                        " IF (SELECT COUNT(*) FROM msdb.dbo.backupset WHERE database_name = @DB) > 0 OR @DB = 'master'" +
                            " BEGIN " +
                                " SET @BackupFile = @BackupDirectory + @DB + '_' + 'Full' + '_' + " +
                                    " CONVERT(varchar, CURRENT_TIMESTAMP , 112)+convert (varchar(10),datepart (hh, (CURRENT_TIMESTAMP)))+convert (varchar(10),datepart (mm, (CURRENT_TIMESTAMP)))+convert (varchar(10),datepart (ss, (CURRENT_TIMESTAMP))) + '.bak' " +
                                " SET @Description = 'Full' + ' BACKUP at ' + CONVERT(varchar, CURRENT_TIMESTAMP) + '.' " +
                            " END" +
                        " ELSE" +
                            " BEGIN " +
                                " SET @BackupFile = @BackupDirectory + @DB + '_' + 'Full' + '_' + " +
                                    " CONVERT(varchar, CURRENT_TIMESTAMP , 112)+convert (varchar(10),datepart (hh, (CURRENT_TIMESTAMP)))+convert (varchar(10),datepart (mm, (CURRENT_TIMESTAMP)))+convert (varchar(10),datepart (ss, (CURRENT_TIMESTAMP))) + '.bak'" +
                                " SET @Description = 'Full' + ' BACKUP at ' + CONVERT(varchar, CURRENT_TIMESTAMP) + '.' " +
                            " END" +
                            " BACKUP DATABASE @DB TO DISK = @BackupFile " +
                            " WITH NAME = @Name, DESCRIPTION = @Description , " +
                            " MEDIANAME = @MediaName, MEDIADESCRIPTION = @Description , " +
                            " STATS = 10" +
                        " FETCH next FROM Database_CURSOR INTO @DB" +
                " END" +
                " CLOSE Database_Cursor" +
                " DEALLOCATE Database_Cursor";
            try
            {
                SqlCommand sqlCmd = new SqlCommand(SqlString, sqlConn);
                sqlConn.Open();
                sqlCmd.ExecuteNonQuery();
                execStatus = true;
            }
            catch (Exception ex)
            {
                throw;
            }

            sqlConn.Close();

            return execStatus;
        }
        #endregion

        #region Licance Add
        [AllowAnonymous]
        public ActionResult AddLicance()
        {
            LicanceModel objLicanceModel = new LicanceModel();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                ModelState.Clear();
                ViewData.Model = objLicanceModel;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddLicance(LicanceModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    bool velidCheck = _bllAdmin.FuncReturnValidLicance(viewModel.Licance);
                    if (velidCheck)
                    {
                        bool opStatus = _bllAdmin.FuncSaveLicanceInfo(viewModel);
                        if (opStatus)
                        {
                            return RedirectToAction("LogOn", "Admin");
                        }
                        else
                        {
                            ModelState.Clear();
                            ViewData.Model = viewModel;
                            return View();
                        }
                    }
                    else
                        return AddLicance();
                }

                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("LogOn", "Admin");
                }
                else
                {
                    return AddLicance();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

    }
}
