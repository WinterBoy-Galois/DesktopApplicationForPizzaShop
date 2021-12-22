using System;
using System.Collections.Generic;
using System.Text;

namespace AMDProject
{
	class IngredientsVM
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool IsVisible { get; set; }
		public bool IsDeleted { get; set; }
		public int Stock { get; set; }
		public int Price { get; set; }
		public string Region { get; set; }
	}
}
