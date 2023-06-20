using Microsoft.Extensions.Hosting;
using NetMap.Models;
using NetMap.Service.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NetMap
{
	public static class Program
	{
		public static string PathToSave = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\NetMap";
		[STAThread]
		public static void Main(string[] args)
		{
			Directory.CreateDirectory(PathToSave);
			var app = new App();
			app.InitializeComponent();
			//app.DispatcherUnhandledException += App_DispatcherUnhandledException;
			app.Exit += App_Exit;
			Load();
			app.Run();
			
		}

		private static void App_Exit(object sender, ExitEventArgs e)
		{
			Save();
		}

		private static void Load()
		{
			try
			{
				Settings.Instance.Parametrs = XMLProvider.Load<Parametrs>($"{Directory.GetCurrentDirectory()}\\settings.xml");
			} catch { }
		}
		private static void Save()
		{
			XMLProvider.Save<Parametrs>($"{Directory.GetCurrentDirectory()}\\settings.xml", Settings.Instance.Parametrs);
		}
		private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show($"Fatal Error:\n\n{e.Exception}");
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			var builder = Host.CreateDefaultBuilder(args);
			builder.UseContentRoot(Environment.CurrentDirectory);
			builder.ConfigureServices(App.ConfigureServices);

			return builder;
		}
	}
}
