// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace FoodDb.DietMaker.Wpf
{
	public class FoodItem
	{
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
	}
}