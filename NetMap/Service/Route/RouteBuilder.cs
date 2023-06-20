using NetMap.Models;
using NetMap.Models.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetMap.Service.Route
{
    public static class RouteBuilder
    {
		public static bool IsStop = true;
		public static bool IsAbort = false;

		public static TraceRouteItem FindRoute(string Address, TraceRouteItem Master)
        {
			if (Master.Address == Address)
			{
				return Master;
			}
			foreach (var nextEntry in Master.ChildrenRoutes)
			{
				var result = FindRoute(Address, nextEntry);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		public static IEnumerable<TraceRouteItem> TraceRoute(TraceSettings traceSettings)
		{
			IsStop = false;
			TraceRouteItem Main = new TraceRouteItem() { Address = "127.0.0.1" };
			if (!IPAddress.TryParse(traceSettings.TargetAddress, out IPAddress address))
			{
				IsStop = true;
				throw new ArgumentException(string.Format("{0} is not a valid IP address.", traceSettings.TargetAddress));
			}

			// Max hops should be at least one or else there won't be any data to return.
			if (traceSettings.MaxHops < 1)
			{
				IsStop = true;
				throw new ArgumentException("Max hops can't be lower than 1.");
			}

			// Ensure that the timeout is not set to 0 or a negative number.
			if (traceSettings.Timeout < 1)
			{
				IsStop = true;
				throw new ArgumentException("Timeout value must be higher than 0.");
			}
			Ping ping = new Ping();
			PingOptions pingOptions = new PingOptions(1, true);
			Stopwatch pingReplyTime = new Stopwatch();
			PingReply reply;
			do
			{
				pingReplyTime.Start();
				reply = ping.Send(address, traceSettings.Timeout, new byte[] { 0 }, pingOptions);
				pingReplyTime.Stop();
				string hostname = string.Empty;
				string Dns_name = string.Empty;
				//if (reply.Address != null)
				//{
				//	try
				//	{
				//		IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
				//		hostname = ipHostInfo.HostName;                 //IPAddress ipA = ipHostInfo.AddressList.FirstOrDefault(a => a.Address
				//	}
				//	catch (SocketException) { Console.WriteLine("asda"); }
				//}
				var ent = new TraceRouteItem()
				{
					HopID = pingOptions.Ttl,
					Address = reply.Address == null ? "N/A" : reply.Address.ToString(),
					IsTarget = (reply.Address == null ? "N/A" : reply.Address.ToString()) == traceSettings.TargetAddress,
					ReplyTime = pingReplyTime.ElapsedMilliseconds,
					ReplyStatus = reply.Status,
					ParentRoute = Main,
					IpAddressTarget = traceSettings.TargetAddress,
					DnsAddressTarget = traceSettings.TargetAddressDNS,
				};
				Main.AddChildrenRoute(ent);
				Main = ent;
				pingOptions.Ttl++;
				pingReplyTime.Reset();
				yield return Main;
				if (IsAbort)
					break;
			}
			while (reply.Status != IPStatus.Success && pingOptions.Ttl <= traceSettings.MaxHops);
			IsStop = true;
		}
    }
}
