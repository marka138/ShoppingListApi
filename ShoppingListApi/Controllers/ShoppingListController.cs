using ShoppingListApi.Models.RequestModels;
using ShoppingListApi.Models.ResponseModels;
using ShoppingListApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ShoppingListApi.Controllers
{
    [Route("api/shoppinglist")]
    public class ShoppingListController : ApiController
    {
        IShoppingListRepository _repository;

        public ShoppingListController(IShoppingListRepository respository)
        {
            _repository = respository;
        }

        [HttpGet]
        public IHttpActionResult GetItems([FromUri]ShoppingListItemGetList request = null)
        {
            List<ShoppingListItem> items;

            if (request != null)
            {
                items = _repository.GetItems(request).ToList();
            }
            else
            {
                items = _repository.GetItems().ToList();
            }

            var content = ConvertToShoppingListModel(items);

            return Ok<ShoppingList>(content);
        }

        [HttpGet]
        public IHttpActionResult GetItem(string itemName)
        {
            if (String.IsNullOrWhiteSpace(itemName))
            {
                return BadRequest();
            }

            var item = _repository.GetItem(itemName);

            if (item == null)
            {
                return NotFound();
            }

            return Ok<ShoppingListItem>(item);
        }

        [HttpPost]
        public IHttpActionResult Post(ShoppingListItemCreate request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ItemName))
            {
                return BadRequest();
            }

            var doesAlreadyExist = (_repository.GetItem(request.ItemName) != null);
            if (doesAlreadyExist)
            {
                return BadRequest();
            }

            var createdItem = _repository.Insert(request);

            return Created($"api/shoppinglist/?itemname={createdItem.ItemName}", createdItem);
        }

        [HttpPut]
        public IHttpActionResult Update(ShoppingListItemUpdate request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ItemName))
            {
                return BadRequest();
            }

            var doesAlreadyExist = (_repository.GetItem(request.ItemName) != null);
            if (!doesAlreadyExist)
            {
                return BadRequest();
            }

            _repository.Update(request);

            return Ok();
        }

        [HttpDelete]
        public IHttpActionResult Delete(string itemName)
        {
            if (String.IsNullOrWhiteSpace(itemName))
            {
                return BadRequest();
            }

            _repository.Delete(itemName);

            return Ok();
        }

        private ShoppingList ConvertToShoppingListModel(List<ShoppingListItem> items)
        {
            var model = new ShoppingList
            {
                Data = items,
                Count = items.Count
            };
            return model;
        }
    }
}
