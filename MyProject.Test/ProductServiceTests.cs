using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyProject.Data;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyProject.Models;
using MyProject.Services;
using Moq.EntityFrameworkCore;

namespace MyProject.Test
{
    public class ProductServiceTests
    {
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly ProductService _productService;
        private readonly Mock<ApplicationDbContext> _dbContextMock;
        private readonly int _userId;
        private readonly List<Product> _testProducts;
        public ProductServiceTests()
        {
            _cacheMock = new Mock<IMemoryCache>();
            _dbContextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            _productService = new ProductService(_dbContextMock.Object, _cacheMock.Object);

            _userId = 1;

            _testProducts = new List<Product>
            {
                new Product {Id = 1, Name = "apple", UserId = 1},
                new Product {Id = 2, Name = "banana", UserId = 1},
                new Product {Id = 3, Name = "cherry", UserId = 1},
                new Product {Id = 4, Name = "date", UserId = 1},
            };

            // Assert.NotNull(testProducts);
            // Assert.NotEmpty(testProducts);

            //var mockSet = new Mock<DbSet<Product>>();
            //mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(_testProducts.AsQueryable().Provider);
            //mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(_testProducts.AsQueryable().Expression);
            //mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(_testProducts.AsQueryable().ElementType);
            //mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(_testProducts.GetEnumerator());

            _dbContextMock.Setup(c => c.Products).ReturnsDbSet(_testProducts);
            _cacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);
        }
        [Fact]
        public async Task TestAsceding()
        {
            var result = await _productService.GetProducts(_userId, "", "asc", 1, 2);

            Assert.Equal(2, result.Count);
            Assert.Equal("apple", result[0].Name);
            Assert.Equal("banana", result[1].Name);
        }

        [Fact]
        public async Task TestDesceding()
        {
            var result = await _productService.GetProducts(_userId, "", "desc", 1, 3);

            Assert.Equal(3, result.Count);
            Assert.Equal("date", result[0].Name);
            Assert.Equal("cherry", result[1].Name);
            Assert.Equal("banana", result[2].Name);
        }

        [Fact]
        public async Task TestSortingDefault()
        {
            var result = await _productService.GetProducts(_userId, "", "default", 1, 2);

            Assert.Equal(2, result.Count);
            Assert.Equal("apple", result[0].Name);
            Assert.Equal("banana", result[1].Name);
        }

        [Fact]
        public async Task TestSearching()
        {
            var result = await _productService.GetProducts(_userId, "a", "asc", 1, 3);

            Assert.Equal(3, result.Count);
            Assert.Equal("apple", result[0].Name);
            Assert.Equal("banana", result[1].Name);
            Assert.Equal("date", result[2].Name);
        }
    }
}
