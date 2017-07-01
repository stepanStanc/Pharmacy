using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Pharmacy
{
    class SoldMeds : ITable
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int SoldID { get; set; }
        public int MedicationID { get; set; }

        public SoldMeds()
        {
        }
    }


}
