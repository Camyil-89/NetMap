using Microsoft.Extensions.DependencyInjection;
using NetMap.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetMap.Service.Net
{
	public static class ScannerLocalNetwork
	{
		private static MainVM MainVM => App.Host.Services.GetRequiredService<MainVM>();
		private static int PingSend = 0;
		private static int PingComplete = 0;
		private static bool IsAbort = false;
		public static void ScanNetwork()
		{
			string subnet = "192.168";
			MainVM.EnableButtonClear = false;
			MainVM.TraceButtonEnable = false;
			MainVM.TextButtonScanNetwork = "Отмена";
			StatusBarProvider.ShowMessage("Сканирование локальной сети...");
			PingSend = 0;
			PingComplete = 0;

			Task.Run(() =>
			{
				int actet_count = subnet.Split(".").Length;
				int count_addresses = (int)Math.Pow(256, 4 - actet_count);
				Console.WriteLine(count_addresses);
				byte[] address = new byte[4] { 0, 0, 0, 0 };
				int index = 0;
				foreach (var i in subnet.Split("."))
				{
					address[index] = byte.Parse(i);
					index++;
				}
				while (count_addresses != PingSend && IsAbort == false)
				{
					string ipAddress = string.Join(".", address);
					address = IncrementAddress(address);
					Ping ping = new Ping();
					ping.PingCompleted += Ping_PingCompleted;
					ping.SendAsync(ipAddress, 1000, ipAddress);
					PingSend++;
				}
			});
			Task.Run(() =>
			{
				Thread.Sleep(500);
				while (PingSend != PingComplete && IsAbort == false)
				{
					Thread.Sleep(16);
					MainVM.EnableButtonClear = false;
					MainVM.TraceButtonEnable = false;
				}
				MainVM.EnableButtonClear = true;
				MainVM.TraceButtonEnable = true;
				MainVM.TextButtonScanNetwork = "Сканировать сеть";
				StatusBarProvider.CloseMessage();
			});
		}
		public static void StopScanNetwork()
		{
			IsAbort = true;
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

		private static void Ping_PingCompleted(object sender, PingCompletedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => { PingComplete++; });
			string ipAddress = (string)e.UserState;
			if (e.Reply != null && e.Reply.Status == IPStatus.Success && IsAbort == false)
			{
				TraceRouteProvider.StartTrace(ipAddress);
				Console.WriteLine($">{ipAddress}");
				StatusBarProvider.ShowMessage($"Сканирование локальной сети... [{ipAddress} | {PingComplete}]");
			}
		}
	}
}
