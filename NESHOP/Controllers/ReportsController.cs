using Microsoft.AspNetCore.Mvc;
using NeSHOP.DAL;
using System.Web;
using NeSHOP.Models;
using Newtonsoft.Json;
using NESHOP.Models;
using NESHOP.Contacts;
using Microsoft.AspNetCore.Authorization;
using NESHOP.Auth;
using Microsoft.Reporting.NETCore;
using System.IO;

namespace NeSHOP.Controllers
{
    [MyAuth]
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/
        private DaoUserInfo objDaoUserInfo = new DaoUserInfo();
        private TblAppinfoModel objAppinfo = new TblAppinfoModel();

        //private BllAdmin objBllAdmin = new BllAdmin();
        //BllTran _bllTran=new BllTran();
        //private BllCommon objBllCommon = new BllCommon();
        //private BllSettings _bllSettings = new BllSettings();
        //private BllTran _bllTran = new BllTran();
        //private BllOrder _bllOrder = new BllOrder();

        //private BllAppInfo objBllAppInfo = new BllAppInfo();
        //private BllIteam _bllIteam = new BllIteam();


        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReportsController> _logger;

        private IBllDbConnection _dbConn;

        private IBllUserInfo _bllUserInfo;
        private IBllIteam _bllIteam;
        private IBllCommon _bllCommon;
        private IBllSettings _bllSettings;
        private IBllOrder _bllOrder;
        private IBllTran _bllTran;
        private IBllAdmin _bllAdmin;
        


