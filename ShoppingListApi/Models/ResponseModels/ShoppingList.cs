using System.Collections.Generic;

namespace ShoppingListApi.Models.ResponseModels
{
    public class ShoppingList
    {
        public int Count{ get; set; }
        public List<ShoppingListItem> Data { get; set; }
    }
}