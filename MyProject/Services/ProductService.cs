using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyProject.Data;
using MyProject.DataModel;
using MyProject.Models;
using System.Linq;
using System.Security.Claims;

namespace MyProject.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
        private readonly string Cachekey = "products";

        public ProductService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<Product>> GetProducts(int userId, string search = "", string sort = "default", int page = 1, int pageSize = 5)
        {
            if (_cache.TryGetValue(Cachekey, out List<Product>? cacheProducts))
            {
                var filteredProduct = cacheProducts
                    .Where(p => p.UserId == userId && (string.IsNullOrEmpty(search) || p.Name.Contains(search)))
                    .AsQueryable();

                switch (sort)
                {
                    case "asc":
                        filteredProduct = filteredProduct.OrderBy(p => p.Name);
                        break;
                    case "desc":
                        filteredProduct = filteredProduct.OrderByDescending(p => p.Name);
                        break;
                    default:
                        filteredProduct = filteredProduct.OrderBy(p => p.Id);
                        break;
                }

                return filteredProduct.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            IQueryable<Product> query = _context.Products.Where(p => p.UserId == userId);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            switch (sort)
            {
                case "asc":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "desc":
                    query = query.OrderByDescending(p => p.Name);
                    break;
                default:
                    query = query.OrderBy(p => p.Id);
                    break;
            }

            var products = await query.ToListAsync();

            _cache.Set(Cachekey, products, _cacheDuration);

            products = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return products;
        }

        public async Task<Product?> GetProduct(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateProduct(ProductModel productModel, int userId)
        {
            var product = new Product
            {
                Name = productModel.Name,
                Price = productModel.Price,
                UserId = userId
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            _cache.Remove(Cachekey);

            return product;
        }

        public async Task<Product?> UpdateProduct(int id, ProductModel productModel, int userId)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(productModel => productModel.Id == id && productModel.UserId == userId);

            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = productModel.Name;
            existingProduct.Price = productModel.Price;

            await _context.SaveChangesAsync();

            return existingProduct;
        }
        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            _cache.Remove(Cachekey);

            return true;
        }
    }
}
