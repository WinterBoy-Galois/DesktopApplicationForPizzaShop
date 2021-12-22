using System;
using System.Collections.Generic;
using System.Text;

namespace AMDProject
{
	class CustomerComposedPizza
	{
		public int id { get; set; }
		public int Price { get; set; }
		public string Flavour { get; set; }
		public int Size { get; set; }
		public int IngredientsId { get; set; }
		public string IngredientsName { get; set; }
	}
}
