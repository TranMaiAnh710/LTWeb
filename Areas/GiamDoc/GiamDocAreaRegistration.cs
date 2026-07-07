using System.Web.Mvc;

namespace BTL.Areas.GiamDoc
{
    public class GiamDocAreaRegistration : AreaRegistration
    {
        public override string AreaName => "GiamDoc";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "GiamDoc_default",
                "GiamDoc/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
