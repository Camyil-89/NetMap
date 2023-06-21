
using GraphSharp.Controls;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NetMap.Base.Command;
using NetMap.Models;
using NetMap.Service;
using NetMap.Service.Net;
using QuickGraph;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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
		private static object _lock = new object();
		public MainVM()
		{
			BindingOperations.EnableCollectionSynchronization(ListNetAddresses, _lock);
			#region Commands
			#endregion
			App.Current.MainWindow.Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("IsLoaded");
			AdapterProvider.GetAdapters();
		}

		#region Parametrs

		#region Gpaphs: Description
		/// <summary>Description</summary>
		private BidirectionalGraph<object, IEdge<object>> _Graphs = new BidirectionalGraph<object, IEdge<object>>();
		/// <summary>Description</summary>
		public BidirectionalGraph<object, IEdge<object>> Graphs { get => _Graphs; set => Set(ref _Graphs, value); }
		#endregion

		#region Title: Description
		/// <summary>Description</summary>
		private string _Title = "NetMap";
		/// <summary>Description</summary>
		public string Title { get => _Title; set => Set(ref _Title, value); }
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


		#region EnableScanButton: Description
		/// <summary>Description</summary>
		private bool _EnableScanButton = true;
		/// <summary>Description</summary>
		public bool EnableScanButton { get => _EnableScanButton; set => Set(ref _EnableScanButton, value); }
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

		#region TextButtonScanNetwork: Description
		/// <summary>Description</summary>
		private string _TextButtonScanNetwork = "Сканировать сеть";
		/// <summary>Description</summary>
		public string TextButtonScanNetwork { get => _TextButtonScanNetwork; set => Set(ref _TextButtonScanNetwork, value); }
		#endregion


		#region ToolTipInfoAboutAdapters: Description
		/// <summary>Description</summary>
		private string _ToolTipInfoAboutAdapters;
		/// <summary>Description</summary>
		public string ToolTipInfoAboutAdapters { get => _ToolTipInfoAboutAdapters; set => Set(ref _ToolTipInfoAboutAdapters, value); }
		#endregion

		#region InfoAboutAdapters: Description
		/// <summary>Description</summary>
		private string _InfoAboutAdapters;
		/// <summary>Description</summary>
		public string InfoAboutAdapters { get => _InfoAboutAdapters; set => Set(ref _InfoAboutAdapters, value); }
		#endregion


		#region ListNetAddresses: Description
		/// <summary>Description</summary>
		private ObservableCollection<string> _ListNetAddresses = new ObservableCollection<string>();
		/// <summary>Description</summary>
		public ObservableCollection<string> ListNetAddresses { get => _ListNetAddresses; set => Set(ref _ListNetAddresses, value); }
		#endregion

		


		#region LocalNetworkSubnet: Description
		/// <summary>Description</summary>
		private string[] _LocalNetworkSubnet = new string[4] { "192", "168", "-", "-" };
		/// <summary>Description</summary>
		public string[] LocalNetworkSubnet { get => _LocalNetworkSubnet; set => Set(ref _LocalNetworkSubnet, value); }
		#endregion

		#region ListGCCollectTimeout: Description
		/// <summary>Description</summary>
		private ObservableCollection<int> _ListGCCollectTimeout = new ObservableCollection<int>() { 0, 100, 500, 1000, 2500, 5000, 10000 };
		/// <summary>Description</summary>
		public ObservableCollection<int> ListGCCollectTimeout { get => _ListGCCollectTimeout; set => Set(ref _ListGCCollectTimeout, value); }
		#endregion

		#region ListLayoutAlgorithmType: Description
		/// <summary>Description</summary>
		private ObservableCollection<string> _ListLayoutAlgorithmType = new ObservableCollection<string>()
		{
			"FR",
			"ISOM",
			"EfficientSugiyama",
		};
		/// <summary>Description</summary>
		public ObservableCollection<string> ListLayoutAlgorithmType { get => _ListLayoutAlgorithmType; set => Set(ref _ListLayoutAlgorithmType, value); }
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
			if (MessageBox.Show("Вы уверены что хотите очистить карту?", "NetMap", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				TraceRouteProvider.ClearGraphs();
			}
		}
		#endregion

		#region ScanLocalNetworkCommand: Description
		private ICommand _ScanLocalNetworkCommand;
		public ICommand ScanLocalNetworkCommand => _ScanLocalNetworkCommand ??= new LambdaCommand(OnScanLocalNetworkCommandExecuted, CanScanLocalNetworkCommandExecute);
		private bool CanScanLocalNetworkCommandExecute(object e) => true;
		private void OnScanLocalNetworkCommandExecuted(object e)
		{
			
			if (TextButtonScanNetwork == "Отмена")
			{
				Service.Net.ScannerLocalNetwork.StopScanNetwork();
			}
			else
			{
				byte[] address_min = new byte[4];
				byte[] address_max = new byte[4];
				try
				{
					ListNetAddresses.Clear();
					address_min = ScannerLocalNetwork.ParseAddress(Settings.Parametrs.MinAddress);
					address_max = ScannerLocalNetwork.ParseAddress(Settings.Parametrs.MaxAddress);
					if (ScannerLocalNetwork.MinThenMax(address_min, address_max) == false)
					{
						MessageBox.Show("Левый адрес должен быть меньше чем правый!", "NetMap");
						return;
					}
				} catch { MessageBox.Show("Не правильный формат адресов!", "NetMap"); }
				Service.Net.ScannerLocalNetwork.ScanNetwork(address_min, address_max);
			}
		}
		#endregion


		#region ScanPoolAddressesCommand: Description
		private ICommand _ScanPoolAddressesCommand;
		public ICommand ScanPoolAddressesCommand => _ScanPoolAddressesCommand ??= new LambdaCommand(OnScanPoolAddressesCommandExecuted, CanScanPoolAddressesCommandExecute);
		private bool CanScanPoolAddressesCommandExecute(object e) => ListNetAddresses.Count > 0;
		private void OnScanPoolAddressesCommandExecuted(object e)
		{
			Service.Net.ScannerLocalNetwork.ScanPoolNetwork(ListNetAddresses);
		}
		#endregion

		#region SaveAddressesCommand: Description
		private ICommand _SaveAddressesCommand;
		public ICommand SaveAddressesCommand => _SaveAddressesCommand ??= new LambdaCommand(OnSaveAddressesCommandExecuted, CanSaveAddressesCommandExecute);
		private bool CanSaveAddressesCommandExecute(object e) => ListNetAddresses.Count > 0;
		private void OnSaveAddressesCommandExecuted(object e)
		{
			Directory.CreateDirectory($"{Program.PathToSave}\\saves");
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.InitialDirectory = $"{Program.PathToSave}\\saves";
			dialog.FileName = $"{DateTime.Now.Ticks}.txt";
			if (dialog.ShowDialog() == true)
			{
				File.WriteAllLines(dialog.FileName, ListNetAddresses);
			}

		}
		#endregion

		#region LoadAddressesCommand: Description
		private ICommand _LoadAddressesCommand;
		public ICommand LoadAddressesCommand => _LoadAddressesCommand ??= new LambdaCommand(OnLoadAddressesCommandExecuted, CanLoadAddressesCommandExecute);
		private bool CanLoadAddressesCommandExecute(object e) => true;
		private void OnLoadAddressesCommandExecuted(object e)
		{
			Directory.CreateDirectory($"{Program.PathToSave}\\saves");
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = false;
			dialog.InitialDirectory = $"{Program.PathToSave}\\saves";
			dialog.Multiselect = false;
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileNames.Count() > 0)
			{
				try
				{
					ListNetAddresses.Clear();
					foreach (var i in File.ReadAllLines(dialog.FileName))
					{
						ListNetAddresses.Add(i);
					}
				}
				catch { MessageBox.Show("Не удалось загрузить!", "NetMap"); }
			}
		}
		#endregion
		#endregion

		#region Functions
		#endregion
	}
}
