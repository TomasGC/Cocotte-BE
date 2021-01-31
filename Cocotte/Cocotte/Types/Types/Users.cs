using System;
using System.Collections.Generic;
using CoreLib.Types;

namespace Cocotte.MongoDb.Types {
	/// <summary>
	/// Schedule per day for the meals.
	/// </summary>
	public class DayMealsSchedule {
		/// <summary>
		/// The day.
		/// </summary>
		public DayOfWeek Day { get; set; }
		/// <summary>
		/// The list to know if we need to cook that meal.
		/// </summary>
		public List<KeyValuePair<MealType, bool>> Meals { get; set; }
	};

	/// <summary>
	/// User informations
	/// </summary>
	public class Users : BaseType {
		/// <summary>
		/// Login.
		/// </summary>
		public string Login { get; set; }
		/// <summary>
		/// Password.
		/// </summary>
		public string Password { get; set; }
		/// <summary>
		/// Colors set by the User for it's UI.
		/// </summary>
		public UIColors UIColors { get; set; }
		/// <summary>
		/// List of the meals per week wanted.
		/// </summary>
		public List<DayMealsSchedule> DayMealsSchedule { get; set; }
		/// <summary>
		/// The time before a meal is reused in the generation.
		/// </summary>
		public int TimeBetweenMeals { get; set; }

		#region Constructors
		public Users() { }

		public Users(string login, string password, UIColors colors) {
			Login = login;
			Password = password;
			TimeBetweenMeals = 14;
			UIColors = colors;
			DayMealsSchedule = new List<DayMealsSchedule> {
				new DayMealsSchedule {
					Day = DayOfWeek.Monday,
					Meals = new List<KeyValuePair<MealType, bool>> {
						new KeyValuePair<MealType, bool>(MealType.Breakfast, false),
						new KeyValuePair<MealType, bool>(MealType.Lunch, false),
						new KeyValuePair<MealType, bool>(MealType.Dinner, true)
					}
				},
				new DayMealsSchedule {
					Day = DayOfWeek.Tuesday,
					Meals = new List<KeyValuePair<MealType, bool>> {
						new KeyValuePair<MealType, bool>(MealType.Breakfast, false),
						new KeyValuePair<MealType, bool>(MealType.Lunch, false),
						new KeyValuePair<MealType, bool>(MealType.Dinner, true)
					}
				},
				new DayMealsSchedule {
					Day = DayOfWeek.Wednesday,
					Meals = new List<KeyValuePair<MealType, bool>> {
						new KeyValuePair<MealType, bool>(MealType.Breakfast, false),
						new KeyValuePair<MealType, bool>(MealType.Lunch, false),
						new KeyValuePair<MealType, bool>(MealType.Dinner, true)
					}
				},
				new DayMealsSchedule {
					Day = DayOfWeek.Thursday,
					Meals = new List<KeyValuePair<MealType, bool>> {
						new KeyValuePair<MealType, bool>(MealType.Breakfast, false),
						new KeyValuePair<MealType, bool>(MealType.Lunch, false),
						new KeyValuePair<MealType, bool>(MealType.Dinner, true)
					}
				},
				new DayMealsSchedule {
					Day = DayOfWeek.Friday,
					Meals = new List<KeyValuePair<MealType, bool>> {
						new KeyValuePair<MealType, bool>(MealType.Breakfast, false),
						new KeyValuePair<MealType, bool>(MealType.Lunch, false),
						new KeyValuePair<MealType, bool>(MealType.Dinner, true)
					}
				},
				new DayMealsSchedule {
					Day = DayOfWeek.Saturday,
					Meals = new List<KeyValuePair<MealType, bool>> {
						new KeyValuePair<MealType, bool>(MealType.Breakfast, false),
						new KeyValuePair<MealType, bool>(MealType.Lunch, true),
						new KeyValuePair<MealType, bool>(MealType.Dinner, true)
					}
				},
				new DayMealsSchedule {
					Day = DayOfWeek.Sunday,
					Meals = new List<KeyValuePair<MealType, bool>> {
						new KeyValuePair<MealType, bool>(MealType.Breakfast, false),
						new KeyValuePair<MealType, bool>(MealType.Lunch, true),
						new KeyValuePair<MealType, bool>(MealType.Dinner, true)
					}
				},
			};
		}
		#endregion
	};
}
