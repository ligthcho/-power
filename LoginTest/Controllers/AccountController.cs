using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoginTest.Controllers
{
	[Authorize]
	public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if(this.User.Identity.IsAuthenticated)
			{
				return RedirectPermanent(returnUrl);
			}
			return View();
		}
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string userName,string password,string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if(!string.IsNullOrEmpty(userName) && userName == password)
			{
				var claims = new List<Claim>(){
							  new Claim(ClaimTypes.Name,userName),new Claim("password",password),new Claim("realname","张龙豪")
						   };
				//init the identity instances 
				var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims,"Customer"));
				//signin 
				await HttpContext.SignInAsync("CookieAuth",userPrincipal,new AuthenticationProperties
				{
					ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
					IsPersistent = false,
					AllowRefresh = false
				});
				return RedirectPermanent(returnUrl);
			}
			else
			{
				ViewBag.ErrMsg = "UserName or Password is invalid";
				return View();
			}
		}
	}
}