using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMap.Models
{

	public class Parametrs : Base.ViewModel.BaseViewModel
	{

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
