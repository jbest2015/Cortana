using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace Model
{
    public sealed class Account
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public string Details { get; set; }

        public string AcctIcon { get; set; }

        public bool sourceonly { get; set; }
    }

    public static class Accounts
    {
       // private static ResourceLoader _resourceLoader = new ResourceLoader();
        private static readonly IList<Account> _list = new List<Account>
    {
        new Account { ID = "1", Name = "checking", Details = "Current Balance is 100.56", AcctIcon = "Belgium-flat-icon.png",sourceonly = true},
        new Account { ID = "2", Name = "Savings", Details = "Current Balance is 1000.56", AcctIcon = "France-flat-icon.png",sourceonly = true},
        new Account { ID = "3", Name = "Vacation", Details = "Current Balance is 3400.56", AcctIcon = "UK-flat-icon.png",sourceonly = true},
        new Account { ID = "4", Name = "Daughter", Details = "Abigail Best", AcctIcon = "UK-flat-icon.png",sourceonly = false},
        new Account { ID = "5", Name = "Mother", Details = "Patricia Best", AcctIcon = "UK-flat-icon.png",sourceonly = false}

    };
        public static IList<Account> List => _list;

        static Accounts()
        {
           // _resourceLoader = new ResourceLoader();
        }
    }
}
