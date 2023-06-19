using Microsoft.Extensions.DependencyInjection;
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
		private static TracertEntry EntryMain = new TracertEntry()
		{
			Address = "127.0.0.1\nЭто вы",
			ReplyStatus = System.Net.NetworkInformation.IPStatus.Unknown
		};
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
		public static void StartTrace(string address)
		{
			MainVM.TextButtonTrace = "Отмена";
			MainVM.TraceMode = ModeTrace.Stop;
			MainVM.EnableButtonClear = false;
			Task.Run(() =>
			{
				StatusBarProvider.ShowMessage("Разрешение доменных имен..");
				var ip = GetIPAddressFromDNS(address);
				StatusBarProvider.ShowMessage($"Поиск пути к {ip}");
				bool is_work = true;
				bool is_change = false;
				ReloadMap();
				Task.Run(() =>
				{
					while (is_work)
					{
						if (is_change)
						{
							try
							{
								ReloadMap();
							}
							catch { }
							is_change = false;
						}
						Thread.Sleep(16);
					}
					ReloadMap();
				});
				try
				{
					TargetAddress = ip;
					foreach (var i in TraceRoute.Tracert(ip, 100, 400))
					{
						StatusBarProvider.ShowMessage($"Поиск пути к {ip} | {i.Back.Address} > {i.Address}");

						var find = TracertEntry.RecursiveAddressSearch(EntryMain, i.Back.Address);
						if (find != null)
							find.AddNext(TracertEntry.Copy(find, i));
						else
							EntryMain.AddNext(TracertEntry.Copy(EntryMain, i));

						is_change = true;
					}
				}
				catch (Exception ex) { MessageBox.Show($"Error:\n{ex.Message}"); }
				is_work = false;
				MainVM.EnableButtonClear = true;
				MainVM.TraceMode = ModeTrace.Start;
				MainVM.TextButtonTrace = "Cтарт";
			});
		}
		public static void ReloadMap()
		{
			MainVM.Graphs = new BidirectionalGraph<object, IEdge<object>>();
			void Next(TracertEntry master, TracertEntry slave)
			{
				string node_1 = $"{master.Address}";
				string node_2 = "";

				if (master.Address == TargetAddress)
				{
					node_1 = $"{master.Address}\nКонечный узел";
				}

				if (slave.Address == TargetAddress)
					node_2 = $"{slave.Address}\nКонечный узел";
				else if (slave.Find)
					node_2 = $"{slave.Address}\nНеподтвержденный";
				else
					node_2 = $"{slave.Address}";


				App.Current.Dispatcher.Invoke(() =>
				{
					if (MainVM.Graphs.Vertices.Contains(node_1) == false)
						MainVM.Graphs.AddVertex(node_1);
					if (MainVM.Graphs.Vertices.Contains(node_2) == false)
						MainVM.Graphs.AddVertex(node_2);
					MainVM.Graphs.AddEdge(new Edge<object>(node_1, node_2));
				});
				foreach (var i in slave.Next)
				{
					Next(slave, i);
				}
			}
			foreach (var i in EntryMain.Next)
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
		public static void ClearGraphs()
		{
			EntryMain = new TracertEntry()
			{
				Address = "127.0.0.1\nЭто вы",
				ReplyStatus = System.Net.NetworkInformation.IPStatus.Unknown
			};
			MainVM.Graphs.Clear();
			MainVM.Graphs = new BidirectionalGraph<object, IEdge<object>>();
			ReloadMap();
		}

		private static void StopTraceAndWait()
		{
			TraceRoute.IsAbort = true;
			while (TraceRoute.IsStop == false)
			{
				Thread.Sleep(16);
			}
			TraceRoute.IsAbort = false;
		}
	}
}
