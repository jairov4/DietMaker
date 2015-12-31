using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

namespace FoodDb.DietMaker.Wpf
{
	/// <summary>
	/// Interaction logic for FoodRepositoryViewer.xaml
	/// </summary>
	public partial class FoodRepositoryViewer : UserControl
	{
		public FoodRepositoryViewer()
		{
			InitializeComponent();
			CreateDataSet();
		}

		void CreateDataSet()
		{
			var dt = new DataTable();

			dt.Columns.Add(new DataColumn("Nombre"));
			dataGrid.Columns.Add(new DataGridTextColumn()
			{
				Binding = new Binding("ItemArray[0]"),
				Header = "Nombre"
			});

			var ds = App.Current.FoodData;
			dt.BeginLoadData();
			foreach (var food in ds.Foods)
			{
				var row = dt.NewRow();
				row[0] = food.Name;
				foreach (var foodInfoAttribute in food.Attributes)
				{
					var aname = $"{foodInfoAttribute.Descriptor.Name} {foodInfoAttribute.Unit}";
					if (!dt.Columns.Contains(aname))
					{
						dt.Columns.Add(new DataColumn(aname));
						var id = dataGrid.Columns.Count;
						dataGrid.Columns.Add(new DataGridTextColumn()
						{
							Header = aname,
							Binding = new Binding($"ItemArray[{id}]")
						});
					}

					row[aname] = foodInfoAttribute.Value;
				}

				dt.Rows.Add(row);
			}

			dt.EndLoadData();
			dataGrid.ItemsSource = dt.Rows;
		}
	}
}
