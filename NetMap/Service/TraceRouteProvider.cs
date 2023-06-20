using Microsoft.Extensions.DependencyInjection;
using NetMap.Models;
using NetMap.Models.Net;
using NetMap.Service.Route;
using NetMap.ViewModels.Windows;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetMap.Service
{
	public static class TraceRouteProvider
	{
		private static TraceRouteItem EntryMain;
		private static string TargetAddress = "";
		private static MainVM MainVM => App.Host.Services.GetRequiredService<MainVM>();
		public static string GetIPAddressFromDNS(string hostname)
		{
			try
			{
				IPAddress.Parse(hostname);
				return hostname;
			}
			catch { }
			try
			{
				var ip = Dns.GetHostEntry(hostname).AddressList[0];
				return string.Join(".", ip.ToString().Split('.'));
			}
			catch { }
			return null;
		}
		public static string IsDNS(string hostname)
		{
			try
			{
				IPAddress.Parse(hostname);
				return null;
			}
			catch { }
			return hostname;
		}
		public static void StartTrace(string address)
		{
			if (EntryMain == null)
				CreateMainRoute();
			MainVM.TextButtonTrace = "Отмена";
			MainVM.TraceMode = ModeTrace.Stop;
			MainVM.EnableButtonClear = false;
			MainVM.EnableScanButton = false;
			Task.Run(() =>
			{
				StatusBarProvider.ShowMessage("Разрешение доменных имен..");
				var ip = GetIPAddressFromDNS(address);
				StatusBarProvider.ShowMessage($"Поиск пути к {ip}");
				bool is_work = true;
				bool is_change = false;
				try
				{
					TargetAddress = ip;
					TraceSettings traceSettings = new TraceSettings()
					{
						TTL = Settings.Instance.Parametrs.TTL,
						TargetAddress = ip,
						TargetAddressDNS = IsDNS(address),
						Timeout = Settings.Instance.Parametrs.Timeout,
						MaxHops = Settings.Instance.Parametrs.TTL,
					};
					foreach (var route in RouteBuilder.TraceRoute(traceSettings))
					{
						App.Current.Dispatcher.Invoke(() =>
						{
							var find_route = RouteBuilder.FindRoute(route.ParentRoute.Address, EntryMain);
							var copy_route = route.Copy();
							find_route.AddChildrenRoute(copy_route);
							if (FindVertices(find_route) == null)
								MainVM.Graphs.AddVertex(find_route);
							if (FindVertices(copy_route) == null)
								MainVM.Graphs.AddVertex(copy_route);
							MainVM.Graphs.AddEdge(new Edge<object>(FindVertices(find_route), FindVertices(copy_route)));
						});
					}
				}
				catch (Exception ex) { MessageBox.Show($"Error:\n{ex.Message}"); }
				is_work = false;
				MainVM.EnableButtonClear = true;
				MainVM.EnableScanButton = true;
				MainVM.TraceMode = ModeTrace.Start;
				MainVM.TextButtonTrace = "Cтарт";
				StatusBarProvider.CloseMessage();
			});
		}
		private static object FindVertices(TraceRouteItem route)
		{
			return MainVM.Graphs.Vertices.FirstOrDefault((i) => ((TraceRouteItem)i).Address == route.Address);
		}
		public static void ReloadMap()
		{
			MainVM.Graphs = new BidirectionalGraph<object, IEdge<object>>();
			void Next(TraceRouteItem master, TraceRouteItem slave)
			{

				App.Current.Dispatcher.Invoke(() =>
				{
					if (FindVertices(master) == null)
						MainVM.Graphs.AddVertex(master);
					if (FindVertices(slave) == null)
						MainVM.Graphs.AddVertex(slave);
					MainVM.Graphs.AddEdge(new Edge<object>(FindVertices(master), FindVertices(slave)));
				});
				foreach (var i in slave.ChildrenRoutes)
				{
					Next(slave, i);
				}
			}
			foreach (var i in EntryMain.ChildrenRoutes)
			{
				Next(EntryMain, i);
			}
		}
		public static void StopTrace()
		{
			StatusBarProvider.ShowMessage("Остановка поиска пути..");
			MainVM.TraceButtonEnable = false;
			Task.Run(() =>
			{
				StopTraceAndWait();
				StatusBarProvider.CloseMessage();
				MainVM.TextButtonTrace = "Cтарт";
				MainVM.TraceButtonEnable = true;
				MainVM.TraceMode = ModeTrace.Start;
			});

		}
		private static void CreateMainRoute()
		{
			EntryMain = new TraceRouteItem()
			{
				Address = "127.0.0.1",
			};
		}
		public static void ClearGraphs()
		{
			CreateMainRoute();
			MainVM.Graphs.Clear();
			MainVM.Graphs = new BidirectionalGraph<object, IEdge<object>>();
			ReloadMap();
		}

		private static void StopTraceAndWait()
		{
			RouteBuilder.IsAbort = true;
			while (RouteBuilder.IsStop == false)
			{
				Thread.Sleep(16);
			}
			RouteBuilder.IsAbort = false;
		}
	}
}
