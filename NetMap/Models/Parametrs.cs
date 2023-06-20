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
	}
}
