using System.Web;



namespace Flip.Common.Web.Web
{

	public interface IWebAccessor
	{

		HttpCookie GetCookieByName(string name);
		void RemoveCookie(string name);
		void SetCookie(HttpCookie cookie);

	}

}
