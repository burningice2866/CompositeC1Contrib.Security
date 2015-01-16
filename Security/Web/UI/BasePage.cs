using System.Web.UI;

namespace CompositeC1Contrib.Security.Web.UI
{
    public abstract class BasePage : Page
    {
        protected string EntityToken
        {
            get { return Request.QueryString["EntityToken"]; }
        }

        protected string ConsoleId
        {
            get { return Request.QueryString["consoleId"]; }
        }
    }
}
