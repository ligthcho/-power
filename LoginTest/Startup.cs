﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using LoginTest.PermissionMiddleware;

namespace LoginTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			 .AddCookie(options =>
			 {
				 options.LoginPath = new PathString("/login");
				 options.AccessDeniedPath = new PathString("/denied");
			 });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();
			//验证中间件
			app.UseAuthentication();
			////添加权限中间件, 一定要放在app.UseAuthentication后
			app.UsePermission(new PermissionMiddlewareOption()
			{
				LoginAction = @"/login",
				NoPermissionAction = @"/denied",
				//这个集合从数据库中查出所有用户的全部权限
				UserPerssions = new List<UserPermission>()
				 {
					 new UserPermission { Url="/", UserName="a"},
					 new UserPermission { Url="/home/contact", UserName="a"},
					 new UserPermission { Url="/home/about", UserName="b"},
					 new UserPermission { Url="/", UserName="b"}
				 }
			});
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
