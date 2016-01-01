// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

using System.Collections.Generic;
using ReactiveUI;

namespace FoodDb.DietMaker.Wpf
{
	public class DietPlanItem : ReactiveObject
	{
		private static readonly IReadOnlyCollection<string> Empty = new string[0];
		private float? _carbohydrates;
		private float? _cholesterol;
		private string _comment = string.Empty;
		private float? _energy;
		private float? _fat;
		private float? _fattyAcidsMonoUnsaturated;
		private float? _fattyAcidsPolyUnsaturated;
		private float? _fattyAcidsSaturated;
		private FoodItem _food;

		private string _foodTime = string.Empty;
		private IReadOnlyCollection<string> _foodTimes = Empty;
		private float _mass;
		private float? _protein;
		private float? _sodium;

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
				UpdateStats();
			}
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

		private void UpdateStats()
		{
			var ratio = Mass/Food.Mass;
			Energy = Food.Energy*ratio;
			Sodium = Food.Sodium*ratio;
			Protein = Food.Protein*ratio;
			Cholesterol = Food.Cholesterol*ratio;
		}
	}
}