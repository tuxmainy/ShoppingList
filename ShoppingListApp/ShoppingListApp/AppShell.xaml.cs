﻿using ShoppingListApp.ViewModels;
using ShoppingListApp.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ShoppingListApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(NewShoppingListPage), typeof(NewShoppingListPage));
            Routing.RegisterRoute(nameof(ShoppingListPage), typeof(ShoppingListPage));
            Routing.RegisterRoute(nameof(NewShoppingItemPage), typeof(NewShoppingItemPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}