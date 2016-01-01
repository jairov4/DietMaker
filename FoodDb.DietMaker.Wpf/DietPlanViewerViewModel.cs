// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace FoodDb.DietMaker.Wpf
{
	public class DietPlanViewerViewModel : ReactiveObject
	{
		private float _actualMass;
		private float _energyDemand;
		private IReadOnlyList<FoodItem> _foods;
		private float _proteinDemand;
		private string _searchText;
		private FoodItem _selectedFoodItem;
		private DietPlanItem _selecteDietPlanItem;
		private float _totalCarbohydrates;
		private float _totalCholesterol;
		private float _totalEnergy;
		private float _totalFat;
		private float _totalFattyAcidsMonoUnsaturated;
		private float _totalFattyAcidsPolyUnsaturated;
		private float _totalFattyAcidsSaturated;
		private float _totalMass;
		private float _totalProtein;
		private float _totalSodium;
		private float _weightLoss;

		public DietPlanViewerViewModel()
		{
			Diet = new ObservableCollection<DietPlanItem>();
			Diet.CollectionChanged += Diet_CollectionChanged;
			var foods = App.Current.FoodData.Foods;
			var foods2 = new List<FoodItem>();
			var useful = from c in foods where c.Attributes != null && !string.IsNullOrWhiteSpace(c.Name) select c;
			foreach (var food in useful)
			{
				Func<int, float?> getAtt = i =>
				{
					var r = food.Attributes.FirstOrDefault(x => x.Id == i);
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
				var food2 = new FoodItem(food.Name, 100, energy, sodium, protein, cholesterol, fat, famu, fapu, fas, carbs,
					food.Category2);
				foods2.Add(food2);
			}

			Foods = foods2;

			var canAddFoodItem = this.WhenAny(x => x.SelectedFoodItem, x => x.Value != null);
			var addSelectedFoodItemCommand = ReactiveCommand.Create(canAddFoodItem);
			AddSelectedFoodItemToDietCommand = addSelectedFoodItemCommand;
			addSelectedFoodItemCommand.Subscribe(_ => AddSelectedFoodItemToDietPlan());

			var canRemoveDietItem = this.WhenAny(x => x.SelectedDietPlanItem, x => x.Value != null);
			var removeDietItemCommand = ReactiveCommand.Create(canRemoveDietItem);
			RemoveSelectedDietItemCommand = removeDietItemCommand;
			removeDietItemCommand.Subscribe(_ => RemoveSelectedDietItem());

			_searchText = string.Empty;

			this.WhenAnyValue(x => x.SearchText)
				.Throttle(TimeSpan.FromSeconds(0.3), RxApp.MainThreadScheduler)
				.Subscribe(async x =>
				{
					if (string.IsNullOrWhiteSpace(x))
					{
						Foods = foods2;
						return;
					}

					var results = await Task.Run(() => foods2.Where(t => t.Name.ToLower().Contains(x.ToLower())).ToList());
					Foods = results;
				});

			FoodTimes = new[]
			{
				"Desayuno", "Media mañana", "Almuerzo", "Tarde", "Cena"
			};
		}

		public ICommand AddSelectedFoodItemToDietCommand { get; }

		public ICommand RemoveSelectedDietItemCommand { get; }

		public ObservableCollection<DietPlanItem> Diet { get; }

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
			set { this.RaiseAndSetIfChanged(ref _actualMass, value); }
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

			UpdateProperty(nameof(DietPlanItem.Mass));
			UpdateProperty(nameof(DietPlanItem.Energy));
			UpdateProperty(nameof(DietPlanItem.Protein));
			UpdateProperty(nameof(DietPlanItem.Carbohydrates));
			UpdateProperty(nameof(DietPlanItem.Cholesterol));
			UpdateProperty(nameof(DietPlanItem.Fat));
			UpdateProperty(nameof(DietPlanItem.FattyAcidsMonoUnsaturated));
			UpdateProperty(nameof(DietPlanItem.FattyAcidsPolyUnsaturated));
			UpdateProperty(nameof(DietPlanItem.FattyAcidsSaturated));
			UpdateProperty(nameof(DietPlanItem.Sodium));
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var prop = e.PropertyName;
			UpdateProperty(prop);
		}

		private void UpdateProperty(string prop)
		{
			if (prop == nameof(DietPlanItem.Mass))
			{
				TotalMass = Diet.Sum(x => x.Mass);
			}

			if (prop == nameof(DietPlanItem.Energy))
			{
				TotalEnergy = Diet.Sum(x => x.Energy ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Protein))
			{
				TotalProtein = Diet.Sum(x => x.Protein ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Sodium))
			{
				TotalSodium = Diet.Sum(x => x.Sodium ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Cholesterol))
			{
				TotalCholesterol = Diet.Sum(x => x.Cholesterol ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Fat))
			{
				TotalFat = Diet.Sum(x => x.Fat ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.FattyAcidsMonoUnsaturated))
			{
				TotalFattyAcidsMonoUnsaturated = Diet.Sum(x => x.FattyAcidsMonoUnsaturated ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.FattyAcidsPolyUnsaturated))
			{
				TotalFattyAcidsPolyUnsaturated = Diet.Sum(x => x.FattyAcidsPolyUnsaturated ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.FattyAcidsMonoUnsaturated))
			{
				TotalFattyAcidsMonoUnsaturated = Diet.Sum(x => x.FattyAcidsMonoUnsaturated ?? 0.0f);
			}

			if (prop == nameof(DietPlanItem.Carbohydrates))
			{
				TotalCarbohydrates = Diet.Sum(x => x.Carbohydrates ?? 0.0f);
			}
		}

		public void AddSelectedFoodItemToDietPlan()
		{
			if (SelectedFoodItem == null) return;

			var item = new DietPlanItem
			{
				Food = SelectedFoodItem,
				Mass = 10,
				FoodTimes = FoodTimes
			};
			Diet.Add(item);
		}

		private void RemoveSelectedDietItem()
		{
			if (SelectedDietPlanItem == null) return;

			Diet.Remove(SelectedDietPlanItem);
		}
	}
}