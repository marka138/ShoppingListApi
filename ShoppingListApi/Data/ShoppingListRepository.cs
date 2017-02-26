using System.Collections.Generic;
using System.Linq;
using ShoppingListApi.Models.RequestModels;
using ShoppingListApi.Models.ResponseModels;

namespace ShoppingListApi.Data
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        List<ShoppingListItem> _dataStore;

        public ShoppingListRepository()
        {
            _dataStore = InMemoryDataStore.GetInstance();
        }

        public IList<ShoppingListItem> GetItems()
        {
            return GetItems(null);
        }

        public IList<ShoppingListItem> GetItems(ShoppingListItemGetList options)
        {
            var items = new List<ShoppingListItem>();

            items = _dataStore.ToList();

            if (options != null)
            {
                if (options.PageNumber.HasValue && options.PageSize.HasValue
                    && options.PageNumber.Value > 0 && options.PageSize > 0)
                {
                    items = items
                        .Skip((options.PageNumber.Value - 1) * options.PageSize.Value)
                        .Take(options.PageSize.Value)
                        .ToList();
                }
            }
            
            return items;
        }

        public ShoppingListItem GetItem(string itemName)
        {
            return _dataStore.Where(x => x.ItemName == itemName).SingleOrDefault();
        }

        public ShoppingListItem Insert(ShoppingListItemCreate request)
        {
            var item = new ShoppingListItem { ItemName = request.ItemName, Quantity = request.Quantity };
            _dataStore.Add(item);
            return item;
        }

        public void Update(ShoppingListItemUpdate request)
        {
            var item = _dataStore.Find(x => x.ItemName == request.ItemName);
            item.Quantity = request.Quantity;
        }

        public void Delete(string itemName)
        {
            var item = _dataStore.Find(x => x.ItemName == itemName);
            _dataStore.Remove(item);
        }
    }
}