using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Pharmacy
{
    public class Utils
    {
        public Utils()
        {

        }
        /// <summary>
        /// Generalizovaná funkce pro přehazování stejných itemů mezi dvěma listy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <param name="invert"></param>
        public void manageSwicthClick<T>(ListView list1, ListView list2, bool invert) where T : ITable, new()
        {
            var item = (T)Convert.ChangeType((invert) ? list2.SelectedItem : list1.SelectedItem, typeof(T)); //vybraný prvek - konvertuje T na zastupce třídy kterou při požití zastupuje
            if (item != null) //pokud byl nějaký vybrán - zabraňuje akci pokud by změna byla na žádné označené
            {
                var source = list1.ItemsSource as List<T>; //získá všechny z aktuálního listView - musí být jakmile byl jednou použit Itemssource
                if (!invert)
                {
                    list2.Items.Add(item); //Přehodí do druhého lsitu              
                    source.Remove(item);//odstraní vybraný prvek
                }
                else
                {
                    list2.Items.Remove(item); //Přehodí do druhého lsitu              
                    source.Add(item);//odstraní vybraný prvek               
                }
                list1.Items.Refresh(); // obnoví se změnami
            }
        }
    }
}
