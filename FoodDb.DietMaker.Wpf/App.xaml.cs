// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

using System.Windows;
using FoodDbCon;

namespace FoodDb.DietMaker.Wpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public MainWindow Window => (MainWindow) MainWindow;

		public new static App Current => (App) Application.Current;

		public FoodDataSet FoodData { get; set; }
	}
}