using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetMap.Service.Net
{
    public static class AdapterProvider
    {
		public static void GetAdapters()
		{
			StatusBarProvider.ShowInfoAboutAdapters("Получение информации об адаптерах...", "");
			string main_address = "";
			string info = "";
			string ip4 = "";
			string ip6 = "";

			NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in networkInterfaces)
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					IPInterfaceProperties properties = networkInterface.GetIPProperties();

					foreach (UnicastIPAddressInformation address in properties.UnicastAddresses)
					{
						if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
							ip4 = address.Address.ToString();
							if (main_address == "")
								main_address = $"{networkInterface.Name}:{ip4}";
						}
						else if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
						{
							ip6 = address.Address.ToString();
						}
					}
					PhysicalAddress physicalAddress = networkInterface.GetPhysicalAddress();
					byte[] bytes = physicalAddress.GetAddressBytes();
					string macAddress = BitConverter.ToString(bytes);
					info += $"{networkInterface.Name}:\n\tMAC: {macAddress}\n\tIPv4: {ip4}\n\tIPv6: {ip6}\n";
					StatusBarProvider.ShowInfoAboutAdapters($"{main_address}", info);
				}
			}
			StatusBarProvider.ShowInfoAboutAdapters($"{main_address}", info.Trim());
		}
    }
}
