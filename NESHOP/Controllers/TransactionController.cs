using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class TransactionController : Controller
    {
        DaoUserInfo objDaoUserInfo = new DaoUserInfo();
        TranModels objTranModels = new TranModels();
      

        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TransactionController> _logger;

        private IBllDbConnection _dbConn;

        private IBllUserInfo _bllUserInfo;
        private IBllCommon _bllCommon;
        private IBllTran _bllTran;
        private IBllOrder _bllOrder;
        private IBllSettings _bllSettings;
        
        


        public TransactionController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<TransactionController> logger, IBllDbConnection dbConn, IBllUserInfo bllUserInfo, IBllCommon bllCommon, IBllSettings bllSettings, IBllOrder bllOrder, IBllTran bllTran)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;

            _bllUserInfo = bllUserInfo;
            _bllCommon = bllCommon;
            _bllSettings = bllSettings;
            _bllOrder = bllOrder;
            _bllTran = bllTran;
        }


        #region Chalan Entry
        public ActionResult ChallanEntry()
        {
            var viewModel = new ChallanModels();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("0");
                //viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList("0");
                viewModel.BrandList = _bllCommon.FuncReturnVenList("0");
                viewModel.ModelList = _bllSettings.FuncReturnModelList("0");
                //viewModel.ProductList = _bllSettings.FuncReturnProductList("0");
                viewModel.ChallanDate = objDaoUserInfo.BusinessDate;
                //Session["TempOrder"] = null;
                HttpContext.Session.SetString("TempOrder", "");
                //ViewData["ChallanSuccess"] =null;
                ModelState.Clear();
            ViewData.Model = viewModel;
            return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult JsonFuncChallanAdd(String ids)
        {
            var data = new IteamModels();
            var OrderIteamModelList = new List<IteamModels>();
            int cont = 0;
            var jsonTempData = HttpContext.Session.GetString("TempOrder");
            if (!string.IsNullOrEmpty(jsonTempData))
            {
                //OrderIteamModelList = (List<IteamModels>)Session["TempOrder"];
                OrderIteamModelList = JsonConvert.DeserializeObject<List<IteamModels>>(jsonTempData);

                cont = OrderIteamModelList.Count();
            }
            cont = cont + 1;
            ids = ids + "+" + cont.ToString();
            data = _bllOrder.FuncReturnParseChallanData(ids);
            OrderIteamModelList.Add(data);
            //Session["TempOrder"] = OrderIteamModelList;
            HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(OrderIteamModelList));

            return Json(data);
        }

        public JsonResult JsonFuncChallanDelete(String ids)
        {
            String data = "Operation Fail............";
            List<IteamModels> objIteamlList = new List<IteamModels>();
            //HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(objNewIteamlList));
            
            string json = HttpContext.Session.GetString("TempOrder");
            objIteamlList = JsonConvert.DeserializeObject<List<IteamModels>>(json);  //(List<IteamModels>)Session["TempOrder"];
            
            List<IteamModels> objNewIteamlList = new List<IteamModels>();
            objNewIteamlList = _bllOrder.FuncReturnDeleteChalaData(objIteamlList, ids);
            if (objNewIteamlList.Count >= 0) data = "Remove Successfull............";
            //Session["TempOrder"] = objNewIteamlList;
            HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(objNewIteamlList));
            return Json(data);
        }

        public JsonResult JfuncModelSlNo(String Sel_StateName)
        {
            String data = "";
            data = _bllOrder.FuncReturnModelSlNo(Sel_StateName);

            return Json(data);
        }

       [HttpPost]
        public ActionResult ChallanEntry(ChallanModels viewModel, string returnUrl)
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
                        viewModel.ChallanNo = _bllOrder.FuncReturnChallanTranNo(objDaoUserInfo);
                        
                        string jsonDataList = HttpContext.Session.GetString("TempOrder");                        

                        if (!string.IsNullOrEmpty(jsonDataList))
                            {
                                //viewModel.IteamList = (List<IteamModels>)Session["TempOrder"];
                            viewModel.IteamList = JsonConvert.DeserializeObject<List<IteamModels>>(jsonDataList);

                            bool opStatus = _bllOrder.FuncChallanEntry(viewModel, objDaoUserInfo);
                                if (opStatus)
                                {
                                    ViewData["SuccessStat"] = "1";
                                    ViewData["ChallanTranno"] = viewModel.ChallanNo.Substring(4);
                                }
                                else ViewData["SuccessStat"] = "3";
                            }
                            return ChallanEntry();
                        
                    }
                    catch (Exception ex)
                    {
                        ViewData["SuccessStat"] = "3";
                        return ChallanEntry();
                    }
                }
                else if (viewModel.Btn == "Delete")
                {
                    bool opStatus = _bllOrder.FuncDeleteChallanInfo(viewModel.ChallanNo);
                    if (opStatus)
                    {
                        ViewData["SuccessStat"] = "2";
                        return ChallanEntry();
                    }
                    else
                    {
                        return ChallanEntry();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {

                    return ChallanEntry();
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return ChallanEntry();
                }
                #endregion 

                #region Add/Remove
                /*
                else if (tranModel.Btn == "Add")
                {
                    rowCount = tranModel.TranIteams.Count;
                    if (rowCount < 6)
                    {
                        List<TranIteam> TranIteams = new List<TranIteam>();
                        TranIteam objTranIteam = new TranIteam();
                        for (int i = 0; i < rowCount; i++)
                        {
                            objTranIteam = new TranIteam();
                            objTranIteam.PaymentTypeList = objBllCommon.FuncPaymentTypeList(tranModel.TranIteams[i].PayType);
                            
                            TranIteams.Add(objTranIteam);
                        }

                        objTranIteam = new TranIteam();
                        objTranIteam.PaymentTypeList = objBllCommon.FuncPaymentTypeList("");
                        TranIteams.Add(objTranIteam);
                        
                        tranModel.TranIteams = TranIteams;
                        ViewData["NoRows"] = rowCount + 1;
                    }
                    ViewData.Model = tranModel;
                    return View();
                }
                else if (tranModel.Btn == "Remove")
                {
                    rowCount = tranModel.TranIteams.Count;
                    if (rowCount > 1)
                    {
                        List<TranIteam> TranIteams = new List<TranIteam>();
                        TranIteam objTranIteam = new TranIteam();
                        for (int i = 0; i < rowCount; i++)
                        {
                            objTranIteam = new TranIteam();
                            objTranIteam.PaymentTypeList = objBllCommon.FuncPaymentTypeList(tranModel.TranIteams[i].PayType);
                            TranIteams.Add(objTranIteam);
                        }
                        
                        TranIteams.RemoveAt(rowCount - 1);
                        tranModel.TranIteams = TranIteams;
                        ViewData["NoRows"] = rowCount - 1;
                        //ModelState.Clear();
                       }
                    ViewData.Model = tranModel;
                    return View();
                }
                #endregion
                #region search
                else if (tranModel.Btn == "Search")
                {

                    tranModel = _bllTran.FuncReturnTranList(tranModel.TranNo);
                    rowCount = tranModel.TranIteams.Count;
                    if (rowCount < 6)
                    {
                        ModelState.Clear();
                        ViewData["NoRows"] = rowCount;
                        ViewData.Model = tranModel;
                        return View();
                    }
                    else
                    {
                        return ChallanEntry();
                    }
                   
                }
                #endregion
                #region Update
                else if (tranModel.Btn == "Update")
                {

                    rowCount = tranModel.TranIteams.Count;
                    int opCount = 0;
                    try
                    {
                        //objTran.TranNo = tranModel.TranNo;
                        //objTran.TranType = tranModel.TranType;
                        //objTran.VenCode = tranModel.VenCode;
                        //objTran.TranDate = tranModel.TranDate;
                        //objTran.TranComment = tranModel.TranComment;
                        //objTran.TranAmount = tranModel.TranAmount;
                        //objTran.OtherChargAmount = tranModel.OtherChargAmount;
                        //objTran.PaymentType = tranModel.PaymentType;
                        //objTran.DelverMediaCode = tranModel.DelverMediaCode;
                        //objTran.EntryBy = objDaoUserInfo.UserId;
                        //objTran.EntryDate = objDaoUserInfo.BusinessDate;
                        //objTran.DelverMediaCode = tranModel.DelverMediaCode;
                        //objTran.ChargType = tranModel.ChargType;
                        //objTran.IsActive = true;
                        //for (int i = 0; i < rowCount; i++)
                        //{
                        //    objTran.OrdTranNo = tranModel.TranIteams[i].OrdTranNo;
                        //    objTran.IteamCode = tranModel.TranIteams[i].IteamCode;
                        //    objTran.QuantityPerIteam = tranModel.TranIteams[i].QuantityPerIteam;
                        //    objTran.UnitPricePerIteam = tranModel.TranIteams[i].UnitPricePerIteam;
                        //    objTran.VatPercentage = tranModel.TranIteams[i].VatPercentage;
                        //    objTran.DiscountPercentage = tranModel.TranIteams[i].DiscountPercentage;

                        //    bool enStatus = _bllTran.FuncTransactionUpdate(objTran);
                        //    if (enStatus) opCount++;
                        //}
                    }
                    catch (Exception ex)
                    {
                        return ChallanEntry();
                    }


                    if (opCount == rowCount)
                    {
                        //TranModels objTranModels = new TranModels();
                        //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
                        //List<TranIteam> TranIteams = new List<TranIteam>();
                        //TranIteam objTranIteam = new TranIteam();
                        //objTranIteam.IteamCodeList = _bllTran.FuncReturnIteamCodeList("0000000000");
                        //TranIteams.Add(objTranIteam);
                        //objTranModels.TranIteams = TranIteams;
                        //if (objDaoUserInfo != null)
                        //    objTranModels.TranNo = _bllTran.FuncReturnTranNo(objDaoUserInfo.BusinessDate);
                        //else objTranModels.TranNo = "000000000000";
                        //objTranModels.TranTypeList = objBllCommon.FuncTranTypeList("0");
                        //objTranModels.TranDate = DateTime.Now.Date;
                        //objTranModels.VenCodeList = objBllCommon.FuncReturnIteamVendorList("00000");
                        //objTranModels.PaymentTypeList = objBllCommon.FuncPaymentTypeList("0000");
                        //objTranModels.DelverMediaCodeList = objBllCommon.FuncDelverMediaCodeList("0000");
                        ModelState.Clear();
                        ViewData.Model = objTranModels;
                        ViewData["NoRows"] = 1;

                        return View();
                    }
                    else
                    {
                        return ChallanEntry();
                    }


                }
                #endregion
                #region Delete
                else if (tranModel.Btn == "Search")
                {

                    bool opstatus = _bllTran.FuncReturnTranDelete(tranModel.TranNo);
                    if (opstatus)
                    {
                        return ChallanEntry();
                    }
                    else
                    {
                        return ChallanEntry();
                    }

                }*/
                #endregion
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        #endregion

        #region Quotation Entry
        public ActionResult Quotation()
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                var objOrderModels = new OrderModels();
                objOrderModels.CustomerList = _bllSettings.FuncReturnCustomerList("0");
                objOrderModels.CatagoryList = _bllSettings.FuncReturnCatagoryList("0");
                objOrderModels.VenList = _bllCommon.FuncReturnVenList("0");
                objOrderModels.ModelList = _bllSettings.FuncReturnModelList("0");
                objOrderModels.ProductList = _bllSettings.FuncReturnProductList("0");
                objOrderModels.TarmsList = _bllSettings.FuncReturnTarmsList("0");
                objOrderModels.EntryDate = DateTime.Now;

                HttpContext.Session.SetString("TempOrder", "");

                ModelState.Clear();
                ViewData.Model = objOrderModels;
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
            //result.Data = vList.ToList();
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return Json(vList.ToList());
        }

        public JsonResult JfuncVenList(string Sel_StateName)
        {
            //JsonResult result = new JsonResult();
            var vList = _bllCommon.FuncReturnSelectVenList(Sel_StateName);
            //result.Data = vList.ToList();
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            // return result;
            return Json(vList.ToList());
        }

        public JsonResult JfuncModelList(string Sel_StateName)
        {
            //JsonResult result = new JsonResult();
            var vList = _bllSettings.FuncReturnSelectModelList(Sel_StateName);
            // result.Data = vList.ToList();
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return Json(vList.ToList());
        }

        public ActionResult JsonFuncOrderAdd(String ids)
        {
            var data = new IteamModels();
            var OrderIteamModelList = new List<IteamModels>();
            int cont = 0;
            string jsonDataList = HttpContext.Session.GetString("TempOrder");
            

            if (!string.IsNullOrEmpty(jsonDataList))
            {
                //OrderIteamModelList = (List<IteamModels>)Session["TempOrder"];
                OrderIteamModelList = JsonConvert.DeserializeObject<List<IteamModels>>(jsonDataList);

                cont =OrderIteamModelList.Count();
            }
            cont = cont + 1;
            ids = ids + "+" + cont.ToString();
            data = _bllOrder.FuncReturnParseOrderData(ids);
            OrderIteamModelList.Add(data);
           // Session["TempOrder"] = OrderIteamModelList;

            HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(OrderIteamModelList));

            return Json(data);
        }


        public ActionResult JsonFuncOrderDelete(String ids)
        {
            String data = "Operation Fail............";
            List<IteamModels>  objIteamlList = new List<IteamModels>();
            //objIteamlList = (List<IteamModels>)Session["TempOrder"];
            string jsonDataList = HttpContext.Session.GetString("TempOrder");
            objIteamlList = JsonConvert.DeserializeObject<List<IteamModels>>(jsonDataList);

            List<IteamModels> objNewIteamlList = new List<IteamModels>();
            objNewIteamlList = _bllOrder.FuncReturnDeleteOrderData(objIteamlList, ids);
            if (objNewIteamlList.Count>0) data = "Remove Successfull............";
            //Session["TempOrder"] = objNewIteamlList;
            HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(objNewIteamlList));

            return Json(data);
        }

        public ActionResult JfuncModelDesc(String Sel_StateName)
        {
            String[] data =new string[2];
            data = _bllOrder.FuncReturnModelDesc(Sel_StateName);
            
            return Json(data);
        }

         [HttpPost]
       
        public ActionResult Quotation(OrderModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
            objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);
            if (viewModel.Btn == "Save")
            {
                viewModel.OrderRefNo = _bllOrder.FuncReturnOrdTranNo(objDaoUserInfo);

                string jsonDataList = HttpContext.Session.GetString("TempOrder");                    
                if (!string.IsNullOrEmpty(jsonDataList))
                {
                    //viewModel.IteamlList = (List<IteamModels>)Session["TempOrder"];
                    viewModel.IteamlList = JsonConvert.DeserializeObject<List<IteamModels>>(jsonDataList);
                    bool opStatus = _bllOrder.FuncOrderEntry(viewModel, objDaoUserInfo);
                    if(viewModel.TarmsList!=null && opStatus)
                            _bllOrder.FuncOrderTermsEntry(viewModel, objDaoUserInfo);

                    if (opStatus)
                    {
                        ViewData["SuccessStat"] = "1";
                        ViewData["QutationTranno"] = viewModel.OrderRefNo.Substring(8);// +"+" + objDaoUserInfo.BusinessDate.ToString("yyyy-MM-dd") + "+" + objDaoUserInfo.BusinessDate.ToString("yyyy-MM-dd");
                        //String st = (string)ViewData["QutationTranno"];
                    }
                    else ViewData["SuccessStat"] = "3";
                }


               return Quotation();
            }
             else if (viewModel.Btn == "Clear")
            {

                return Quotation();
            }
           
            else if (viewModel.Btn == "Exit")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Quotation();
            }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        
        #endregion 

        #region Invoice Entry
         public ActionResult InvoiceEntry()
         {
             InvoiceModels viewModel = new InvoiceModels();
             objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.BrandList = _bllCommon.FuncReturnVenList("0");
                 viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("0");
                 viewModel.CustChallanList = _bllOrder.FuncReturnSelectChallanList("");
                 viewModel.ModelList = _bllOrder.FuncReturnSelectChallanModelList(viewModel.BrandCode);
                 viewModel.InvoiceDate = objDaoUserInfo.BusinessDate;
                 viewModel.EntryDate = objDaoUserInfo.BusinessDate;
                HttpContext.Session.SetString("TempOrder", "");
                ModelState.Clear();
                 ViewData.Model = viewModel;
                 return View();
             }
             else
             {
                 return RedirectToAction("Index", "Home");
             }
         }

         public JsonResult JsonFuncChallanAddAtInvoice(String ids)
         {
             var data = new IteamModels();
             var OrderIteamModelList = new List<IteamModels>();
             int cont = 0,index=0;
            string jsonDataList = HttpContext.Session.GetString("TempOrder");
            
            if (!string.IsNullOrEmpty(jsonDataList))
             {
                 //OrderIteamModelList = (List<IteamModels>)Session["TempOrder"];
                OrderIteamModelList = JsonConvert.DeserializeObject<List<IteamModels>>(jsonDataList);

                index = OrderIteamModelList.Count();
             }

             if (index > 0)
                 cont = Convert.ToInt32(OrderIteamModelList[index - 1].SLNo) + 1;
             else cont = 1;

             ids = ids + "+" + cont.ToString();
             data = _bllOrder.FuncReturnParseChallanInvoiceData(ids);

             OrderIteamModelList.Add(data);
             String[] rSumData = _bllOrder.FuncReturnSumData(OrderIteamModelList);
             data.pDataList = rSumData;

             HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(OrderIteamModelList));

             return Json(data);
         }

         public JsonResult JsonFuncChallanDeleteFromInvoice(String ids)
         {
             var data = new InvoiceModels();
             try
             {
                 var objIteamlList = new List<IteamModels>();
                // objIteamlList = (List<IteamModels>) Session["TempOrder"];
                string jsonDataList = HttpContext.Session.GetString("TempOrder");
                objIteamlList = JsonConvert.DeserializeObject<List<IteamModels>>(jsonDataList);

                var objNewIteamlList = new List<IteamModels>();
                 objNewIteamlList = _bllOrder.FuncReturnDeleteChalaData(objIteamlList, ids);
                 //data.CustChallanList = objNewIteamlList;
                 String[] rSumData = _bllOrder.FuncReturnSumData(objNewIteamlList);
                 data.pDataList = rSumData;

                 HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(objNewIteamlList));

             }
             catch(Exception es)
             {

             }

             return Json(data);
         }

         public JsonResult JfuncChallanProductQuantity(String Sel_StateName)
         {
             String[] data = new string[4];
             data = _bllOrder.FuncReturnProductQuantity(Sel_StateName);

             return Json(data);
         }

         public JsonResult JfuncReturnChallanProductQuantity(String Sel_StateName)
         {
             var data = new List<String>();
             data = _bllOrder.FuncReturnChallanQuantity(Sel_StateName);

             return Json(data);
         }

         public JsonResult JfuncChallanProductList(string Sel_StateName)
         {
             //JsonResult result = new JsonResult();
             var vList = _bllOrder.FuncReturnSelectChallanModelSerialList(Sel_StateName);
            // result.Data = vList;
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            return Json(vList.ToList());
        }

         public JsonResult JfuncChallanList(string Sel_StateName)
         {
            // JsonResult result = new JsonResult();
             var vList = _bllOrder.FuncReturnSelectChallanList(Sel_StateName);
            ///  result.Data = vList;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            return Json(vList.ToList());
        }

         [HttpPost]
         public ActionResult InvoiceEntry(InvoiceModels viewModel, string returnUrl)
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
                         if (viewModel.Btn == "Save")
                         {
                             viewModel.InvoiceNo = _bllOrder.FuncReturnInvoiceTranNo(objDaoUserInfo);

                            string jsonDataList = HttpContext.Session.GetString("TempOrder");
                            if (!string.IsNullOrEmpty(jsonDataList))
                            {
                                viewModel.ChallanList = JsonConvert.DeserializeObject<List<IteamModels>>(jsonDataList);

                                //viewModel.ChallanList = (List<IteamModels>)Session["TempOrder"];
                                 bool opStatus = _bllOrder.FuncInvoiceMasterEntry(viewModel, objDaoUserInfo);

                                 //if (opStatus) _bllOrder.FuncInvoiceChieldEntry(viewModel, objDaoUserInfo);
                                 if (opStatus)
                                 {
                                     _bllOrder.FuncChallanEntryFromInvoice(viewModel, objDaoUserInfo);

                                     ViewData["SuccessStat"] = "1";
                                     ViewData["InvoiceTranno"] = viewModel.InvoiceNo.Substring(4);// +"+" + objDaoUserInfo.BusinessDate.ToString("yyyy-MM-dd") + "+" + objDaoUserInfo.BusinessDate.ToString("yyyy-MM-dd");
                                     //String st = (string)ViewData["QutationTranno"];
                                 }
                                 else
                                 {
                                     //_bllOrder.FuncInvoiceMasterDelete(viewModel, objDaoUserInfo);
                                     ViewData["SuccessStat"] = "3";
                                     
                                 }
                                 
                             }
                         }
                         return InvoiceEntry();
                     }
                     catch (Exception ex)
                     {
                         return InvoiceEntry();
                     }
                 }
                 
                 else if (viewModel.Btn == "Clear")
                 {

                     return InvoiceEntry();
                 }
                 else if (viewModel.Btn == "Exit")
                 {
                     return RedirectToAction("Index", "Home");
                 }
                 else
                 {
                     return InvoiceEntry();
                 }
                 #endregion
             }

             else
             {
                 return RedirectToAction("Index", "Home");
             }

         }

         #endregion

        #region Qutation Search
         public ActionResult QutationSearch()
         {
             objDaoUserInfo = new DaoUserInfo();
             //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
             OrderModels viewModel = new OrderModels();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.TranDateFrom = DateTime.Now;
                 viewModel.TranDateTo = DateTime.Now;
                 viewModel.OrderRefNo = "";
                 viewModel.QutationList = _bllOrder.FuncReturnQutationList(viewModel);
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
         public ActionResult QutationSearch(OrderModels viewModel, string returnUrl)
         {
             objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                if (viewModel.Btn == "Search")
                 {
                    if (!string.IsNullOrEmpty(viewModel.OrderRefNo))
                    {
                        viewModel.OrderRefNo = _bllOrder.FuncReturnOrdRefCode(objDaoUserInfo.InstCode) + "/" + viewModel.OrderRefNo;
                    }
                    else viewModel.OrderRefNo = "";


                     viewModel.QutationList = _bllOrder.FuncReturnQutationList(viewModel);
                     viewModel.OrderRefNo = "";
                     ModelState.Clear();
                     ViewData.Model = viewModel;
                     return View();
                     
                 }
                 else if (viewModel.Btn == "Exit")
                 {
                     return RedirectToAction("Index", "Home");
                 }
                 else if (viewModel.Btn == "Clear")
                 {
                     return QutationSearch();
                 }
                 else
                 {
                     //String qString = viewModel.Btn;
                     //Response.Write("<script langauge='javascript'>"+
                     //    " window.open('/Reports/Rpt_QutationSearch/'"+qString +", 'Report', 'width=1000,height=600,resizeable,scrollbars'); "+
                     //    " </script>");
                     //return QutationSearch();
                     return QutationSearch();
                 }
             }
             else
             {
                 return RedirectToAction("Index", "Home");
             }
         }
         #endregion

        #region Challan Search
         public ActionResult ChallanSearch()
         {
             objDaoUserInfo = new DaoUserInfo();
             //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
             ChallanModels viewModel = new ChallanModels();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.TranDateFrom = DateTime.Now;
                 viewModel.TranDateTo = DateTime.Now;
                 viewModel.ChallanList = _bllOrder.FuncReturnChallanList(viewModel);
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
         public ActionResult ChallanSearch(ChallanModels viewModel, string returnUrl)
         {
             objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Search")
                 {
                     //viewModel.ChallanNo = viewModel.ChallanNo;
                     viewModel.ChallanList = _bllOrder.FuncReturnChallanList(viewModel);
                     ModelState.Clear();
                     ViewData.Model = viewModel;
                     return View();

                 }
                 else if (viewModel.Btn == "Exit")
                 {
                     return RedirectToAction("Index", "Home");
                 }
                 else if (viewModel.Btn == "Clear")
                 {
                     return ChallanSearch();
                 }
                 else
                 {

                     return ChallanSearch();
                 }
             }
             else
             {
                 return RedirectToAction("Index", "Home");
             }
         }
         #endregion

        #region Invoice Search
         public ActionResult InvoiceSearch()
         {
             objDaoUserInfo = new DaoUserInfo();
             //objDaoUserInfo = (DaoUserInfo)Session["loginStatus"];
             InvoiceModels viewModel = new InvoiceModels();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.TranDateFrom = DateTime.Now;
                 viewModel.TranDateTo = DateTime.Now;
                 viewModel.InvoiceList = _bllOrder.FuncReturnInvoiceList(viewModel);
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
         public ActionResult InvoiceSearch(InvoiceModels viewModel, string returnUrl)
         {
             objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Search")
                 {
                     //viewModel.InvoiceNo = viewModel.ChallanNo;
                     viewModel.InvoiceList = _bllOrder.FuncReturnInvoiceList(viewModel);
                     ModelState.Clear();
                     ViewData.Model = viewModel;
                     return View();

                 }
                 else if (viewModel.Btn == "Exit")
                 {
                     return RedirectToAction("Index", "Home");
                 }
                 else if (viewModel.Btn == "Clear")
                 {
                     return InvoiceSearch();
                 }
                 else
                 {

                     return InvoiceSearch();
                 }
             }
             else
             {
                 return RedirectToAction("Index", "Home");
             }
         }
         #endregion

        #region Sales Return
         public ActionResult SalesReturn()
         {
             InvoiceReturnModels viewModel = new InvoiceReturnModels();
             objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                 viewModel.ModelList = _bllOrder.FuncReturnSelectChallanModelList("0");
                 viewModel.TranDate = objDaoUserInfo.BusinessDate;
                 //Session["TempOrder"] = null;
                 HttpContext.Session.SetString("TempOrder", "");

                ModelState.Clear();
                 ViewData.Model = viewModel;
                 return View();
             }
             else
             {
                 return RedirectToAction("Index", "Home");
             }
         }
        
         public JsonResult JfuncInvoiceProductList(string Sel_StateName)
         {
            // JsonResult result = new JsonResult();
             var vList = _bllOrder.FuncReturnSelectInvoiceModelList(Sel_StateName);
            //result.Data = vList;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            return Json(vList.ToList());
        }

         public JsonResult JfuncItemInformation(String Sel_StateName)
         {
             String[] data = new string[2];
             data = _bllOrder.FuncReturnItemInformation(Sel_StateName);

             return Json(data);
         }

         [HttpPost]
         public ActionResult SalesReturn(InvoiceReturnModels viewModel, string returnUrl)
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
                         viewModel.NewChalanNo = _bllOrder.FuncReturnChallanTranNo(objDaoUserInfo);
                         _bllOrder.FuncWarrentyInfoEntry(viewModel, objDaoUserInfo);

                         viewModel.NewChalanNo = _bllOrder.FuncReturnChallanTranNo(objDaoUserInfo);
                         _bllOrder.FuncWarrantyNewInfoEntry(viewModel, objDaoUserInfo);

                         return SalesReturn();
                     }
                     catch (Exception ex)
                     {
                         return SalesReturn();
                     }
                 }
                 
                 else if (viewModel.Btn == "Clear")
                 {

                     return SalesReturn();
                 }
                 else if (viewModel.Btn == "Exit")
                 {
                     return RedirectToAction("Index", "Home");
                 }
                 else
                 {
                     return SalesReturn();
                 }
                 #endregion
             }

             else
             {
                 return RedirectToAction("Index", "Home");
             }

         }

         #endregion

        #region Chalan Modify
         public ActionResult ChallanModfy()
         {
             ChallanModels viewModel = new ChallanModels();
             objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("0");
                 //viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList("0");
                 viewModel.BrandList = _bllCommon.FuncReturnVenList("0");
                 viewModel.ModelList = _bllSettings.FuncReturnModelList("0");
                 //viewModel.ProductList = _bllSettings.FuncReturnProductList("0");
                 viewModel.ChallanDate = objDaoUserInfo.BusinessDate;
                 viewModel.ChallanIteamList = _bllOrder.FuncReturnChallanItemList(viewModel.ChallanNo);
                //Session["TempOrder"] = null;
                HttpContext.Session.SetString("TempOrder", "");
                //ViewData["ChallanSuccess"] =null;
                ModelState.Clear();
                 ViewData.Model = viewModel;
                 return View();
             }
             else
             {
                 return RedirectToAction("Index", "Home");
             }
         }

         public JsonResult JfuncChallanItemList(string Sel_StateName)
         {
             //JsonResult result = new JsonResult();
             var vList = _bllOrder.FuncReturnChallanItemList(Sel_StateName);
            //   result.Data = vList.ToList();
            //  result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //  return result;
            return Json(vList.ToList());
        }

         [HttpPost]
         public ActionResult ChallanModfy(ChallanModels viewModel, string returnUrl)
         {
             objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                #region save
                if (viewModel.Btn == "Add")
                 {
                     try
                     {
                             bool opStatus = _bllOrder.FuncChallanIteamEntry(viewModel, objDaoUserInfo);
                             if (opStatus)
                             {
                                 ViewData["ChallanSuccess"] = "1";
                                 ViewData["ChallanTranno"] = viewModel.ChallanNo;
                             }
                             else ViewData["ChallanSuccess"] = "3";
                         
                         return ChallanModfy();

                     }
                     catch (Exception ex)
                     {
                         ViewData["ChallanSuccess"] = "3";
                         return ChallanModfy();
                     }
                 }
                 else if (viewModel.Btn == "Search")
                 {
                     if (viewModel.ChallanIteam != null)
                     {
                         String[] strData = viewModel.ChallanIteam.Split('+');
                         viewModel.ChallanNo = strData[0];
                         viewModel.ModelCode = strData[1];
                         //viewModel.IteamSLNo = strData[2];

                         viewModel = _bllOrder.FuncSearchChallanItemInfo(viewModel);
                         if (viewModel.ChallanNo!=null)
                         {
                             viewModel.ChallanNo = viewModel.ChallanNo.Substring(4, 8);
                             viewModel.CustomerList = _bllSettings.FuncReturnCustomerList(viewModel.CustomerCode);
                             //viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                             viewModel.BrandList = _bllCommon.FuncReturnVenList(viewModel.BrandCode);
                             viewModel.ModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                             //viewModel.ProductList = _bllSettings.FuncReturnProductList(viewModel.ProductCode);
                             //viewModel.ChallanDate = objDaoUserInfo.BusinessDate;
                             viewModel.ChallanIteamList = _bllOrder.FuncReturnChallanItemList(viewModel.ChallanNo);
                             ModelState.Clear();
                             ViewData.Model = viewModel;
                             return View();
                         }
                         else
                         {
                             return ChallanModfy();
                         }
                     }
                     else return ChallanModfy();
                 }
                 else if (viewModel.Btn == "Delete")
                 {
                     bool opStatus = _bllOrder.FuncDeleteChallanItemInfo(viewModel);
                     if (opStatus)
                     {
                         ViewData["ChallanSuccess"] = "2";
                         return ChallanModfy();
                     }
                     else
                     {
                         return ChallanModfy();
                     }
                 }
                 else if (viewModel.Btn == "Clear")
                 {

                     return ChallanModfy();
                 }
                 else if (viewModel.Btn == "Exit")
                 {
                     return RedirectToAction("Index", "Home");
                 }
                 else
                 {
                     return ChallanModfy();
                 }
                 #endregion
             }

             else
             {
                 return RedirectToAction("Index", "Home");
             }

         }

         #endregion

        #region Invoice Modify
         public ActionResult InvoiceModify()
         {
             InvoiceModels viewModel = new InvoiceModels();
             objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.BrandList = _bllCommon.FuncReturnVenList("0");
                 viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("0");
                 //viewModel.CustChallanList = _bllOrder.FuncReturnSelectChallanList("");
                 viewModel.ModelList = _bllOrder.FuncReturnSelectChallanModelList("0");
                 //viewModel.IsVat = true;
                 viewModel.InvoiceDate = objDaoUserInfo.BusinessDate;
                 viewModel.EntryDate = objDaoUserInfo.BusinessDate;
                 viewModel.InvoiceItemList = _bllOrder.FuncReturnInvoiceItemList(viewModel.InvoiceNo);
                 //Session["TempOrder"] = null;
                 ModelState.Clear();
                 ViewData.Model = viewModel;
                 return View();
             }
             else
             {
                 return RedirectToAction("Index", "Home");
             }
         }

         public JsonResult JsonFuncSearchAllInvoice(String ids)
         {
             var data = new InvoiceModels();
             var OrderIteamModelList = new List<IteamModels>();

             OrderIteamModelList = _bllOrder.FuncReturnAllInvoic(ids);
             
             String[] rSumData = _bllOrder.FuncReturnSumData(OrderIteamModelList);
             rSumData[4] = _bllOrder.FuncReturnCustomerCode(ids);
             data.pDataList = rSumData;
             
             data.ChallanList = OrderIteamModelList;

             //Session["TempOrder"] = OrderIteamModelList;
            HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(OrderIteamModelList));

            return Json(data);
         }

         

         public JsonResult JsonFuncDeleteFromInvoice(String ids)
         {
             var data = new InvoiceModels();
             try
             {
                 //data[5] = "Operation Fail............";
                 var objIteamlList = new List<IteamModels>();
                 //objIteamlList = (List<IteamModels>)Session["TempOrder"];
                string json = HttpContext.Session.GetString("TempOrder");
                objIteamlList = JsonConvert.DeserializeObject<List<IteamModels>>(json);


                var objNewIteamlList = new List<IteamModels>();
                 objNewIteamlList = _bllOrder.FuncReturnDeleteInvoiceIteam(objIteamlList, ids);
                 data.ChallanList = objNewIteamlList;
                 //if (objNewIteamlList.Count > 0) data[5] = "Remove Successfull............";
                 String[] rSumData = _bllOrder.FuncReturnSumData(objNewIteamlList);
                 rSumData[4] = _bllOrder.FuncReturnCustomerCode(ids);
                 data.pDataList = rSumData;
                 //Session["TempOrder"] = objNewIteamlList;
                 HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(objNewIteamlList));
             }
             catch (Exception es)
             {

             }

             return Json(data);
         }

         public JsonResult JfuncInvoiceItemList(string Sel_StateName)
         {
            // JsonResult result = new JsonResult();
             var vList = _bllOrder.FuncReturnInvoiceItemList(Sel_StateName);
            //   result.Data = vList.ToList();
            //  result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            // return result;

            return Json(vList.ToList());
        }
         
         [HttpPost]
         public ActionResult InvoiceModify(InvoiceModels viewModel, string returnUrl)
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
                         viewModel.InvoiceNo = "INV-" + viewModel.InvoiceNo;
                         bool opStatus = _bllOrder.FuncInvoiceMasterUpdate(viewModel, objDaoUserInfo);
                         if (opStatus) ViewData["InvoiceSuccess"] = "1";

                        string jsonDataList = HttpContext.Session.GetString("TempOrder");                        

                        if (!string.IsNullOrEmpty(jsonDataList))
                         {
                           //  viewModel.ChallanList = (List<IteamModels>)Session["TempOrder"];
                            viewModel.ChallanList = JsonConvert.DeserializeObject<List<IteamModels>>(jsonDataList);

                            if (viewModel.ChallanList.Count > 0)
                             {
                                 if (opStatus) _bllOrder.FuncInvoiceChieldEntry(viewModel, objDaoUserInfo);
                                 if (opStatus)
                                 {

                                     _bllOrder.FuncChallanEntryFromInvoice(viewModel, objDaoUserInfo);
                                     ViewData["InvoiceSuccess"] = "1";
                                     ViewData["InvoiceTranno"] = viewModel.InvoiceNo.Substring(4);
                                 }
                                 else
                                 {
                                     _bllOrder.FuncInvoiceMasterDelete(viewModel, objDaoUserInfo);
                                     ViewData["InvoiceSuccess"] = "1";

                                 }
                             }

                         }
                         return InvoiceModify();
                     }
                     catch (Exception ex)
                     {
                         return InvoiceModify();
                     }
                 }                 
                 else if (viewModel.Btn == "Clear")
                 {

                     return InvoiceModify();
                 }
                 else if (viewModel.Btn == "Exit")
                 {
                     return RedirectToAction("Index", "Home");
                 }
                 else
                 {
                     return InvoiceModify();
                 }
                 #endregion
             }

             else
             {
                 return RedirectToAction("Index", "Home");
             }

         }

         #endregion

 }

}
