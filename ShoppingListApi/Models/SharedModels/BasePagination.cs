using System;

namespace ShoppingListApi.Models.SharedModels
{
    public class BasePagination
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
    }
}
