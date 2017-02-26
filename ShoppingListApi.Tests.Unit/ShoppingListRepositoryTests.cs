using ShoppingListApi.Models.RequestModels;
using ShoppingListApi.Models.ResponseModels;
using ShoppingListApi.Data;
using NUnit.Framework;
using System.Collections.Generic;

namespace ShoppingListApi.Tests
{
    [TestFixture]
    public class ShoppingListRepositoryTests
    {
        List<ShoppingListItem> _dataStore;

        [SetUp]
        public void SetUp()
        {
            _dataStore = InMemoryDataStore.GetInstance();
            _dataStore.Clear();
        }

        [Test]
        public void should_get_shopping_list_from_data_store()
        {
            _dataStore.Add(new ShoppingListItem { ItemName = "Item1", Quantity = 1 });
            _dataStore.Add(new ShoppingListItem { ItemName = "Item2", Quantity = 2 });

            var repository = new ShoppingListRepository();
            var items = repository.GetItems();

            Assert.AreEqual(items.Count, 2);
            Assert.AreEqual(items[0].ItemName, "Item1");
            Assert.AreEqual(items[0].Quantity, 1);
            Assert.AreEqual(items[1].ItemName, "Item2");
            Assert.AreEqual(items[1].Quantity, 2);
        }

        [Test]
        public void should_get_shopping_list_from_data_store_with_paging()
        {
            var pageSize = 10;

            for (int i = 1; i <= 50; i++)
            {
                _dataStore.Add(new ShoppingListItem { ItemName = $"Item{i}", Quantity = 1 });
            }

            var getListOptions = new ShoppingListItemGetList { PageNumber = 1, PageSize = pageSize };

            var repository = new ShoppingListRepository();
            var items = repository.GetItems(getListOptions);

            Assert.AreEqual(items.Count, 10);
            Assert.AreEqual(items[0].ItemName, "Item1");
            Assert.AreEqual(items[0].Quantity, 1);
            Assert.AreEqual(items[pageSize - 1].ItemName, "Item10");
            Assert.AreEqual(items[pageSize - 1].Quantity, 1);
        }

        [Test]
        public void should_get_shopping_list_from_data_store_with_paging_when_not_first_page()
        {
            var pageSize = 10;

            for (int i = 1; i <= 50; i++)
            {
                _dataStore.Add(new ShoppingListItem { ItemName = $"Item{i}", Quantity = 1 });
            }

            var getListOptions = new ShoppingListItemGetList { PageNumber = 3, PageSize = pageSize };

            var repository = new ShoppingListRepository();
            var items = repository.GetItems(getListOptions);

            Assert.AreEqual(items.Count, 10);
            Assert.AreEqual(items[0].ItemName, "Item21");
            Assert.AreEqual(items[0].Quantity, 1);
            Assert.AreEqual(items[pageSize - 1].ItemName, "Item30");
            Assert.AreEqual(items[pageSize - 1].Quantity, 1);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void should_not_use_paging_if_page_number_is_invalid(int pageNumber)
        {
            var pageSize = 10;

            for (int i = 1; i <= 20; i++)
            {
                _dataStore.Add(new ShoppingListItem { ItemName = $"Item{i}", Quantity = 1 });
            }

            var getListOptions = new ShoppingListItemGetList { PageNumber = pageNumber, PageSize = pageSize };

            var repository = new ShoppingListRepository();
            var items = repository.GetItems(getListOptions);

            Assert.AreEqual(items.Count, 20);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void should_not_use_paging_if_page_size_is_invalid(int pageSize)
        {
            for (int i = 1; i <= 20; i++)
            {
                _dataStore.Add(new ShoppingListItem { ItemName = $"Item{i}", Quantity = 1 });
            }

            var getListOptions = new ShoppingListItemGetList { PageNumber = 10, PageSize = pageSize };

            var repository = new ShoppingListRepository();
            var items = repository.GetItems(getListOptions);

            Assert.AreEqual(items.Count, 20);
        }

        [Test]
        public void should_insert_shopping_list_item_to_data_store()
        {
            var repository = new ShoppingListRepository();
            var item = new ShoppingListItemCreate { ItemName = "Item1", Quantity = 1 };
            repository.Insert(item);

            Assert.AreEqual(_dataStore.Count, 1);
            Assert.AreEqual(_dataStore[0].ItemName, "Item1");
            Assert.AreEqual(_dataStore[0].Quantity, 1);
        }

        [Test]
        public void should_insert_and_return_shopping_list_item()
        {
            var repository = new ShoppingListRepository();
            var item = new ShoppingListItemCreate { ItemName = "Item1", Quantity = 1 };
            ShoppingListItem createdItem = repository.Insert(item);

            Assert.AreEqual(createdItem.ItemName, "Item1");
            Assert.AreEqual(createdItem.Quantity, 1);
        }

        [Test]
        public void should_update_shopping_list_item_in_data_store()
        {
            _dataStore.Add(new ShoppingListItem { ItemName = "Item1", Quantity = 1 });

            var repository = new ShoppingListRepository();
            var item = new ShoppingListItemUpdate { ItemName = "Item1", Quantity = 5 };
            repository.Update(item);

            Assert.AreEqual(_dataStore[0].ItemName, "Item1");
            Assert.AreEqual(_dataStore[0].Quantity, 5);
        }

        [Test]
        public void should_delete_shopping_list_item_in_data_store()
        {
            _dataStore.Add(new ShoppingListItem { ItemName = "Item1", Quantity = 1 });

            Assert.AreEqual(_dataStore.Count, 1);

            var repository = new ShoppingListRepository();
            repository.Delete("Item1");

            Assert.AreEqual(_dataStore.Count, 0);
        }
    }
}
