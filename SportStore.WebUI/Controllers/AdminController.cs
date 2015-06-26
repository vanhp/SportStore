using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;

namespace SportStore.WebUI.Controllers
{
    [Authorize] //allow all users access
    public class AdminController : Controller
    {
        private IProductsRepository repository;
        public AdminController(IProductsRepository repo)
        {
            repository = repo;
        }
        
        public ViewResult Index()
        {
            return View(repository.Products);
        }
        
        public ViewResult Edit(int productId)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product,HttpPostedFileBase image = null)
        {
            if(ModelState.IsValid)
            {
                if(image != null)
                {   //set to save image to DB
                  
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }
                if(product.ProductID != 0)
                {
                    repository.SaveProduct(product);
                }
                
                //saved msg 'til session end
                TempData["message"] = string.Format("{0} has been saved", product.Name);
                return RedirectToAction("Index");
            }
            else
            { //some thing wrong with data let user fix it
                return View(product);
            }
        }
        public ViewResult Create()
        {
            return View("Edit", new Product());
        }

        [HttpPost]
        public ActionResult Delete(int productID)
        {
            var deleteProduct = repository.DeleteProduct(productID);
            if(deleteProduct != null)
            {
                TempData["message"] = string.Format("{0} was deleted",deleteProduct.Name);
            }
            return RedirectToAction("Index");
        }

    }
}