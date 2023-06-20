using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMap.Models
{


	public class TraceSettings : Base.ViewModel.BaseViewModel
	{

		#region TTL: Description
		/// <summary>Description</summary>
		private byte _TTL = 100;
		/// <summary>Description</summary>
		public byte TTL { get => _TTL; set => Set(ref _TTL, value); }
		#endregion


		#region Timeout: Description
		/// <summary>Description</summary>
		private int _Timeout;
		/// <summary>Description</summary>
		public int Timeout
		{
			get => _Timeout; set
			{
				if (value < 1)
					value = 1;
				Set(ref _Timeout, value);
			}
		}
		#endregion


		#region MaxHops: Description
		/// <summary>Description</summary>
		private byte _MaxHops;
		/// <summary>Description</summary>
		public byte MaxHops { get => _MaxHops; set => Set(ref _MaxHops, value); }
		#endregion

		#region TargetAddress: Description
		/// <summary>Description</summary>
		private string _TargetAddress;
		/// <summary>Description</summary>
		public string TargetAddress { get => _TargetAddress; set => Set(ref _TargetAddress, value); }
		#endregion


		#region TargetAddressDNS: Description
		/// <summary>Description</summary>
		private string _TargetAddressDNS;
		/// <summary>Description</summary>
		public string TargetAddressDNS { get => _TargetAddressDNS; set => Set(ref _TargetAddressDNS, value); }
		#endregion

		#region TraceMap: Description
		/// <summary>Description</summary>
		private bool _TraceMap;
		/// <summary>Description</summary>
		public bool TraceMap { get => _TraceMap; set => Set(ref _TraceMap, value); }
		#endregion


		#region DisableReloadMap: Description
		/// <summary>Description</summary>
		private bool _EnableReloadMap = true;
		/// <summary>Description</summary>
		public bool EnableReloadMap { get => _EnableReloadMap; set => Set(ref _EnableReloadMap, value); }
		#endregion
	}
	public class Parametrs : Base.ViewModel.BaseViewModel
	{

		#region LayoutAlgorithmType: Description
		/// <summary>Description</summary>
		private string _LayoutAlgorithmType = "ISOM";
		/// <summary>Description</summary>
		public string LayoutAlgorithmType { get => _LayoutAlgorithmType; set => Set(ref _LayoutAlgorithmType, value); }
		#endregion

		#region TTL: Description
		/// <summary>Description</summary>
		private byte _TTL = 100;
		/// <summary>Description</summary>
		public byte TTL { get => _TTL; set => Set(ref _TTL, value); }
		#endregion


		#region Timeout: Description
		/// <summary>Description</summary>
		private int _Timeout = 1000;
		/// <summary>Description</summary>
		public int Timeout
		{
			get => _Timeout; set
			{
				if (value < 1)
					value = 1;
				Set(ref _Timeout, value);
			}
		}
		#endregion


		#region TimeoutPingLocalNetwork: Description
		/// <summary>Description</summary>
		private int _TimeoutPingLocalNetwork = 1000;
		/// <summary>Description</summary>
		public int TimeoutPingLocalNetwork
		{
			get => _TimeoutPingLocalNetwork; set
			{
				if (value < 50)
					value = 50;
				Set(ref _TimeoutPingLocalNetwork, value);
			}
		}
		#endregion


		#region GCCollectTimeout: Description
		/// <summary>Description</summary>
		private int _GCCollectTimeout = 1000; // 1000 итераций
		/// <summary>Description</summary>
		public int GCCollectTimeout { get => _GCCollectTimeout; set => Set(ref _GCCollectTimeout, value); }
		#endregion



		#region DynamicTraceMap: Description
		/// <summary>Description</summary>
		private bool _DynamicTraceMap = true;
		/// <summary>Description</summary>
		public bool DynamicTraceMap { get => _DynamicTraceMap; set => Set(ref _DynamicTraceMap, value); }
		#endregion



		#region DynamicTraceMapOnScan: Description
		/// <summary>Description</summary>
		private bool _DynamicTraceMapOnScan = true;
		/// <summary>Description</summary>
		public bool DynamicTraceMapOnScan { get => _DynamicTraceMapOnScan; set => Set(ref _DynamicTraceMapOnScan, value); }
		#endregion



		#region EnableTraceMap: Description
		/// <summary>Description</summary>
		private bool _EnableTraceMap = true;
		/// <summary>Description</summary>
		public bool EnableTraceMap { get => _EnableTraceMap; set => Set(ref _EnableTraceMap, value); }
		#endregion
	}
}
