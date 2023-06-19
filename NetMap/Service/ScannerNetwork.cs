using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetMap.Service
{
	public class ScanInfo
	{

		public string Address = "";
	}
	public static class ScannerNetwork
	{
		public static List<ScanInfo> ScanInfos = new List<ScanInfo>();
		private static int SendPings = 0;
		private static int RecievePings = 0;
		public static void GetAddresses()
		{
			SendPings = 0;
			RecievePings = 0;
			for (int i = 0; i < 255; i++)
			{
				for (int j = 0; j < 255; j++)
				{
					PingHost($"10.102.{i}.{j}");
					Thread.Sleep(1000);
				}
				Console.WriteLine(i);
			}
			while(SendPings != RecievePings)
			{
				Console.WriteLine($"{RecievePings}\\{SendPings}");
				Thread.Sleep(1000);
			}
			Console.WriteLine("end");
		}
		static void PingHost(string ip)
		{
			using (Ping ping = new Ping())
			{
				ping.PingCompleted += PingCompletedCallback;
				ping.SendAsync(ip, 1000, ip);
				SendPings++;
			}
		}
		static void PingCompletedCallback(object sender, PingCompletedEventArgs e)
		{
			string ip = (string)e.UserState;
			RecievePings++;
			if (e.Reply != null && e.Reply.Status == IPStatus.Success)
			{
				ScanInfos.Add(new ScanInfo() { Address = ip });
				Console.WriteLine($"Host {ip} is reachable.");
			}
			else
			{
				Console.WriteLine($">>{ip}");
			}
		}
	}
}
