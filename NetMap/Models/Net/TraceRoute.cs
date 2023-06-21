using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetMap.Models.Net
{
	public class TraceRouteItem : Base.ViewModel.BaseViewModel
	{

		#region Address: Description
		/// <summary>Description</summary>
		private string _Address;
		/// <summary>Description</summary>
		public string Address { get => _Address; set => Set(ref _Address, value); }
		#endregion


		#region ReplyStatus: Description
		/// <summary>Description</summary>
		private IPStatus _ReplyStatus = IPStatus.Unknown;
		/// <summary>Description</summary>
		public IPStatus ReplyStatus { get => _ReplyStatus; set => Set(ref _ReplyStatus, value); }
		#endregion

		#region ReplyTime: Description
		/// <summary>Description</summary>
		private long _ReplyTime;
		/// <summary>Description</summary>
		public long ReplyTime { get => _ReplyTime; set => Set(ref _ReplyTime, value); }
		#endregion

		#region HopID: Description
		/// <summary>Description</summary>
		private int _HopID;
		/// <summary>Description</summary>
		public int HopID { get => _HopID; set => Set(ref _HopID, value); }
		#endregion

		#region IsTarget: Description
		/// <summary>Description</summary>
		private bool _IsTarget;
		/// <summary>Description</summary>
		public bool IsTarget { get => _IsTarget; set => Set(ref _IsTarget, value); }
		#endregion


		#region IpAddressTarget: Description
		/// <summary>Description</summary>
		private string _IpAddressTarget;
		/// <summary>Description</summary>
		public string IpAddressTarget { get => _IpAddressTarget; set => Set(ref _IpAddressTarget, value); }
		#endregion


		#region DnsAddressTarget: Description
		/// <summary>Description</summary>
		private string _DnsAddressTarget;
		/// <summary>Description</summary>
		public string DnsAddressTarget { get => _DnsAddressTarget; set => Set(ref _DnsAddressTarget, value); }
		#endregion

		#region ParentRoute: Description
		/// <summary>Description</summary>
		private TraceRouteItem _ParentRoute;
		/// <summary>Description</summary>
		public TraceRouteItem ParentRoute { get => _ParentRoute; set => Set(ref _ParentRoute, value); }
		#endregion


		#region ChildrenRoutes: Description
		/// <summary>Description</summary>
		private ObservableCollection<TraceRouteItem> _ChildrenRoutes = new ObservableCollection<TraceRouteItem>();
		/// <summary>Description</summary>
		public ObservableCollection<TraceRouteItem> ChildrenRoutes { get => _ChildrenRoutes; set => Set(ref _ChildrenRoutes, value); }
		#endregion
		public override string ToString()
		{
			var info = "";
			if (string.IsNullOrEmpty(DnsAddressTarget) == false && IsTarget)
				info = $" [{DnsAddressTarget}]";
			if (Address == "127.0.0.1")
				info = "\nЭто вы";
			else if (IsTarget)
				info += "\nКонечный узел";

			return $"{Address}{info}";
		}
		public void AddChildrenRoute(TraceRouteItem route)
		{
			if (ChildrenRoutes.FirstOrDefault(i => i.Address == route.Address) == null)
			{
				App.Current.Dispatcher.Invoke(() =>
				{
					ChildrenRoutes.Add(route);
				});

			}
		}

		public TraceRouteItem Copy() => new TraceRouteItem()
		{
			Address = Address,
			HopID = HopID,
			IsTarget = IsTarget,
			ParentRoute = ParentRoute,
			ReplyStatus = ReplyStatus,
			ReplyTime = ReplyTime,
			DnsAddressTarget = DnsAddressTarget,
			IpAddressTarget = IpAddressTarget,
		};
	}
}
