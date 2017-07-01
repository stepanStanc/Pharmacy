using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Views
{
    class SoldView : ITable
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public int Price { get; set; }
        public string Date { get; set; }
        public string Medications { get; set; }
    }
}
