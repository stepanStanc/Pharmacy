using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Pharmacy
{
    public class Patient : ITable
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime YearBirth { get; set; }

        public Patient()
        {
        }

        public override string ToString()
        {
            
            return "Name: " + Name + ", Surname: " + Surname;
        }
    }
}
