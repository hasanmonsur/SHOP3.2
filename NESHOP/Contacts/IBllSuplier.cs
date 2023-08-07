using NeSHOP.Models;
using NESHOP.Models;

namespace NeSHOP.Contacts
{
    public interface IBllSuplier
    {
        string FuncIteamSuplierCode();
        string FuncReturnVendorCode(string productCode);
        VendorModels FuncReturnVendorInfo(string BrandCode);
        bool FuncSuplierEntry(DaoSuplier objDaoSuplier);
        bool FuncVendorDelete(VendorModels objDaoVendor);
        bool FuncVendorEntry(VendorModels objDaoVendor, DaoUserInfo objDaoUserInfo);
        bool FuncVendorUpdate(VendorModels objDaoVendor, DaoUserInfo objDaoUserInfo);
    }
}