using Microsoft.AspNetCore.Html;

namespace NESHOP.Helpers
{
    public interface INotificationService
    {
        IHtmlContent ShowNotification(string message, string type = "info");
        IHtmlContent ShowNotificationFromCode(string msgCode);
    }
}
