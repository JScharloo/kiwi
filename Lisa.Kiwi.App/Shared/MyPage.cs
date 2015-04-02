using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Shared
{
	public class App 
	{
		public static Page GetMainPage()
		{
			var reports = _proxy.GetAsync();
			return new ContentPage {
				Content = new Label {
					Text = "vissen",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				},
			};
		}
		private static readonly Proxy<Report> _proxy = new Proxy<Report>("http://localhost:20151","/reports");
	}
}

