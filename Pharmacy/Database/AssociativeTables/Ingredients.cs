using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Pharmacy
{
    public class Ingredients : ITable
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int MedicationID { get; set; }
        public int IngredientID { get; set; }

        public Ingredients()
        {
        }


    }
}
