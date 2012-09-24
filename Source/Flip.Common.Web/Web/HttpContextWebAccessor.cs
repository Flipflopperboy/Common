using System;
using System.Web;



namespace Flip.Common.Web.Web
{

	public sealed class HttpContextWebAccessor : IWebAccessor
	{

		public HttpContextWebAccessor(HttpContextBase httpContext)
		{
			_httpContext = httpContext;
		}



		public HttpCookie GetCookieByName(string name)
		{
			return _httpContext.Request.Cookies[name];
		}

		public void RemoveCookie(string name)
		{
			if (_httpContext.Request.Cookies[name] != null)
			{
				var cookie = new HttpCookie(name);
				cookie.Expires = DateTime.Now.AddDays(-1);
				_httpContext.Request.Cookies.Add(cookie);
			}
		}

		public void SetCookie(HttpCookie cookie)
		{
			if (_httpContext.Request.Cookies[cookie.Name] != null)
			{
				_httpContext.Response.Cookies.Remove(cookie.Name);
			}
			_httpContext.Response.Cookies.Add(cookie);
		}



		private readonly HttpContextBase _httpContext;

	}

}
