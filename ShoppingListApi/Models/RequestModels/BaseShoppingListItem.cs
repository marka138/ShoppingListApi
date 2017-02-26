namespace ShoppingListApi.Models.RequestModels
{
    public abstract class BaseShoppingListItem
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}
