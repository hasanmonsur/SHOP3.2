
using Microsoft.AspNetCore.Mvc.RazorPages;
using NESHOP.Contacts;

namespace NeSHOP.DAL
{
    public class BllDbConnection : IBllDbConnection
    {
        private readonly IConfiguration Configuration;

        public BllDbConnection(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string FunReturnConString()
        {
            string returnString = Configuration.GetConnectionString("DefaultConnection");

            return returnString;
        }

        public string FunReturnDbConnectionStringDb()
        {
            string returnString = Configuration.GetConnectionString("DefaultConnection");

            return returnString;
        }
    }
}
