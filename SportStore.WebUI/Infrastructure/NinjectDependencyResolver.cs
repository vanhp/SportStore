using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Moq;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using SportStore.Domain.Concrete;
using System.Configuration;
using SportStore.WebUI.Infrastructure.Abstract;
using SportStore.WebUI.Infrastructure.Concrete;

namespace SportStore.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            //to register all objects to be resolved
            // //mock out the stand in repository 
            // Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            // mock.Setup(m => m.Products).Returns(new List<Product>
            //     {
            //         new Product { Name = "Kayak", Description = "A boat for one person", Category = "Watersport",Price = 275.00},
            //         new Product { Name = "Lifejacket", Description = "Protective and fashionable", Category = "Watersport",Price = 48.95},
            //         new Product { Name = "Soccer Ball", Description = "FIFA - approved size and weight", Category = "Soccer",Price = 19.50},
            //         new Product { Name = "Corner Flags", Description = " Give your playing field a professional touch", Category = "Soccer",Price = 34.95},
            //         new Product { Name = "Stadium", Description = " Flat - packed 35, 000 - seat stadium", Category = "Soccer",Price = 79500.00},
            //         new Product { Name = "Thinking Cap", Description = "Improve your brain efficiency by 75 %", Category = "Chess",Price = 16.00},
            //         new Product { Name = "Unsteady Chair", Description = " Secretely give your opponent a disadvantage", Category = "Chess",Price = 29.95},
            //         new Product { Name = "Human Chess Board", Description = "A fun game for the family", Category = "Chess",Price = 75.00},
            //         new Product { Name = "Bling - Bling King", Description = "Gold - plated, diamond - studded King", Category = "Chess",Price = 1200.00}
            //      });

            //// Ninject to use the same instance for every request
            //  kernel.Bind<IProductsRepository>().ToConstant(mock.Object);

            //using the real DB now
            kernel.Bind<IProductsRepository>().To<EFProductRepository>();
            EmailSettings emailSettings = new EmailSettings
            {
                WriteAsFile = bool.Parse(ConfigurationManager
                                .AppSettings["Email.WriteAsFile"] ?? "false")
            };
            kernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>()
                    .WithConstructorArgument("settings", emailSettings);
            kernel.Bind<IAuthProvider>().To<FormsAuthProvider>();
        }
     
    }
}