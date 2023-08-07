using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NESHOP.Models;
using NESHOP.Contacts;
using Microsoft.Extensions.Configuration;
using NeSHOP.DAL;
using NeSHOP.Models;
using Microsoft.AspNetCore.Authorization;

namespace NeSHOP.Controllers
{
    public class HomeController : Controller
    {
        DaoUserInfo objDaoUserInfo = new DaoUserInfo();

        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private IBllUserInfo _bllUserInfo;
        private IBllAdmin _bllAdmin;
        private IBllCommon _bllCommon;
        public HomeController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<HomeController> logger, IBllUserInfo bllUserInfo, IBllAdmin bllAdmin, IBllCommon bllCommon)
        {
            _environment = environment;
            _configuration = configuration;
            _logger = logger;

            _bllUserInfo = bllUserInfo;
            _bllAdmin = bllAdmin;
            _bllCommon = bllCommon;
        }


        #region Index page
        [AllowAnonymous]
        public ActionResult Index()
        {
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

            }

            ViewData["Message"] = "Welcome To NE-ATMS";

            return View();
        }
        #endregion

        #region About info
        public ActionResult About()
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Exit form
        [AllowAnonymous]
        public ActionResult Exit()
        {
            HttpContext.Session.Clear();


            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Manual info
        [AllowAnonymous]
        public ActionResult Manual()
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region User logon
        // **************************************
        // URL: /Account/LogOn
        // **************************************
        public ActionResult LogOn()
        {
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                _logger.LogInformation("Hello Welcome ");

                return RedirectToAction("Index", "Home");
            }
            else return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            _logger.LogInformation("Hello Welcome ");

            objDaoUserInfo.UserId = model.UserId;
            objDaoUserInfo.Password = model.Password;
            var daoUserInfo = new DaoUserInfo();
            daoUserInfo = _bllUserInfo.FunValidateUser(objDaoUserInfo);
            if (daoUserInfo.IsActive)
            {
                int validLicence = _bllAdmin.FuncValidLicanceAuthentication();
                if (validLicence > 0)
                {
                    daoUserInfo.LicenceDays = validLicence;
                    HttpContext.Session.SetString("loginStatus", JsonConvert.SerializeObject(daoUserInfo));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("AddLicance", "Admin");
                }
            }
            else
            {
                ViewData["SuccessStat"] = "2";
                return View(model);
            }


        }
        #endregion
    }
}
