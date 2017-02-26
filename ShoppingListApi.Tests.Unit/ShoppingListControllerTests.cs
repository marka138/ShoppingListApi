using ShoppingListApi.Controllers;
using ShoppingListApi.Models.RequestModels;
using ShoppingListApi.Models.ResponseModels;
using ShoppingListApi.Data;
using System.Web.Http.Results;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;

namespace ShoppingListApi.Tests
{
    [TestFixture]
    public class ShoppingListControllerTests
    {
        private ShoppingListController _controller;
        private Mock<IShoppingListRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IShoppingListRepository>();
            _controller = new ShoppingListController(_repositoryMock.Object);
        }

        [Test]
        public void post_should_not_save_to_repository_if_item_null()
        {
            ShoppingListItemCreate request = null;

            var result = _controller.Post(request);

            _repositoryMock.Verify(x => x.Insert(request), Times.Never);
        }

        [Test]
        public void post_should_save_to_repository_if_valid_item()
        {
            var request = new ShoppingListItemCreate { ItemName = "Item1", Quantity = 1 };

            var repositoryItem = new ShoppingListItem { ItemName = "Item1", Quantity = 1 };
            _repositoryMock.Setup(x => x.Insert(request)).Returns(repositoryItem);

            var result = _controller.Post(request);

            _repositoryMock.Verify(x => x.Insert(request));
        }

        [Test]
        public void post_should_return_bad_request_if_item_null()
        {
            ShoppingListItemCreate request = null;

            var result = _controller.Post(request);

            Assert.AreEqual(result.GetType(), typeof(BadRequestResult));
        }

        [Test]
        public void post_should_return_bad_request_if_item_name_is_empty()
        {
            var request = new ShoppingListItemCreate { ItemName = "" };

            var result = _controller.Post(request);

            Assert.AreEqual(result.GetType(), typeof(BadRequestResult));
        }

        [Test]
        public void should_post_shopping_list_item_and_return_created_with_correct_content()
        {
            var request = new ShoppingListItemCreate { ItemName = "Item1", Quantity = 1 };

            var repositoryItem = new ShoppingListItem { ItemName = "Item1", Quantity = 1 };
            _repositoryMock.Setup(x => x.Insert(request)).Returns(repositoryItem);

            var result = _controller.Post(request);

            Assert.AreEqual(result.GetType(), typeof(CreatedNegotiatedContentResult<ShoppingListItem>));
            var createdResult = (CreatedNegotiatedContentResult<ShoppingListItem>)result;
            Assert.AreEqual(createdResult.Location, $"api/shoppinglist/?itemname={request.ItemName}");
            Assert.AreEqual(createdResult.Content.ItemName, request.ItemName);
            Assert.AreEqual(createdResult.Content.Quantity, request.Quantity);
        }

        [Test]
        public void post_should_return_bad_request_if_item_already_exists()
        {
            var request = new ShoppingListItemCreate { ItemName = "Item1", Quantity = 1 };

            var repositoryItem = new ShoppingListItem { ItemName = "Item1", Quantity = 1 };
            _repositoryMock.Setup(x => x.GetItem(request.ItemName)).Returns(repositoryItem);

            var result = _controller.Post(request);

            Assert.AreEqual(result.GetType(), typeof(BadRequestResult));
        }

        [Test]
        public void get_shopping_list_should_call_repository()
        {
            _repositoryMock.Setup(x => x.GetItems()).Returns(new List<ShoppingListItem>());

            var result = _controller.GetItems(); 
            _repositoryMock.Verify(x => x.GetItems());
        }

        [Test]
        public void get_shopping_list_should_call_repository_with_paging()
        {
            var options = new ShoppingListItemGetList { PageNumber = 2, PageSize = 10 };

            _repositoryMock.Setup(x => x.GetItems(options)).Returns(new List<ShoppingListItem>());

            var result = _controller.GetItems(options);
            _repositoryMock.Verify(x => x.GetItems(options));
        }

        [Test]
        public void should_get_shopping_list_and_return_ok_with_empty_list_when_repository_has_no_items()
        {
            var shoppingListItems = new List<ShoppingListItem>();
            _repositoryMock.Setup(x => x.GetItems()).Returns(shoppingListItems);

            var result = _controller.GetItems();

            Assert.AreEqual(result.GetType(), typeof(OkNegotiatedContentResult<ShoppingList>));
            var actualShoppingListItems = ((OkNegotiatedContentResult<ShoppingList>)result).Content;
            Assert.AreEqual(actualShoppingListItems.Count, 0);
        }

