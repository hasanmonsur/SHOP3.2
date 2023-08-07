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
using NESHOP.Controllers;
using NESHOP.Models;
using Newtonsoft.Json;

namespace NeSHOP.Controllers
{
    [MyAuth]
    public class AccountController : Controller
    {
        //BllCommon objBllCommon=new BllCommon();
        DaoUserInfo objDaoUserInfo=new DaoUserInfo();
        //BllSettings _bllSettings=new BllSettings();
        //BllOrder _bllOrder=new BllOrder();
        //BllTran _bllTran = new BllTran();
        //BllAdmin objBllAdmin=new BllAdmin();

        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        private IBllDbConnection _dbConn;

        private IBllOrder _bllOrder;
        private IBllUserInfo _bllUserInfo;
        private IBllTran _bllTran;
        private IBllCommon _bllCommon;
        private IBllSettings _bllSettings;
        private IBllAdmin _bllAdmin;
        


        public AccountController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<AccountController> logger, IBllDbConnection dbConn, IBllUserInfo bllUserInfo, IBllTran bllTran, IBllCommon bllCommon, IBllSettings bllSettings, IBllOrder bllOrder, IBllAdmin bllAdmin)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;
            _bllUserInfo = bllUserInfo;
            _bllTran = bllTran;
            _bllCommon = bllCommon;
            _bllSettings = bllSettings;
            _bllOrder = bllOrder;
            _bllAdmin = bllAdmin;
        }



        #region Voucher Entry
        public ActionResult VoucherEntry()
        {

            AcTranModel viewModel = new AcTranModel();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");            
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("0");
                viewModel.PaymentTypeList = _bllCommon.FuncPaymentTypeList("0");
                viewModel.TranStatusList = _bllCommon.FuncInvTranTypeList("0");
                viewModel.InvoiceList = _bllTran.FuncReturnSelectInvoiceList("0");
                viewModel.InvoiceDate = objDaoUserInfo.BusinessDate;
                viewModel.CollectionDate = objDaoUserInfo.BusinessDate;
                viewModel.AcTranList = _bllTran.FuncReturnTranList("0");
                viewModel.EmpList = _bllAdmin.FuncReturnEmpList("");
                viewModel.BankList = _bllSettings.FuncReturnBankList("");
                ModelState.Clear();
                ViewData.Model = viewModel;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult JfuncInvoiceInfo(String Sel_StateName)
        {
            InvoiceModels data = _bllTran.FuncInvoiceInfo(Sel_StateName);
            return Json(data);
        }

        public JsonResult JfuncInvoiceList(string Sel_StateName)
        {
            //JsonResult result = new JsonResult();
            var vList = _bllTran.FuncReturnSelectInvoiceList(Sel_StateName);
            //result.Data = vList.ToList();
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return Json(vList.ToList()); ;
        }

        [HttpPost]
        public ActionResult VoucherEntry(AcTranModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                #region save
                if (viewModel.Btn == "Save")
                {
                    try
                    {
                        viewModel.TranNo = _bllTran.FuncReturnTranNo(objDaoUserInfo.BusinessDate);
                        viewModel.TranType = "C";

                        bool opStatus = _bllTran.FuncTransactionEntry(viewModel, objDaoUserInfo);
                        if (opStatus) _bllOrder.FuncReturnInvoiceStatusUpdate(viewModel.InvoiceNo,viewModel.TranStatus);
                        
                        return VoucherEntry();
                    }
                    catch (Exception ex)
                    {
                        return VoucherEntry();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {

                    return VoucherEntry();
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return VoucherEntry();
                }
                #endregion
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion

        #region Voucher Modify cancel
        public ActionResult VoucherCancel()
        {

            AcTranModel viewModel = new AcTranModel();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("0");
                viewModel.PaymentTypeList = _bllCommon.FuncPaymentTypeList("0");
                viewModel.TranStatusList = _bllCommon.FuncInvTranTypeList("0");
                viewModel.InvoiceList = _bllTran.FuncReturnSelectInvoiceList("0");
                //viewModel.InvoiceDate = objDaoUserInfo.BusinessDate;
                viewModel.CollectionDate = objDaoUserInfo.BusinessDate;
                viewModel.EmpList = _bllAdmin.FuncReturnEmpList("");
                viewModel.BankList = _bllSettings.FuncReturnBankList("");

                ModelState.Clear();
                ViewData.Model = viewModel;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult JfuncTransactionInfo(String Sel_StateName)
        {
            AcTranModel data = _bllTran.FuncTransactionInfo(Sel_StateName);

            return Json(data);
        }

        public JsonResult JfuncModifyInvoiceList(string Sel_StateName)
        {
           // JsonResult result = new JsonResult();
            var vList = _bllTran.FuncReturnSelectModifyInvoiceList(Sel_StateName);
            //  result.Data = vList;
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            // return result;
            return Json(vList);
        }

       [HttpPost]
        public ActionResult VoucherCancel(AcTranModel viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                #region save
                if (viewModel.Btn == "Update")
                {
                    try
                    {
                        bool opStatus = _bllTran.FuncTransactionUpdate(viewModel);
                        if (opStatus) _bllOrder.FuncReturnInvoiceStatusUpdate(viewModel.InvoiceNo, viewModel.TranStatus);

                        return VoucherCancel();
                    }
                    catch (Exception ex)
                    {
                        return VoucherCancel();
                    }
                }
                else if (viewModel.Btn == "Cancel")
                {
                    try
                    {
                        bool opStatus = _bllTran.FuncReturnTranDelete(viewModel.TranNo);
                        if (opStatus) _bllOrder.FuncReturnInvoiceStatusUpdate(viewModel.InvoiceNo,"P");

                        return VoucherCancel();
                    }
                    catch (Exception ex)
                    {
                        return VoucherCancel();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {

                    return VoucherCancel();
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return VoucherCancel();
                }
                #endregion
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion

        #region Voucher Report
       public ActionResult VoucherSearch()
       {
           objDaoUserInfo = new DaoUserInfo();
           //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
           AcTranModel viewModel = new AcTranModel();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

               viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("0");
               viewModel.PaymentTypeList = _bllCommon.FuncPaymentTypeList("0");
               viewModel.TranStatusList = _bllCommon.FuncInvTranTypeList("0");
               viewModel.InvoiceList = _bllTran.FuncReturnSelectInvoiceList("0");
               viewModel.TranDateFrom = objDaoUserInfo.BusinessDate;
               viewModel.TranDateTo = objDaoUserInfo.BusinessDate;
               viewModel.AcTranList = _bllTran.FuncReturnTranList("0");
               ModelState.Clear();
               ViewData.Model = viewModel;
               return View();
           }
           else
           {
               return RedirectToAction("Index", "Home");
           }
       }


       [HttpPost]
       public ActionResult VoucherSearch(AcTranModel viewModel, string returnUrl)
       {
           objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                if (viewModel.Btn == "Search")
               {
                   if (viewModel.SearchType == "2") viewModel.TranType = "C";
                   else if (viewModel.SearchType == "3") viewModel.TranType = "D";
                   else viewModel.TranType = "";
                   
                   viewModel.AcTranList = _bllTran.FuncReturnSearchTranList(viewModel);
                   if (viewModel.AcTranList.Count > 0)
                   {

                       viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("0");
                       viewModel.PaymentTypeList = _bllCommon.FuncPaymentTypeList("0");
                       viewModel.TranStatusList = _bllCommon.FuncInvTranTypeList("0");
                       viewModel.InvoiceList = _bllTran.FuncReturnSelectInvoiceList("0");
                       viewModel.TranDateFrom = objDaoUserInfo.BusinessDate;
                       viewModel.TranDateTo = objDaoUserInfo.BusinessDate;
                       ModelState.Clear();
                       ViewData.Model = viewModel;
                       return View();
                   }
                   else
                   {
                       return VoucherSearch();

                   }
               }

               else if (viewModel.Btn == "Exit")
               {
                   return RedirectToAction("Index", "Home");
               }
               else if (viewModel.Btn == "Clear")
               {
                   return VoucherSearch();
               }
               else
               {

                   return VoucherSearch();
               }
           }
           else
           {
               return RedirectToAction("Index", "Home");
           }
       }

       #endregion

        #region Voucher Payment
       public ActionResult VoucherPayment()
       {

           AcTranPaymentModel viewModel = new AcTranPaymentModel();
           objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);
                viewModel.VenList = _bllCommon.FuncReturnVenList("0");
               viewModel.PaymentTypeList = _bllCommon.FuncPaymentTypeList("0");
               viewModel.PaymentDate = objDaoUserInfo.BusinessDate;
               viewModel.AcTranList = _bllTran.FuncReturnTranPaymentList("0");
               viewModel.EmpList = _bllAdmin.FuncReturnEmpList("");
               viewModel.BankList = _bllSettings.FuncReturnBankList("");
               ModelState.Clear();
               ViewData.Model = viewModel;
               return View();
           }
           else
           {
               return RedirectToAction("Index", "Home");
           }
       }

       

       [HttpPost]
       public ActionResult VoucherPayment(AcTranPaymentModel viewModel, string returnUrl)
       {
           objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                #region save
                if (viewModel.Btn == "Save")
               {
                   try
                   {
                       viewModel.TranNo = _bllTran.FuncReturnTranNo(objDaoUserInfo.BusinessDate);
                       viewModel.TranType = "D";

                       bool opStatus = _bllTran.FuncTransactionPaymentEntry(viewModel, objDaoUserInfo);
                       //if (opStatus) _bllOrder.FuncReturnInvoiceStatusUpdate(viewModel.InvoiceNo, viewModel.TranStatus);

                       return VoucherPayment();
                   }
                   catch (Exception ex)
                   {
                       return VoucherPayment();
                   }
               }
               else if (viewModel.Btn == "Clear")
               {

                   return VoucherPayment();
               }
               else if (viewModel.Btn == "Exit")
               {
                   return RedirectToAction("Index", "Home");
               }
               else
               {
                   return VoucherPayment();
               }
               #endregion
           }

           else
           {
               return RedirectToAction("Index", "Home");
           }

       }

       #endregion

        #region Voucher Modify
       public ActionResult VoucherModify()
       {

           AcTranPaymentModel viewModel = new AcTranPaymentModel();
           objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.VenList = _bllCommon.FuncReturnVenList("0");
               viewModel.PaymentTypeList = _bllCommon.FuncPaymentTypeList("0");
               viewModel.InvoiceList = _bllTran.FuncReturnSelectInvoiceList("0");
               //viewModel.InvoiceDate = objDaoUserInfo.BusinessDate;
               viewModel.PaymentDate = objDaoUserInfo.BusinessDate;
               viewModel.EmpList = _bllAdmin.FuncReturnEmpList("");
               viewModel.BankList = _bllSettings.FuncReturnBankList("");
               ModelState.Clear();
               ViewData.Model = viewModel;
               return View();
           }
           else
           {
               return RedirectToAction("Index", "Home");
           }
       }

       public ActionResult JfuncTransactionPaymentInfo(String Sel_StateName)
       {
           AcTranPaymentModel data = _bllTran.FuncTransactionPaymentInfo(Sel_StateName);

           return Json(data);
       }

      [HttpPost]
       public ActionResult VoucherModify(AcTranPaymentModel viewModel, string returnUrl)
       {
           objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                #region save
                if (viewModel.Btn == "Update")
               {
                   try
                   {
                       bool opStatus = _bllTran.FuncTransactionPaymentUpdate(viewModel);
                       //if (opStatus) _bllOrder.FuncReturnInvoiceStatusUpdate(viewModel.InvoiceNo, viewModel.TranStatus);

                       return VoucherModify();
                   }
                   catch (Exception ex)
                   {
                       return VoucherModify();
                   }
               }
               else if (viewModel.Btn == "Cancel")
               {
                   try
                   {
                       bool opStatus = _bllTran.FuncReturnTranDelete(viewModel.TranNo);
                       //if (opStatus) _bllOrder.FuncReturnInvoiceStatusUpdate(viewModel.InvoiceNo, "P");

                       return VoucherModify();
                   }
                   catch (Exception ex)
                   {
                       return VoucherCancel();
                   }
               }
               else if (viewModel.Btn == "Clear")
               {

                   return VoucherModify();
               }
               else if (viewModel.Btn == "Exit")
               {
                   return RedirectToAction("Index", "Home");
               }
               else
               {
                   return VoucherModify();
               }
               #endregion
           }
           else
           {
               return RedirectToAction("Index", "Home");
           }

       }

       #endregion

        #region Account Report
      public ActionResult AccountsReport()
      {
          objDaoUserInfo = new DaoUserInfo();
          //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
          AcTranModel viewModel = new AcTranModel();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
          if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.CustomerList = _bllSettings.FuncReturnPartyList("");
              viewModel.TranDateFrom = objDaoUserInfo.BusinessDate;
              viewModel.TranDateTo = objDaoUserInfo.BusinessDate;
              viewModel.PTypeList = _bllCommon.FuncReturnPTypeList("");
              viewModel.AcList = _bllTran.FuncReturnDailyTranList(viewModel);
              ModelState.Clear();
              ViewData.Model = viewModel;
              return View();
          }
          else
          {
              return RedirectToAction("Index", "Home");
          }
      }

      public JsonResult JfuncCustomerList(string Sel_StateName)
      {
         // JsonResult result = new JsonResult();
          var vList = _bllSettings.FuncReturnPartyList(Sel_StateName);
            //  result.Data = vList;
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            // return result;
            return Json(vList.ToList());
        }
      [HttpPost]
      public ActionResult AccountsReport(AcTranModel viewModel, string returnUrl)
      {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                if (viewModel.Btn == "Search")
              {
                  viewModel.AccountList = _bllTran.FuncReturnAccountDataList(viewModel);
                  if (viewModel.AccountList.Count > 0)
                  {

                      viewModel.CustomerList = _bllSettings.FuncReturnPartyList(viewModel.CustomerCode);
                      viewModel.TranDateFrom = viewModel.TranDateFrom;
                      viewModel.TranDateTo = viewModel.TranDateTo;
                      viewModel.PTypeList = _bllCommon.FuncReturnPTypeList(viewModel.PType);
                      viewModel.AcList = _bllTran.FuncReturnDailyTranList(viewModel);
                      ModelState.Clear();
                      ViewData.Model = viewModel;
                      return View();
                  }
                  else
                  {
                      return AccountsReport();

                  }
              }

              else if (viewModel.Btn == "Exit")
              {
                  return RedirectToAction("Index", "Home");
              }
              else if (viewModel.Btn == "Clear")
              {
                  return AccountsReport();
              }
              else
              {

                  return AccountsReport();
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
