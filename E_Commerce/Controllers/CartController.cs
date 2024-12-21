using E_Commerce.Migrations;
using E_Commerce.Models;
using E_Commerce.Repository;
using E_Commerce.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace E_Commerce.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;

        public CartController(UserManager<ApplicationUser> userManager , 
            IShoppingCartRepository shoppingCartRepository ,
            IProductRepository productRepository, IOrderRepository orderRepository )
        {
            this.userManager = userManager;
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
        }
        public IActionResult Index(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
               
                return RedirectToAction("Login", "Account");
            }
            var userId = userManager.GetUserId(User);
            if (id!= 0)
            {
                ShoppingCart cart = new()
                {
                    ProductId = id,
                    count = 1,
                    ApplicationUserId = userId
                };
                shoppingCartRepository.Add(cart);
            }
           var result= shoppingCartRepository.Get(e => e.ApplicationUserId == userId ,e=>e.product);
            TempData["shoppingCart"] = JsonConvert.SerializeObject(result);
            ViewBag.Total = result.Sum(e => e.count * e.product.Price);
            return View(result);
        }

        public IActionResult Increament(int cartid)
        {
            var result = shoppingCartRepository.GetOne(e => e.Id == cartid);
            result.count += 1;
            shoppingCartRepository.commit();
            return RedirectToAction("Index");
        }

        //decrement
        public IActionResult Decreament(int cartid)
        {
            var result = shoppingCartRepository.GetOne(e => e.Id == cartid);
            if (result.count == 1)
            {
                shoppingCartRepository.Delete(result);
            }
            else
            {
                result.count -= 1;
                shoppingCartRepository.commit();
            }

            return RedirectToAction("Index");
        }

        //delete
    
        public IActionResult Remove(int cartid)
        {
            var result = shoppingCartRepository.GetOne(e => e.Id == cartid);
            if (result != null)
            {
                shoppingCartRepository.Delete(result);
                return RedirectToAction("Index");
            }
            else
                return NotFound();
        }

        public IActionResult Pay()
        {
            var items = JsonConvert.DeserializeObject<IEnumerable<ShoppingCart>>((string)TempData["shoppingCart"]);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };
            foreach (var model in items)
            {
                var result = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = model.product.Name,
                            Description=model.product.Description
                            
                        },
                        UnitAmount = (long)model.product.Price * 100,
                    },
                    Quantity = model.count,
                };
                options.LineItems.Add(result);
            }

            var service = new SessionService();
            var session = service.Create(options);


            if (session != null)
            {
                var userId = userManager.GetUserId(User);

                // Retrieve items from the shopping cart stored in the database
                var cartItemsDb = shoppingCartRepository.Get(e => e.ApplicationUserId == userId, e => e.product);

                // Create a new order for the user
                var order = new Order
                {

                    ApplicationUserId = userId,
                    orderDate = DateTime.Now,
                    OrderStatusID = 1,
                    //totalprice = (decimal)TempData["Total"],
                    OrderItems = new List<OrderItem>() // Initialize list for order items
                };

                // Add items from the shopping cart to the order
                foreach (var cartItem in cartItemsDb)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        count = cartItem.count,
                       cost = cartItem.product.Price,
                        OrderId = order.OrderId // Link OrderItem to the newly created order
                    };

                    var existingProduct = productRepository.GetOne(e => e.ProductId == cartItem.ProductId);
                    if (existingProduct != null)
                    {
                        existingProduct.Qty -= cartItem.count;
                        // Update other properties if necessary...
                        productRepository.Update(existingProduct);
                    }
                    //var Product = new Product
                    //{

                    //    Qty = cartItem.Product.Qty- cartItem.Count,
                    //    ProductName = cartItem.Product.ProductName,
                    //    Description = cartItem.Product.Description,
                    //    Price = cartItem.Product.Price, 
                    //    imgurl= cartItem.Product.imgurl,
                    //    CategoryID = cartItem.Product.CategoryID,
                    //};
                    //productRepository.Update(Product);
                    order.OrderItems.Add(orderItem); // Add to in-memory list
                }

                // Add the new order to the database and save changes
                orderRepository.Add(order);
                orderRepository.commit();
                // Save to generate OrderId

                productRepository.commit();// Save to generate OrderId

                // Clear the shopping cart after the order is created
                shoppingCartRepository.DeleteRange(cartItemsDb);
                shoppingCartRepository.commit();
            }
            return Redirect(session.Url);
        }
    }

}
