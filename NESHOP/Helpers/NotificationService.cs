using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NESHOP.Helpers
{
    public class NotificationService : INotificationService
    {
        public IHtmlContent ShowNotification(string message, string type = "info")
        {
            var divTag = new TagBuilder("div");
            divTag.AddCssClass($"alert alert-{type} alert-dismissible fade show");
            divTag.Attributes.Add("role", "alert");
            divTag.Attributes.Add("style", "position: fixed; top: 20px; right: 20px; z-index: 1050; min-width: 300px;");

            var strongTag = new TagBuilder("strong");
            strongTag.InnerHtml.Append(GetNotificationTitle(type));

            var messageSpan = new TagBuilder("span");
            messageSpan.InnerHtml.Append(message);

            var buttonTag = new TagBuilder("button");
            buttonTag.AddCssClass("btn-close");
            buttonTag.Attributes.Add("type", "button");
            buttonTag.Attributes.Add("data-bs-dismiss", "alert");
            buttonTag.Attributes.Add("aria-label", "Close");

            divTag.InnerHtml.AppendHtml(strongTag);
            divTag.InnerHtml.AppendHtml(messageSpan);
            divTag.InnerHtml.AppendHtml(buttonTag);

            // Convert TagBuilder to HtmlString
            using var writer = new StringWriter();
            divTag.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }

        public IHtmlContent ShowNotificationFromCode(string msgCode)
        {
            return msgCode switch
            {
                "1" => ShowNotification("Item Entry Successful!", "success"),
                "2" => ShowNotification("Item Update Successful!", "success"),
                "3" => ShowNotification("Data Insert Failed. Please Check Valid Information!", "danger"),
                "4" => ShowNotification("Item Deleted Successfully!", "warning"),
                _ => ShowNotification("Operation completed!", "info")
            };
        }

        private string GetNotificationTitle(string type)
        {
            return type switch
            {
                "success" => "Success! ",
                "danger" => "Error! ",
                "warning" => "Warning! ",
                "info" => "Info! ",
                _ => ""
            };
        }
    }
}
