﻿using ShoppingListApp.Models;
using ShoppingListApp.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShoppingListApp.ViewModels
{
    [QueryProperty("ShoppingListId", "ShoppingListId")]
    class NewShoppingItemViewModel : BaseShoppingListViewModel
    {
        private const int SEARCH_ITEM_LIMIT = 5;
        private StoreItem selectedStoreItem;
        private string text;
        private uint amount = 1;
        private string unit;
        private IEnumerable<StoreItem> searchResult;

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command ScanCommand { get; }


        public NewShoppingItemViewModel()
        {
            Title = "New Shopping Item";
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            ScanCommand = new Command(OnScan);

            // erzeugt eine neue Funktion die zu PropertyChangedEventHandler passt und ruft darun SaveCommand.ChangeCanExecute auf => WTF????
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();

            unit = StaticValues.Units.First();
        }

        public StoreItem SelectedStoreItem
        {
            get => selectedStoreItem;
            set
            {
                selectedStoreItem = value;
                Text = value.Text;
                Unit = value.Unit;
            }
        }

        public IEnumerable<StoreItem> SearchResult
        {
            get => searchResult;
            set => SetProperty(ref searchResult, value);
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public uint Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        public string Unit
        {
            get => unit;
            set => SetProperty(ref unit, value);
        }

        public int ShoppingListId { get; set; }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(Text)
                && Amount > 0
                && !string.IsNullOrWhiteSpace(Unit);
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            StoreItem storeItem;
            if (selectedStoreItem == null || selectedStoreItem.Text != Text)
            {
                storeItem = new StoreItem()
                {
                    Text = Text,
                    Unit = Unit
                };
                await ShoppingListDataStore.AddStoreItemAsync(storeItem);
            }
            else
            {
                storeItem = selectedStoreItem;
            }

            ShoppingItem shoppingItem = new ShoppingItem()
            {
                StoreItem = storeItem,
                Amount = Amount,
                Unit = Unit
            };
            await ShoppingListDataStore.AddShoppingItemAsync(ShoppingListId, shoppingItem);

            // This will pop the current page off the navigation stack
            
            await Shell.Current.GoToAsync("..");
        }

        public async Task SearchStoreItemsAsync(string searchText)
        {
            SearchResult = await ShoppingListDataStore.SearchStoreItemsAsync(searchText, null, SEARCH_ITEM_LIMIT);
        }

        private async void OnScan()
        {
            ScanPage sp = new ScanPage();
            sp.OnScan += Sp_OnScan;
            await Application.Current.MainPage.Navigation.PushAsync(sp);
        }

        private void Sp_OnScan(string barcode)
        {
            if (barcode != null)
            {
                IEnumerable<StoreItem> result = ShoppingListDataStore.SearchStoreItemsAsync(null, barcode, null).Result;
                if (result.Count() > 1)
                {
                    SearchResult = result;
                }
                else if (result.Count() < 1)
                {
                    SearchResult = null;
                }
                else
                {
                    // THERE CAN ONLY BE ONE
                    SelectedStoreItem = result.First();
                }
            }
        }
    }
}
