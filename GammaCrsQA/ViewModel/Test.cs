using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQA.ViewModel
{
    public class GridItem
    {
        public string Name { get; set; }
        public int CompanyID { get; set; }
    }

    public class CompanyItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class ViewModel
    {
        public ViewModel()
        {
            GridItems = new ObservableCollection<GridItem>() {
            new GridItem() { Name = "Jim", CompanyID = 1 } };

            CompanyItems = new ObservableCollection<CompanyItem>() {
            new CompanyItem() { ID = 1, Name = "Company 1" },
            new CompanyItem() { ID = 2, Name = "Company 2" } };
        }

        public ObservableCollection<GridItem> GridItems { get; set; }
        public ObservableCollection<CompanyItem> CompanyItems { get; set; }
    }
}
