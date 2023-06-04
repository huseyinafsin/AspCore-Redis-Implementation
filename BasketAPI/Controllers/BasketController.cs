
using BasketAPI.Models;
using BasketAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;

        public BasketController(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        [HttpGet("GetBasket/{customerId}")]
        public async Task<ActionResult> GetCustomerBasket(string customerId)
        {
            var basket =await _basketRepository.GetCustomerBasketAsync(customerId);
            if (basket == null)
            {
                return BadRequest();
            }
            return  Ok(basket);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCustomerBasket(string customerId, List<BasketItem> basketItems)
        {
            var customeerBasket = await _basketRepository.GetCustomerBasketAsync(customerId);
            if (customeerBasket == null)
            {
                var basket = new Basket
                {
                    CustomerId = customerId,
                    Items = basketItems,
                    TotalAmount = basketItems.Sum(x => (x.Price * x.Quantity))
                };

              customeerBasket = await _basketRepository.UpdateBasketAsync(basket);

            }
            else
            {

                customeerBasket.Items.AddRange(basketItems);
                customeerBasket.TotalAmount += basketItems.Sum(x => (x.Price * x.Quantity));
            }

            customeerBasket = await _basketRepository.UpdateBasketAsync(customeerBasket);
            return Ok(customeerBasket);
        }

        [HttpDelete("deleteBasket/{basketId}")]
        public async Task<ActionResult> DeleteBasket(string basketId)
        {
            await _basketRepository.DeleteBasketAsync(basketId);
            return Ok();
        }

        [HttpDelete("removeItem")]
        public async Task<ActionResult> RemoveItemFromBasket(string customerId, string itemId)
        {
            await _basketRepository.RemoveItemFromBasket(customerId, itemId);
            return Ok();
        }
    }
}
