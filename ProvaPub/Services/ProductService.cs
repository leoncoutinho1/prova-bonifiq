using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
	public class ProductService
	{
		TestDbContext _ctx;

		public ProductService(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public CustomList<Product>  ListProducts(int page)
		{
			return new CustomList<Product>() {  HasNext=false, TotalCount =10, Items = _ctx.Products.OrderBy(p => p.Id).Skip((page - 1) * 10).Take(10).ToList() };
		}

	}
}
