using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core.Tests.Helpers;
using TrackableEntities.EF.Core.Tests.Mocks;
using TrackableEntities.EF.Core.Tests.NorthwindModels;
using Xunit;

namespace TrackableEntities.EF.Core.Tests
{
    [Collection("NorthwindDbContext")]
    public class NorthwindDbContextTests
	{
            private readonly NorthwindDbContextFixture _fixture;

            public NorthwindDbContextTests(NorthwindDbContextFixture fixture)
            {
                _fixture = fixture;
                _fixture.Initialize();
            }

        #region Product: Single Entity

        [Fact]
		public void Apply_Changes_Should_Mark_Product_Unchanged()
		{
            // Arrange
            var context = _fixture.GetContext();
            var product = new Product();
			product.TrackingState = TrackingState.Unchanged;

			// Act
			context.ApplyChanges(product);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Product_Added()
		{
            // Arrange
            var context = _fixture.GetContext();
            var product = new Product();
			product.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(product);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(product).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Product_Modified()
		{
            // Arrange
            var context = _fixture.GetContext();
            var product = new Product();
			product.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(product);

			// Assert
			Assert.Equal(EntityState.Modified, context.Entry(product).State);
		}

        [Fact]
        public void Apply_Changes_Should_Mark_Product_Property_Modified()
        {
            // Arrange
            var context = _fixture.GetContext();
            var product = new Product();
            product.TrackingState = TrackingState.Modified;
            product.ModifiedProperties = new List<string> { nameof(Product.UnitPrice) };

            // Act
            context.ApplyChanges(product);

            // Assert
            var priceProp = context.Entry(product).Properties.Single(p => p.Metadata.Name == nameof(Product.UnitPrice));
            var nameProp = context.Entry(product).Properties.Single(p => p.Metadata.Name == nameof(Product.ProductName));
            Assert.True(priceProp.IsModified);
            Assert.False(nameProp.IsModified);
            Assert.Equal(EntityState.Modified, context.Entry(product).State);
        }

        [Fact]
		public void Apply_Changes_Should_Mark_Product_Deleted()
		{
            // Arrange
            var context = _fixture.GetContext();
            var product = new Product();
			product.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(product);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(product).State);
		}

		#endregion

		#region Product: Multiple Entities

		[Fact]
		public void Apply_Changes_Should_Mark_Products_Unchanged()
		{
            // Arrange
            var context = _fixture.GetContext();
            var products = new List<Product>
			{
				new Product {ProductId = 1}, 
				new Product { ProductId = 2}
			};
			products.ForEach(p => p.TrackingState = TrackingState.Unchanged);

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(products[0]).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(products[1]).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Products_Added()
		{
            // Arrange
            var context = _fixture.GetContext();
            var products = new List<Product>
			{
				new Product {ProductId = 1}, 
				new Product { ProductId = 2}
			};
			products.ForEach(p => p.TrackingState = TrackingState.Added);

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(products[0]).State);
			Assert.Equal(EntityState.Added, context.Entry(products[1]).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Products_Modified()
		{
            // Arrange
            var context = _fixture.GetContext();
            var products = new List<Product>
			{
				new Product {ProductId = 1}, 
				new Product { ProductId = 2}
			};
			products.ForEach(p => p.TrackingState = TrackingState.Modified);

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.Equal(EntityState.Modified, context.Entry(products[0]).State);
			Assert.Equal(EntityState.Modified, context.Entry(products[1]).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Products_Deleted()
		{
            // Arrange
            var context = _fixture.GetContext();
            var products = new List<Product>
			{
				new Product {ProductId = 1}, 
				new Product { ProductId = 2}
			};
			products.ForEach(p => p.TrackingState = TrackingState.Deleted);

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(products[0]).State);
			Assert.Equal(EntityState.Deleted, context.Entry(products[1]).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Products()
		{
            // Arrange
            var context = _fixture.GetContext();
            var products = new List<Product>
			{
				new Product { ProductId = 1}, 
				new Product { ProductId = 2},
				new Product { ProductId = 3},
				new Product { ProductId = 4},
			};
			products[1].TrackingState = TrackingState.Modified;
			products[2].TrackingState = TrackingState.Added;
			products[3].TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(products);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(products[0]).State);
			Assert.Equal(EntityState.Modified, context.Entry(products[1]).State);
			Assert.Equal(EntityState.Added, context.Entry(products[2]).State);
			Assert.Equal(EntityState.Deleted, context.Entry(products[3]).State);
		}

		#endregion

		#region Order: One to Many

		[Fact]
		public void Apply_Changes_Should_Mark_Order_Unchanged()
		{
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(detail1).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(detail2).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(detail3).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(detail4).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Order_Added()
		{
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			order.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(order).State);
			Assert.Equal(EntityState.Added, context.Entry(detail1).State);
			Assert.Equal(EntityState.Added, context.Entry(detail2).State);
			Assert.Equal(EntityState.Added, context.Entry(detail3).State);
			Assert.Equal(EntityState.Added, context.Entry(detail4).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Order_Deleted()
		{
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
			order.CustomerId = null;
			order.Customer = null;
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			order.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(order).State);
			Assert.Equal(EntityState.Deleted, context.Entry(detail1).State);
			Assert.Equal(EntityState.Deleted, context.Entry(detail2).State);
			Assert.Equal(EntityState.Deleted, context.Entry(detail3).State);
			Assert.Equal(EntityState.Deleted, context.Entry(detail4).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Order_Only_Modified()
		{
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			order.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Modified, context.Entry(order).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(detail1).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(detail2).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(detail3).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(detail4).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Order_Details_Only_Modified()
		{
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			detail1.TrackingState = TrackingState.Modified;
			detail2.TrackingState = TrackingState.Modified;
			detail3.TrackingState = TrackingState.Modified;
			detail4.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
			Assert.Equal(EntityState.Modified, context.Entry(detail1).State);
			Assert.Equal(EntityState.Modified, context.Entry(detail2).State);
			Assert.Equal(EntityState.Modified, context.Entry(detail3).State);
			Assert.Equal(EntityState.Modified, context.Entry(detail4).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Order_With_Details_Modified()
		{
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			order.TrackingState = TrackingState.Modified;
			detail1.TrackingState = TrackingState.Modified;
			detail2.TrackingState = TrackingState.Modified;
			detail3.TrackingState = TrackingState.Modified;
			detail4.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Modified, context.Entry(order).State);
			Assert.Equal(EntityState.Modified, context.Entry(detail1).State);
			Assert.Equal(EntityState.Modified, context.Entry(detail2).State);
			Assert.Equal(EntityState.Modified, context.Entry(detail3).State);
			Assert.Equal(EntityState.Modified, context.Entry(detail4).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Order_Unchanged_With_OrderDetails_Added_Modified_Deleted_Unchanged()
		{
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
			var detail1 = order.OrderDetails[0];
		    detail1.OrderDetailId = 0;
			var detail2 = order.OrderDetails[1];
			var detail3 = order.OrderDetails[2];
			var detail4 = order.OrderDetails[3];
			detail1.TrackingState = TrackingState.Added;
			detail2.TrackingState = TrackingState.Modified;
			detail3.TrackingState = TrackingState.Deleted;
			detail4.TrackingState = TrackingState.Unchanged;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
			Assert.Equal(EntityState.Added, context.Entry(detail1).State);
			Assert.Equal(EntityState.Modified, context.Entry(detail2).State);
			Assert.Equal(EntityState.Deleted, context.Entry(detail3).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(detail4).State);
		}

        [Fact]
        public void Apply_Changes_Should_Mark_Order_Modified_With_OrderDetails_Added_Modified_Deleted_Unchanged()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            detail1.OrderDetailId = 0;
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];
            var detail4 = order.OrderDetails[3];
            order.TrackingState = TrackingState.Modified;
            detail1.TrackingState = TrackingState.Added;
            detail2.TrackingState = TrackingState.Modified;
            detail3.TrackingState = TrackingState.Deleted;
            detail4.TrackingState = TrackingState.Unchanged;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(order).State);
            Assert.Equal(EntityState.Added, context.Entry(detail1).State);
            Assert.Equal(EntityState.Modified, context.Entry(detail2).State);
            Assert.Equal(EntityState.Deleted, context.Entry(detail3).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(detail4).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(detail1.Product).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Product_Of_Added_OrderDetail_Of_Added_Order_As_Unchanged()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            var orderDetail = order.OrderDetails[0];
            var product = orderDetail.Product;
            order.TrackingState = TrackingState.Added;
            orderDetail.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Added, context.Entry(order).State);
            Assert.Equal(EntityState.Added, context.Entry(orderDetail).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_With_Multiple_OrderDetails_Added()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            detail1.OrderDetailId = 0;
            detail2.OrderDetailId = 0;
            detail1.TrackingState = TrackingState.Added;
            detail2.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            Assert.Equal(EntityState.Added, context.Entry(detail1).State);
            Assert.Equal(EntityState.Added, context.Entry(detail2).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Modified_Order_With_Multiple_OrderDetails_Added()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            detail1.OrderDetailId = 0;
            detail2.OrderDetailId = 0;
            detail1.TrackingState = TrackingState.Added;
            detail2.TrackingState = TrackingState.Added;
            order.TrackingState = TrackingState.Modified;
            order.ModifiedProperties = new List<string> {"OrderDate"};

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(order).State);
            Assert.Equal(EntityState.Added, context.Entry(detail1).State);
            Assert.Equal(EntityState.Added, context.Entry(detail2).State);
        }

        #endregion

        #region Order: Many to One to Many

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Unchanged_Customer_With_Addresses_Multiple_Added()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress>
                { address1, address2 };
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Unchanged_Customer_With_Addresses_Mutliple_Added_And_Modified()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3 };
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
            Assert.Equal(EntityState.Modified, context.Entry(address3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Unchanged_Customer_With_Addresses_Multiple_Added_And_Deleted()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3 };
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
            Assert.Equal(EntityState.Deleted, context.Entry(address3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Unchanged_Customer_With_Addresses_Multiple_Added_And_Unchanged()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3 };
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Unchanged;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(address3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Modified_Customer_With_Addresses_Multiple_Added()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2 };
            order.Customer.TrackingState = TrackingState.Modified;
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            Assert.Equal(EntityState.Modified, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Modified_Customer_With_Addresses_Mutliple_Added_And_Modified()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3 };
            order.Customer.TrackingState = TrackingState.Modified;
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            Assert.Equal(EntityState.Modified, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
            Assert.Equal(EntityState.Modified, context.Entry(address3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Modified_Customer_With_Addresses_Multiple_Added_And_Deleted()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3 };
            order.Customer.TrackingState = TrackingState.Modified;
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            Assert.Equal(EntityState.Modified, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
            Assert.Equal(EntityState.Deleted, context.Entry(address3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Modified_Customer_With_Addresses_Multiple_Added_And_Unchanged()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3 };
            order.Customer.TrackingState = TrackingState.Modified;
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Unchanged;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            Assert.Equal(EntityState.Modified, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(address3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Order_Deleted_Customer_With_Addresses_Multiple_Added()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2 };
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            order.Customer.TrackingState = TrackingState.Deleted;

            // Act
            // Note: When navigating from Order (many side) to Customer (one side), the Customer's
            // Deleted state is overridden to Unchanged since the Customer may be related to other entities.
            // However, the Customer's TrackingState property is still Deleted, so when processing
            // child entities (CustomerAddresses), they are set to Deleted based on the parent's TrackingState.
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
            // Customer is set to Unchanged because it's reached via OneToMany navigation from Order
            Assert.Equal(EntityState.Unchanged, context.Entry(order.Customer).State);
            // Addresses are set to Deleted because parent's TrackingState is Deleted (even though parent EntityState is Unchanged)
            Assert.Equal(EntityState.Deleted, context.Entry(address1).State);
            Assert.Equal(EntityState.Deleted, context.Entry(address2).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Modified_Order_Unchanged_Customer_With_Addresses_Multiple_Added()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2 };
            order.TrackingState = TrackingState.Modified;
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(order).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Modified_Order_Unchanged_Customer_With_Addresses_Mutliple_Added_And_Modified()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3 };
            order.TrackingState = TrackingState.Modified;
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(order).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
            Assert.Equal(EntityState.Modified, context.Entry(address3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Modified_Order_Unchanged_Customer_With_Addresses_Multiple_Added_And_Deleted()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3 };
            order.TrackingState = TrackingState.Modified;
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(order).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
            Assert.Equal(EntityState.Deleted, context.Entry(address3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Modified_Order_Unchanged_Customer_With_Addresses_Multiple_Added_And_Unchanged()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            order.OrderDetails = null;
            var address1 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street1",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address2 = new CustomerAddress
            {
                CustomerAddressId = 0,
                Street = "Street2",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            var address3 = new CustomerAddress
            {
                CustomerAddressId = 1,
                Street = "Street3",
                CustomerId = order.Customer.CustomerId,
                Customer = order.Customer
            };
            order.Customer.CustomerAddresses = new List<CustomerAddress> { address1, address2, address3 };
            order.TrackingState = TrackingState.Modified;
            address1.TrackingState = TrackingState.Added;
            address2.TrackingState = TrackingState.Added;
            address3.TrackingState = TrackingState.Unchanged;

            // Act
            context.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(order).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(order.Customer).State);
            Assert.Equal(EntityState.Added, context.Entry(address1).State);
            Assert.Equal(EntityState.Added, context.Entry(address2).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(address3).State);
        }
        #endregion

        #region Employee-Territory: Many to Many

        [Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Unchanged_Territories_As_Unchanged()
		{
            // Arrange
            var context = _fixture.GetContext();
            var nw = new MockNorthwind();
			var employee = nw.Employees[0];
            var territory1 = employee.EmployeeTerritories[0].Territory;
            var territory2 = employee.EmployeeTerritories[1].Territory;
            var territory3 = employee.EmployeeTerritories[2].Territory;

            // Act
            context.ApplyChanges(employee);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(employee).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory3).State);
        }

		[Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Territories_As_Added_Modified()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
		    var territory1 = employee.EmployeeTerritories[0].Territory;
		    var territory2 = employee.EmployeeTerritories[1].Territory;
            territory1.TrackingState = TrackingState.Added;
		    territory2.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(employee);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(employee).State);
            Assert.Equal(EntityState.Added, context.Entry(territory1).State);
            Assert.Equal(EntityState.Modified, context.Entry(territory2).State);
        }

	    [Fact]
	    public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Deleted_Territory_As_Unchanged()
	    {
            // NOTE: Deleting a related M-M entity will simply mark it as unchanged,
            // because it may have other related entities.

	        // Arrange
	        var context = _fixture.GetContext();
	        var nw = new MockNorthwind();
	        var employee = nw.Employees[0];
	        var territory1 = employee.EmployeeTerritories[0].Territory;
	        territory1.TrackingState = TrackingState.Deleted;

	        // Act
	        context.ApplyChanges(employee);

	        // Assert
	        Assert.Equal(EntityState.Unchanged, context.Entry(employee).State);
	        Assert.Equal(EntityState.Unchanged, context.Entry(territory1).State);
	    }

	    [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Added_EmployeeTerritories_As_Added()
        {
            // Arrange
            var context = _fixture.GetContext();
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var empTerritory = employee.EmployeeTerritories[0];
            empTerritory.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(employee).State);
            Assert.Equal(EntityState.Added, context.Entry(empTerritory).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Deleted_EmployeeTerritories_As_Deleted()
        {
            // Arrange
            var context = _fixture.GetContext();
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var empTerritory = employee.EmployeeTerritories[0];
            empTerritory.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(employee).State);
            Assert.Equal(EntityState.Deleted, context.Entry(empTerritory).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Unchanged_Employee_As_Unchanged_And_Unchanged_Territories_With_Modified_Area_As_Modified()
        {
            // Ensure that changes are applied across M-M relationships.

            // Arrange
            var context = _fixture.GetContext();
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.EmployeeTerritories[0].Territory;
            var territory2 = employee.EmployeeTerritories[1].Territory;
            var territory3 = employee.EmployeeTerritories[2].Territory;
            var area = new Area
            {
                AreaId = 1,
                AreaName = "Northern",
                TrackingState = TrackingState.Modified
            };
            territory3.AreaId = 1;
            territory3.Area = area;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(employee).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory3).State);
            Assert.Equal(EntityState.Modified, context.Entry(area).State);
        }

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Unchanged_Territories_As_Unchanged()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Added;
		    var territory1 = employee.EmployeeTerritories[0].Territory;
		    var territory2 = employee.EmployeeTerritories[1].Territory;
		    var territory3 = employee.EmployeeTerritories[2].Territory;

            // Act
            context.ApplyChanges(employee);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(employee).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory3).State);
        }

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Territories_As_Added_Modified()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Added;
		    var territory1 = employee.EmployeeTerritories[0].Territory;
		    var territory2 = employee.EmployeeTerritories[1].Territory;
		    territory1.TrackingState = TrackingState.Added;
		    territory2.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(employee);

            // Assert
		    Assert.Equal(EntityState.Added, context.Entry(employee).State);
		    Assert.Equal(EntityState.Added, context.Entry(territory1).State);
		    Assert.Equal(EntityState.Modified, context.Entry(territory2).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Deleted_Territories_As_Unchanged()
		{
		    // NOTE: Deleting a related M-M entity will simply mark it as unchanged,
		    // because it may have other related entities.
			
            // Arrange
            var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var employee = nw.Employees[0];
			employee.TrackingState = TrackingState.Added;
		    var territory1 = employee.EmployeeTerritories[0].Territory;
            territory1.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(employee);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(employee).State);
		    Assert.Equal(EntityState.Unchanged, context.Entry(territory1).State);
		}

	    [Fact]
	    public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Unchanged_EmployeeTerritories_As_Added()
	    {
	        // NOTE: If parent is Added, M-M relation entity will be marked as Added,
	        // even if it is marked as Unchanged.
	
            // Arrange
            var context = _fixture.GetContext();
	        var nw = new MockNorthwind();
	        var employee = nw.Employees[0];
	        var empTerritory = employee.EmployeeTerritories[0];
	        employee.TrackingState = TrackingState.Added;

	        // Act
	        context.ApplyChanges(employee);

	        // Assert
	        Assert.Equal(EntityState.Added, context.Entry(employee).State);
	        Assert.Equal(EntityState.Added, context.Entry(empTerritory).State);
	    }

	    [Fact]
	    public void Apply_Changes_Should_Mark_Added_Employee_As_Added_And_Deleted_EmployeeTerritories_As_Deleted()
	    {
            // NOTE: If parent is Added, M-M relation entity will be marked as Added,
            // even if it is marked as Deleted.

	        // Arrange
	        var context = _fixture.GetContext();
	        var nw = new MockNorthwind();
	        var employee = nw.Employees[0];
	        var empTerritory = employee.EmployeeTerritories[0];
	        employee.TrackingState = TrackingState.Added;
	        empTerritory.TrackingState = TrackingState.Deleted;

	        // Act
	        context.ApplyChanges(employee);

	        // Assert
	        Assert.Equal(EntityState.Added, context.Entry(employee).State);
	        Assert.Equal(EntityState.Added, context.Entry(empTerritory).State);
	    }

        [Fact]
        public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Unchanged_Territories_As_Unchanged()
        {
            // Arrange
            var context = _fixture.GetContext();
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Deleted;
            var territory1 = employee.EmployeeTerritories[0].Territory;
            var territory2 = employee.EmployeeTerritories[1].Territory;
            var territory3 = employee.EmployeeTerritories[2].Territory;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.Equal(EntityState.Deleted, context.Entry(employee).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory1).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory2).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Deleted_Employee_As_Added_And_Territories_As_Added_Modified()
        {
            // Arrange
            var context = _fixture.GetContext();
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Deleted;
            var territory1 = employee.EmployeeTerritories[0].Territory;
            var territory2 = employee.EmployeeTerritories[1].Territory;
            territory1.TrackingState = TrackingState.Added;
            territory2.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.Equal(EntityState.Deleted, context.Entry(employee).State);
            Assert.Equal(EntityState.Added, context.Entry(territory1).State);
            Assert.Equal(EntityState.Modified, context.Entry(territory2).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Deleted_Employee_As_Added_And_Deleted_Territories_As_Unchanged()
        {
            // NOTE: Deleting a related M-M entity will simply mark it as unchanged,
            // because it may have other related entities.

            // Arrange
            var context = _fixture.GetContext();
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            employee.TrackingState = TrackingState.Deleted;
            var territory1 = employee.EmployeeTerritories[0].Territory;
            territory1.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(employee);

            // Assert
            Assert.Equal(EntityState.Deleted, context.Entry(employee).State);
            Assert.Equal(EntityState.Unchanged, context.Entry(territory1).State);
        }

        [Fact]
	    public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Unchanged_EmployeeTerritories_As_Deleted()
	    {
	        // NOTE: If parent is Deleted, M-M relation entity will be marked as Deleted,
	        // even if it is marked as Unchanged.

	        // Arrange
	        var context = _fixture.GetContext();
	        var nw = new MockNorthwind();
	        var employee = nw.Employees[0];
	        var empTerritory = employee.EmployeeTerritories[0];
	        employee.TrackingState = TrackingState.Deleted;

	        // Act
	        context.ApplyChanges(employee);

	        // Assert
	        Assert.Equal(EntityState.Deleted, context.Entry(employee).State);
	        Assert.Equal(EntityState.Deleted, context.Entry(empTerritory).State);
	    }

	    [Fact]
	    public void Apply_Changes_Should_Mark_Deleted_Employee_As_Deleted_And_Added_EmployeeTerritories_As_Deleted()
	    {
	        // NOTE: If parent is Deleted, M-M relation entity will be marked as Deleted,
	        // even if it is marked as Added.

	        // Arrange
	        var context = _fixture.GetContext();
	        var nw = new MockNorthwind();
	        var employee = nw.Employees[0];
	        var empTerritory = employee.EmployeeTerritories[0];
	        employee.TrackingState = TrackingState.Deleted;
	        empTerritory.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(employee);

	        // Assert
	        Assert.Equal(EntityState.Deleted, context.Entry(employee).State);
	        Assert.Equal(EntityState.Deleted, context.Entry(empTerritory).State);
	    }

        #endregion

        #region Customer-CustomerSetting: One to One

        [Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Customer_As_Unchanged_And_Unchanged_Setting_As_Unchanged()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			var setting = customer.CustomerSetting = new CustomerSetting 
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(setting).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Customer_As_Unchanged_And_Modified_Setting_As_Modified()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			var setting = customer.CustomerSetting = new CustomerSetting 
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
			Assert.Equal(EntityState.Modified, context.Entry(setting).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Customer_As_Unchanged_And_Added_Setting_As_Added()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			var setting = customer.CustomerSetting = new CustomerSetting 
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
			Assert.Equal(EntityState.Added, context.Entry(setting).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Customer_As_Unchanged_And_Deleted_Setting_As_Deleted()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			var setting = customer.CustomerSetting = new CustomerSetting 
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
			Assert.Equal(EntityState.Deleted, context.Entry(setting).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Customer_As_Added_And_Unchanged_Setting_As_Added()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Added;
			var setting = customer.CustomerSetting = new CustomerSetting
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(customer).State);
			Assert.Equal(EntityState.Added, context.Entry(setting).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Customer_As_Added_And_Modified_Setting_As_Added()
		{
			// NOTE: Because customer is added, modified setting will be added.

			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Added;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(customer).State);
			Assert.Equal(EntityState.Added, context.Entry(setting).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Customer_As_Added_And_Added_Setting_As_Added()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Added;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(customer).State);
			Assert.Equal(EntityState.Added, context.Entry(setting).State);
		}

        [Fact]
        public void Apply_Changes_Should_Mark_Added_Customer_As_Added_And_Unchanged_Setting_Order_OrderDetail_As_Added()
        {
            // NOTE: Customer is added, Order and OrderDetail are added due to 1-M relation

            // Arrange
            var context = _fixture.GetContext();
            var nw = new MockNorthwind();

            var customer = nw.Customers[0];
            customer.TrackingState = TrackingState.Added;

            var customerSetting = new CustomerSetting() { CustomerId = customer.CustomerId, Setting = "Setting1" };
            customer.CustomerSetting = customerSetting;

            var order = new Order() { OrderDate = DateTime.Now };
            customer.Orders = new List<Order>() { order };

            var orderDetail = new OrderDetail() { ProductId = nw.Products[0].ProductId, Quantity = 1, UnitPrice = 1 };
            order.OrderDetails = new List<OrderDetail>() { orderDetail };

            // Act
            context.ApplyChanges(customer);

            // Assert
            Assert.Equal(EntityState.Added, context.Entry(customer).State);
            Assert.Equal(EntityState.Added, context.Entry(customerSetting).State);
            Assert.Equal(EntityState.Added, context.Entry(order).State);
            Assert.Equal(EntityState.Added, context.Entry(orderDetail).State);
        }

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Customer_As_Modified_And_Deleted_Setting_As_Deleted()
		{
			// NOTE: Because customer is added, removing setting is ignored

			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Added;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(customer).State);
			Assert.Equal(EntityState.Added, context.Entry(setting).State);
		}

        [Fact]
		public void Apply_Changes_Should_Mark_Deleted_Customer_As_Deleted_And_Unchanged_Setting_As_Deleted()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Deleted;
			var setting = customer.CustomerSetting = new CustomerSetting
				{ CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(customer).State);
			Assert.Equal(EntityState.Deleted, context.Entry(setting).State);
		}

        [Fact]
		public void Apply_Changes_Should_Mark_Deleted_Customer_As_Deleted_And_Added_Setting_As_Deleted()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Deleted;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(customer).State);
			Assert.Equal(EntityState.Deleted, context.Entry(setting).State);
		}

        [Fact]
		public void Apply_Changes_Should_Mark_Deleted_Customer_As_Deleted_And_Deleted_Setting_As_Deleted()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var customer = nw.Customers[0];
			customer.TrackingState = TrackingState.Deleted;
			var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
			setting.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(customer);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(customer).State);
			Assert.Equal(EntityState.Deleted, context.Entry(setting).State);
		}

		#endregion

		#region Order-Customer: Many-to-One

		[Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Order_As_Unchanged_And_Unchanged_Customer_As_Unchanged()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			var customer = order.Customer;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Order_As_Unchanged_And_Modified_Customer_As_Modified()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
			Assert.Equal(EntityState.Modified, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Order_As_Unchanged_And_Added_Customer_As_Added()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
			Assert.Equal(EntityState.Added, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Unchanged_Order_As_Unchanged_And_Deleted_Customer_As_Unchanged()
		{
			// NOTE: We ignore deletes of related M-1 entities to because it may be related
			// to other entities.

			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Unchanged, context.Entry(order).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Order_As_Added_And_Unchanged_Customer_As_Unchanged()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Added;
			var customer = order.Customer;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(order).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Order_As_Added_And_Modified_Customer_As_Modified()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Added;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(order).State);
			Assert.Equal(EntityState.Modified, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Order_As_Added_And_Added_Customer_As_Added()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Added;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(order).State);
			Assert.Equal(EntityState.Added, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Added_Order_As_Added_And_Deleted_Customer_As_Unchanged()
		{
			// NOTE: We ignore deletes of related M-1 entities to because it may be related
			// to other entities.

			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Added;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Added, context.Entry(order).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Deleted_Order_As_Deleted_And_Unchanged_Customer_As_Unchanged()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Deleted;
			var customer = order.Customer;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(order).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Deleted_Order_As_Deleted_And_Modified_Customer_As_Modified()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Deleted;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Modified;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(order).State);
			Assert.Equal(EntityState.Modified, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Deleted_Order_As_Deleted_And_Added_Customer_As_Added()
		{
			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Deleted;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Added;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(order).State);
			Assert.Equal(EntityState.Added, context.Entry(customer).State);
		}

		[Fact]
		public void Apply_Changes_Should_Mark_Deleted_Order_As_Deleted_And_Deleted_Customer_As_Unchanged()
		{
			// NOTE: We ignore deletes of related M-1 entities to because it may be related
			// to other entities.

			// Arrange
			var context = _fixture.GetContext();
			var nw = new MockNorthwind();
			var order = nw.Orders[0];
			order.TrackingState = TrackingState.Deleted;
			var customer = order.Customer;
			customer.TrackingState = TrackingState.Deleted;

			// Act
			context.ApplyChanges(order);

			// Assert
			Assert.Equal(EntityState.Deleted, context.Entry(order).State);
			Assert.Equal(EntityState.Unchanged, context.Entry(customer).State);
		}

        #endregion

        #region OneToMany AcceptChanges Tests
        
        [Fact]
        public void Accept_Changes_Should_Mark_Multiple_Orders_As_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order1 = northwind.Orders[0];
            order1.TrackingState = TrackingState.Modified;
            order1.Customer.TrackingState = TrackingState.Modified;
            order1.OrderDetails[1].TrackingState = TrackingState.Modified;
            order1.OrderDetails[2].TrackingState = TrackingState.Added;
            order1.OrderDetails[3].TrackingState = TrackingState.Deleted;

            var order2 = northwind.Orders[2];
            order2.Customer.TrackingState = TrackingState.Modified;
            order2.OrderDetails[0].TrackingState = TrackingState.Modified;
            order2.OrderDetails[1].TrackingState = TrackingState.Added;
            order2.OrderDetails[2].TrackingState = TrackingState.Deleted;

            // Act
            var orders = new List<Order> { order1, order2 };
            var context = _fixture.GetContext();
            context.AcceptChanges(orders);

            // Assert
            Assert.Equal(TrackingState.Unchanged, order1.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order1.Customer.TrackingState);
            Assert.DoesNotContain(order1.OrderDetails, d => d.TrackingState != TrackingState.Unchanged);
            Assert.Equal(TrackingState.Unchanged, order2.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order2.Customer.TrackingState);
            Assert.DoesNotContain(order2.OrderDetails, d => d.TrackingState != TrackingState.Unchanged);
        }

        #endregion

        #region ManyToOne AcceptChanges Tests

        [Fact]
        public void Accept_Changes_Should_Mark_Order_With_Modified_Customer_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            order.TrackingState = TrackingState.Modified;
            order.Customer.TrackingState = TrackingState.Modified;

            // Act
            var context = _fixture.GetContext();
            context.AcceptChanges(order);

            // Assert
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order.Customer.TrackingState);
        }

        [Fact]
        public void Accept_Changes_Should_Mark_Order_With_Added_Customer_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            order.TrackingState = TrackingState.Modified;
            order.Customer.TrackingState = TrackingState.Added;

            // Act
            var context = _fixture.GetContext();
            context.AcceptChanges(order);

            // Assert
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order.Customer.TrackingState);
        }

        [Fact]
        public void Accept_Changes_Should_Mark_Order_With_Deleted_Customer_Unchanged()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            order.TrackingState = TrackingState.Modified;
            order.Customer.TrackingState = TrackingState.Deleted;

            // Act
            var context = _fixture.GetContext();
            context.AcceptChanges(order);

            // Assert
            Assert.Equal(TrackingState.Unchanged, order.TrackingState);
            Assert.Equal(TrackingState.Unchanged, order.Customer.TrackingState);
        }

        [Fact]
        public void Accept_Changes_Should_Remove_ModifiedProperties_From_Order_With_Customer()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            order.TrackingState = TrackingState.Modified;
            order.ModifiedProperties = new List<string> { "OrderDate" };
            order.Customer.TrackingState = TrackingState.Modified;
            order.Customer.ModifiedProperties = new List<string> { "CustomerName" };

            // Act
            var context = _fixture.GetContext();
            context.AcceptChanges(order);

            // Assert
            Assert.DoesNotContain(context.GetModifiedProperties(order), p => p?.Count > 0);
            Assert.DoesNotContain(context.GetModifiedProperties(order.Customer), p => p?.Count > 0);
        }

        #endregion

        #region OriginalValues Tests

        [Fact]
        public void OriginalValues_Should_Store_Original_Value_For_Modified_Property()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var product = northwind.Products[0];
            var originalPrice = product.UnitPrice;
            
            // Act - Set original value and modify property
            product.SetOriginalValue(nameof(Product.UnitPrice), originalPrice);
            product.UnitPrice = 99.99m;
            product.ModifiedProperties = new List<string> { nameof(Product.UnitPrice) };
            
            // Assert
            Assert.NotNull(product.OriginalValues);
            Assert.Contains(nameof(Product.UnitPrice), product.OriginalValues.Keys);
            Assert.Equal(originalPrice, product.OriginalValues[nameof(Product.UnitPrice)]);
        }

        [Fact]
        public void GetOriginalValue_Should_Return_Correct_Value()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var product = northwind.Products[0];
            var originalPrice = product.UnitPrice;
            
            // Act
            product.SetOriginalValue(nameof(Product.UnitPrice), originalPrice);
            product.UnitPrice = 99.99m;
            
            // Assert
            bool exists = product.GetOriginalValue<decimal>(nameof(Product.UnitPrice), out var retrievedValue);
            Assert.True(exists);
            Assert.Equal(originalPrice, retrievedValue);
        }

        [Fact]
        public void GetPropertyChanges_Should_Return_All_Changes()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var product = northwind.Products[0];
            var originalPrice = product.UnitPrice;
            var originalName = product.ProductName;
            
            // Act
            product.SetOriginalValue(nameof(Product.UnitPrice), originalPrice);
            product.SetOriginalValue(nameof(Product.ProductName), originalName);
            product.UnitPrice = 99.99m;
            product.ProductName = "New Name";
            product.ModifiedProperties = new List<string> { nameof(Product.UnitPrice), nameof(Product.ProductName) };
            
            // Assert
            var changes = product.GetPropertyChanges();
            Assert.Equal(2, changes.Count);
            
            var priceChange = changes[nameof(Product.UnitPrice)];
            Assert.Equal(originalPrice, priceChange.OriginalValue);
            Assert.Equal(99.99m, priceChange.CurrentValue);
            
            var nameChange = changes[nameof(Product.ProductName)];
            Assert.Equal(originalName, nameChange.OriginalValue);
            Assert.Equal("New Name", nameChange.CurrentValue);
        }

        [Fact]
        public void GetPropertyChanges_Should_Return_Empty_When_No_ModifiedProperties()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var product = northwind.Products[0];
            
            // Act
            product.SetOriginalValue(nameof(Product.UnitPrice), 10.00m);
            product.UnitPrice = 99.99m;
            // Note: ModifiedProperties is null
            
            // Assert
            var changes = product.GetPropertyChanges();
            Assert.Empty(changes);
        }

        [Fact]
        public void OriginalValues_Should_Be_Applied_To_EF_Change_Tracker()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var product = northwind.Products[0];
            var originalPrice = product.UnitPrice;
            
            // Act
            product.SetOriginalValue(nameof(Product.UnitPrice), originalPrice);
            product.UnitPrice = 99.99m;
            product.ModifiedProperties = new List<string> { nameof(Product.UnitPrice) };
            product.TrackingState = TrackingState.Modified;
            
            var context = _fixture.GetContext();
            context.ApplyChanges(product);
            
            // Assert
            var entry = context.Entry(product);
            Assert.Equal(EntityState.Modified, entry.State);
            Assert.True(entry.Property(nameof(Product.UnitPrice)).IsModified);
            Assert.Equal(originalPrice, entry.Property(nameof(Product.UnitPrice)).OriginalValue);
            Assert.Equal(99.99m, entry.Property(nameof(Product.UnitPrice)).CurrentValue);
        }

        [Fact]
        public void OriginalValues_Should_Be_Applied_Only_For_Modified_Properties()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var product = northwind.Products[0];
            var originalPrice = product.UnitPrice;
            var originalName = product.ProductName;
            
            // Act
            product.SetOriginalValue(nameof(Product.UnitPrice), originalPrice);
            product.SetOriginalValue(nameof(Product.ProductName), originalName);
            product.UnitPrice = 99.99m;
            product.ProductName = "New Name";
            product.ModifiedProperties = new List<string> { nameof(Product.UnitPrice) }; // Only UnitPrice is modified
            product.TrackingState = TrackingState.Modified;
            
            var context = _fixture.GetContext();
            context.ApplyChanges(product);
            
            // Assert
            var entry = context.Entry(product);
            Assert.True(entry.Property(nameof(Product.UnitPrice)).IsModified);
            Assert.Equal(originalPrice, entry.Property(nameof(Product.UnitPrice)).OriginalValue);
            
            Assert.False(entry.Property(nameof(Product.ProductName)).IsModified);
            // For non-modified properties, EF Core sets OriginalValue to CurrentValue
            Assert.Equal("New Name", entry.Property(nameof(Product.ProductName)).OriginalValue);
        }

        [Fact]
        public void AcceptChanges_Should_Clear_OriginalValues()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var product = northwind.Products[0];
            product.SetOriginalValue(nameof(Product.UnitPrice), 10.00m);
            product.ModifiedProperties = new List<string> { nameof(Product.UnitPrice) };
            product.TrackingState = TrackingState.Modified;
            
            // Act
            var context = _fixture.GetContext();
            context.ApplyChanges(product);
            context.AcceptChanges(product);
            
            // Assert
            Assert.Null(product.OriginalValues);
            Assert.Null(product.ModifiedProperties);
            Assert.Equal(TrackingState.Unchanged, product.TrackingState);
        }

        [Fact]
        public void OriginalValues_Should_Work_With_Object_Graph()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var order = northwind.Orders[0];
            var originalOrderDate = order.OrderDate;
            var originalCustomerName = order.Customer.CustomerName;
            
            // Act
            order.SetOriginalValue(nameof(Order.OrderDate), originalOrderDate);
            order.Customer.SetOriginalValue(nameof(Customer.CustomerName), originalCustomerName);
            order.OrderDate = DateTime.Now;
            order.Customer.CustomerName = "New Customer Name";
            order.ModifiedProperties = new List<string> { nameof(Order.OrderDate) };
            order.Customer.ModifiedProperties = new List<string> { nameof(Customer.CustomerName) };
            order.TrackingState = TrackingState.Modified;
            order.Customer.TrackingState = TrackingState.Modified;
            
            var context = _fixture.GetContext();
            context.ApplyChanges(order);
            
            // Assert
            var orderEntry = context.Entry(order);
            Assert.Equal(originalOrderDate, orderEntry.Property(nameof(Order.OrderDate)).OriginalValue);
            
            var customerEntry = context.Entry(order.Customer);
            Assert.Equal(originalCustomerName, customerEntry.Property(nameof(Customer.CustomerName)).OriginalValue);
        }

        [Fact]
        public void OriginalValues_Should_Work_With_Nullable_Types()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var product = northwind.Products[0];
            product.Discontinued = false;
            
            // Act
            product.SetOriginalValue(nameof(Product.Discontinued), false);
            product.Discontinued = true;
            product.ModifiedProperties = new List<string> { nameof(Product.Discontinued) };
            product.TrackingState = TrackingState.Modified;
            
            var context = _fixture.GetContext();
            context.ApplyChanges(product);
            
            // Assert
            var entry = context.Entry(product);
            Assert.True(entry.Property(nameof(Product.Discontinued)).IsModified);
            Assert.False((bool)entry.Property(nameof(Product.Discontinued)).OriginalValue);
            Assert.True((bool)entry.Property(nameof(Product.Discontinued)).CurrentValue);
        }

        [Fact]
        public void OriginalValues_Should_Work_With_String_Types()
        {
            // Arrange
            var northwind = new MockNorthwind();
            var customer = northwind.Customers[0];
            var originalName = customer.CustomerName;
            
            // Act
            customer.SetOriginalValue(nameof(Customer.CustomerName), originalName);
            customer.CustomerName = "New Customer Name";
            customer.ModifiedProperties = new List<string> { nameof(Customer.CustomerName) };
            customer.TrackingState = TrackingState.Modified;
            
            var context = _fixture.GetContext();
            context.ApplyChanges(customer);
            
            // Assert
            var entry = context.Entry(customer);
            Assert.True(entry.Property(nameof(Customer.CustomerName)).IsModified);
            Assert.Equal(originalName, entry.Property(nameof(Customer.CustomerName)).OriginalValue);
            Assert.Equal("New Customer Name", entry.Property(nameof(Customer.CustomerName)).CurrentValue);
        }

        #endregion
    }
}
