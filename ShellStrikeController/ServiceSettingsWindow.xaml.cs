using ShellStrike;
using ShellStrike.WCFService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShellStrikeController
{
    /// <summary>
    /// Interaction logic for ServiceSettingsWindow.xaml
    /// </summary>
    public partial class ServiceSettingsWindow : Window
    {
        IShellData WCFIShellData;

        public ServiceSettingsWindow()
        {
            InitializeComponent();
        }

        public ServiceSettingsWindow(IShellData _WCFIShellData)
        {
            InitializeComponent();
            WCFIShellData = _WCFIShellData;
        }



        private async void btnSetValToParam_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = await WCFIShellData.SetCacheValueOf(drpParam.Text, txtValue.Text, false);
                if (a != 1) { MessageBox.Show("Error Updating Value"); }
                await PopulateGrid();
            }
            catch (Exception t) { Logger.Error(t); }
        }

        private async void btnSetValToParamPermanent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = await WCFIShellData.SetCacheValueOf(drpParam.Text, txtValue.Text, true);
                if (a != 1) { MessageBox.Show("Error Updating Value"); }
                await PopulateGrid();
            }
            catch (Exception t) { Logger.Error(t); }
        }

        private async void btnGetParamValues_Click(object sender, RoutedEventArgs e)
        {
            await PopulateGrid();

        }

        async Task<int> PopulateGrid()
        {
            try
            {
                gridParamValues.Items.Clear();
                drpParam.Items.Clear();

                var pairs = await WCFIShellData.GetCacheValues();
                pairs.ForEach(X =>
                {
                    gridParamValues.Items.Add(X);
                    drpParam.Items.Add(X.Name);
                });
            }
            catch (Exception t) { Logger.Error(t); }
            return 1;

        }
    }
}
