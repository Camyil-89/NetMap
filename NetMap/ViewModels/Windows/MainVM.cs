
using NetMap.Base.Command;
using NetMap.Models;
using NetMap.Service;
using QuickGraph;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NetMap.ViewModels.Windows
{
	public enum ModeTrace : byte
	{
		Stop = 0,
		Start = 1,
	}
	public class MainVM : Base.ViewModel.BaseViewModel
	{
		public MainVM()
		{

			#region Commands
			#endregion

		}

		#region Parametrs

		#region Gpaphs: Description
		/// <summary>Description</summary>
		private BidirectionalGraph<object, IEdge<object>> _Graphs = new BidirectionalGraph<object, IEdge<object>>();
		/// <summary>Description</summary>
		public BidirectionalGraph<object, IEdge<object>> Graphs { get => _Graphs; set => Set(ref _Graphs, value); }
		#endregion

		public Settings Settings => Settings.Instance;

		#region TraceMode: Description
		/// <summary>Description</summary>
		private ModeTrace _TraceMode = ModeTrace.Start;
		/// <summary>Description</summary>
		public ModeTrace TraceMode { get => _TraceMode; set => Set(ref _TraceMode, value); }
		#endregion

		#region EnableButtonClear: Description
		/// <summary>Description</summary>
		private bool _EnableButtonClear = true;
		/// <summary>Description</summary>
		public bool EnableButtonClear { get => _EnableButtonClear; set => Set(ref _EnableButtonClear, value); }
		#endregion

		#region TraceButtonEnable: Description
		/// <summary>Description</summary>
		private bool _TraceButtonEnable = true;
		/// <summary>Description</summary>
		public bool TraceButtonEnable { get => _TraceButtonEnable; set => Set(ref _TraceButtonEnable, value); }
		#endregion

		#region TextButtonTrace: Description
		/// <summary>Description</summary>
		private string _TextButtonTrace = "Старт";
		/// <summary>Description</summary>
		public string TextButtonTrace { get => _TextButtonTrace; set => Set(ref _TextButtonTrace, value); }
		#endregion

		#region TextStatusBottomLeft: Description
		/// <summary>Description</summary>
		private string _TextStatusBottomLeft;
		/// <summary>Description</summary>
		public string TextStatusBottomLeft { get => _TextStatusBottomLeft; set => Set(ref _TextStatusBottomLeft, value); }
		#endregion

		#region VisibilityBottomLeftStatus: Description
		/// <summary>Description</summary>
		private Visibility _VisibilityBottomLeftStatus = Visibility.Collapsed;
		/// <summary>Description</summary>
		public Visibility VisibilityBottomLeftStatus { get => _VisibilityBottomLeftStatus; set => Set(ref _VisibilityBottomLeftStatus, value); }
		#endregion

		#region TraceAddress: Description
		/// <summary>Description</summary>
		private string _TraceAddress = "google.com";
		/// <summary>Description</summary>
		public string TraceAddress { get => _TraceAddress; set => Set(ref _TraceAddress, value); }
		#endregion
		#endregion

		#region Commands

		#region TraceCommand: Description
		private ICommand _TraceCommand;
		public ICommand TraceCommand => _TraceCommand ??= new LambdaCommand(OnTraceCommandExecuted, CanTraceCommandExecute);
		private bool CanTraceCommandExecute(object e) => true;
		private void OnTraceCommandExecuted(object e)
		{
			if (TraceMode == ModeTrace.Start)
			{
				Service.TraceRouteProvider.StartTrace(TraceAddress);
			}
			else
			{
				Service.TraceRouteProvider.StopTrace();
			}
		}
		#endregion

		#region ReloadMapCommand: Description
		private ICommand _ReloadMapCommand;
		public ICommand ReloadMapCommand => _ReloadMapCommand ??= new LambdaCommand(OnReloadMapCommandExecuted, CanReloadMapCommandExecute);
		private bool CanReloadMapCommandExecute(object e) => true;
		private void OnReloadMapCommandExecuted(object e)
		{
			TraceRouteProvider.ReloadMap();
		}
		#endregion

		#region ClearTraceMapCommand: Description
		private ICommand _ClearTraceMapCommand;
		public ICommand ClearTraceMapCommand => _ClearTraceMapCommand ??= new LambdaCommand(OnClearTraceMapCommandExecuted, CanClearTraceMapCommandExecute);
		private bool CanClearTraceMapCommandExecute(object e) => true;
		private void OnClearTraceMapCommandExecuted(object e)
		{
			TraceRouteProvider.ClearGraphs();
		}
		#endregion
		#endregion

		#region Functions
		#endregion
	}
}
