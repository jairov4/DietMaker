// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

using System.Windows.Controls;

namespace FoodDb.DietMaker.Wpf
{
	/// <summary>
	/// Interaction logic for DietPlanViewer.xaml
	/// </summary>
	public partial class DietPlanViewer : UserControl
	{
		public DietPlanViewer()
		{
			InitializeComponent();
			DataContext = new DietPlanViewerViewModel();
		}
	}
}