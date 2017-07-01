using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Pharmacy
{
    public class Allergies : ITable
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int PatientID { get; set; }
        public int AllergenID { get; set; }

        public Allergies()
        {
        }

        
    }
}
