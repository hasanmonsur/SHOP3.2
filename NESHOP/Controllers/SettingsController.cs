using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using BarcodeLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NeSHOP.Contacts;
using NeSHOP.Models;
using NESHOP.Auth;
using NESHOP.Contacts;
using NESHOP.Models;
using Newtonsoft.Json;


namespace NeSHOP.Controllers
{
    [MyAuth]
    public class SettingsController : Controller
    {
        DaoVendor objDaoVendor = new DaoVendor();
        DaoUserInfo objDaoUserInfo = new DaoUserInfo();
                

        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SettingsController> _logger;

        private IBllDbConnection _dbConn;

        private IBllCommon _bllCommon;
        private IBllSettings _bllSettings;
        private IBllSuplier _bllSuplier;
        //private IBllBarcode _bllBarcode;

        public SettingsController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<SettingsController> logger, IBllDbConnection dbConn, IBllCommon bllCommon, IBllSettings bllSettings, IBllSuplier bllSuplier)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
            _dbConn = dbConn;

            _bllCommon = bllCommon;
            _bllSettings = bllSettings;
            _bllSuplier = bllSuplier;
            //_bllBarcode = bllBarcode;
        }




        // POST: /Vendor Alls Operation
        #region Brand
        public ActionResult VendorInfo()
        {
            VendorModels objVendor = new VendorModels();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                objVendor.BrandList = _bllCommon.FuncReturnVenList("");
                objVendor.SupplierList = _bllSettings.FuncReturnSupplierList("");
                ModelState.Clear();
                ViewData.Model = objVendor;
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
            //result.value = vList.ToList();
           // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return Json(vList.ToList());
        }
        [HttpPost]
        public ActionResult VendorInfo(VendorModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    viewModel.BrandCode = _bllSuplier.FuncReturnVendorCode("");
                    bool enStatus = _bllSuplier.FuncVendorEntry(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnVenList("");
                        viewModel.SupplierList = _bllSettings.FuncReturnSupplierList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnVenList("");
                        viewModel.SupplierList = _bllSettings.FuncReturnSupplierList("");
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    if (Convert.ToInt32(viewModel.SearchKey) <= 0) viewModel.SearchKey = viewModel.BrandCode;
                    viewModel = _bllSuplier.FuncReturnVendorInfo(viewModel.SearchKey);
                    if (viewModel != null)
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnVenList(viewModel.BrandCode);
                        viewModel.SupplierList = _bllSettings.FuncReturnSupplierList(viewModel.SupplierCode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return VendorInfo();
                    }
                }
                else if (viewModel.Btn == "Update")
                {

                    bool enStatus = _bllSuplier.FuncVendorUpdate(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnVenList("");
                        viewModel.SupplierList = _bllSettings.FuncReturnSupplierList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnVenList("");
                        viewModel.SupplierList = _bllSettings.FuncReturnSupplierList("");
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return VendorInfo();
                }
                else if (viewModel.Btn == "Delete")
                {
                    bool enStatus = _bllSuplier.FuncVendorDelete(viewModel);

                    if (enStatus)
                    {
                        return VendorInfo();
                    }
                    else
                    {
                        return VendorInfo();
                    }
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return VendorInfo();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion 

        // POST: /Catagory Alls Operation
        #region Catagory
        public ActionResult CatagoryInfo()
        {
            CatagoryModels objCatagory = new CatagoryModels();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                objCatagory.CatagoryCode = _bllSettings.FuncReturnCatagoryCode();
                objCatagory.CatagoryList = _bllSettings.FuncReturnCatagoryList("");
                ModelState.Clear();
                ViewData.Model = objCatagory;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult CatagoryInfo(CatagoryModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {

                    bool enStatus = _bllSettings.FuncCatagoryEntry(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.CatagoryCode = _bllSettings.FuncReturnCatagoryCode();
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.CatagoryCode = _bllSettings.FuncReturnCatagoryCode();
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    if (Convert.ToInt32(viewModel.SearchKey) <= 0) viewModel.SearchKey = viewModel.CatagoryCode;
                    viewModel = _bllSettings.FuncReturnCatagoryInfo(viewModel.SearchKey);
                    if (viewModel != null)
                    {
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return CatagoryInfo();
                    }
                }
                else if (viewModel.Btn == "Update")
                {

                    bool enStatus = _bllSettings.FuncCatagoryUpdate(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.CatagoryCode = _bllSettings.FuncReturnCatagoryCode();
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.CatagoryCode = _bllSettings.FuncReturnCatagoryCode();
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return CatagoryInfo();
                }
                else if (viewModel.Btn == "Delete")
                {
                    bool enStatus = _bllSettings.FuncCatagoryDelete(viewModel);

                    if (enStatus)
                    {
                        return CatagoryInfo();
                    }
                    else
                    {
                        return CatagoryInfo();
                    }
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return CatagoryInfo();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion 


        // POST: /Product Alls Operation
        #region Product
        public ActionResult ProductInfo()
        {
            ProductModels objProduct = new ProductModels();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                //objProduct.ProductCode = _bllSettings.FuncReturnProductCode();
                objProduct.ProductList = _bllSettings.FuncReturnProductList("");

                //objProduct.CatagoryCode = _bllSettings.FuncReturnCatagoryCode();
                objProduct.CatagoryList = _bllSettings.FuncReturnCatagoryList("");
                ModelState.Clear();
                ViewData.Model = objProduct;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult ProductInfo(ProductModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    viewModel.ProductCode = _bllSettings.FuncReturnProductCode(viewModel.CatagoryCode);
                    bool enStatus = _bllSettings.FuncProductEntry(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        //objProduct.ProductCode = _bllSettings.FuncReturnProductCode();
                        viewModel.ProductList = _bllSettings.FuncReturnProductList("");
                        //viewModel.CatagoryCode = _bllSettings.FuncReturnCatagoryCode();
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.ProductList = _bllSettings.FuncReturnProductList(viewModel.ProductCode);
                        //viewModel.CatagoryCode = _bllSettings.FuncReturnCatagoryCode();
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    if (Convert.ToInt32(viewModel.SearchKey) <= 0) viewModel.SearchKey = viewModel.ProductCode;
                    viewModel = _bllSettings.FuncReturnProductInfo(viewModel.SearchKey);
                    if (viewModel != null)
                    {
                        viewModel.ProductList = _bllSettings.FuncReturnProductList(viewModel.ProductCode);
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return ProductInfo();
                    }
                }
                else if (viewModel.Btn == "Update")
                {

                    bool enStatus = _bllSettings.FuncProductUpdate(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {

                        viewModel.ProductList = _bllSettings.FuncReturnProductList("");
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.ProductList = _bllSettings.FuncReturnProductList(viewModel.ProductCode);
                        viewModel.CatagoryList = _bllSettings.FuncReturnCatagoryList(viewModel.CatagoryCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return ProductInfo();
                }
                else if (viewModel.Btn == "Delete")
                {
                    bool enStatus = _bllSettings.FuncProductDelete(viewModel);

                    if (enStatus)
                    {
                        return ProductInfo();
                    }
                    else
                    {
                        return ProductInfo();
                    }
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return ProductInfo();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion 

        // POST: /Vendor Alls Operation
        #region Model
        public ActionResult ModelInfo()
        {
            IteamModelModels objVendor = new IteamModelModels();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                objVendor.BrandList = _bllCommon.FuncReturnSelectVenList("");
                objVendor.IteamModelList = _bllSettings.FuncReturnModelList("");
                ModelState.Clear();               

                ViewData.Model = objVendor;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult JfuncVenList(string Sel_StateName)
        {
            //JsonResult result = new JsonResult();
            var vList = _bllCommon.FuncReturnSelectVenList(Sel_StateName);
            //result.Data = vList.ToList();
            // result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            
            return Json(vList.ToList());
        }

        //public string GenerateBarcode11(string SearchKey)
        //{
        //    var viewModel = _bllSettings.FuncReturnModelInfo(SearchKey);
        //    var barcodeText = viewModel.ModelCode + "|" + viewModel.ModelName;
        //    if(string.IsNullOrEmpty(SearchKey)) barcodeText="Test0002";

        //    //var writer = new BarcodeWriter<Bitmap>();
        //    //writer.Format = BarcodeFormat.CODE_128;

        //    //var barcodeBitmap = writer.Write(barcodeText);
        //    //var barcodeBytes = _bllBarcode.BitmapToBytes(barcodeBitmap);
        //    //return File(barcodeBytes, "image/png");

        //    try
        //    {
        //        GeneratedBarcode barcode = IronBarCode.BarcodeWriter.CreateBarcode(barcodeText, BarcodeWriterEncoding.Code128);
        //        barcode.ResizeTo(400, 120);
        //        barcode.AddBarcodeValueTextBelowBarcode();
        //        // Styling a Barcode and adding annotation text
        //        barcode.ChangeBarCodeColor(Color.BlueViolet);
        //        barcode.SetMargins(10);
        //        string path = Path.Combine(_environment.WebRootPath, "GeneratedBarcode");
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        string filePath = Path.Combine(_environment.WebRootPath, "GeneratedBarcode/barcode.png");
        //        barcode.SaveAsPng(filePath);
        //        string fileName = Path.GetFileName(filePath);
        //        string imageUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/GeneratedBarcode/" + fileName;
        //        ViewBag.QrCodeUri = imageUrl;

        //        return imageUrl;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;

        //        return "";
        //    }          

        //}

        [HttpPost]
        public ActionResult ModelInfo(IteamModelModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    viewModel.ModelCode = _bllSettings.FuncReturnModelCode(viewModel.BrandCode);
                    bool enStatus = _bllSettings.FuncModelEntry(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return ModelInfo();
                    }
                    else
                    {
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return ModelInfo();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    if (Convert.ToDouble(viewModel.SearchKey) <= 0) viewModel.SearchKey = viewModel.ModelCode;

                    viewModel = _bllSettings.FuncReturnModelInfo(viewModel.SearchKey);
                    if (viewModel != null)
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnSelectVenList(viewModel.BrandCode);
                        viewModel.IteamModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return ModelInfo();
                    }
                }
                else if (viewModel.Btn == "Barcode")
                {
                    if (Convert.ToDouble(viewModel.SearchKey) <= 0) viewModel.SearchKey = viewModel.ModelCode;

                    viewModel = _bllSettings.FuncReturnModelInfo(viewModel.SearchKey);

                    viewModel.BrandList = _bllCommon.FuncReturnSelectVenList(viewModel.BrandCode);
                    viewModel.IteamModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);


                    if (!string.IsNullOrEmpty(viewModel.ModelCode))
                    {
                        BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
                        {
                            IncludeLabel = true,
                            Alignment = AlignmentPositions.CENTER,
                            Width = 300,
                            Height = 100,
                            RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                            BackColor = Color.White,
                            ForeColor = Color.Black,
                        };

                        //string bracodeString = viewModel.ModelCode + "|" + viewModel.ModelName;
                        string bracodeString = viewModel.ModelCode;
                        if (bracodeString.Length > 39)
                            bracodeString = bracodeString.Substring(0, 39);

                        //bracodeString= bracodeString+ System.Environment.NewLine + "done";

                        //Image img = barcode.Encode(TYPE.CODE128B, viewModel.ModelCode);
                        Image img = barcode.Encode(TYPE.CODE128B, bracodeString);
           

                        string filePath = Path.Combine(_environment.WebRootPath, "barcodes\\"+ viewModel.ModelCode+".png");
                        img.Save(filePath);

                        string fileName = Path.GetFileName(filePath);
                        string imageUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/barcodes/" + fileName;
                        ViewBag.QrCodeUri = imageUrl;
                        viewModel.Barcodeurl = imageUrl;

                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return ModelInfo();
                    }
                }
                else if (viewModel.Btn == "Update")
                {

                    bool enStatus = _bllSettings.FuncModelUpdate(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnSelectVenList("");
                        viewModel.IteamModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                        viewModel.MsgCode = "1";
                        //ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnSelectVenList(viewModel.BrandCode);
                        viewModel.IteamModelList = _bllSettings.FuncReturnModelList("");
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View(viewModel);
                    }
                }
                
                else if (viewModel.Btn == "Clear")
                {
                    return ModelInfo();
                }
                else if (viewModel.Btn == "Delete")
                {
                    bool enStatus = _bllSettings.FuncModelDelete(viewModel);

                    if (enStatus)
                    {
                        return ModelInfo();
                    }
                    else
                    {
                        return ModelInfo();
                    }
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return ModelInfo();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion 

        // POST: /Catagory Alls Operation
        #region Customer Info
        public ActionResult CustomerInfo()
        {
            CustomerModels viewModel = new CustomerModels();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("");
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
        public ActionResult CustomerInfo(CustomerModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    viewModel.CustomerCode = _bllSettings.FuncReturnCustomerCode();
                    bool enStatus = _bllSettings.FuncCustomerEntry(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return CustomerInfo();
                    }
                    else
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnCustomerList(viewModel.CustomerCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return CustomerInfo();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    if (Convert.ToInt32(viewModel.SearchKey) <= 0) viewModel.SearchKey = viewModel.CustomerCode;
                    viewModel = _bllSettings.FuncReturnCustomerInfo(viewModel.SearchKey);
                    if (viewModel != null)
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnCustomerList(viewModel.CustomerCode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return CustomerInfo();
                    }
                }
                else if (viewModel.Btn == "Update")
                {

                    bool enStatus = _bllSettings.FuncCustomerUpdate(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnCustomerList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnCustomerList(viewModel.CustomerCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return CustomerInfo();
                }
                else if (viewModel.Btn == "Delete")
                {
                    bool enStatus = _bllSettings.FuncCustomerDelete(viewModel);

                    if (enStatus)
                    {
                        return CustomerInfo();
                    }
                    else
                    {
                        return CustomerInfo();
                    }
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return CustomerInfo();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion 


        #region TarmsInfo
        public ActionResult TarmsInfo()
        {
            TarmsModels objCatagory = new TarmsModels();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                objCatagory.TarmsCode = _bllSettings.FuncReturnTarmsCode();
                objCatagory.TarmsList = _bllSettings.FuncReturnTarmsList("");
                ModelState.Clear();
                ViewData.Model = objCatagory;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult TarmsInfo(TarmsModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {

                    bool enStatus = _bllSettings.FuncTarmsEntry(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.TarmsCode = _bllSettings.FuncReturnTarmsCode();
                        viewModel.TarmsList = _bllSettings.FuncReturnTarmsList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.TarmsCode = _bllSettings.FuncReturnTarmsCode();
                        viewModel.TarmsList = _bllSettings.FuncReturnTarmsList(viewModel.TarmsCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    if (Convert.ToInt32(viewModel.SearchKey) <= 0) viewModel.SearchKey = viewModel.TarmsCode;
                    viewModel = _bllSettings.FuncReturnTarmsInfo(viewModel.SearchKey);
                    if (viewModel != null)
                    {
                        viewModel.TarmsList = _bllSettings.FuncReturnTarmsList(viewModel.TarmsCode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return TarmsInfo();
                    }
                }
                else if (viewModel.Btn == "Update")
                {

                    bool enStatus = _bllSettings.FuncTarmsUpdate(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.TarmsCode = _bllSettings.FuncReturnTarmsCode();
                        viewModel.TarmsList = _bllSettings.FuncReturnTarmsList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.TarmsCode = _bllSettings.FuncReturnTarmsCode();
                        viewModel.TarmsList = _bllSettings.FuncReturnTarmsList(viewModel.TarmsCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return TarmsInfo();
                }
                else if (viewModel.Btn == "Delete")
                {
                    bool enStatus = _bllSettings.FuncTarmsDelete(viewModel);

                    if (enStatus)
                    {
                        return TarmsInfo();
                    }
                    else
                    {
                        return TarmsInfo();
                    }
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return TarmsInfo();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion 

        

        #region Supplier Info
        public ActionResult SupplierInfo()
        {
            SupplierModels viewModel = new SupplierModels();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                viewModel.CustomerList = _bllSettings.FuncReturnSupplierList("");
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
        public ActionResult SupplierInfo(SupplierModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    viewModel.SupplierCode = _bllSettings.FuncReturnSupplierCode();
                    bool enStatus = _bllSettings.FuncSupplierEntry(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnSupplierList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return SupplierInfo();
                    }
                    else
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnSupplierList(viewModel.SupplierCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return SupplierInfo();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    if (Convert.ToInt32(viewModel.SearchKey) <= 0) viewModel.SearchKey = viewModel.SupplierCode;
                    viewModel = _bllSettings.FuncReturnSupplierInfo(viewModel.SearchKey);
                    if (viewModel != null)
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnSupplierList(viewModel.SupplierCode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return SupplierInfo();
                    }
                }
                else if (viewModel.Btn == "Update")
                {

                    bool enStatus = _bllSettings.FuncSupplierUpdate(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnSupplierList("");
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.CustomerList = _bllSettings.FuncReturnSupplierList(viewModel.SupplierCode);
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return SupplierInfo();
                }
                else if (viewModel.Btn == "Delete")
                {
                    bool enStatus = _bllSettings.FuncSupplierDelete(viewModel);

                    if (enStatus)
                    {
                        return SupplierInfo();
                    }
                    else
                    {
                        return SupplierInfo();
                    }
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return SupplierInfo();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion 


        #region AddNewIteam
        public ActionResult AddNewIteam()
        {
            IteamModelModels objVendor = new IteamModelModels();
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                objVendor.BrandList = _bllCommon.FuncReturnSelectVenList("");
                objVendor.IteamModelList = _bllSettings.FuncReturnModelList("");
                ModelState.Clear();
                ViewData.Model = objVendor;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        
        [HttpPost]
        public ActionResult AddNewIteam(IteamModelModels viewModel, string returnUrl)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                if (viewModel.Btn == "Save")
                {
                    viewModel.ModelCode = _bllSettings.FuncReturnModelCode(viewModel.BrandCode);
                    bool enStatus = _bllSettings.FuncModelEntry(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.MsgCode = "1";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return AddNewIteam();
                    }
                    else
                    {
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return AddNewIteam();
                    }
                }
                else if (viewModel.Btn == "Search")
                {
                    if (Convert.ToDouble(viewModel.SearchKey) <= 0) viewModel.SearchKey = viewModel.ModelCode;

                    viewModel = _bllSettings.FuncReturnModelInfo(viewModel.SearchKey);
                    if (viewModel != null)
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnSelectVenList(viewModel.BrandCode);
                        viewModel.IteamModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        return AddNewIteam();
                    }
                }
                else if (viewModel.Btn == "Update")
                {

                    bool enStatus = _bllSettings.FuncModelUpdate(viewModel, objDaoUserInfo);

                    if (enStatus)
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnSelectVenList("");
                        viewModel.IteamModelList = _bllSettings.FuncReturnModelList(viewModel.ModelCode);
                        viewModel.MsgCode = "1";
                        //ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                    else
                    {
                        viewModel.BrandList = _bllCommon.FuncReturnSelectVenList(viewModel.BrandCode);
                        viewModel.IteamModelList = _bllSettings.FuncReturnModelList("");
                        viewModel.MsgCode = "3";
                        ModelState.Clear();
                        ViewData.Model = viewModel;
                        return View();
                    }
                }
                else if (viewModel.Btn == "Clear")
                {
                    return AddNewIteam();
                }
                else if (viewModel.Btn == "Delete")
                {
                    bool enStatus = _bllSettings.FuncModelDelete(viewModel);

                    if (enStatus)
                    {
                        return AddNewIteam();
                    }
                    else
                    {
                        return AddNewIteam();
                    }
                }
                else if (viewModel.Btn == "Exit")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return AddNewIteam();
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
