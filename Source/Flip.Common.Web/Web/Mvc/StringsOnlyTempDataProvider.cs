using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace Flip.Common.Web.Web.Mvc
{

	public class StringsOnlyTempDataProvider : ITempDataProvider
	{

		public StringsOnlyTempDataProvider(IWebAccessor webAccessor)
		{
			_webAccessor = webAccessor;
		}



		public IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
		{
			HttpCookie cookie = _webAccessor.GetCookieByName(cookieName);
			if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
			{
				return ToDictionary(cookie.Values);
			}

			return new Dictionary<string, object>();
		}

		public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
		{
			if (values == null || values.Count == 0)
			{
				_webAccessor.RemoveCookie(cookieName);
			}
			else
			{
				_webAccessor.SetCookie(ToCookie(values));
			}
		}



		private static Dictionary<string, object> ToDictionary(NameValueCollection cookieValues)
		{
			return cookieValues.AllKeys.ToDictionary(key => key, key => (object)cookieValues[key], StringComparer.OrdinalIgnoreCase);
		}

		private HttpCookie ToCookie(IDictionary<string, object> values)
		{
			throw new NotImplementedException();
		}



		internal const string cookieName = "__ControllerTempData";
		private readonly IWebAccessor _webAccessor;

	}

}
