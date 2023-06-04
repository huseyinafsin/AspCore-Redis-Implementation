using BasketAPI.Models;

namespace BasketAPI.Repository
{
    public interface IBasketRepository
    {
        Task<Basket> GetCustomerBasketAsync(string customerId);
        Task<Basket> UpdateBasketAsync(Basket basket);
        Task<bool> DeleteBasketAsync(string id);
        Task<bool> RemoveItemFromBasket(string customerId, string itemId);
    }
}
