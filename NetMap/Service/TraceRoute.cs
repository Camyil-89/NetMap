using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using QuickGraph.Serialization;
using System.Windows.Media.Animation;

namespace NetMap.Service
{
	internal class TracertEntry
	{
		public int HopID { get; set; }
		public string Address { get; set; }
		public string Hostname { get; set; }
		public string Dns { get; set; }
		public bool Find { get; set; } = false;
		public long ReplyTime { get; set; }
		public IPStatus ReplyStatus { get; set; }
		public List<TracertEntry> Next { get; set; } = new List<TracertEntry>();
		public TracertEntry Back { get; set; } = null;

		public void AddNext(TracertEntry entry)
		{
			Console.WriteLine($"{Address}|{entry.Address}|{HopID}|{entry.HopID}");
			if (Next.FirstOrDefault((i) => i.Address == entry.Address) == null && Address != entry.Address)
			{
				Next.Add(entry);
			}
		}
		public static TracertEntry RecursiveAddressSearch(TracertEntry entry, string address)
		{
			if (entry.Address == address)
			{
				return entry;
			}

			foreach (var nextEntry in entry.Next)
			{
				var result = RecursiveAddressSearch(nextEntry, address);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}
		public static TracertEntry Copy(TracertEntry master, TracertEntry slave)
		{
			return new TracertEntry()
			{
				Address = slave.Address,
				Back = master,
				Dns = slave.Dns,
				Find = slave.Find,
				HopID = slave.HopID,
				Hostname = slave.Hostname,
				ReplyStatus = slave.ReplyStatus,
				ReplyTime = slave.ReplyTime
			};
		}
	}
	internal class TraceRoute
	{
		public static bool IsAbort = false;
		public static bool IsStop = true;
		public static IEnumerable<TracertEntry> Tracert(string ipAddress, int maxHops, int timeout)
		{
			TracertEntry Main = new TracertEntry() { Address = "127.0.0.1" };
			foreach (var i in Tracert(ipAddress, maxHops, timeout, Main))
				yield return i;
		}
		public static IEnumerable<TracertEntry> Tracert(string ipAddress, int maxHops, int timeout, TracertEntry Main)
		{
			IsStop = false;
			TracertEntry Entry = Main;
			Entry.ReplyStatus = IPStatus.Unknown;
			// Ensure that the argument address is valid.
			if (!IPAddress.TryParse(ipAddress, out IPAddress address))
			{
				IsStop = true;
				throw new ArgumentException(string.Format("{0} is not a valid IP address.", ipAddress));
			}

			// Max hops should be at least one or else there won't be any data to return.
			if (maxHops < 1)
			{
				IsStop = true;
				throw new ArgumentException("Max hops can't be lower than 1.");
			}

			// Ensure that the timeout is not set to 0 or a negative number.
			if (timeout < 1)
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
				reply = ping.Send(address, timeout, new byte[] { 0 }, pingOptions);
				pingReplyTime.Stop();
				string hostname = string.Empty;
				string Dns_name = string.Empty;
				if (reply.Address != null)
				{
					try
					{
						IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
						hostname = ipHostInfo.HostName;                 //IPAddress ipA = ipHostInfo.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
					}
					catch (SocketException) { Console.WriteLine("asda"); }
				}
				var ent = new TracertEntry()
				{
					HopID = pingOptions.Ttl,
					Address = reply.Address == null ? "N/A" : reply.Address.ToString(),
					Hostname = hostname,
					Find = (reply.Address == null ? "N/A" : reply.Address.ToString()) == ipAddress,
					ReplyTime = pingReplyTime.ElapsedMilliseconds,
					ReplyStatus = reply.Status,
					Dns = Dns_name,
					Back = Entry
				};
				AddNextEntry(Entry, ent);
				Entry = ent;
				pingOptions.Ttl++;
				pingReplyTime.Reset();
				yield return Entry;
				if (IsAbort)
					break;
			}
			while (reply.Status != IPStatus.Success && pingOptions.Ttl <= maxHops);
			IsStop = true;
		}

		public static void AddNextEntry(TracertEntry main, TracertEntry entry)
		{
			var find = TracertEntry.RecursiveAddressSearch(main, entry.Address);
			if (find == null)
			{
				main.AddNext(entry);
			}
		}
	}
}
