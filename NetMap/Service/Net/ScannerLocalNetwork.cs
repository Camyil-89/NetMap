using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using NetMap.Models;
using NetMap.Service.Notification;
using NetMap.Service.Route;
using NetMap.ViewModels.Windows;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetMap.Service.Net
{
    public static class ScannerLocalNetwork
	{
		private static MainVM MainVM => App.Host.Services.GetRequiredService<MainVM>();
		private static int PingSend = 0;
		private static int PingComplete = 0;
		private static bool IsAbort = false;
		private static int CountStartTrace = 0;
		private static int CountEndTrace = 0;
		public static void ScanNetwork(byte[] address_min, byte[] address_max)
		{
			MainVM.EnableButtonClear = false;
			IsAbort = false;
			MainVM.TraceButtonEnable = false;
			MainVM.TextButtonScanNetwork = "Отмена";
			StatusBarProvider.ShowMessage("Сканирование локальной сети...");
			Stopwatch stopwatch = Stopwatch.StartNew();
			PingSend = 0;
			PingComplete = 0;
			CountStartTrace = 0;
			CountEndTrace = 0;
			bool IsSend = true;
			string subnet = $"{string.Join(".", address_min)}-{string.Join(".", address_max)}";
			int count_addresses = CountAddressesInRange(address_min, address_max);
			Task.Run(() =>
			{
				while (MinThenMax(address_min, address_max) && IsAbort == false)
				{
					string ipAddress = string.Join(".", address_min);
					address_min = IncrementAddress(address_min);
					Ping ping = new Ping();
					ping.PingCompleted += Ping_PingCompleted;
					ping.SendAsync(ipAddress, Settings.Instance.Parametrs.TimeoutPingLocalNetwork, ipAddress);
					PingSend++;
					if (Settings.Instance.Parametrs.GCCollectTimeout != 0 && PingSend % Settings.Instance.Parametrs.GCCollectTimeout == 0)
					{
						Thread.Sleep(100);
						GC.Collect();
					}
				}
				IsSend = false;
			});
			Task.Run(() =>
			{
				Thread.Sleep(500);
				int last_send = PingSend;
				int time_to_end = -1;
				Stopwatch stopwatch1 = Stopwatch.StartNew();
				while (IsAbort == false)
				{
					try
					{
						if (PingComplete >= PingSend)
							break;
						Thread.Sleep(100);
						MainVM.EnableButtonClear = false;
						MainVM.TraceButtonEnable = false;
						if (stopwatch1.ElapsedMilliseconds > 1000)
						{
							stopwatch1.Restart();
							time_to_end = ((count_addresses - last_send) / (PingSend - last_send));
							last_send = PingSend;
						}
						StatusBarProvider.ShowMessage($"Сканирование локальной сети... [{subnet}] [Найдено: {MainVM.ListNetAddresses.Count}] [Ответов: {PingComplete} Отправлено: {PingSend} ({Math.Round(((double)PingSend / (double)count_addresses) * 100, 1)}%) Всего: {count_addresses} ({Math.Round(stopwatch.Elapsed.TotalSeconds, 1)} сек.)] Осталось примерно: {time_to_end / 60 / 60} Ч {time_to_end / 60 % 60} М {time_to_end % 60} С");
					}
					catch { }
				}
				MainVM.EnableButtonClear = true;
				MainVM.TraceButtonEnable = true;
				MainVM.TextButtonScanNetwork = "Сканировать сеть";
				GC.Collect();
				StatusBarProvider.CloseMessage();
			});
		}
		public static void ScanPoolNetwork(IEnumerable<string> pool)
		{
			MainVM.EnableButtonClear = false;
			IsAbort = false;
			MainVM.TraceButtonEnable = false;
			MainVM.TextButtonScanNetwork = "Отмена";
			StatusBarProvider.ShowMessage("Сканирование пула адресов...");
			Task.Run(() =>
			{
				try
				{
					int count = 0;
					List<Task> tasks = new List<Task>();
					foreach (var address in pool)
					{
						count++;
						if (IsAbort)
							break;
						if (TraceRouteProvider.EntryMain == null)
							TraceRouteProvider.CreateMainRoute();

						StatusBarProvider.ShowMessage($"Сканирование пула адресов... [{address}] [{count}\\{pool.Count()}]");

						var ts = Task.Run(() =>
						{
							TraceSettings traceSettings = new TraceSettings()
							{
								TTL = Settings.Instance.Parametrs.TTL,
								TargetAddress = address,
								TargetAddressDNS = "",
								TraceMap = Settings.Instance.Parametrs.DynamicTraceMapOnScan,
								EnableReloadMap = Settings.Instance.Parametrs.DynamicTraceMapOnScan,
								Timeout = Settings.Instance.Parametrs.Timeout,
								MaxHops = Settings.Instance.Parametrs.TTL,
							};
							TraceRouteProvider.Trace(traceSettings);
						});
						tasks.Add(ts);
					}
					Task.Run(() =>
					{
						int count = 0;
						while (count < tasks.Count)
						{
							foreach (var i in tasks)
							{
								if (i.IsCompleted)
									count++;
							}
							Thread.Sleep(1000);
							StatusBarProvider.ShowMessage($"Сканирование пула адресов... [{count}\\{pool.Count()}]");
						}
						App.Current.Dispatcher.Invoke(() => { TraceRouteProvider.ReloadMap(); });
						MainVM.EnableButtonClear = true;
						MainVM.TraceButtonEnable = true;
						MainVM.TextButtonScanNetwork = "Сканировать сеть";
						StatusBarProvider.CloseMessage();
					});
				}
				catch (Exception ex) { Console.WriteLine(ex); }

			});
		}
		public static void StopScanNetwork()
		{
			IsAbort = true;
			Task.Run(() =>
			{

				RouteBuilder.IsAbort = true;
				int count = 0;
				while (count != 20)
				{
					StatusBarProvider.ShowMessage($"Остановка сканирования...");
					count++;
					Thread.Sleep(100);
					GC.Collect();
				}
				MainVM.EnableButtonClear = false;
				MainVM.TraceButtonEnable = false;
				MainVM.EnableScanButton = false;

				RouteBuilder.IsAbort = false;
				MainVM.EnableButtonClear = true;
				MainVM.TraceButtonEnable = true;
				MainVM.EnableScanButton = true;
				MainVM.TextButtonScanNetwork = "Сканировать сеть";
				StatusBarProvider.CloseMessage();
			});
		}
		public static byte[] ParseAddress(string address)
		{
			byte[] address_byte = new byte[4] { 0, 0, 0, 0 };
			int index = 0;
			foreach (var i in address.Split("."))
			{
				address_byte[index] = byte.Parse(i);
				index++;
			}
			return address_byte;
		}
		private static byte[] IncrementAddress(byte[] array, int index = 3)
		{
			if (index == -1)
			{
				array[0] = 0;
				return array;
			}
			if (array[index] == 255)
			{
				array[index] = 0;
				return IncrementAddress(array, index - 1);
			}
			array[index]++;
			return array;
		}
		public static int CountAddressesInRange(byte[] array_min, byte[] array_max)
		{
			var actet_1 = (int)((array_max[0] - array_min[0]) * Math.Pow(256, 3));
			var actet_2 = (int)((array_max[1] - array_min[1]) * Math.Pow(256, 2));
			var actet_3 = (int)((array_max[2] - array_min[2]) * Math.Pow(256, 1));
			var actet_4 = (array_max[3] - array_min[3]);
			return actet_1 + actet_2 + actet_3 + actet_4;
		}
		public static bool MinThenMax(byte[] array, byte[] array_max)
		{
			if (array[0] < array_max[0])
				return true;
			if (array[1] < array_max[1])
				return true;
			if (array[2] < array_max[2])
				return true;
			if (array[3] < array_max[3])
				return true;
			return false;
		}

		private static void Ping_PingCompleted(object sender, PingCompletedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => { PingComplete++; });
			string ipAddress = (string)e.UserState;
			if (e.Reply != null && e.Reply.Status == IPStatus.Success && IsAbort == false)
			{
				App.Current.Dispatcher.Invoke(() =>
				{
					if (MainVM.ListNetAddresses.Contains(ipAddress) == false)
						MainVM.ListNetAddresses.Add(ipAddress);
				});
				App.Current.Dispatcher.Invoke(() => { CountStartTrace++; });
				Task.Run(() =>
				{
					try
					{
						if (TraceRouteProvider.EntryMain == null)
							TraceRouteProvider.CreateMainRoute();
						TraceRouteProvider.Trace(ipAddress);
					}
					catch (Exception ex) { Console.WriteLine(ex); }
					App.Current.Dispatcher.Invoke(() => { CountEndTrace++; });
				});

			}
		}
	}
}
