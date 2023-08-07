using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeSHOP.DAL;
using NeSHOP.Models;
using NESHOP.Auth;
using NESHOP.Contacts;
using NESHOP.Models;
using Newtonsoft.Json;

namespace NeSHOP.Controllers
{
    [MyAuth]
    public class InventoryController : Controller
    {
        DaoUserInfo objDaoUserInfo=new DaoUserInfo();
        DaoVendor objDaoVendor = new DaoVendor();

        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<InventoryController> _logger;

        private IBllDbConnection _dbConn;

        private IBllUserInfo _bllUserInfo;
        private IBllIteam _bllIteam;
        private IBllCommon _bllCommon;
        private IBllSettings _bllSettings;
        private IBllOrder _bllOrder;

        public InventoryController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<InventoryController> logger, IBllDbConnection dbConn, IBllUserInfo bllUserInfo, IBllIteam bllIteam, IBllCommon bllCommon, IBllSettings bllSettings, IBllOrder bllOrder)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
            _bllUserInfo = bllUserInfo;
            _bllIteam = bllIteam;
            _bllCommon = bllCommon;
            _bllSettings = bllSettings;
            _bllOrder = bllOrder;
        }



        // all order function...................................
        #region Stock In
        public ActionResult StockIn()
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                var viewModel = new IteamModels();
                //viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList("00");
                viewModel.BrandList = _bllCommon.FuncReturnVenList("00000");
                viewModel.ModelList = _bllSettings.FuncReturnModelList("000");
                //viewModel.ProductList = _bllSettings.FuncReturnProductList("000");
                viewModel.IteamList = _bllCommon.FuncReturnIteamList("0");
                viewModel.CostPrice = 0;
                ViewData["Role"] = "1";
                ModelState.Clear();
                ViewData.Model = viewModel;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult JfuncProductList(string Sel_StateName)
        {
            //JsonResult result = new JsonResult();
            var vList = _bllSettings.FuncReturnSelectProductList(Sel_StateName);
            //  result.Data = vList.ToList();
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            // return result;
            return Json(vList.ToList());
        }

        public JsonResult JfuncVenList(string Sel_StateName)
        {
            //JsonResult result = new JsonResult();
            var vList = _bllCommon.FuncReturnSelectVenList(Sel_StateName);
            //result.Data = vList.ToList();
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            return Json(vList.ToList());
        }

        public JsonResult JfuncModelList(string Sel_StateName)
        {
            //JsonResult result = new JsonResult();
            var vList = _bllSettings.FuncReturnSelectModelList(Sel_StateName);
            //result.Data = vList.ToList();
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //  return result;
            return Json(vList.ToList());
        }

        public JsonResult JFuncRetunStockInfo(string ids)
        {
            var data = new IteamModels();
            data = _bllIteam.FuncReturnIteamInfo(ids);
            return Json(data);
        }
        public ActionResult JfuncModelDesc(String Sel_StateName)
        {
            String[] data =new string[2];
            data = _bllOrder.FuncReturnModelDesc(Sel_StateName);

            return Json(data);
        }
        [HttpPost]
        public ActionResult StockIn(IteamModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                if (viewModel.Btn == "Save")
            {
                viewModel.IteamCode = _bllIteam.FuncReturnIteamCode(viewModel.ModelCode);
                viewModel.TranType = "1";
                bool enStatus = _bllIteam.FuncIteamStockEntry(viewModel, objDaoUserInfo);

                if (enStatus)
                {
                    ViewData["SuccessStat"] = "1";
                    return StockIn();
                }
                else
                {
                    ViewData["SuccessStat"] = "3";
                    return StockIn();
                }
                
            }
            else if (viewModel.Btn == "Search")
            {
                viewModel = _bllIteam.FuncReturnIteamInfo(viewModel.IteamCode);
                if (viewModel != null)
                {
                    viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                    viewModel.BrandList = _bllCommon.FuncReturnVenList(viewModel.BrandCode);
                    viewModel.ModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                    viewModel.ProductList = _bllSettings.FuncReturnProductList(viewModel.ProductCode);
                    viewModel.IteamList = _bllCommon.FuncReturnIteamList(viewModel.IteamCode);
                    ModelState.Clear();
                    ViewData.Model = viewModel;
                    return View();
                }
                else
                {
                    return StockIn();
                    
                }
            }
            else if (viewModel.Btn == "Update")
            {
                bool enStatus = _bllIteam.FuncIteamDetailsUpdate(viewModel, objDaoUserInfo);

                if (enStatus)
                {
                    ViewData["SuccessStat"] = "1";
                    return StockIn();
                }
                else
                {
                    ViewData["SuccessStat"] = "3";
                    return StockIn();
                }
               
            }
            else if (viewModel.Btn == "Exit")
            {
                return RedirectToAction("Index", "Home");
            }
            else if (viewModel.Btn == "Clear")
            {
                return StockIn();
            }
            else
            {

                return StockIn();
            }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
        // all order function...................................
        #region Stock Search
        public ActionResult StockSearch()
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                var objIteamModels = new IteamModels();
                //objIteamModels.CatagoryList = _bllSettings.FuncReturnCatagoryList("00");
                objIteamModels.BrandList = _bllCommon.FuncReturnVenList("00000");
                objIteamModels.ModelList = _bllSettings.FuncReturnModelList("000");
                //objIteamModels.ProductList = _bllSettings.FuncReturnProductList("000");
                objIteamModels.IteamList = null;// objBllCommon.FuncReturnIteamList("0");
                objIteamModels.TranDateFrom = DateTime.ParseExact("01/01/2010", "dd/MM/yyyy", null);
                objIteamModels.TranDateTo = DateTime.Now;
                ModelState.Clear();
                ViewData.Model = objIteamModels;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult StockSearch(IteamModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                if (viewModel.Btn == "Search")
                {
                    //viewModel = new IteamModels();
                    viewModel.IteamList = _bllSettings.FuncReturnSearchIteamList(viewModel);
                    if (viewModel.IteamList.Count>0)
                    {
                        
                       // viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList("");
                        viewModel.BrandList = _bllCommon.FuncReturnVenList("");
                        viewModel.ModelList = _bllSettings.FuncReturnModelList("");
                        //viewModel.ProductList = _bllSettings.FuncReturnProductList("");
                        //viewModel.IteamList = objBllCommon.FuncReturnIteamList("");
                        viewModel.TranDateFrom = DateTime.ParseExact("01/01/2000", "dd/MM/yyyy", null);
                        viewModel.TranDateTo = DateTime.Now;
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return StockSearch();

                    }
                }

                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (viewModel.Btn == "Clear")
                {
                    return StockSearch();
                }
                else
                {

                    return StockSearch();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Stock Report
        public ActionResult StockDetails()
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                var objIteamModels = new IteamModels();
                //objIteamModels.CatagoryList = _bllSettings.FuncReturnCatagoryList("00");
                objIteamModels.BrandList = _bllCommon.FuncReturnVenList("00000");
                objIteamModels.ModelList = _bllSettings.FuncReturnModelList("000");
                //objIteamModels.ProductList = _bllSettings.FuncReturnProductList("000");
                objIteamModels.IteamList = null;// objBllCommon.FuncReturnIteamList("0");
                objIteamModels.TranDateFrom = DateTime.ParseExact("01/01/2000", "dd/MM/yyyy", null);
                objIteamModels.TranDateTo = DateTime.Now;
                ModelState.Clear();
                ViewData.Model = objIteamModels;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

      
        [HttpPost]
        public ActionResult StockDetails(IteamModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Search")
                {
                    //viewModel = new IteamModels();
                    viewModel.IteamList = _bllSettings.FuncReturnSearchStockList(viewModel);
                    if (viewModel.IteamList.Count > 0)
                    {

                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList("");
                        viewModel.BrandList = _bllCommon.FuncReturnVenList("");
                        viewModel.ModelList = _bllSettings.FuncReturnModelList("");
                        viewModel.ProductList = _bllSettings.FuncReturnProductList("");
                        //viewModel.IteamList = objBllCommon.FuncReturnIteamList("");
                        viewModel.TranDateFrom = DateTime.ParseExact("01/01/2000", "dd/MM/yyyy", null);
                        viewModel.TranDateTo = DateTime.Now;
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return StockDetails();

                    }
                }

                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (viewModel.Btn == "Clear")
                {
                    return StockDetails();
                }
                else
                {

                    return StockDetails();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region Stock Mpodify
        public ActionResult StockModify()
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                var objIteamModels = new IteamModels();
                //objIteamModels.CatagoryList = _bllSettings.FuncReturnCatagoryList("00");
                objIteamModels.BrandList = _bllCommon.FuncReturnVenList("00000");
                objIteamModels.ModelList = _bllSettings.FuncReturnModelList("000");
                //objIteamModels.ProductList = _bllSettings.FuncReturnProductList("000");
                objIteamModels.IteamList = _bllCommon.FuncReturnIteamList("0");
                objIteamModels.CostPrice = 0;
                ViewData["Role"] = "1";
                ModelState.Clear();
                ViewData.Model = objIteamModels;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

       [HttpPost]
        public ActionResult StockModify(IteamModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                if (viewModel.Btn == "Search")
                {
                    viewModel = _bllIteam.FuncReturnIteamInfo(viewModel.IteamCode);
                    if (viewModel != null)
                    {
                        //viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                        viewModel.BrandList = _bllCommon.FuncReturnVenList(viewModel.BrandCode);
                        viewModel.ModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                        //viewModel.ProductList = _bllSettings.FuncReturnProductList(viewModel.ProductCode);
                        viewModel.IteamList = _bllCommon.FuncReturnIteamList(viewModel.IteamCode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return StockModify();

                    }
                }
                else if (viewModel.Btn == "Update")
                {
                    bool enStatus = _bllIteam.FuncIteamDetailsUpdate(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.MsgCode = "2";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return StockModify();
                    }
                    else
                    {
                        return StockModify();
                    }

                }
                else if (viewModel.Btn == "Delete")
                {
                    bool enStatus = _bllIteam.FuncIteamDelete(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        return StockModify();
                    }
                    else return StockModify();
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (viewModel.Btn == "Clear")
                {
                    return StockModify();
                }
                else
                {

                    return StockModify();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Stock Fault Update
       public ActionResult StockFaultUpdate()
       {
           objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                var objIteamModels = new IteamModels();
               
               objIteamModels.ModelList = _bllSettings.FuncReturnModelList("000");
               //objIteamModels.ProductList = _bllSettings.FuncReturnProductList("000");
               objIteamModels.IteamList = _bllCommon.FuncReturnFaultIteamList("");
               objIteamModels.CostPrice = 0;
               ViewData["Role"] = "1";
               ModelState.Clear();
               ViewData.Model = objIteamModels;
               return View();
           }
           else
           {
               return RedirectToAction("Index", "Home");
           }
       }

       [HttpPost]
       public ActionResult StockFaultUpdate(IteamModels viewModel, string returnUrl)
       {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                if (viewModel.Btn == "Search")
               {
                   viewModel = _bllIteam.FuncReturnFaultIteamInfo(viewModel.ModelCode, viewModel.Quantity.ToString());
                   if (viewModel != null)
                   {
                       
                       viewModel.ModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                       //viewModel.ProductList = _bllSettings.FuncReturnProductList(viewModel.ProductCode);
                       viewModel.IteamList = _bllCommon.FuncReturnFaultIteamList(viewModel.IteamCode);
                       ModelState.Clear();
                       ViewData.Model = viewModel;
                       return View();
                   }
                   else
                   {
                       return StockFaultUpdate();

                   }
               }
               else if (viewModel.Btn == "Update")
               {
                   bool enStatus = _bllIteam.FuncIteamFaultUpdate(viewModel, objDaoUserInfo);

                   if (enStatus)
                   {
                       viewModel.MsgCode = "2";
                       
                       viewModel.ModelList = _bllSettings.FuncReturnModelList("000");
                       
                       viewModel.IteamList = _bllCommon.FuncReturnFaultIteamList("");
                       ModelState.Clear();
                       ViewData.Model = viewModel;
                       return View();
                   }
                   else
                   {
                       viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                       viewModel.BrandList = _bllCommon.FuncReturnVenList(viewModel.BrandCode);
                       viewModel.ModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                       viewModel.ProductList = _bllSettings.FuncReturnProductList(viewModel.ProductCode);
                       viewModel.IteamList = _bllCommon.FuncReturnFaultIteamList(viewModel.IteamCode);
                       ModelState.Clear();
                       ViewData.Model = viewModel;
                       return View();
                   }

               }
               
               else if (viewModel.Btn == "Exit")
               {
                   return RedirectToAction("Index", "Home");
               }
               else if (viewModel.Btn == "Clear")
               {
                   return StockFaultUpdate();
               }
               else
               {

                   return StockFaultUpdate();
               }
           }
           else
           {
               return RedirectToAction("Index", "Home");
           }
       }
       #endregion

        #region Duplicate Stock Info
       public ActionResult DuplicateIteam()
       {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                var objIteamModels = new IteamModels();

               objIteamModels.ModelList = _bllSettings.FuncReturnModelList("000");
               objIteamModels.IteamList = _bllIteam.FuncReturnDuplicateIteamInfo("", "");
               objIteamModels.CostPrice = 0;
               ViewData["Role"] = "1";
               ModelState.Clear();
               ViewData.Model = objIteamModels;
               return View();
           }
           else
           {
               return RedirectToAction("Index", "Home");
           }
       }

       [HttpPost]
       public ActionResult DuplicateIteam(IteamModels viewModel, string returnUrl)
       {
           objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                if (viewModel.Btn == "Search")
               {
                   viewModel.IteamList = _bllIteam.FuncReturnDuplicateIteamInfo(viewModel.ModelCode, viewModel.Quantity.ToString());
                   if (viewModel != null)
                   {

                       viewModel.ModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                       //viewModel.IteamList = objBllCommon.FuncReturnFaultIteamList(viewModel.IteamCode);
                       ModelState.Clear();
                       ViewData.Model = viewModel;
                       return View();
                   }
                   else
                   {
                       return DuplicateIteam();

                   }
               }
               else if (viewModel.Btn == "Exit")
               {
                   return RedirectToAction("Index", "Home");
               }
               else if (viewModel.Btn == "Clear")
               {
                   return DuplicateIteam();
               }
               else
               {

                   return DuplicateIteam();
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
