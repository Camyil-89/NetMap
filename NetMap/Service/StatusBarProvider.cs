using Microsoft.Extensions.DependencyInjection;
using NetMap.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMap.Service
{
	public static class StatusBarProvider
	{
		private static MainVM MainVM => App.Host.Services.GetRequiredService<MainVM>();
		public static void ShowMessage(string text)
		{
			MainVM.VisibilityBottomLeftStatus = System.Windows.Visibility.Visible;
			MainVM.TextStatusBottomLeft = text;

		}
		public static void CloseMessage()
		{
			MainVM.VisibilityBottomLeftStatus = System.Windows.Visibility.Collapsed;
		}
	}
}
