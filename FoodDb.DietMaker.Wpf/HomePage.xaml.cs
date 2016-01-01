// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
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
			var ser = new DataContractSerializer(typeof (FoodDataSet));
			var foodData = (FoodDataSet) ser.ReadObject(fs);
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