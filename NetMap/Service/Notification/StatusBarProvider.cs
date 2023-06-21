using Microsoft.Extensions.DependencyInjection;
using NetMap.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace NetMap.Service.Notification
{
    public static class StatusBarProvider
    {
        private static MainVM MainVM => App.Host.Services.GetRequiredService<MainVM>();
        public static void ShowMessage(string text)
        {
            MainVM.VisibilityBottomLeftStatus = System.Windows.Visibility.Visible;
            MainVM.TextStatusBottomLeft = $"| {text}";

        }
        public static void ShowInfoAboutAdapters(string text, string ToolTip)
        {
            MainVM.InfoAboutAdapters = text;
            MainVM.ToolTipInfoAboutAdapters = ToolTip;
        }
        public static void CloseMessage()
        {
            MainVM.VisibilityBottomLeftStatus = System.Windows.Visibility.Collapsed;
        }
    }
}
