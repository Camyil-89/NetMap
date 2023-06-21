using Microsoft.Extensions.DependencyInjection;
using NetMap.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMap.Service.Notification
{
	public static class TitleProvider
	{
		private static MainVM MainVM => App.Host.Services.GetRequiredService<MainVM>();
		public static void ChangeTitle(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				MainVM.Title = "NetMap";
				return;
			} 
			MainVM.Title = $"NetMap | {text}";
		}
	}
}
