using ShoppingListApi.Models.RequestModels;
using ShoppingListApi.Models.ResponseModels;
using System.Collections.Generic;

namespace ShoppingListApi.Data
{
    public interface IShoppingListRepository
    {
        IList<ShoppingListItem> GetItems();
        IList<ShoppingListItem> GetItems(ShoppingListItemGetList request);
        ShoppingListItem GetItem(string itemName);
        ShoppingListItem Insert(ShoppingListItemCreate request);
        void Update(ShoppingListItemUpdate request);
        void Delete(string itemName);
    }
}