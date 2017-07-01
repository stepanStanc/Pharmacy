using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Pharmacy
{
    public interface ITable
    {
        [PrimaryKey, AutoIncrement]
        int ID { get; set; }
    }
}
