﻿using ShoppingListApp.Views;
using System;
using Xamarin.Forms;

namespace ShoppingListApp.ViewModels
{
    public class LoginViewModel : BaseShoppingListViewModel
    {
        public Command LoginCommand { get; }
        public Command ScanCommand { get; }


        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            ScanCommand = new Command(OnScan);

            if (CheckConfiguration())
            {
                try
                {
                    SetConfiguration();
                    _ = Shell.Current.GoToAsync($"//{nameof(ShoppingListsPage)}");
                }
                catch { }
            }
        }

        public string Host
        {
            set => Application.Current.Properties["host"] = Uri.TryCreate(value, UriKind.Absolute, out _) ? value : "";
        }

        public string Username
        {
            set => Application.Current.Properties["username"] = value;
        }

        public string Password
        {
            set => Application.Current.Properties["password"] = value;
        }

        public string Barcode { get; set; }

        private void SetConfiguration()
        {
            IO.Swagger.Client.Configuration.DefaultApiClient = new IO.Swagger.Client.ApiClient(Application.Current.Properties["host"].ToString());
            IO.Swagger.Client.Configuration.Username = Application.Current.Properties["username"].ToString();
            IO.Swagger.Client.Configuration.Password = Application.Current.Properties["password"].ToString();

            ShoppingListDataStore.LoginUpdate();
        }

        private bool CheckConfiguration()
        {
            return Application.Current.Properties.ContainsKey("host")
                && Application.Current.Properties.ContainsKey("username")
                && Application.Current.Properties.ContainsKey("password");
        }

        private async void OnLoginClicked(object obj)
        {
            try
            {
                if (CheckConfiguration())
                {
                    await Application.Current.SavePropertiesAsync();
                    SetConfiguration();
                    await Shell.Current.GoToAsync($"//{nameof(ShoppingListsPage)}");
                }
            }
            catch { }
        }

        private async void OnScan(object obj)
        {
            Barcode = "";
            ScanPage sp = new ScanPage();
            sp.OnScan += Sp_OnScan;
            await Application.Current.MainPage.Navigation.PushAsync(sp);
        }

        private void Sp_OnScan(string obj)
        {
            Barcode = obj;
        }
    }
}
