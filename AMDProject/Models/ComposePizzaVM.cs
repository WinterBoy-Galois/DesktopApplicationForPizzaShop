using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AMDProject
{
	class ComposePizzaVM
	{
		public int Id { get; set; }
		public string Ingredient { get; set; }
		public bool isOrdered { get; set; }
		public int Price { get; set; }
	}
}