        public ReportsController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<ReportsController> logger, IBllDbConnection dbConn, IBllUserInfo bllUserInfo, IBllIteam bllIteam, IBllCommon bllCommon, IBllSettings bllSettings, IBllOrder bllOrder, IBllTran bllTran, IBllAdmin bllAdmin)
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
            _bllTran = bllTran;
            _bllAdmin = bllAdmin;
        }


        #region A/C Tran Report

        public ActionResult Rpt_AcTranReport(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                AcTranModel objActran = new AcTranModel();

                String[] qString = id.Split('+');
                if (qString[1] == "C")
                {
                    objActran.PTypeName = "Customer Transaction";
                    objActran.PType = "C";
                }
                else if (qString[1] == "V")
                {
                    objActran.PTypeName = "Vendor Transaction";
                    objActran.PType = "V";
                }
                else objActran.PTypeName = "All Transaction";

                objActran.CustomerCode = qString[2].ToString();
                objActran.TranDateFrom = Convert.ToDateTime(qString[3]);
                objActran.TranDateTo = Convert.ToDateTime(qString[4]);

                if (qString[0] == "2")
                {
                    objActran.AccountList = _bllTran.FuncReturnAccountDetailsData(objActran);
                    if (objActran.AccountList.Count > 0)
                    {
                        objAppinfo = new TblAppinfoModel();
                        objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                        LocalReport localReport = new LocalReport();
                        var contentPath = _environment.WebRootPath;
                        var path = Path.Combine(contentPath, "Reports");
                        localReport.ReportPath = Path.Combine(path, "RdlRpt_AcTranReportDetails.rdlc"); //Server.MapPath("~/Views/Reports/RdlRpt_AcTranReportDetails.rdlc");
                        ReportDataSource reportDataSourceData = new ReportDataSource();
                        reportDataSourceData.Name = "DsAcTranReport";
                        reportDataSourceData.Value = objActran.AccountList;

                        //= new ReportDataSource("", objActran.AccountList);
                        localReport.DataSources.Add(reportDataSourceData);

                        CustomerModels CustomerStr = new CustomerModels();
                        CustomerStr = _bllSettings.FuncReturnCustomerInfo(objActran.CustomerCode);
                        //------------------Fixed -------------------
                        ReportParameter pInstName = new ReportParameter();
                        pInstName.Name = "P_InstName";
                        pInstName.Values.Add(objAppinfo.Instname);
                        localReport.SetParameters(pInstName);
                        ReportParameter pInstAddress = new ReportParameter();
                        pInstAddress.Name = "P_InstAddress";
                        pInstAddress.Values.Add(objAppinfo.Instaddress);
                        localReport.SetParameters(pInstAddress);
                        ReportParameter pReportName = new ReportParameter();
                        pReportName.Name = "P_ReportName";
                        pReportName.Values.Add("Account Details");
                        localReport.SetParameters(pReportName);
                        ReportParameter pBDate = new ReportParameter();
                        pBDate.Name = "P_BDate";
                        pBDate.Values.Add(DateTime.Now.ToString("dd/MM/yyyy"));
                        localReport.SetParameters(pBDate);

                        ReportParameter pCustomer = new ReportParameter();
                        pCustomer.Name = "P_Customer";
                        pCustomer.Values.Add(CustomerStr.CompanyName + "<br>" + CustomerStr.CompanyAddress);
                        localReport.SetParameters(pCustomer);
                        ReportParameter pAttantion = new ReportParameter();
                        pAttantion.Name = "P_Attantion";
                        pAttantion.Values.Add(CustomerStr.ContactPerson);
                        localReport.SetParameters(pAttantion);
                        //--------------------------------------------------------

                        string reportType = "pdf";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;

                        string deviceInfo =
                            "<DeviceInfo>" +
                            "  <OutputFormat>PDF</OutputFormat>" +
                            "  <PageWidth>8.27in</PageWidth>" +
                            "  <PageHeight>11in</PageHeight>" +
                            "  <MarginTop>0in</MarginTop>" +
                            "  <MarginLeft>1in</MarginLeft>" +
                            "  <MarginRight>0in</MarginRight>" +
                            "  <MarginBottom>0in</MarginBottom>" +
                            "</DeviceInfo>";
                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;

                        renderedBytes = localReport.Render(
                            reportType,
                            deviceInfo,
                            out mimeType,
                            out encoding,
                            out fileNameExtension,
                            out streams,
                            out warnings);

                        return File(renderedBytes, mimeType);
                    }
                    else return null;
                }
                else
                {
                    objActran.AccountList = _bllTran.FuncReturnAccountDataList(objActran);
                    if (objActran.AccountList.Count > 0)
                    {
                        objAppinfo = new TblAppinfoModel();
                        objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                        LocalReport localReport = new LocalReport();
                        ReportDataSource reportDataSourceData = new ReportDataSource();
                        var contentPath = _environment.WebRootPath;
                        var path = Path.Combine(contentPath, "Reports");

                        //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_AcTranReport.rdlc");
                        localReport.ReportPath = Path.Combine(path, "RdlRpt_AcTranReport.rdlc");

                        //reportDataSourceData = new ReportDataSource("DsAcTranReport", objActran.AccountList);
                        ////}

                        reportDataSourceData.Name = "DsAcTranReport";
                        reportDataSourceData.Value = objActran.AccountList;

                        localReport.DataSources.Add(reportDataSourceData);
                        //------------------Fixed -------------------
                        ReportParameter pInstName = new ReportParameter();
                        pInstName.Name = "P_InstName";
                        pInstName.Values.Add(objAppinfo.Instname);
                        localReport.SetParameters(pInstName);
                        ReportParameter pInstAddress = new ReportParameter();
                        pInstAddress.Name = "P_InstAddress";
                        pInstAddress.Values.Add(objAppinfo.Instaddress);
                        localReport.SetParameters(pInstAddress);
                        ReportParameter pReportName = new ReportParameter();
                        pReportName.Name = "P_ReportName";
                        pReportName.Values.Add(objActran.PTypeName);
                        localReport.SetParameters(pReportName);
                        ReportParameter pBDate = new ReportParameter();
                        pBDate.Name = "P_BDate";
                        pBDate.Values.Add(DateTime.Now.ToString("dd/MM/yyyy"));
                        localReport.SetParameters(pBDate);

                        ReportParameter pTranDateFrom = new ReportParameter();
                        pTranDateFrom.Name = "P_TranDateFrom";
                        pTranDateFrom.Values.Add(objActran.TranDateFrom.ToString("dd/MM/yyyy"));
                        localReport.SetParameters(pTranDateFrom);
                        ReportParameter pTranDateTo = new ReportParameter();
                        pTranDateTo.Name = "P_TranDateTo";
                        pTranDateTo.Values.Add(objActran.TranDateTo.ToString("dd/MM/yyyy"));
                        localReport.SetParameters(pTranDateTo);


                        //--------------------------------------------------------

                        string reportType = "pdf";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;

                        string deviceInfo =
                            "<DeviceInfo>" +
                            "  <OutputFormat>PDF</OutputFormat>" +
                            "  <PageWidth>8.27in</PageWidth>" +
                            "  <PageHeight>11in</PageHeight>" +
                            "  <MarginTop>0in</MarginTop>" +
                            "  <MarginLeft>1in</MarginLeft>" +
                            "  <MarginRight>0in</MarginRight>" +
                            "  <MarginBottom>0in</MarginBottom>" +
                            "</DeviceInfo>";
                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;

                        renderedBytes = localReport.Render(
                            reportType,
                            deviceInfo,
                            out mimeType,
                            out encoding,
                            out fileNameExtension,
                            out streams,
                            out warnings);

                        return File(renderedBytes, mimeType);
                    }
                    else return null;
                }
            }
            else return null;
        }

        #endregion

        #region Un used 

        public ActionResult Rpt_RptDailyBuyTran(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                ReportsModel objReportsModel = new ReportsModel();
                String[] qString = id.Split('-');
                objReportsModel.ReportName = "Buy Report ";

                objReportsModel.InstInfoDetails = _bllAdmin.FuncReturnInstInformation(objDaoUserInfo.InstCode);
                objReportsModel.InstInfoDetails.DateFrom = qString[1] + "-" + qString[2] + "-" + qString[3];
                objReportsModel.InstInfoDetails.DateTo = qString[4] + "-" + qString[5] + "-" + qString[6];
                //-----------Report Body----------------
                //objReportsModel.BuyTranData = _bllTran.FuncBuyTranData(qString);
                objReportsModel.BuyTranDataSum = _bllTran.FuncBuyTranDataSum(objReportsModel.BuyTranData);

                ModelState.Clear();
                ViewData.Model = objReportsModel;
            }
            return View();
        }

        //
        // GET: /Reports/Create

        public ActionResult Rpt_RptDailySalesTran(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                ReportsModel objReportsModel = new ReportsModel();
                String[] qString = id.Split('-');
                objReportsModel.ReportName = "Sales Report ";

                objReportsModel.InstInfoDetails = _bllAdmin.FuncReturnInstInformation(objDaoUserInfo.InstCode);
                objReportsModel.InstInfoDetails.DateFrom = qString[1] + "-" + qString[2] + "-" + qString[3];
                objReportsModel.InstInfoDetails.DateTo = qString[4] + "-" + qString[5] + "-" + qString[6];
                //-----------Report Body----------------
                //objReportsModel.BuyTranData = _bllTran.FuncBuyTranData(qString);
                objReportsModel.BuyTranDataSum = _bllTran.FuncBuyTranDataSum(objReportsModel.BuyTranData);

                ModelState.Clear();
                ViewData.Model = objReportsModel;
            }
            return View();
        }

        ////
        //// POST: /Reports/Create

        public ActionResult Rpt_RptDailyProdInTran(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                ReportsModel objReportsModel = new ReportsModel();
                String[] qString = id.Split('-');
                objReportsModel.ReportName = "Production In Report ";

                objReportsModel.InstInfoDetails = _bllAdmin.FuncReturnInstInformation(objDaoUserInfo.InstCode);
                objReportsModel.InstInfoDetails.DateFrom = qString[1] + "-" + qString[2] + "-" + qString[3];
                objReportsModel.InstInfoDetails.DateTo = qString[4] + "-" + qString[5] + "-" + qString[6];
                //-----------Report Body----------------
                //objReportsModel.BuyTranData = _bllTran.FuncBuyTranData(qString);
                objReportsModel.BuyTranDataSum = _bllTran.FuncBuyTranDataSum(objReportsModel.BuyTranData);

                ModelState.Clear();
                ViewData.Model = objReportsModel;
            }
            return View();
        }

        ////
        //// GET: /Reports/Edit/5

        public ActionResult Rpt_RptDailyProdOutTran(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);


                ReportsModel objReportsModel = new ReportsModel();
                String[] qString = id.Split('-');
                objReportsModel.ReportName = "Production Out Report ";

                objReportsModel.InstInfoDetails = _bllAdmin.FuncReturnInstInformation(objDaoUserInfo.InstCode);
                objReportsModel.InstInfoDetails.DateFrom = qString[1] + "-" + qString[2] + "-" + qString[3];
                objReportsModel.InstInfoDetails.DateTo = qString[4] + "-" + qString[5] + "-" + qString[6];
                //-----------Report Body----------------
                //objReportsModel.BuyTranData = _bllTran.FuncBuyTranData(qString);
                objReportsModel.BuyTranDataSum = _bllTran.FuncBuyTranDataSum(objReportsModel.BuyTranData);

                ModelState.Clear();
                ViewData.Model = objReportsModel;
            }
            return View();
        }

        #endregion

        #region Stock Search Report

        public ActionResult Rpt_StockSearch(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                String[] qString = id.Split('+');
                List<IteamModels> objIteamList = new List<IteamModels>();
                IteamModels objIteamModels = new IteamModels();
                if (qString[0] == "1")
                    objIteamModels.SearchType = "1";
                else
                    objIteamModels.SearchType = "2";

                objIteamModels.ChallanNo = qString[1].ToString();
                //objIteamModels.IteamSLNo = qString[2].ToString();
                objIteamModels.TranDateFrom = Convert.ToDateTime(qString[2]);
                objIteamModels.TranDateTo = Convert.ToDateTime(qString[3]);
                //objIteamModels.CatagoryCode = qString[5].ToString();
                //objIteamModels.ProductCode = qString[6].ToString();
                objIteamModels.BrandCode = qString[4].ToString();
                objIteamModels.ModelCode = qString[5].ToString();

                objIteamList = _bllSettings.FuncReturnSearchIteamList(objIteamModels);
                if (objIteamList.Count > 0)
                {
                    LocalReport localReport = new LocalReport();

                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");
                    //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_StockSearch.rdlc");
                    localReport.ReportPath = Path.Combine(path, "RdlRpt_StockSearch.rdlc");
                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);

                    //ReportDataSource reportDataSourceData = new ReportDataSource("DataSetRdlRpt_StockSearch",
                    //                                                             objIteamList);

                    ReportDataSource reportDataSourceData = new ReportDataSource();
                    reportDataSourceData.Name = "DataSetRdlRpt_StockSearch";
                    reportDataSourceData.Value = objIteamList;

                    localReport.DataSources.Add(reportDataSourceData);
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);

                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    if (qString[0] == "1")
                        pReportName.Values.Add("Stock In");
                    else if (qString[0] == "2")
                        pReportName.Values.Add("Stock Out");
                    else if (qString[0] == "1")
                        pReportName.Values.Add("Current Stock");
                    localReport.SetParameters(pReportName);
                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(DateTime.ParseExact(qString[3], "yyyy-MM-dd", null).ToString("dd/MM/yyyy"));
                    localReport.SetParameters(pBDate);
                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else
                {
                    ViewData["Message"] = "Fail to View Report.........";
                    return View();
                }
            }
            else
            {
                ViewData["Message"] = "Fail To Connect With Server";
                return View();
            }
        }

        #endregion

        #region Stock Details Report

        public ActionResult Rpt_StockDetails(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                String[] qString = id.Split('+');
                List<IteamModels> objIteamList = new List<IteamModels>();
                IteamModels objIteamModels = new IteamModels();
                objIteamModels.SearchType = qString[0].ToString();
                //objIteamModels.ChallanNo = qString[1].ToString();
                //objIteamModels.IteamSLNo = qString[2].ToString();
                objIteamModels.TranDateFrom = Convert.ToDateTime(qString[1]);
                objIteamModels.TranDateTo = Convert.ToDateTime(qString[2]);
                //objIteamModels.CatagoryCode = qString[5].ToString();
                //objIteamModels.ProductCode = qString[6].ToString();
                objIteamModels.BrandCode = qString[3].ToString();
                objIteamModels.ModelCode = qString[4].ToString();

                objIteamList = _bllSettings.FuncReturnSearchStockReport(objIteamModels);
                if (objIteamList.Count > 0)
                {
                    LocalReport localReport = new LocalReport();

                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");
                    //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_StockDetails.rdlc");
                    localReport.ReportPath = Path.Combine(path, "RdlRpt_StockDetails.rdlc");
                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                    ReportDataSource reportDataSourceData = new ReportDataSource();
                    //ReportDataSource reportDataSourceData = new ReportDataSource("DataSetRdlRpt_StockDetails",
                    //                                                             objIteamList);

                    reportDataSourceData.Name = "DataSetRdlRpt_StockDetails";
                    reportDataSourceData.Value = objIteamList;

                    localReport.DataSources.Add(reportDataSourceData);
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);

                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    if (qString[0] == "1")
                        pReportName.Values.Add("Stock In");
                    else if (qString[0] == "2")
                        pReportName.Values.Add("Stock Out");
                    else if (qString[0] == "3")
                        pReportName.Values.Add("Current Stock");
                    localReport.SetParameters(pReportName);
                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(DateTime.ParseExact(qString[2], "yyyy-MM-dd", null).ToString("dd/MM/yyyy"));
                    localReport.SetParameters(pBDate);
                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else
                {
                    ViewData["Message"] = "Fail to View Report.........";
                    return View();
                }
            }
            else
            {
                ViewData["Message"] = "Fail To Connect With Server";
                return View();
            }
        }

        #endregion

        public ActionResult RptOrderTran()
        {
            ReportsModel objReportsModel = new ReportsModel();

            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                objReportsModel.OrdStatusList = _bllCommon.FuncIteamOrdStatusList("0");
                objReportsModel.FromDate = objDaoUserInfo.BusinessDate;
                objReportsModel.ToDate = objDaoUserInfo.BusinessDate;
                objReportsModel.ReportType = "1";
                ModelState.Clear();
                ViewData.Model = objReportsModel;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #region Voucher Report

        public ActionResult Rpt_VoucherReport(String id)
        {
            objDaoUserInfo = new DaoUserInfo();

            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                AcTranModel objIteam = new AcTranModel();

                String TranNo = id.ToString();

                objIteam = _bllTran.FuncTransactionInfo(TranNo);
                if (objIteam.TranNo != null)
                {
                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                    LocalReport localReport = new LocalReport();

                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");
                    //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_TranVoucher.rdlc");
                    localReport.ReportPath = Path.Combine(path, "RdlRpt_TranVoucher.rdlc");
                    //ReportDataSource reportDataSourceData = new ReportDataSource("DataSetRdlRpt_StockSearch", objIteam);
                    //localReport.DataSources.Add(reportDataSourceData);
                    //------------------Fixed -------------------
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);

                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(DateTime.Now.ToString("dd/MM/yyyy"));
                    localReport.SetParameters(pBDate);
                    //--------------------------------------------------------

                    //------------------Fixed -------------------
                    ReportParameter pTranNo = new ReportParameter();
                    pTranNo.Name = "P_TranNo";
                    pTranNo.Values.Add(TranNo);
                    localReport.SetParameters(pTranNo);
                    ReportParameter pInvoiceNo = new ReportParameter();
                    pInvoiceNo.Name = "P_InvoiceNo";
                    pInvoiceNo.Values.Add(objIteam.InvoiceNo);
                    localReport.SetParameters(pInvoiceNo);
                    ReportParameter pMoneyReceptNo = new ReportParameter();
                    pMoneyReceptNo.Name = "P_MoneyReceptNo";
                    pMoneyReceptNo.Values.Add(objIteam.MoneyReceptNo);
                    localReport.SetParameters(pMoneyReceptNo);
                    ReportParameter pTranDate = new ReportParameter();
                    pTranDate.Name = "P_TranDate";
                    pTranDate.Values.Add(objIteam.TranDatetemp);
                    localReport.SetParameters(pTranDate);

                    ReportParameter pAmount = new ReportParameter();
                    pAmount.Name = "P_Amount";
                    pAmount.Values.Add(objIteam.CollectedAmount.ToString());
                    localReport.SetParameters(pAmount);

                    ReportParameter pCollectBy = new ReportParameter();
                    pCollectBy.Name = "P_CollectedBy";
                    pCollectBy.Values.Add(objIteam.CollectedBy.ToString());
                    localReport.SetParameters(pCollectBy);

                    ReportParameter pCustomer = new ReportParameter();
                    pCustomer.Name = "P_Customer";
                    pCustomer.Values.Add(objIteam.Customer.ToString());
                    localReport.SetParameters(pCustomer);

                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    if (objIteam.TranType == "D")
                        pReportName.Values.Add("Payment Voucher");
                    else pReportName.Values.Add("Receive Voucher");
                    localReport.SetParameters(pReportName);

                    ReportParameter pPartyType = new ReportParameter();
                    pPartyType.Name = "P_PartyType";
                    if (objIteam.TranType == "D")
                        pPartyType.Values.Add("Paid To");
                    else pPartyType.Values.Add("Received From ");
                    localReport.SetParameters(pPartyType);


                    //--------------------------------------------------------
                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0in</MarginTop>" +
                        "  <MarginLeft>0in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else return null;
            }
            else return null;
        }

        #endregion

        #region Tran Saction Report

        public ActionResult Rpt_TransactionReport(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                AcTranModel objActran = new AcTranModel();
                String ReportName = "";
                String[] qString = id.Split('+');
                if (qString[0] == "2")
                {
                    objActran.TranType = "C";
                    ReportName = "Receive Transaction";
                }
                else if (qString[0] == "3")
                {
                    objActran.TranType = "D";
                    ReportName = "Payment Transaction";
                }

                objActran.TranNo = qString[1].ToString();
                objActran.IsActive = Convert.ToBoolean(qString[2]);
                objActran.TranDateFrom = Convert.ToDateTime(qString[3]);
                objActran.TranDateTo = Convert.ToDateTime(qString[4]);
                objActran.CustomerCode = qString[5].ToString();
                objActran.InvoiceNo = qString[6].ToString();
                List<AcTranModel> objTranList = new List<AcTranModel>();
                objTranList = _bllTran.FuncReturnSearchTranList(objActran);
                if (objTranList.Count > 0)
                {
                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                    LocalReport localReport = new LocalReport();

                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");

                    //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_TranReport.rdlc");

                    localReport.ReportPath = Path.Combine(path, "RdlRpt_TranReport.rdlc");

                    //ReportDataSource reportDataSourceData = new ReportDataSource("DSRdlRpt_TranReport", objTranList);
                    ReportDataSource reportDataSourceData = new ReportDataSource();
                    reportDataSourceData.Name = "DSRdlRpt_TranReport";
                    reportDataSourceData.Value = objTranList;

                    localReport.DataSources.Add(reportDataSourceData);
                    //------------------Fixed -------------------
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);
                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    pReportName.Values.Add("Transaction Voucher");
                    localReport.SetParameters(pReportName);
                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(DateTime.Now.ToString("dd/MM/yyyy"));
                    localReport.SetParameters(pBDate);
                    //--------------------------------------------------------

                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0in</MarginTop>" +
                        "  <MarginLeft>0in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else return null;
            }
            else return null;
        }

        #endregion

        #region Quatation Report

        public ActionResult Rpt_QutationSearch(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                OrderModels objOrder = new OrderModels();

                String[] qString = id.Split('-');

                objOrder.OrderRefNo = _bllOrder.FuncReturnOrdRefCode(objDaoUserInfo.InstCode) + "/" +
                                      qString[1].ToString();
                List<OrderModel> objTranList = new List<OrderModel>();

                objTranList = _bllOrder.FuncReturnQutationDetailsList(objOrder);
                if (objTranList.Count > 0)
                {
                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                    LocalReport localReport = new LocalReport();
                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");

                    if (qString[0] == "1")
                        //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_QutationReportWithHeader.rdlc");
                            localReport.ReportPath = Path.Combine(path, "RdlRpt_QutationReportWithHeader.rdlc");
                    else
                      //  localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_QutationReport.rdlc");
                            localReport.ReportPath = Path.Combine(path, "RdlRpt_QutationReport.rdlc");

                    //ReportDataSource reportDataSourceData = new ReportDataSource("DSRdlRpt_QutationWithHeader", objTranList);

                    ReportDataSource reportDataSourceData = new ReportDataSource();
                    reportDataSourceData.Name = "DSRdlRpt_QutationWithHeader";
                    reportDataSourceData.Value = objTranList;


                    localReport.DataSources.Add(reportDataSourceData);
                    List<TermsModel> objTermsList = new List<TermsModel>();
                    objTermsList = _bllOrder.FuncReturnTermsList(objOrder.OrderRefNo);
                    //ReportDataSource reportConditionData = new ReportDataSource("DSRdlRpt_Condition", objTermsList);

                    ReportDataSource reportConditionData = new ReportDataSource();
                    reportConditionData.Name = "DSRdlRpt_Condition";
                    reportConditionData.Value = objTermsList;


                    localReport.DataSources.Add(reportConditionData);

                    //------------------Fixed -------------------
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);
                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    pReportName.Values.Add("QUTATION");
                    localReport.SetParameters(pReportName);
                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(DateTime.Now.ToString("dd/MM/yyyy"));
                    localReport.SetParameters(pBDate);

                    //-------------------------------
                    ReportParameter pQutationNo = new ReportParameter();
                    pQutationNo.Name = "P_QutationNo";
                    pQutationNo.Values.Add(objTranList[0].OrderRefNo);
                    localReport.SetParameters(pQutationNo);

                    ReportParameter pCustomer = new ReportParameter();
                    pCustomer.Name = "P_Customer";
                    pCustomer.Values.Add(objTranList[0].Customer);
                    localReport.SetParameters(pCustomer);

                    ReportParameter pCustomerAddress = new ReportParameter();
                    pCustomerAddress.Name = "P_CustomerAddress";
                    pCustomerAddress.Values.Add(objTranList[0].CustomerAddress);
                    localReport.SetParameters(pCustomerAddress);

                    ReportParameter pQutationSub = new ReportParameter();
                    pQutationSub.Name = "P_QutationSub";
                    pQutationSub.Values.Add(objTranList[0].OrderSub);
                    localReport.SetParameters(pQutationSub);

                    ReportParameter pAmountinWard = new ReportParameter();
                    pAmountinWard.Name = "P_AmountinWard";
                    pAmountinWard.Values.Add(_bllCommon.funcAmountToWord(objTranList[0].tCostAmount));
                    localReport.SetParameters(pAmountinWard);
                    //---------------------------------------------------
                    ReportParameter pPrepiredBy = new ReportParameter();
                    pPrepiredBy.Name = "P_PrepiredBy";
                    pPrepiredBy.Values.Add(objDaoUserInfo.FullName);
                    localReport.SetParameters(pPrepiredBy);

                    ReportParameter pContact = new ReportParameter();
                    pContact.Name = "P_Contact";
                    pContact.Values.Add(objDaoUserInfo.EmpPhone);
                    localReport.SetParameters(pContact);
                    //--------------------------------------------------------

                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else return null;
            }
            else return null;
        }

        #endregion

        #region Challan Report

        public ActionResult Rpt_ChallanReport(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                ChallanModels objOrder = new ChallanModels();

                String[] qString = id.Split('-');

                objOrder.ChallanNo = "CHL-" + qString[1].ToString();
                //objOrder.TranDateFrom = Convert.ToDateTime(qString[2]);
                //objOrder.TranDateTo = objOrder.TranDateFrom;

                List<ChallanModel> objTranList = new List<ChallanModel>();

                objTranList = _bllOrder.FuncReturnChallanReport(objOrder);
                if (objTranList.Count > 0)
                {
                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                    LocalReport localReport = new LocalReport();
                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");
                    localReport.ReportPath = Path.Combine(path, "RdlRpt_ChallanRepot.rdlc");
                  //  ReportDataSource reportDataSourceData = new ReportDataSource("DSChallanRepot", objTranList);
                    ReportDataSource reportDataSourceData = new ReportDataSource();
                    reportDataSourceData.Name = "DSChallanRepot";
                    reportDataSourceData.Value = objTranList;

                    localReport.DataSources.Add(reportDataSourceData);
                    //------------------Fixed -------------------
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);
                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    pReportName.Values.Add("CHALLAN");
                    localReport.SetParameters(pReportName);
                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(objTranList[0].ChallanDate.ToString("dd/MM/yyyy"));
                    localReport.SetParameters(pBDate);

                    //-------------------------------
                    ReportParameter pQutationNo = new ReportParameter();
                    pQutationNo.Name = "P_QutationNo";
                    pQutationNo.Values.Add(objTranList[0].ChallanNo);
                    localReport.SetParameters(pQutationNo);

                    ReportParameter pCustomer = new ReportParameter();
                    pCustomer.Name = "P_Customer";
                    pCustomer.Values.Add(objTranList[0].Customer);
                    localReport.SetParameters(pCustomer);

                    ReportParameter pMessageInst = new ReportParameter();
                    pMessageInst.Name = "P_MessageInst";
                    pMessageInst.Values.Add("On Behalf of  " + objAppinfo.Instname);
                    localReport.SetParameters(pMessageInst);

                    ReportParameter pChallanPO = new ReportParameter();
                    pChallanPO.Name = "P_ChallanPO";
                    pChallanPO.Values.Add(objTranList[0].PONo);
                    localReport.SetParameters(pChallanPO);
                    //---------------------------------------------------
                    ReportParameter pPrepiredBy = new ReportParameter();
                    pPrepiredBy.Name = "P_PrepiredBy";
                    pPrepiredBy.Values.Add(objDaoUserInfo.FullName);
                    localReport.SetParameters(pPrepiredBy);

                    ReportParameter pContact = new ReportParameter();
                    pContact.Name = "P_Contact";
                    pContact.Values.Add(objDaoUserInfo.EmpPhone);
                    localReport.SetParameters(pContact);
                    //--------------------------------------------------------

                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else return null;
            }
            else return null;
        }


        #endregion

        #region Return Challan Report

        public ActionResult Rpt_ReturnChallanReport(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                ChallanModels objOrder = new ChallanModels();

                String[] qString = id.Split('-');

                objOrder.ChallanNo = "CHL-" + qString[1].ToString();
                //objOrder.TranDateFrom = Convert.ToDateTime(qString[2]);
                //objOrder.TranDateTo = objOrder.TranDateFrom;

                List<ChallanModel> objTranList = new List<ChallanModel>();

                objTranList = _bllOrder.FuncReturnSalesReturnChallanData(objOrder);
                if (objTranList.Count > 0)
                {
                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                    LocalReport localReport = new LocalReport();
                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");

                    //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_ChallanRepot.rdlc");
                    localReport.ReportPath = Path.Combine(path, "RdlRpt_ChallanRepot.rdlc");

                    //ReportDataSource reportDataSourceData = new ReportDataSource("DSChallanRepot", objTranList);

                    ReportDataSource reportDataSourceData = new ReportDataSource();
                    reportDataSourceData.Name = "DSChallanRepot";
                    reportDataSourceData.Value = objTranList;

                    localReport.DataSources.Add(reportDataSourceData);
                    //------------------Fixed -------------------
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);
                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    pReportName.Values.Add("SALES RETURN CHALLAN");
                    localReport.SetParameters(pReportName);
                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(objTranList[0].ChallanDate.ToString("dd/MM/yyyy"));
                    localReport.SetParameters(pBDate);

                    //-------------------------------
                    ReportParameter pQutationNo = new ReportParameter();
                    pQutationNo.Name = "P_QutationNo";
                    pQutationNo.Values.Add(objTranList[0].ChallanNo);
                    localReport.SetParameters(pQutationNo);

                    ReportParameter pCustomer = new ReportParameter();
                    pCustomer.Name = "P_Customer";
                    pCustomer.Values.Add(objTranList[0].Customer);
                    localReport.SetParameters(pCustomer);

                    ReportParameter pMessageInst = new ReportParameter();
                    pMessageInst.Name = "P_MessageInst";
                    pMessageInst.Values.Add("On Behalf of  " + objAppinfo.Instname);
                    localReport.SetParameters(pMessageInst);

                    ReportParameter pChallanPO = new ReportParameter();
                    pChallanPO.Name = "P_ChallanPO";
                    pChallanPO.Values.Add(objTranList[0].PONo);
                    localReport.SetParameters(pChallanPO);
                    //---------------------------------------------------
                    ReportParameter pPrepiredBy = new ReportParameter();
                    pPrepiredBy.Name = "P_PrepiredBy";
                    pPrepiredBy.Values.Add(objDaoUserInfo.FullName);
                    localReport.SetParameters(pPrepiredBy);

                    ReportParameter pContact = new ReportParameter();
                    pContact.Name = "P_Contact";
                    pContact.Values.Add(objDaoUserInfo.EmpPhone);
                    localReport.SetParameters(pContact);
                    //--------------------------------------------------------

                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else return null;
            }
            else return null;
        }

        #endregion

        #region Invoice Report

        public ActionResult Rpt_InvoiceReport(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                InvoiceModels objOrder = new InvoiceModels();

                String[] qString = id.Split('-');

                objOrder.InvoiceNo = qString[1].ToString();

                var objTranList = new List<InvoiceModel>();

                if (objOrder.InvoiceNo != "")
                {
                    objTranList = _bllOrder.FuncReturnInvoiceReport(objOrder);
                    if (objTranList.Count > 0)
                    {
                        objAppinfo = new TblAppinfoModel();
                        objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                        LocalReport localReport = new LocalReport();
                        var contentPath = _environment.WebRootPath;
                        var path = Path.Combine(contentPath, "Reports");

                        if (qString[0].ToString() == "1")
                        {
                            //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_InvoiceReport.rdlc");
                            localReport.ReportPath = Path.Combine(path, "RdlRpt_InvoiceReport.rdlc");
                            //ReportDataSource reportDataSourceData = new ReportDataSource("DSInvoiceReport", objTranList);

                            ReportDataSource reportDataSourceData = new ReportDataSource();
                            reportDataSourceData.Name = "DSInvoiceReport";
                            reportDataSourceData.Value = objTranList;

                            localReport.DataSources.Add(reportDataSourceData);
                        }
                        else
                        {
                           // localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_InvoicePrePadReport.rdlc");
                            localReport.ReportPath = Path.Combine(path, "RdlRpt_InvoicePrePadReport.rdlc");

                            //ReportDataSource reportDataSourceData = new ReportDataSource("DSInvoiceReport", objTranList);

                            ReportDataSource reportDataSourceData = new ReportDataSource();
                            reportDataSourceData.Name = "DSInvoiceReport";
                            reportDataSourceData.Value = objTranList;

                            localReport.DataSources.Add(reportDataSourceData);
                        }
                        //List<TermsModel> objTermsList = new List<TermsModel>();
                        //objTermsList = _bllOrder.FuncReturnTermsList(objOrder.ChallanNo);
                        //ReportDataSource reportConditionData = new ReportDataSource("DSRdlRpt_Condition", objTermsList);
                        //localReport.DataSources.Add(reportConditionData);

                        //------------------Fixed -------------------
                        ReportParameter pInstName = new ReportParameter();
                        pInstName.Name = "P_InstName";
                        pInstName.Values.Add(objAppinfo.Instname);
                        localReport.SetParameters(pInstName);
                        ReportParameter pInstAddress = new ReportParameter();
                        pInstAddress.Name = "P_InstAddress";
                        pInstAddress.Values.Add(objAppinfo.Instaddress);
                        localReport.SetParameters(pInstAddress);
                        ReportParameter pReportName = new ReportParameter();
                        pReportName.Name = "P_ReportName";
                        pReportName.Values.Add("INVOICE");
                        localReport.SetParameters(pReportName);


                        //-------------------------------
                        InvoiceModels objOrderMaster = new InvoiceModels();
                        objOrderMaster = _bllOrder.FuncReturnInvoiceReportMaster(objOrder.InvoiceNo);

                        ReportParameter pQutationNo = new ReportParameter();
                        pQutationNo.Name = "P_QutationNo";
                        pQutationNo.Values.Add(objOrderMaster.InvoiceNo);
                        localReport.SetParameters(pQutationNo);

                        ReportParameter pBDate = new ReportParameter();
                        pBDate.Name = "P_BDate";
                        pBDate.Values.Add(objOrderMaster.InvoiceDate.ToString("dd/MM/yyyy"));
                        localReport.SetParameters(pBDate);

                        ReportParameter pCustomer = new ReportParameter();
                        pCustomer.Name = "P_Customer";
                        pCustomer.Values.Add(objOrderMaster.Customer);
                        localReport.SetParameters(pCustomer);

                        ReportParameter pTVatAmount = new ReportParameter();
                        pTVatAmount.Name = "P_TVatAmount";
                        pTVatAmount.Values.Add(objOrderMaster.TVatAmount.ToString());
                        localReport.SetParameters(pTVatAmount);

                        ReportParameter pTTaxAmount = new ReportParameter();
                        pTTaxAmount.Name = "P_TTaxAmount";
                        pTTaxAmount.Values.Add(objOrderMaster.TTaxAmount.ToString());
                        localReport.SetParameters(pTTaxAmount);

                        ReportParameter pTCommAmount = new ReportParameter();
                        pTCommAmount.Name = "P_TDisAmount";
                        pTCommAmount.Values.Add(objOrderMaster.TDisAmount.ToString());
                        localReport.SetParameters(pTCommAmount);

                        ReportParameter pTAmount = new ReportParameter();
                        pTAmount.Name = "P_TAmount";
                        pTAmount.Values.Add(objOrderMaster.TAmount.ToString());
                        localReport.SetParameters(pTAmount);

                        ReportParameter pAdvanceAmount = new ReportParameter();
                        pAdvanceAmount.Name = "P_AdvanceAmount";
                        pAdvanceAmount.Values.Add(objOrderMaster.TransportAmount.ToString());
                        localReport.SetParameters(pAdvanceAmount);

                        ReportParameter pNetAmount = new ReportParameter();
                        pNetAmount.Name = "P_NetAmount";
                        pNetAmount.Values.Add(objOrderMaster.NetAmount.ToString());
                        localReport.SetParameters(pNetAmount);

                        ReportParameter pChallanNo = new ReportParameter();
                        pChallanNo.Name = "P_ChallanNo";
                        pChallanNo.Values.Add(objTranList[0].ChalanNo);
                        localReport.SetParameters(pChallanNo);

                        ReportParameter pChallanDate = new ReportParameter();
                        pChallanDate.Name = "P_ChallanDate";
                        pChallanDate.Values.Add(objTranList[0].ChallanDate.ToString("dd/MM/yyyy"));
                        localReport.SetParameters(pChallanDate);


                        ReportParameter pChallanPO = new ReportParameter();
                        pChallanPO.Name = "P_ChallanPO";
                        pChallanPO.Values.Add(objTranList[0].ChalanPO);
                        localReport.SetParameters(pChallanPO);

                        ReportParameter pAmountward = new ReportParameter();
                        pAmountward.Name = "P_AmountInWard";
                        pAmountward.Values.Add(_bllCommon.funcAmountToWord(objOrderMaster.NetAmount));
                        localReport.SetParameters(pAmountward);

                        ReportParameter pMessageInst = new ReportParameter();
                        pMessageInst.Name = "P_MessageInst";
                        pMessageInst.Values.Add("On Behalf of  " + objAppinfo.Instname);
                        localReport.SetParameters(pMessageInst);

                        ReportParameter pTaxDesc = new ReportParameter();
                        pTaxDesc.Name = "P_TaxDesc";
                        pTaxDesc.Values.Add("Vat & Tax");
                        localReport.SetParameters(pTaxDesc);

                        //---------------------------------------------------
                        ReportParameter pPrepiredBy = new ReportParameter();
                        pPrepiredBy.Name = "P_PrepiredBy";
                        pPrepiredBy.Values.Add(objDaoUserInfo.FullName);
                        localReport.SetParameters(pPrepiredBy);

                        ReportParameter pContact = new ReportParameter();
                        pContact.Name = "P_Contact";
                        pContact.Values.Add(objDaoUserInfo.EmpPhone);
                        localReport.SetParameters(pContact);
                        //--------------------------------------------------------

                        string reportType = "pdf";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;
                        string deviceInfo = "";
                        if (qString[0].ToString() == "1")
                        {
                            deviceInfo =
                                "<DeviceInfo>" +
                                "  <OutputFormat>PDF</OutputFormat>" +
                                "  <PageWidth>8.27in</PageWidth>" +
                                "  <PageHeight>11in</PageHeight>" +
                                "  <MarginTop>0in</MarginTop>" +
                                "  <MarginLeft>.7in</MarginLeft>" +
                                "  <MarginRight>0in</MarginRight>" +
                                "  <MarginBottom>0in</MarginBottom>" +
                                "</DeviceInfo>";
                        }
                        else
                        {
                            deviceInfo =
                                "<DeviceInfo>" +
                                "  <OutputFormat>PDF</OutputFormat>" +
                                "  <PageWidth>8.27in</PageWidth>" +
                                "  <PageHeight>11in</PageHeight>" +
                                "  <MarginTop>0in</MarginTop>" +
                                "  <MarginLeft>0.5in</MarginLeft>" +
                                "  <MarginRight>0in</MarginRight>" +
                                "  <MarginBottom>0in</MarginBottom>" +
                                "</DeviceInfo>";
                        }
                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;

                        renderedBytes = localReport.Render(
                            reportType,
                            deviceInfo,
                            out mimeType,
                            out encoding,
                            out fileNameExtension,
                            out streams,
                            out warnings);

                        return File(renderedBytes, mimeType);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    objTranList = _bllOrder.FuncReturnInvoiceListReport(objOrder);
                    if (objTranList.Count > 0)
                    {
                        objAppinfo = new TblAppinfoModel();
                        objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);
                        LocalReport localReport = new LocalReport();
                        var contentPath = _environment.WebRootPath;
                        var path = Path.Combine(contentPath, "Reports");

                        if (qString[0].ToString() == "1")
                        {
                            //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_InvoiceList.rdlc");
                            localReport.ReportPath = Path.Combine(path, "RdlRpt_InvoiceList.rdlc");
                            //ReportDataSource reportDataSourceData = new ReportDataSource("DSInvoiceReport", objTranList);
                            ReportDataSource reportDataSourceData = new ReportDataSource();
                            reportDataSourceData.Name = "DSInvoiceReport";
                            reportDataSourceData.Value = objTranList;

                            localReport.DataSources.Add(reportDataSourceData);
                        }
                        else
                        {
                            //localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_InvoicePrePadReport.rdlc");
                            localReport.ReportPath = Path.Combine(path, "RdlRpt_InvoicePrePadReport.rdlc");
                            //ReportDataSource reportDataSourceData = new ReportDataSource("DSInvoiceReport", objTranList);

                            ReportDataSource reportDataSourceData = new ReportDataSource();
                            reportDataSourceData.Name = "DSInvoiceReport";
                            reportDataSourceData.Value = objTranList;

                            localReport.DataSources.Add(reportDataSourceData);
                        }
                        //List<TermsModel> objTermsList = new List<TermsModel>();
                        //objTermsList = _bllOrder.FuncReturnTermsList(objOrder.ChallanNo);
                        //ReportDataSource reportConditionData = new ReportDataSource("DSRdlRpt_Condition", objTermsList);
                        //localReport.DataSources.Add(reportConditionData);

                        //------------------Fixed -------------------
                        ReportParameter pInstName = new ReportParameter();
                        pInstName.Name = "P_InstName";
                        pInstName.Values.Add(objAppinfo.Instname);
                        localReport.SetParameters(pInstName);
                        ReportParameter pInstAddress = new ReportParameter();
                        pInstAddress.Name = "P_InstAddress";
                        pInstAddress.Values.Add(objAppinfo.Instaddress);
                        localReport.SetParameters(pInstAddress);
                        ReportParameter pReportName = new ReportParameter();
                        pReportName.Name = "P_ReportName";
                        pReportName.Values.Add("INVOICE LIST");
                        localReport.SetParameters(pReportName);


                        //-------------------------------
                        InvoiceModels objOrderMaster = new InvoiceModels();
                        objOrderMaster = _bllOrder.FuncReturnInvoiceReportMaster(objOrder.InvoiceNo);
                        ReportParameter pQutationNo = new ReportParameter();
                        pQutationNo.Name = "P_QutationNo";
                        pQutationNo.Values.Add(objOrderMaster.InvoiceNo);
                        localReport.SetParameters(pQutationNo);

                        ReportParameter pBDate = new ReportParameter();
                        pBDate.Name = "P_BDate";
                        pBDate.Values.Add(objOrderMaster.InvoiceDate.ToString("dd/MM/yyyy"));
                        localReport.SetParameters(pBDate);

                        ReportParameter pCustomer = new ReportParameter();
                        pCustomer.Name = "P_Customer";
                        pCustomer.Values.Add(objOrderMaster.Customer);
                        localReport.SetParameters(pCustomer);

                        ReportParameter pTVatAmount = new ReportParameter();
                        pTVatAmount.Name = "P_TVatAmount";
                        pTVatAmount.Values.Add(objOrderMaster.TDisAmount.ToString());
                        localReport.SetParameters(pTVatAmount);

                        ReportParameter pTAmount = new ReportParameter();
                        pTAmount.Name = "P_TAmount";
                        pTAmount.Values.Add(objOrderMaster.TAmount.ToString());
                        localReport.SetParameters(pTAmount);

                        ReportParameter pAdvanceAmount = new ReportParameter();
                        pAdvanceAmount.Name = "P_AdvanceAmount";
                        pAdvanceAmount.Values.Add(objOrderMaster.TransportAmount.ToString());
                        localReport.SetParameters(pAdvanceAmount);

                        ReportParameter pNetAmount = new ReportParameter();
                        pNetAmount.Name = "P_NetAmount";
                        pNetAmount.Values.Add(objOrderMaster.NetAmount.ToString());
                        localReport.SetParameters(pNetAmount);

                        ReportParameter pChallanNo = new ReportParameter();
                        pChallanNo.Name = "P_ChallanNo";
                        pChallanNo.Values.Add(objTranList[0].ChalanNo);
                        localReport.SetParameters(pChallanNo);

                        ReportParameter pChallanDate = new ReportParameter();
                        pChallanDate.Name = "P_ChallanDate";
                        pChallanDate.Values.Add(objTranList[0].ChallanDate.ToString("dd/MM/yyyy"));
                        localReport.SetParameters(pChallanDate);


                        ReportParameter pChallanPO = new ReportParameter();
                        pChallanPO.Name = "P_ChallanPO";
                        pChallanPO.Values.Add(objTranList[0].ChalanPO);
                        localReport.SetParameters(pChallanPO);

                        ReportParameter pAmountward = new ReportParameter();
                        pAmountward.Name = "P_AmountInWard";
                        pAmountward.Values.Add(_bllCommon.funcAmountToWord(objOrderMaster.NetAmount));
                        localReport.SetParameters(pAmountward);

                        ReportParameter pMessageInst = new ReportParameter();
                        pMessageInst.Name = "P_MessageInst";
                        pMessageInst.Values.Add("On Behalf of  " + objAppinfo.Instname);
                        localReport.SetParameters(pMessageInst);

                        ReportParameter pTaxDesc = new ReportParameter();
                        pTaxDesc.Name = "P_TaxDesc";
                        pTaxDesc.Values.Add("Vat & Tax");
                        localReport.SetParameters(pTaxDesc);

                        //---------------------------------------------------
                        ReportParameter pPrepiredBy = new ReportParameter();
                        pPrepiredBy.Name = "P_PrepiredBy";
                        pPrepiredBy.Values.Add(objDaoUserInfo.FullName);
                        localReport.SetParameters(pPrepiredBy);

                        ReportParameter pContact = new ReportParameter();
                        pContact.Name = "P_Contact";
                        pContact.Values.Add(objDaoUserInfo.EmpPhone);
                        localReport.SetParameters(pContact);
                        //--------------------------------------------------------

                        string reportType = "pdf";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;
                        string deviceInfo = "";
                        if (qString[0].ToString() == "1")
                        {
                            deviceInfo =
                                "<DeviceInfo>" +
                                "  <OutputFormat>PDF</OutputFormat>" +
                                "  <PageWidth>8.27in</PageWidth>" +
                                "  <PageHeight>11in</PageHeight>" +
                                "  <MarginTop>0in</MarginTop>" +
                                "  <MarginLeft>.7in</MarginLeft>" +
                                "  <MarginRight>0in</MarginRight>" +
                                "  <MarginBottom>0in</MarginBottom>" +
                                "</DeviceInfo>";
                        }
                        else
                        {
                            deviceInfo =
                                "<DeviceInfo>" +
                                "  <OutputFormat>PDF</OutputFormat>" +
                                "  <PageWidth>8.27in</PageWidth>" +
                                "  <PageHeight>11in</PageHeight>" +
                                "  <MarginTop>0in</MarginTop>" +
                                "  <MarginLeft>0.5in</MarginLeft>" +
                                "  <MarginRight>0in</MarginRight>" +
                                "  <MarginBottom>0in</MarginBottom>" +
                                "</DeviceInfo>";
                        }
                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;

                        renderedBytes = localReport.Render(
                            reportType,
                            deviceInfo,
                            out mimeType,
                            out encoding,
                            out fileNameExtension,
                            out streams,
                            out warnings);

                        return File(renderedBytes, mimeType);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else return null;
        }

        #endregion

        #region Stock Warentry Replace Report

        public ActionResult Rpt_WarrantyReplaceReport(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                List<IteamModels> objIteamList = new List<IteamModels>();

                objIteamList = _bllSettings.FuncReturnReplaceIteamList();
                if (objIteamList.Count > 0)
                {
                    LocalReport localReport = new LocalReport();
                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");

                    // localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_StockWarranty.rdlc");
                    localReport.ReportPath = Path.Combine(path, "RdlRpt_StockWarranty.rdlc");

                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);

                    //ReportDataSource reportDataSourceData = new ReportDataSource("DataSetRdlRpt_StockDetails", objIteamList);

                    ReportDataSource reportDataSourceData = new ReportDataSource();
                    reportDataSourceData.Name = "DataSetRdlRpt_StockDetails";
                    reportDataSourceData.Value = objIteamList;

                    localReport.DataSources.Add(reportDataSourceData);
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);

                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    pReportName.Values.Add("Stok of Warranty Replace");
                    localReport.SetParameters(pReportName);
                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(DateTime.Today.ToString("dd/MM/yyyy"));
                    localReport.SetParameters(pBDate);
                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else
                {
                    ViewData["Message"] = "Fail To Connect With Server";
                    return View();
                }
            }
            else
            {
                ViewData["Message"] = "Fail To Connect With Server";
                return View();
            }
        }

        #endregion

        #region Stock Faulty  Report

        public ActionResult Rpt_StockFaultySearch(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                String[] qString = id.Split('+');
                List<IteamModels> objIteamList = new List<IteamModels>();
                IteamModels objIteamModels = new IteamModels();

                if (qString[0].ToString() == "undefine") objIteamModels.ModelCode = "";
                objIteamModels.ModelCode = qString[0].ToString();
                if (qString[1].ToString() == "undefine") objIteamModels.Quantity = 0;
                objIteamModels.Quantity = Convert.ToInt32(qString[1]);

                objIteamList = _bllSettings.FuncReturnSearchFultStockReport(objIteamModels);
                if (objIteamList.Count > 0)
                {
                    LocalReport localReport = new LocalReport();
                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");

                    // localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_FaultStock.rdlc");
                    localReport.ReportPath = Path.Combine(path, "RdlRpt_FaultStock.rdlc");

                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);

                    //ReportDataSource reportDataSourceData = new ReportDataSource("DataSetRdlRpt_StockDetails",
                    //                                                             objIteamList);

                    ReportDataSource reportDataSourceData = new ReportDataSource();
                    reportDataSourceData.Name = "DataSetRdlRpt_StockDetails";
                    reportDataSourceData.Value = objIteamList;

                    localReport.DataSources.Add(reportDataSourceData);
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);

                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    pReportName.Values.Add("Fault Stock Report");
                    localReport.SetParameters(pReportName);

                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(objDaoUserInfo.BusinessDate.ToString("dd/mm/yyyy"));
                    localReport.SetParameters(pBDate);
                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else
                {
                    ViewData["Message"] = "Fail to View Report.........";
                    return View();
                }
            }
            else
            {
                ViewData["Message"] = "Fail To Connect With Server";
                return View();
            }
        }

        #endregion

        #region Stock Duplicate  Report

        public ActionResult Rpt_StockDuplicateSearch(String id)
        {
            objDaoUserInfo = new DaoUserInfo();
            var jsonLogin = HttpContext.Session.GetString("loginStatus");
            if (!string.IsNullOrEmpty(jsonLogin))
            {
                objDaoUserInfo = JsonConvert.DeserializeObject<DaoUserInfo>(jsonLogin);

                String[] qString = id.Split('+');
                List<IteamModels> objIteamList = new List<IteamModels>();
                IteamModels objIteamModels = new IteamModels();

                if (qString[0].ToString() == "undefine") objIteamModels.ModelCode = "";
                objIteamModels.ModelCode = qString[0].ToString();
                if (qString[1].ToString() == "undefine") objIteamModels.Quantity = 0;
                objIteamModels.Quantity = Convert.ToInt32(qString[1]);

                objIteamList = _bllIteam.FuncReturnDuplicateIteamInfo(objIteamModels.ModelCode,
                                                                        objIteamModels.Quantity.ToString());
                if (objIteamList.Count > 0)
                {
                    LocalReport localReport = new LocalReport();
                    var contentPath = _environment.WebRootPath;
                    var path = Path.Combine(contentPath, "Reports");

                    // localReport.ReportPath = Server.MapPath("~/Views/Reports/RdlRpt_DuplicateStock.rdlc");
                    localReport.ReportPath = Path.Combine(path, "RdlRpt_DuplicateStock.rdlc");

                    objAppinfo = new TblAppinfoModel();
                    objAppinfo = _bllCommon.FuncReturnAppInformation(objDaoUserInfo.InstCode);

                    //ReportDataSource reportDataSourceData = new ReportDataSource("DataSetRdlRpt_StockDetails",
                    //                                                             objIteamList);

                    ReportDataSource reportDataSourceData = new ReportDataSource();
                    reportDataSourceData.Name = "DataSetRdlRpt_StockDetails";
                    reportDataSourceData.Value = objIteamList;

                    localReport.DataSources.Add(reportDataSourceData);
                    ReportParameter pInstName = new ReportParameter();
                    pInstName.Name = "P_InstName";
                    pInstName.Values.Add(objAppinfo.Instname);
                    localReport.SetParameters(pInstName);
                    ReportParameter pInstAddress = new ReportParameter();
                    pInstAddress.Name = "P_InstAddress";
                    pInstAddress.Values.Add(objAppinfo.Instaddress);
                    localReport.SetParameters(pInstAddress);

                    ReportParameter pReportName = new ReportParameter();
                    pReportName.Name = "P_ReportName";
                    pReportName.Values.Add("Duplicate Stock Report");
                    localReport.SetParameters(pReportName);

                    ReportParameter pBDate = new ReportParameter();
                    pBDate.Name = "P_BDate";
                    pBDate.Values.Add(objDaoUserInfo.BusinessDate.ToString("dd/mm/yyyy"));
                    localReport.SetParameters(pBDate);
                    string reportType = "pdf";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =
                        "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.27in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>0in</MarginRight>" +
                        "  <MarginBottom>0in</MarginBottom>" +
                        "</DeviceInfo>";
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = localReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    return File(renderedBytes, mimeType);
                }
                else
                {
                    ViewData["Message"] = "Fail to View Report.........";
                    return View();
                }
            }
            else
            {
                ViewData["Message"] = "Fail To Connect With Server";
                return View();
            }
        }

        #endregion


        public async Task<ActionResult> Rpt_BarcodeDownload(String id)
        {
            var contentPath = _environment.WebRootPath;

            var path = Path.Combine(contentPath, "barcodes", id + ".png");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            _logger.LogInformation(path);

            return File(memory, contentType: "image/png", Path.GetFileName(path));

        }
    }
}