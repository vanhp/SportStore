using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;
using SportStore.WebUI.HtmlHelpers;
using SportStore.WebUI.Models;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using SportStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;



namespace SportStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private ProductController SetupRepositoryProduct()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            return controller;
        }
        [TestMethod]
        public void Can_Paginate()
        {
            // Arrange
            //Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            //mock.Setup(m => m.Products).Returns(new Product[] {
            //    new Product {ProductID = 1, Name = "P1"},
            //    new Product {ProductID = 2, Name = "P2"},
            //    new Product {ProductID = 3, Name = "P3"},
            //    new Product {ProductID = 4, Name = "P4"},
            //    new Product {ProductID = 5, Name = "P5"}
            //});
            //ProductController controller = new ProductController(mock.Object);
            //controller.PageSize = 3;

            //Arrange
            ProductController controller = SetupRepositoryProduct();

            // Act
            //  IEnumerable<Product> result =(IEnumerable<Product>)controller.List(2).Model;
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // Arrange - define an HTML helper - we need to do this
            // in order to apply the extension method
            HtmlHelper myHelper = null;
            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };
            // Arrange - set up the delegate using a lambda expression
            //  Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Act
            //     MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, i => "Page" + i);

            // Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
            + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
            result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //// Arrange
            //Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            //mock.Setup(m => m.Products).Returns(new Product[] {
            //        new Product {ProductID = 1, Name = "P1"},
            //        new Product {ProductID = 2, Name = "P2"},
            //        new Product {ProductID = 3, Name = "P3"},
            //        new Product {ProductID = 4, Name = "P4"},
            //        new Product {ProductID = 5, Name = "P5"}
            //        });
            //// Arrange
            //ProductController controller = new ProductController(mock.Object);
            //controller.PageSize = 3;

            //Arrange
            ProductController controller = SetupRepositoryProduct();

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }
        [TestMethod]
        public void Can_Filter_Products()
        {
            ////Arrange
            //ProductController controller = SetupRepositoryProduct();

            //// Action
            //Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();
            //// Assert
            //Assert.AreEqual(result.Length, 2);
            //Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            //Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
            // Arrange
            // -create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                });
            // Arrange - create a controller and make the page size 3 items
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;
            // Action
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();
            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }
        [TestMethod]
        public void Can_Create_Categories()
        {
            // Arrange
            // - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"},
                });
            // Arrange - create the controller
            NavController target = new NavController(mock.Object);
            // Act = get the set of categories
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();
            // Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }
        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Arrange
            // - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1", Category = "Apples"},
            new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
            });
            // Arrange - create the controller
            NavController target = new NavController(mock.Object);
            // Arrange - define the category to selected
            string categoryToSelect = "Apples";
            // Action
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;
            // Assert
            Assert.AreEqual(categoryToSelect, result);
        }
        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange
            // - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                });
            // Arrange - create a controller and make the page size 3 items
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;
            // Action - test the product counts for different categories
            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;
            // Assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
        [TestMethod]
        public void Can_Add_To_Cart()
        {
            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[]
                {
                    new Product { ProductID = 1, Name = "P1", Category = "Apples"}
                }.AsQueryable());
            // Arrange - create a Cart
            Cart cart = new Cart();
            // Arrange - create the controller
            CartController target = new CartController(mock.Object, null);

            // Act - add a product to the cart
            target.AddToCart(cart, 1, null);
            // Assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }
        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                }.AsQueryable());
            // Arrange - create a Cart
            Cart cart = new Cart();
            // Arrange - create the controller
            CartController target = new CartController(mock.Object, null);
            // Act - add a product to the cart
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");
            // Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }
        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            // Arrange - create a Cart
            Cart cart = new Cart();
            // Arrange - create the controller
            CartController target = new CartController(null, null);
            // Act - call the Index action method
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;
            // Assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }
        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            // Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - create an empty cart
            Cart cart = new Cart();
            // Arrange - create shipping details
            ShippingDetails shippingDetails = new ShippingDetails();
            // Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            // Act
            ViewResult result = target.Checkout(cart, shippingDetails);

            // Assert - check that the order hasn't been passed on to the processor 
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);
            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }
        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);
            // Arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");
            // Act - try to checkout
            ViewResult result = target.Checkout(cart, new ShippingDetails());
            // Assert - check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);
            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }
        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            // Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);
            // Act - try to checkout
            ViewResult result = target.Checkout(cart, new ShippingDetails());
            // Assert - check that the order has been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(),
            It.IsAny<ShippingDetails>()), Times.Once());
            // Assert - check that the method is returning the Completed view
            Assert.AreEqual("Completed", result.ViewName);
            // Assert - check that I am passing a valid model to the view
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                });
            // Arrange - create a controller
            AdminController target = new AdminController(mock.Object);
            // Action
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();
            // Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }
        [TestMethod]
        public void Can_Edit_Product()
        {
            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                });
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Act
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;
            // Assert
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }
        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    });
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Act
            Product result = (Product)target.Edit(4).ViewData.Model;
            // Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            // Arrange - create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Arrange - create a product
            Product product = new Product { Name = "Test" };
            // Act - try to save the product
            ActionResult result = target.Edit(product);
            // Assert - check that the repository was called
            mock.Verify(m => m.SaveProduct(product));
            // Assert - check the method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        
        }
        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Arrange - create a product
            Product product = new Product { Name = "Test" };
            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");
            // Act - try to save the product
            ActionResult result = target.Edit(product);
            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            // Assert - check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            // Arrange - create a Product
            Product prod = new Product { ProductID = 2, Name = "Test" };
            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                    new Product {ProductID = 1, Name = "P1"},prod,
                    new Product {ProductID = 3, Name = "P3"},
                });
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Act - delete the product
            target.Delete(prod.ProductID);

            // Assert - ensure that the repository delete method was
            // called with the correct Product
            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }
    }
 }
