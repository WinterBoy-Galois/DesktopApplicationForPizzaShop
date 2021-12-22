using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using Npgsql;

namespace AMDProject
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<SupplierVM> supplierList = new List<SupplierVM>();
		List<IngredientsVM> ingredientList = new List<IngredientsVM>();
		List<IngredientsVM> ingredientListCustomer = new List<IngredientsVM>();
		List<FlavoursVM> flavoursList = new List<FlavoursVM>();
		List<SizesVM> sizesList = new List<SizesVM>();
		List<ComposePizzaVM> composePizzaList = new List<ComposePizzaVM>();
		List<CustomerComposedPizza> customerComposedPizzaList = new List<CustomerComposedPizza>();
		List<CustomerOrderedPizza> customerOrderdPizzaList = new List<CustomerOrderedPizza>();

		Random _random = new Random();

		public static string cs = "Host=localhost;Username=postgres;Password=7777;Database=pizzashop";
		NpgsqlConnection con = new NpgsqlConnection(cs);

		public MainWindow()
		{
			InitializeComponent();
			getAllSuppliers();
			getAllIngredients();
			getAllIngredientsForCustomer();
			getAllPizzaFlavours();
			getAllPizzaSizes();
			getAllOrderedPizzas();
			getAllComposedPizzas();
		}


		#region CustomerIngredients

		public void getAllIngredientsForCustomer()
		{
			//comboboxIngredients.Items.Clear();
			listViewCustomerIngredients.ItemsSource = null;
			listViewAddIngredients.ItemsSource = null;
			composePizzaList.Clear();
			ingredientListCustomer.Clear();
			con.Open();

			string sql = "select * from funcgetingredientswithstockcustomer();";
			using var cmd = new NpgsqlCommand(sql, con);

			using NpgsqlDataReader rdr = cmd.ExecuteReader();

			while ( rdr.Read() )
			{
				//comboboxIngredients.Items.Add(rdr.GetString(1) + "-" + rdr.GetString(5));
				ingredientListCustomer.Add(new IngredientsVM()
				{
					Id = rdr.GetInt32(0),
					Name = rdr.GetString(1),
					IsVisible = rdr.GetBoolean(2),
					IsDeleted = rdr.GetBoolean(3),
					Price = rdr.GetInt32(4),	
					Region = rdr.GetString(5),
					Stock = rdr.GetInt32(6)
				});
				composePizzaList.Add(new ComposePizzaVM()
				{
					Id = rdr.GetInt32(0),
					Ingredient = rdr.GetString(1) + " - " + rdr.GetString(5),
					Price = rdr.GetInt32(4)
				});
			}
			listViewAddIngredients.ItemsSource = composePizzaList;
			listViewCustomerIngredients.ItemsSource = ingredientListCustomer;
			con.Close();
		}

		private void setIngredientOrder(object sender, RoutedEventArgs e)
		{
			var Id = (int)(sender as CheckBox).Tag;
			var isChecked = (bool)(sender as CheckBox).IsChecked;
			composePizzaList.Where(x => x.Id == Id).FirstOrDefault().isOrdered = isChecked;
		}
		#endregion

		#region Suppliers
		public void getAllSuppliers()
		{
			comboboxSupplier.Items.Clear();
			listViewSuppliers.ItemsSource = null;
			supplierList.Clear();
			con.Open();

			string sql = "SELECT * FROM tblsupplier where isdeleted = false";
			using var cmd = new NpgsqlCommand(sql, con);

			using NpgsqlDataReader rdr = cmd.ExecuteReader();

			while ( rdr.Read() )
			{
				supplierList.Add(new SupplierVM()
				{
					Id = rdr.GetInt32(0),
					Name = rdr.GetString(1),
					IsVisible = rdr.GetBoolean(2),
					IsDeleted = rdr.GetBoolean(3),
				});
			}
			
			foreach(var supplier in supplierList.Where(x=> x.IsVisible == true) )
			{
				comboboxSupplier.Items.Add(supplier.Name);
			}

			listViewSuppliers.ItemsSource = supplierList;
			con.Close();
		}

		private void addNewSupplier(object sender, RoutedEventArgs e)
		{
			string supplierName = txtSupplierName.Text;
			con.Open();
			using ( var cmd = new NpgsqlCommand("CALL procaddsupplier('"+supplierName+"')", con) )
				cmd.ExecuteNonQuery();
			con.Close();
			getAllSuppliers();
		}

		private void deleteSupplier(object sender, RoutedEventArgs e)
		{
			var supplierId = (sender as Button).CommandParameter;
			con.Open();

			using ( var cmd = new NpgsqlCommand("CALL procdeletesupplier('" + supplierId + "')", con) )
				cmd.ExecuteNonQuery();
			con.Close();
			getAllSuppliers();
		}

		private void SetSupplierVisibility(object sender, RoutedEventArgs e)
		{
			var Id = (sender as CheckBox).Tag;
			var isChecked = (sender as CheckBox).IsChecked;

			con.Open();

			using ( var cmd = new NpgsqlCommand("CALL procsetsuppliervisibility('" + Id + "','" + isChecked + "')", con) )
				cmd.ExecuteNonQuery();
			con.Close();
			getAllSuppliers();
		}
		#endregion

		#region Ingredients
		public void getAllIngredients()
		{
			comboboxIngredients.Items.Clear();
			listViewIngredients.ItemsSource = null;
			ingredientList.Clear();
			con.Open();

			string sql = "select * from funcgetingredientswithstockbaker();";
			using var cmd = new NpgsqlCommand(sql, con);

			using NpgsqlDataReader rdr = cmd.ExecuteReader();

			while ( rdr.Read() )
			{
				comboboxIngredients.Items.Add(rdr.GetString(1) + "-" +rdr.GetString(5));
				ingredientList.Add(new IngredientsVM()
				{
					Id = rdr.GetInt32(0),
					Name = rdr.GetString(1),
					IsVisible = rdr.GetBoolean(2),
					IsDeleted = rdr.GetBoolean(3),
					Price = rdr.GetInt32(4),
					Region = rdr.GetString(5),
					Stock = rdr.GetInt32(6)
				});
			}

			listViewIngredients.ItemsSource = ingredientList;
			con.Close();
		}

		private void addNewIngredient(object sender, RoutedEventArgs e)
		{
			string ingredientName = txtIngredientName.Text;
			string ingredientRegion = txtIngredientRegion.Text;
			int ingredientPrice = Convert.ToInt32(txtIngredientPrice.Text);
			con.Open();

			using ( var cmd = new NpgsqlCommand("CALL procaddingredient('" + ingredientName + "','" + ingredientPrice + "','" + ingredientRegion + "')", con) )
				cmd.ExecuteNonQuery();

			con.Close();
			getAllIngredients();
			getAllIngredientsForCustomer();
		}

		private void deleteIngredient(object sender, RoutedEventArgs e)
		{
			var ingredientId = (sender as Button).CommandParameter;
			con.Open();

			using ( var cmd = new NpgsqlCommand("CALL procdeleteingredient('" + ingredientId + "')", con) )
				cmd.ExecuteNonQuery();

			con.Close();
			getAllIngredients();
			getAllIngredientsForCustomer();
		}

		private void SetIngredientVisibility(object sender, RoutedEventArgs e)
		{
			var Id = (sender as CheckBox).Tag;
			var isChecked = (sender as CheckBox).IsChecked;

			con.Open();

			using ( var cmd = new NpgsqlCommand("CALL procsetingredientvisibility('" + Id + "','" + isChecked + "')", con) )
				cmd.ExecuteNonQuery();

			con.Close();
			getAllIngredients();
			getAllIngredientsForCustomer();
		}
		#endregion

		#region BasePizza
		public void getAllPizzaFlavours()
		{
			comboboxBasePizzaCustomer.Items.Clear();
			listViewFlavours.ItemsSource = null;
			flavoursList.Clear();
			con.Open();

			string sql = "SELECT * FROM tblbasepizza";
			using var cmd = new NpgsqlCommand(sql, con);

			using NpgsqlDataReader rdr = cmd.ExecuteReader();

			while ( rdr.Read() )
			{
				comboboxBasePizzaCustomer.Items.Add(rdr.GetString(1));
				flavoursList.Add(new FlavoursVM()
				{
					Id = rdr.GetInt32(0),
					Flavour = rdr.GetString(1),
					Price = rdr.GetInt32(2)
				});
			}
			listViewFlavours.ItemsSource = flavoursList;
			con.Close();

		}

		private void addNewBasePizza(object sender, RoutedEventArgs e)
		{
			string basePizzaName = txtBasePizza.Text;
			int basePizzaPrice = Convert.ToInt32(txtBasePizzaPrice.Text);
			con.Open();

			using ( var cmd = new NpgsqlCommand("CALL procaddbasepizza('" + basePizzaName + "','" + basePizzaPrice + "')", con) )
				cmd.ExecuteNonQuery();

			con.Close();
			getAllPizzaFlavours();
		}

		#endregion

		#region PizzaSize
		public void getAllPizzaSizes()
		{
			comboboxPizzaSizeCustomer.Items.Clear();
			sizesList.Clear();
			listViewSizes.ItemsSource = null;
			con.Open();

			string sql = "SELECT * FROM tblpizzasize";
			using var cmd = new NpgsqlCommand(sql, con);

			using NpgsqlDataReader rdr = cmd.ExecuteReader();

			while ( rdr.Read() )
			{
				comboboxPizzaSizeCustomer.Items.Add(rdr.GetInt32(1));
				sizesList.Add(new SizesVM()
				{
					Id = rdr.GetInt32(0),
					Size = rdr.GetInt32(1),
					Price = rdr.GetInt32(2),
					Name = rdr.GetString(3)
				});
			}
			listViewSizes.ItemsSource = sizesList;
			con.Close();

		}
		#endregion

		#region Stock
		private void addNewStock(object sender, RoutedEventArgs e)
		{
			string ingredientName = comboboxIngredients.Text.Split("-")[0];
			string ingredientRegion = comboboxIngredients.Text.Split("-")[1];
			int supplierId = supplierList.Where(x => x.Name == comboboxSupplier.Text).FirstOrDefault().Id;
			int ingredientId = ingredientList.Where(x => x.Name == ingredientName && x.Region == ingredientRegion).FirstOrDefault().Id;
			int stock = Convert.ToInt32(txtStock.Text);

			con.Open();

			using ( var cmd = new NpgsqlCommand("CALL procaddstock('" + supplierId + "','" + ingredientId + "','" + stock + "')", con) )
				cmd.ExecuteNonQuery();

			con.Close();
			
			getAllIngredients();
			getAllIngredientsForCustomer();
		}
		#endregion

		#region ComposePizza
		private void composeNewPizza(object sender, RoutedEventArgs e)
		{
			int randomNumber = _random.Next(1000);
			string basePizza = comboboxBasePizzaCustomer.Text;
			int pizzaSize = sizesList.Where(x => x.Size == Convert.ToInt32(comboboxPizzaSizeCustomer.Text)).FirstOrDefault().Id;
			var composedIngredietList = composePizzaList.Where(x => x.isOrdered == true).ToList();
			List<int> ingredientsList = new List<int>();
			foreach ( var value in composedIngredietList )
			{
				addComposedPizzaIngredients(randomNumber, value.Id);
				ingredientsList.Add(value.Id);
			}

			int[] ingredientsArray = ingredientsList.ToArray();

			int basePizzaId = flavoursList.Where(x => x.Flavour == basePizza).FirstOrDefault().Id;

			addComposedPizza(basePizzaId, pizzaSize, randomNumber, ingredientsArray);

			getAllComposedPizzas();
		}

		public void addComposedPizzaIngredients(int randNum, int IngredientId)
		{
			con.Open();
			using ( var cmd = new NpgsqlCommand("CALL procaddcomposedpizzaingredients('" + randNum + "','" + IngredientId + "')", con) )
				cmd.ExecuteNonQuery();
			con.Close();
		}

		//CALL procaddcomposedpizza(2,6,499,'{11,9}')
		public void addComposedPizza(int basePizzaId, int pizzaSizeId, int randNum, int[] ingredientsId)
		{
			var concatVal = string.Join(",", ingredientsId);
			con.Open();
			using ( var cmd = new NpgsqlCommand("CALL procaddcomposedpizza('" + basePizzaId + "','" + pizzaSizeId + "','" + randNum + "','{"+ concatVal + "}')", con) )
				cmd.ExecuteNonQuery();
			con.Close();
		}

		public void getAllComposedPizzas()
		{
			//comboboxIngredients.Items.Clear();
			listViewCustomerComposedPizzas.ItemsSource = null;
			customerComposedPizzaList.Clear();
			con.Open();

			string sql = "select * from funcgetcomposedpizza();";
			using var cmd = new NpgsqlCommand(sql, con);

			using NpgsqlDataReader rdr = cmd.ExecuteReader();

			while ( rdr.Read() )
			{
				//comboboxIngredients.Items.Add(rdr.GetString(1) + "-" + rdr.GetString(5));
				customerComposedPizzaList.Add(new CustomerComposedPizza()
				{
					id = rdr.GetInt32(0),
					Price = rdr.GetInt32(1),
					Flavour = rdr.GetString(2),
					Size = rdr.GetInt32(3),
					IngredientsName = rdr.GetString(4),
				});
			}
			listViewCustomerComposedPizzas.ItemsSource = customerComposedPizzaList;
			con.Close();
		}

		//public string getConcatinatedIngredientNames(int val)
		//{
		//	string css = "Host=localhost;Username=postgres;Password=7777;Database=pizzashop";
		//	NpgsqlConnection cone = new NpgsqlConnection(css);
		//	cone.Open();
		//	string sqlw = "select * from funcgetingredientsofcomposedpizza('"+val+"');";
		//	using var cmde = new NpgsqlCommand(sqlw, cone);
		//	var returnedvalues = "";

		//	using NpgsqlDataReader rdre = cmde.ExecuteReader();
		//	while(rdre.Read())
		//	{
		//		returnedvalues = rdre.GetString(0);
		//	}
		//	return returnedvalues;
		//}

		#endregion

		#region orderPizza
		private void orderNewPizza(object sender, RoutedEventArgs e)
		{
			var composedpizzaid = (sender as Button).Tag;
			con.Open();
			using ( var cmd = new NpgsqlCommand("CALL procorderpizza('" + composedpizzaid + "')", con) )
				cmd.ExecuteNonQuery();
			con.Close();

			getAllOrderedPizzas();
		}

		public void getAllOrderedPizzas()
		{
			//comboboxIngredients.Items.Clear();
			listViewOrderedPizzasBaker.ItemsSource = null;
			listViewOrderedPizzasCustomer.ItemsSource = null;
			customerOrderdPizzaList.Clear();
			con.Open();

			string sql = "select * from funcgetorderedpizza();";
			using var cmd = new NpgsqlCommand(sql, con);

			using NpgsqlDataReader rdr = cmd.ExecuteReader();

			while ( rdr.Read() )
			{
				//comboboxIngredients.Items.Add(rdr.GetString(1) + "-" + rdr.GetString(5));
				customerOrderdPizzaList.Add(new CustomerOrderedPizza()
				{
					Flavour = rdr.GetString(0),
					Size = rdr.GetInt32(1),
					Price = rdr.GetInt32(2),
					Datetime = Convert.ToString(rdr.GetDateTime(3)),
					Ingredients = rdr.GetString(4)
				});
			}
			listViewOrderedPizzasBaker.ItemsSource = customerOrderdPizzaList;
			listViewOrderedPizzasCustomer.ItemsSource = customerOrderdPizzaList;
			con.Close();
		}

		#endregion

		private void ComposePizzaSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}
