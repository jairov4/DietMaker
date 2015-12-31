using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FoodDbCon;
using Microsoft.Win32;

namespace FoodDb.DietMaker.Wpf
{
	/// <summary>
	/// Interaction logic for HomePage.xaml
	/// </summary>
	public partial class HomePage : UserControl
	{
		public HomePage()
		{
			InitializeComponent();
		}

		private void btnLoadFoodData_OnClick(object sender, RoutedEventArgs e)
		{
			var file = new OpenFileDialog();
			file.Title = "Open FoodDB Food data file";
			file.Filter = "FoodDB Food data file (*.xml)|*.xml";
			var r = file.ShowDialog(App.Current.MainWindow);
			if (!r.HasValue || !r.Value)
			{
				return;
			}

			var fs = file.OpenFile();
			var ser = new DataContractSerializer(typeof(FoodDbCon.FoodDataSet));
			var foodData = (FoodDbCon.FoodDataSet)ser.ReadObject(fs);
			fs.Close();

			App.Current.FoodData = foodData;
		}

		private void btnViewFoodData_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainWindow.Content = new FoodRepositoryViewer();
		}

		private void btnCreateDietPlan_Click(object sender, RoutedEventArgs e)
		{
			App.Current.MainWindow.Content = new DietPlanViewer();
		}
	}
}
