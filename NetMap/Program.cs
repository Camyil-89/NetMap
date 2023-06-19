using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NetMap
{
	public static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var app = new App();
			app.InitializeComponent();
			//app.DispatcherUnhandledException += App_DispatcherUnhandledException;
			Load();
			app.Run();
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
		private static void Load()
		{

		}
	}
}
