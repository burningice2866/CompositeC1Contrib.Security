using System.Web.UI;

namespace CompositeC1Contrib.Security.Web.UI
{
    public abstract class BasePage : Page
    {
        protected string EntityToken => Request.QueryString["EntityToken"];

        protected string ConsoleId => Request.QueryString["consoleId"];
    }
}
