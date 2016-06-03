using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeControl
{
   public class History
    {
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime TransDate { get; set; }
        public double Balance { get; set; }

        public History()
        {
            this.Description = "Starbucks ";
            this.Amount = 32.63;
            this.TransDate = new DateTime(2016, 09, 28);
            this.Balance = 5000;
        }

        public string OneLineSummary
        {
            get
            {
                return $"{this.Description} for  {this.Amount}, on: "
                    + this.TransDate.ToString("d") + "Balance:" + this.Balance.ToString();

            }
        }


        public class HistoryViewModel
        {
            private History defaultHistory = new History();
            public History DefaultHistory { get { return this.defaultHistory; } }

            private ObservableCollection<History> histories = new ObservableCollection<History>();
            public ObservableCollection<History> Histories { get { return this.histories; } }

            public HistoryViewModel()
            {
                this.histories.Add(new History()
                {
                    Description = "Chic-Fila",
                    Amount = 17.86,
                    TransDate = new DateTime(2016, 09, 27),
                    Balance = 5000

                });
                this.histories.Add(new History()
                {
                    Description = "Loaf and Jug",
                    Amount = 37.89,
                    TransDate = new DateTime(2016, 09, 27),
                    Balance = 5000
                });
                this.histories.Add(new History()
                {
                    Description = "Safeway",
                    Amount = 239.08,
                    TransDate = new DateTime(2016, 09, 26),
                    Balance = 5000
                });
            }
        }
    }
}

