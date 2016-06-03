using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace CortanaComponent.Model
{
    public sealed class Account
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public string Details { get; set; }

        public string FlagIcon { get; set; }
    }

    public static class Accounts
    {
        private static ResourceLoader _resourceLoader = new ResourceLoader();
        private static readonly IList<Account> _list = new List<Account>
    {
        new Account { ID = "1", Name = "checking", Details = "Current Balance is 100.56", FlagIcon = "Belgium-flat-icon.png"},
        new Account { ID = "2", Name = "Savings", Details = "Current Balance is 1000.56", FlagIcon = "France-flat-icon.png"},
        new Account { ID = "3", Name = "Vacation", Details = "Current Balance is 3400.56", FlagIcon = "France-flat-icon.png"},
       
    };
        public static IList<Account> List => _list;

        static Accounts()
        {
            _resourceLoader = new ResourceLoader();
        }
    }
}