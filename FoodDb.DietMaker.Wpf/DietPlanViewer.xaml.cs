using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
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
using ReactiveUI;

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

	public class DietPlanViewerViewModel : ReactiveObject
	{
		private float _totalMass;
		private float _totalEnergy;
		private float _totalSodium;
		private float _totalProtein;
		private float _weightLoss;
		private float _actualMass;
		private float _energyDemand;
		private float _proteinDemand;
		private float _totalCholesterol;
		private float _totalFat;
		private float _totalFattyAcidsMonoUnsaturated;
		private float _totalFattyAcidsPolyUnsaturated;
		private float _totalFattyAcidsSaturated;
		private float _totalCarbohydrates;
		private DietPlanItem _selecteDietPlanItem;
		private FoodItem _selectedFoodItem;
		private string _searchText;
		private IReadOnlyList<FoodItem> _foods;

		public ICommand AddSelectedFoodItemToDietCommand { get; }

		public ICommand RemoveSelectedDietItemCommand { get; }

		public ObservableCollection<DietPlanItem> Diet { get; private set; }

		public IReadOnlyCollection<string> FoodTimes { get; }

		public string SearchText
		{
			get { return _searchText; }
			set { this.RaiseAndSetIfChanged(ref _searchText, value); }
		}

		public DietPlanItem SelectedDietPlanItem
		{
			get { return _selecteDietPlanItem; }
			set { this.RaiseAndSetIfChanged(ref _selecteDietPlanItem, value); }
		}

		public FoodItem SelectedFoodItem
		{
			get { return _selectedFoodItem; }
			set { this.RaiseAndSetIfChanged(ref _selectedFoodItem, value); }
		}

		public IReadOnlyList<FoodItem> Foods
		{
			get { return _foods; }
			set { this.RaiseAndSetIfChanged(ref _foods, value); }
		}

		public float TotalMass
		{
			get { return _totalMass; }
			private set { this.RaiseAndSetIfChanged(ref _totalMass, value); }
		}

		public float TotalEnergy
		{
			get { return _totalEnergy; }
			private set { this.RaiseAndSetIfChanged(ref _totalEnergy, value); }
		}

		public float TotalProtein
		{
			get { return _totalProtein; }
			private set { this.RaiseAndSetIfChanged(ref _totalProtein, value); }
		}

		public float TotalSodium
		{
			get { return _totalSodium; }
			private set { this.RaiseAndSetIfChanged(ref _totalSodium, value); }
		}

		public float TotalCholesterol
		{
			get { return _totalCholesterol; }
			private set { this.RaiseAndSetIfChanged(ref _totalCholesterol, value); }
		}
		
		public float TotalCarbohydrates
		{
			get { return _totalCarbohydrates; }
			set { this.RaiseAndSetIfChanged(ref _totalCarbohydrates, value); }
		}

		public float TotalFat
		{
			get { return _totalFat; }
			set { this.RaiseAndSetIfChanged(ref _totalFat, value); }
		}

		public float TotalFattyAcidsMonoUnsaturated
		{
			get { return _totalFattyAcidsMonoUnsaturated; }
			set { this.RaiseAndSetIfChanged(ref _totalFattyAcidsMonoUnsaturated, value); }
		}

		public float TotalFattyAcidsPolyUnsaturated
		{
			get { return _totalFattyAcidsPolyUnsaturated; }
			set { this.RaiseAndSetIfChanged(ref _totalFattyAcidsPolyUnsaturated, value); }
		}

		public float TotalFattyAcidsSaturated
		{
			get { return _totalFattyAcidsSaturated; }
			set { this.RaiseAndSetIfChanged(ref _totalFattyAcidsSaturated, value); }
		}

		public float WeightLoss
		{
			get { return _weightLoss; }
			private set { this.RaiseAndSetIfChanged(ref _weightLoss, value); }
		}

		public float ActualMass
		{
			get { return _actualMass; }
			set { this.RaiseAndSetIfChanged(ref _actualMass , value); }
		}

		public float EnergyDemand
		{
			get { return _energyDemand; }
			private set { this.RaiseAndSetIfChanged(ref _energyDemand, value); }
		}

		public float ProteinDemand
		{
			get { return _proteinDemand; }
			private set { this.RaiseAndSetIfChanged(ref _proteinDemand, value); }
		}

		public DietPlanViewerViewModel()
		{
			this.Diet = new ObservableCollection<DietPlanItem>();
			this.Diet.CollectionChanged += Diet_CollectionChanged;
			var foods = App.Current.FoodData.Foods;
			var foods2 = new List<FoodItem>();
			var useful = from c in foods where c.Attributes != null && !string.IsNullOrWhiteSpace(c.Name) select c;
			foreach (var food in useful)
			{
				Func<int, float?> getAtt = i =>
				{
					var r  = food.Attributes.FirstOrDefault(x => x.Id == i);
					return r?.Value;
				};
				var energy = getAtt(409);
				var protein = getAtt(416);
				var sodium = getAtt(323);
				var cholesterol = getAtt(433);
				var fat = getAtt(410);
				var famu = getAtt(282);
				var fapu = getAtt(287);
				var fas = getAtt(299);
				var carbs = getAtt(53);
				var food2 = new FoodItem(food.Name, 100, energy, sodium, protein, cholesterol, fat, famu, fapu, fas, carbs, food.Category2);
				foods2.Add(food2);
			}

			this.Foods = foods2;

			var canAddFoodItem = this.WhenAny(x => x.SelectedFoodItem, x => x.Value != null);
			var addSelectedFoodItemCommand = ReactiveCommand.Create(canAddFoodItem);
			AddSelectedFoodItemToDietCommand = addSelectedFoodItemCommand;
			addSelectedFoodItemCommand.Subscribe(_ => AddSelectedFoodItemToDietPlan());

			var canRemoveDietItem = this.WhenAny(x => x.SelectedDietPlanItem, x => x != null);
			var removeDietItemCommand = ReactiveCommand.Create(canRemoveDietItem);
			RemoveSelectedDietItemCommand = removeDietItemCommand;
			removeDietItemCommand.Subscribe(_ => RemoveSelectedDietItem());

			this._searchText = string.Empty;
			
			this.WhenAnyValue(x => x.SearchText)
				.Throttle(TimeSpan.FromSeconds(0.3), RxApp.MainThreadScheduler)
				.Subscribe(async x =>
			{
				if (string.IsNullOrWhiteSpace(x))
				{
					this.Foods = foods2;
					return;
				}

				var results = await Task.Run(() => foods2.Where(t => t.Name.ToLower().Contains(x.ToLower())).ToList());
				this.Foods = results;
			});

			this.FoodTimes = new []
			{
				"Desayuno", "Media mañana", "Almuerzo", "Tarde", "Cena"
			};
		}
		
		private void Diet_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (DietPlanItem item in e.OldItems)
				{
					item.PropertyChanged -= Item_PropertyChanged;
				}
			}

			if (e.NewItems != null)
			{
				foreach (DietPlanItem item in e.NewItems)
				{
					item.PropertyChanged += Item_PropertyChanged;
				}
			}

			this.UpdateProperty(nameof(DietPlanItem.Mass));
			this.UpdateProperty(nameof(DietPlanItem.Energy));
			this.UpdateProperty(nameof(DietPlanItem.Protein));
			this.UpdateProperty(nameof(DietPlanItem.Carbohydrates));
			this.UpdateProperty(nameof(DietPlanItem.Cholesterol));
			this.UpdateProperty(nameof(DietPlanItem.Fat));
			this.UpdateProperty(nameof(DietPlanItem.FattyAcidsMonoUnsaturated));
			this.UpdateProperty(nameof(DietPlanItem.FattyAcidsPolyUnsaturated));
			this.UpdateProperty(nameof(DietPlanItem.FattyAcidsSaturated));
			this.UpdateProperty(nameof(DietPlanItem.Sodium));
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var prop = e.PropertyName;
			this.UpdateProperty(prop);
		}

		private void UpdateProperty(string prop)
		{
			if (prop == nameof(DietPlanItem.Mass))
			{
				TotalMass = this.Diet.Sum(x => x.Mass);
			}

			if (prop == nameof(DietPlanItem.Energy))
			{
				TotalEnergy = this.Diet.Sum(x => x.Energy ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Protein))
			{
				TotalProtein = this.Diet.Sum(x => x.Protein ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Sodium))
			{
				TotalSodium = this.Diet.Sum(x => x.Sodium ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Cholesterol))
			{
				TotalCholesterol = this.Diet.Sum(x => x.Cholesterol ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Fat))
			{
				TotalFat = this.Diet.Sum(x => x.Fat ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.FattyAcidsMonoUnsaturated))
			{
				TotalFattyAcidsMonoUnsaturated = this.Diet.Sum(x => x.FattyAcidsMonoUnsaturated ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.FattyAcidsPolyUnsaturated))
			{
				TotalFattyAcidsPolyUnsaturated = this.Diet.Sum(x => x.FattyAcidsPolyUnsaturated ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.FattyAcidsMonoUnsaturated))
			{
				TotalFattyAcidsMonoUnsaturated = this.Diet.Sum(x => x.FattyAcidsMonoUnsaturated ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Carbohydrates))
			{
				TotalCarbohydrates = this.Diet.Sum(x => x.Carbohydrates ?? 0.0f);
			}
		}

		public void AddSelectedFoodItemToDietPlan()
		{
			if(this.SelectedFoodItem == null) return;

			var item = new DietPlanItem
			{
				Food = SelectedFoodItem,
				Mass = 10,
				FoodTimes = this.FoodTimes
			};
			this.Diet.Add(item);
		}
		
		private void RemoveSelectedDietItem()
		{
			if(this.SelectedDietPlanItem == null) return;

			this.Diet.Remove(this.SelectedDietPlanItem);
		}
	}

	public class FoodItem
	{
		public string Name { get; }

		public float Mass { get; }

		public float? Energy { get; }

		public float? Sodium { get; }

		public float? Protein { get; }
		
		public float? Cholesterol { get; }

		public float? Fat { get; }

		public float? FattyAcidsMonoUnsaturated { get; }

		public float? FattyAcidsPolyUnsaturated { get; }

		public float? FattyAcidsSaturated { get; }

		public float? Carbohydrates { get; }

		public string Category { get; }

		public FoodItem(string name, float mass, float? energy, float? sodium, float? protein, float? cholesterol, float? fat,
			float? fattyAcidsMonoUnsaturated, float? fattyAcidsPolyUnsaturated, float? fattyAcidsSaturated, float? carbohydrates,
			string category)
		{
			Name = name;
			Mass = mass;
			Energy = energy;
			Sodium = sodium;
			Protein = protein;
			Category = category;
			Fat = fat;
			FattyAcidsMonoUnsaturated = fattyAcidsMonoUnsaturated;
			FattyAcidsPolyUnsaturated = fattyAcidsPolyUnsaturated;
			FattyAcidsSaturated = fattyAcidsSaturated;
			Carbohydrates = carbohydrates;
			Cholesterol = cholesterol;
		}
	}

	public class DietPlanItem : ReactiveObject
	{
		private static readonly IReadOnlyCollection<string> Empty = new string[0];

		private string _foodTime = string.Empty;
		private FoodItem _food;
		private string _comment = string.Empty;
		private float _mass;
		private float? _energy;
		private float? _sodium;
		private float? _protein;
		private float? _cholesterol;
		private float? _fat;
		private float? _fattyAcidsMonoUnsaturated;
		private float? _fattyAcidsPolyUnsaturated;
		private float? _fattyAcidsSaturated;
		private float? _carbohydrates;
		private IReadOnlyCollection<string> _foodTimes = Empty;

		public string FoodTime
		{
			get { return _foodTime; }
			set { this.RaiseAndSetIfChanged(ref _foodTime, value); }
		}

		public FoodItem Food
		{
			get { return _food; }
			set { this.RaiseAndSetIfChanged(ref _food, value); }
		}

		public string Comment
		{
			get { return _comment; }
			set { this.RaiseAndSetIfChanged(ref _comment, value); }
		}

		public float Mass
		{
			get { return _mass; }
			set
			{
				this.RaiseAndSetIfChanged(ref _mass, value);
				this.UpdateStats();
			}
		}

		private void UpdateStats()
		{
			var ratio = Mass / Food.Mass;
			Energy = Food.Energy*ratio;
			Sodium = Food.Sodium*ratio;
			Protein = Food.Protein*ratio;
			Cholesterol = Food.Cholesterol*ratio;
		}

		public float? Energy
		{
			get { return _energy; }
			private set { this.RaiseAndSetIfChanged(ref _energy, value); }
		}

		public float? Sodium
		{
			get { return _sodium; }
			private set { this.RaiseAndSetIfChanged(ref _sodium, value); }
		}

		public float? Protein
		{
			get { return _protein; }
			private set { this.RaiseAndSetIfChanged(ref _protein, value); }
		}

		public float? Cholesterol
		{
			get { return _cholesterol; }
			private set { this.RaiseAndSetIfChanged(ref _cholesterol, value); }
		}

		public float? Carbohydrates
		{
			get { return _carbohydrates; }
			set { this.RaiseAndSetIfChanged(ref _carbohydrates, value); }
		}

		public float? Fat
		{
			get { return _fat; }
			set { this.RaiseAndSetIfChanged(ref _fat, value); }
		}

		public float? FattyAcidsMonoUnsaturated
		{
			get { return _fattyAcidsMonoUnsaturated; }
			set { this.RaiseAndSetIfChanged(ref _fattyAcidsMonoUnsaturated, value); }
		}

		public float? FattyAcidsPolyUnsaturated
		{
			get { return _fattyAcidsPolyUnsaturated; }
			set { this.RaiseAndSetIfChanged(ref _fattyAcidsPolyUnsaturated, value); }
		}

		public float? FattyAcidsSaturated
		{
			get { return _fattyAcidsSaturated; }
			set { this.RaiseAndSetIfChanged(ref _fattyAcidsSaturated, value); }
		}

		public IReadOnlyCollection<string> FoodTimes
		{
			get { return _foodTimes; }
			set { this.RaiseAndSetIfChanged(ref _foodTimes, value); }
		}
	}
}
