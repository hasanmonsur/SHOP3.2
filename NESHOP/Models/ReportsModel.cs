using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NeSHOP.Models
{
    public class ReportsModel
    {
        public List<String> BuyTranDataSum { get; set; }
        public List<ChallanModels> BuyTranData { get; set; }

        [DisplayName("Iteam")]
        public string IteamCode { get; set; }
        public SelectList IteamCodeList { get; set; }

        [DisplayName("Ref. #")]
        [StringLength(15, ErrorMessage = "Must be less than 11 characters")]
        public string OrdTranNo { get; set; }


        [DisplayName("Report Type")]
        public string ReportType { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date From")]
        public DateTime FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date To")]
        public DateTime ToDate { get; set; }

        public string ReportName { get; set; }

        public TblAppinfoModel InstInfoDetails { get; set; }

        public String OrdStatus { get; set; }

        public SelectList OrdStatusList { get; set; }
    }

    public class TranDataSumList
    {
        
        [DisplayName("Total Iteam")]
        public string TotalIteam { get; set; }

        [DisplayName("Total Amount")]
        [StringLength(15, ErrorMessage = "Must be less than 11 characters")]
        public string TotalAmount { get; set; }

        
    }
}