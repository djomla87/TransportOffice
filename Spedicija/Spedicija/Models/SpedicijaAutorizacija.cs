using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Spedicija.Models
{
     [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class SpedicijaAutorizacija : AuthorizeAttribute
    {

        private string[] UserProfilesRequired { get; set; }

        public SpedicijaAutorizacija(params object[] userProfilesRequired)
        {
            if (userProfilesRequired.Any(p => p.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("userProfilesRequired");

            this.UserProfilesRequired = userProfilesRequired.Select(p => Enum.GetName(p.GetType(), p)).ToArray();
        }

        public override void OnAuthorization(AuthorizationContext context)
        {

            bool authorized = false;

            String rola = this.Roles;
            uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

            Korisnik korisnik = db.Korisnik.Single(c => c.KorisnickoIme.Equals(HttpContext.Current.User.Identity.Name));


            if (korisnik.Role.Equals(rola))
                authorized = true;

            if (!authorized)
            {
                var url = new UrlHelper(context.RequestContext);
                var logonUrl = url.Action("AuthorizationFailed", "Home");
                context.Result = new RedirectResult(logonUrl);

                return;
            }
            



        }

         public static bool ImaRolu(String rola)
         {
             uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();
             int korisnik = db.Korisnik.Where(k => k.KorisnickoIme.Equals(HttpContext.Current.User.Identity.Name) && k.Role.Equals(rola)).Count();
             return (korisnik == 1);
         }

    }
}