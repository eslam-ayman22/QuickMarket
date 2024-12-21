using E_Commerce.Models;
using E_Commerce.Repository;
using E_Commerce.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageOrderController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOrderRepository orderRepository;
        private readonly EmailService emailService;

        public ManageOrderController(UserManager<ApplicationUser> userManager ,
            IOrderRepository orderRepository , EmailService emailService)
        {
            this.userManager = userManager;
            this.orderRepository = orderRepository;
            this.emailService = emailService;
        }

        //Get All Order
        public async Task<IActionResult> GetAll()
        {
            var orders = orderRepository.Getorder(null,
            query => query.Include(o => o.OrderItems).ThenInclude(oi => oi.Product),
           query => query.Include(o => o.ApplicationUser)
          ).OrderByDescending(o => o.orderDate);

            // Assign the UserName to review.ApplicationUser.UserName
            foreach (var orderItem in orders)
            {
                if (!string.IsNullOrEmpty(orderItem.ApplicationUserId))
                {
                    // Fetch the ApplicationUser using UserManager
                    var user = await userManager.FindByIdAsync(orderItem.ApplicationUserId);
                    if (user != null)
                    {
                        // Assign the UserName to review.ApplicationUser.UserName
                        if (orderItem.ApplicationUser == null)
                        {
                            orderItem.ApplicationUser = new ApplicationUser(); // Initialize if null
                        }
                        orderItem.ApplicationUser.Email = user.Email; // Assign the username
                    }
                    else
                    {
                        // Handle case where user is not found (optional)
                        if (orderItem.ApplicationUser == null)
                        {
                            orderItem.ApplicationUser = new ApplicationUser(); // Initialize if null
                        }
                        orderItem.ApplicationUser.Email = "Unknown User"; // Default value
                    }
                }
            }
            return View(orders);
        }


        //Confirm Order

        //Confirm Order
        public async Task<IActionResult> Confirm(int id)
        {

            var order = orderRepository.Getorder(
            e => e.OrderId == id,
            query => query.Include(o => o.OrderItems).ThenInclude(oi => oi.Product),
          
            query => query.Include(o => o.ApplicationUser)
           ).FirstOrDefault();

            if (order != null)
            {
                foreach (var OrderItem in order.OrderItems)
                {

                    if (!string.IsNullOrEmpty(OrderItem.Order.ApplicationUserId))
                    {
                        // Fetch the ApplicationUser using UserManager
                        var user = await userManager.FindByIdAsync(OrderItem.Order.ApplicationUserId);
                        if (user != null)
                        {
                            // Assign the UserName to review.ApplicationUser.UserName
                            if (OrderItem.Order.ApplicationUser == null)
                            {
                                OrderItem.Order.ApplicationUser = new ApplicationUser(); // Initialize if null
                            }
                            OrderItem.Order.ApplicationUser.Email = user.Email; // Assign the username
                            OrderItem.Order.ApplicationUser.UserName = user.UserName;

                        }
                    }
                }

                string toEmail = order.ApplicationUser.Email;
                string subject = $"Order Confirmation - Order #{order.OrderId}";


                var totalCost = order.OrderItems.Sum(item => item.cost * item.count);
                var itemList = string.Join("<br/>", order.OrderItems.Select(item =>
                    $"{item.Product?.Name} - Quantity: {item.count} - Price: {item.cost * item.count} EGP"));

                string body = $"Dear {order.ApplicationUser?.UserName},<br/><br/>" +
                              $"Your order has been confirmed.<br/>" +
                              $"Order ID: {order.OrderId}<br/>" +
                              $"Total Price: {totalCost} EGP<br/>" +
                              $"Items:<br/>{itemList}<br/>" +
                              $"Thank you for shopping with us!";

                emailService.SendOrderConfirmationEmail(toEmail, subject, body);
                TempData["confirm"] = "Order confirmed and email sent successfully.";
                order.IsConfirm = true;
                orderRepository.commit();
                TempData["orderId"] = order.OrderId;
                return RedirectToAction("GetAll");
            }
            return NotFound();

        }


        //Cancel Order
        public IActionResult CancelOrder(int id)
        {
            var order = orderRepository.GetOne(e => e.OrderId == id, e => e.ApplicationUser, e => e.OrderItems);

            if (order != null)
            {
                orderRepository.Delete(order);
                orderRepository.commit();
                var customerEmail = order.ApplicationUser?.Email; // جلب بريد العميل
                string subject = $"Order #{order.OrderId} Cancellation";
                string body = $"Dear {order.ApplicationUser?.UserName},<br/><br/>" +
                                  $"Your order has been canceled<br/>" +
                                  $"Order ID: {order.OrderId}<br/><br/>" +
                                  "Thank you for using our services.";

                emailService.SendOrderConfirmationEmail(customerEmail, subject, body);

                
                orderRepository.commit();

                TempData["canceled"] = "Order Canceled and email sent successfully.";

                return RedirectToAction("GetAll");
            }

            return NotFound();
        }


    }
}
