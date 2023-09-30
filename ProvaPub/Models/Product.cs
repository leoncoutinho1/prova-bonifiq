namespace ProvaPub.Models
{
	public class Product : IEquatable<Product>
	{
		public int Id { get; set; }	

		public string Name { get; set; }

		public bool Equals(Product? other) => (this.Id == other?.Id); 
	}
}
