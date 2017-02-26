using System.Collections.Generic;
using ShoppingListApi.Models.ResponseModels;

namespace ShoppingListApi.Data
{
    public class InMemoryDataStore
    {
        private static List<ShoppingListItem> _instance;

        private InMemoryDataStore()
        {
        }

        public static List<ShoppingListItem> GetInstance()
        {
            if (_instance == null)
            {
                _instance = new List<ShoppingListItem>();
            }           
            return _instance;
        }
    }
}