        [Test]
        public void should_get_shopping_list_and_return_ok_with_shopping_list_data()
        {
            var shoppingListItems = new List<ShoppingListItem> {
                new ShoppingListItem { ItemName="Item1", Quantity = 1 },
                new ShoppingListItem { ItemName="Item2", Quantity = 2 }
            };
            _repositoryMock.Setup(x => x.GetItems()).Returns(shoppingListItems);

            var result = _controller.GetItems();

            Assert.AreEqual(result.GetType(), typeof(OkNegotiatedContentResult<ShoppingList>));
            var actualShoppingListItems = ((OkNegotiatedContentResult<ShoppingList>)result).Content;
            Assert.AreEqual(actualShoppingListItems.Count, 2);
            Assert.AreEqual(actualShoppingListItems.Data.Count, 2);
            Assert.AreEqual(actualShoppingListItems.Data[0].ItemName, "Item1");
            Assert.AreEqual(actualShoppingListItems.Data[0].Quantity, 1);
            Assert.AreEqual(actualShoppingListItems.Data[1].ItemName, "Item2");
            Assert.AreEqual(actualShoppingListItems.Data[1].Quantity, 2);
        }

        [Test]
        public void get_shopping_list_item_should_return_bad_request_when_request_is_null()
        {
            string itemName = null;

            var result = _controller.GetItem(itemName);

            Assert.AreEqual(result.GetType(), typeof(BadRequestResult));
        }

        [Test]
        public void get_shopping_list_item_should_return_bad_request_when_item_name_is_empty()
        {
            string itemName = "";

            var result = _controller.GetItem(itemName);

            Assert.AreEqual(result.GetType(), typeof(BadRequestResult));
        }

        [Test]
        public void get_shopping_list_item_should_call_repository_with_correct_data()
        {
            var itemName = "Item1";

            var result = _controller.GetItem(itemName);

            _repositoryMock.Verify(x => x.GetItem(itemName));
        }

        [Test]
        public void get_shopping_list_item_should_return_not_found_when_item_is_not_in_shopping_list()
        {
            var itemName = "Item1";

            _repositoryMock.Setup(x => x.GetItem(itemName)).Returns((ShoppingListItem)null);

            var result = _controller.GetItem(itemName);

            Assert.AreEqual(result.GetType(), typeof(NotFoundResult));
        }

        [Test]
        public void get_shopping_list_item_should_return_ok_with_shopping_item_data_when_valid()
        {
            var itemName = "Item1";

            var shoppingListItem = new ShoppingListItem { ItemName = "Item1", Quantity = 1 };

            _repositoryMock.Setup(x => x.GetItem(itemName)).Returns(shoppingListItem);

            var result = _controller.GetItem(itemName);

            Assert.AreEqual(result.GetType(), typeof(OkNegotiatedContentResult<ShoppingListItem>));
            var actualShoppingListItem = ((OkNegotiatedContentResult<ShoppingListItem>)result).Content;
            Assert.AreEqual(actualShoppingListItem.ItemName, "Item1");
            Assert.AreEqual(actualShoppingListItem.Quantity, 1);
        }

        [Test]
        public void update_should_return_bad_request_if_item_null()
        {
            ShoppingListItemUpdate request = null;

            var result = _controller.Update(request);

            Assert.AreEqual(result.GetType(), typeof(BadRequestResult));
        }

        [Test]
        public void update_should_return_bad_request_if_item_name_is_empty()
        {
            var request = new ShoppingListItemUpdate {  ItemName = "" };

            var result = _controller.Update(request);

            Assert.AreEqual(result.GetType(), typeof(BadRequestResult));
        }

        [Test]
        public void update_should_return_ok_if_item_valid()
        {
            var request = new ShoppingListItemUpdate { ItemName = "Item1", Quantity = 1 };

            _repositoryMock.Setup(x => x.GetItem(request.ItemName)).Returns(new ShoppingListItem { ItemName = "Item1", Quantity = 5 });

            var result = _controller.Update(request);

            Assert.AreEqual(result.GetType(), typeof(OkResult));
        }

        [Test]
        public void update_should_call_update_repository_with_correct_data_if_item_valid()
        {
            var request = new ShoppingListItemUpdate { ItemName = "Item1", Quantity = 1 };

            _repositoryMock.Setup(x => x.GetItem(request.ItemName)).Returns(new ShoppingListItem { ItemName = "Item1", Quantity = 5 });

            var result = _controller.Update(request);

            _repositoryMock.Verify(x => x.Update(request));
        }

        [Test]
        public void update_should_return_bad_request_if_item_name_does_not_exist()
        {
            ShoppingListItemUpdate request = new ShoppingListItemUpdate { ItemName = "Item1", Quantity = 1 };

            _repositoryMock.Setup(x => x.GetItem(request.ItemName)).Returns((ShoppingListItem)null);

            var result = _controller.Update(request);

            Assert.AreEqual(result.GetType(), typeof(BadRequestResult));
        }

        [Test]
        public void delete_should_return_bad_request_if_item_name_is_empty()
        {
            string itemName = "";

            var result = _controller.Delete(itemName);

            Assert.AreEqual(result.GetType(), typeof(BadRequestResult));
        }

        [Test]
        public void delete_should_return_ok_if_item_valid()
        {
            string itemName = "Item1";

            var result = _controller.Delete(itemName);

            Assert.AreEqual(result.GetType(), typeof(OkResult));
        }

        [Test]
        public void delete_should_call_delete_repository_with_correct_data_if_item_valid()
        {
            string itemName = "Item1";

            var result = _controller.Delete(itemName);

            _repositoryMock.Verify(x => x.Delete(itemName));
        }
    }
}
