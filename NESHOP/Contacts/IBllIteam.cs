using NeSHOP.Models;
using NESHOP.Models;

namespace NESHOP.Contacts
{
    public interface IBllIteam
    {
        void FuncInventoryUpdate(IteamModels objDaoIteam);
        bool FuncIteamDelete(IteamModels viewModel, DaoUserInfo objDaoUserInfo);
        bool FuncIteamDetailsUpdate(IteamModels objDaoIteam, DaoUserInfo objDaoUserInfo);
        bool FuncIteamFaultUpdate(IteamModels objDaoIteam, DaoUserInfo objDaoUserInfo);
        bool FuncIteamStockEntry(IteamModels objDaoIteam, DaoUserInfo objDaoUserInfo);
        List<IteamModels> FuncReturnDuplicateIteamInfo(string ModelCode, string iteamSlNo);
        IteamModels FuncReturnFaultIteamInfo(string ModelCode, string iteamSlNo);
        IteamModels FuncReturnInventoryInfo(string ModelCode);
        string FuncReturnIteamCode(string ModelCode);
        IteamModels FuncReturnIteamInfo(string IteamCode);
        void FuncUpdateInventoryInfo(IteamModels objDaoIteam);
    }
}