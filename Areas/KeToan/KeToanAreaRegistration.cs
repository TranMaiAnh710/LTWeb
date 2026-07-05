using System.Web.Mvc;

namespace BTL.Areas.KeToan
{
    public class KeToanAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "KeToan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "KeToan_default",
                "KeToan/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}