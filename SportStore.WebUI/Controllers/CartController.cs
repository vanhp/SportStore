using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using SportStore.WebUI.Models;

namespace SportStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductsRepository repository;
        private IOrderProcessor orderprocessor;

        public CartController(IProductsRepository repo,IOrderProcessor order)
        {
            repository = repo;
            orderprocessor = order;
        }

        //private Cart GetCart()
        //{
        //    //use the ASP.NET session state feature to store and
        //    //retrieve Cart objects per user session and persist
        //    //across request and expired after a period
        //    // it's hard to test!
        //    Cart cart = (Cart)Session["Cart"];
        //    if (cart == null)
        //    {
        //        cart = new Cart();
        //        Session["Cart"] = cart; //add to the session
        //    }
        //    return cart;
        //}

        public RedirectToRouteResult AddToCart(Cart cart,int productId, string returnUrl)
        {
            Product product = repository.Products
            .FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                //GetCart().AddItem(product, 1);
                cart.AddItem(product, 1);
            }
            // back to browser
            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart,int productId, string returnUrl)
        {
            Product product = repository.Products
            .FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                //GetCart().RemoveLine(product);
                cart.RemoveLine(product);
            }
            // back to browser
            return RedirectToAction("Index", new { returnUrl });
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }
        [HttpPost]
        public ViewResult Checkout(Cart cart,ShippingDetails shippingDetails)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid)
            {
                orderprocessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(shippingDetails);
            }
        }
    
        // GET: Cart
        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel { ReturnUrl = returnUrl, Cart = cart });
        }
    }
}