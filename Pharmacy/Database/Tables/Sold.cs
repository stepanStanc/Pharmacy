using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Pharmacy
{
    class Sold : ITable
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int PatientID { get; set; }
        public int Price { get; set; } //jelikož se cena léků může změnit
        public DateTime Date { get; set; }

        public Sold()
        {
        }
    }

    
}
