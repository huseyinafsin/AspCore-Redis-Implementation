using BasketAPI.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace BasketAPI.Repository
{
    public class RedisBasketRepository : IBasketRepository
    {
        private readonly IDatabase _database ;
        private readonly IConnectionMultiplexer _redis;

        public RedisBasketRepository(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }
        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        public  Task<Basket> GetCustomerBasketAsync(string customerId)
        {
            var basketData = _database.StringGet(customerId);
            if (!basketData.HasValue)
            {
                return Task.FromResult<Basket>(new Basket());
            }
            var basket = JsonSerializer.Deserialize<Basket>(basketData);

            return Task.FromResult(basket);
        }

        public async Task<bool> RemoveItemFromBasket(string customerId, string itemId)
        {
            var basketData = await _database.StringGetAsync(customerId);

            if (!basketData.HasValue)
            {
                return false;
            }

            var basket = JsonSerializer.Deserialize<Basket>(basketData);

            // Find and remove the item from the basket's item list
            var itemToRemove = basket.Items.FirstOrDefault(item => item.Id == itemId);
            if (itemToRemove != null)
            {
                basket.Items.Remove(itemToRemove);
                    RecalculateTotalAmount(basket);
            }

            // Update the basket in Redis
            var serializedBasket = JsonSerializer.Serialize(basket);
            await _database.StringSetAsync(customerId, serializedBasket);

            return true;
        }

        public async Task<Basket> UpdateBasketAsync(Basket basket)
        {
            var serializedBasket = JsonSerializer.Serialize(basket);

            var updated = await _database.StringSetAsync(basket.CustomerId, serializedBasket);

            if (!updated)
            {
                return null;
            }

            return await GetCustomerBasketAsync(basket.CustomerId);
        }

        private void RecalculateTotalAmount(Basket basket)
        {
            basket.TotalAmount = basket.Items.Sum(item => item.Amount);
        }
    }
}
