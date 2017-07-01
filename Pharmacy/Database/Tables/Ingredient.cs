using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Pharmacy
{
    public class Ingredient : ITable
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [Unique]
        public string Name { get; set; }

        public Ingredient()
        {
        }

        public override string ToString()
        {
            return "Name: " + Name;
        }
    }
}
