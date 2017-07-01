using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy
{
    /// <summary>
    /// Třída pro zobrazení všech informací které souvisí s léky - MEdication
    /// </summary>
  public class MedicationView : ITable
  {
    public int ID { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public string Ingredients { get; set; }
  }
}
