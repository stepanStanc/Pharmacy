using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Pharmacy
{
    public class ItemDatabase
    {
        // SQLite connection
        private SQLiteAsyncConnection database;

        public ItemDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Ingredient>();
            database.CreateTableAsync<Medication>();
            database.CreateTableAsync<Patient>();
            database.CreateTableAsync<Sold>();
            database.CreateTableAsync<SoldMeds>();
            database.CreateTableAsync<Allergies>();
            database.CreateTableAsync<Ingredients>();
        }

        // Query
        public Task<List<T>> GetItemsAsync<T>() where T : ITable, new()
        {
            return database.Table<T>().ToListAsync();
        }      

        // sql dotaz iniverzální v typu výsledku a v hodnotě i sloupci
        public Task<List<T>> GetItemsByColumnValue<T>(int id,string tableRow) where T : ITable, new()
        {
            string pom = String.Format("SELECT * FROM {2} WHERE {1} = {0}", id,tableRow, typeof(T).Name);
            return database.QueryAsync<T>(pom);
        }
        // přes JOIN načte všechny provázané hodnoty patřící ke sloupci podle ID přes vazební tabulku
        public Task<List<T>> GetAssociatedL<T>(int id,string table,string tableMainRow,string tableSecondaryRow) where T : ITable, new()
        {
            string pom = String.Format("SELECT * FROM {4} JOIN [{1}] ON [{1}].[{2}] = {0} WHERE [{1}].[{3}] = [{4}].[ID]", id,table,tableMainRow,tableSecondaryRow, typeof(T).Name);

            return database.QueryAsync<T>(pom);
        }

        /// <summary>
        /// Získá poslední použité ID léku
        /// </summary>
        /// <returns></returns>
        public Task<List<T>> GetLastID<T>() where T : ITable, new()
        {
            string pom = String.Format("SELECT * FROM {0} WHERE ID = (SELECT MAX(ID) FROM {0})", typeof(T).Name);
            return database.QueryAsync<T>(pom);
        }
        //SELECT [seq] FROM [sqlite_sequence] WHERE [name] = 'Medication'

        // Query using LINQ
        public Task<T> GetItemAsync<T>(int id) where T : ITable, new()
        {
            return database.Table<T>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }      

        public Task<int> SaveItemAsync<T>(T item) where T : ITable, new()
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                               

            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        
        public Task<int> DeleteItemAsync<T>(T item) where T : ITable, new()
        {
            return database.DeleteAsync(item);
        }

        public Task<List<int>> DeleteItemAsyncByID<T>(int id) where T : ITable, new()
        {
            string pom = String.Format("DELETE FROM {0} WHERE ID = {1}", typeof(T).Name, id);
            return database.QueryAsync<int>(pom);
        }

    }
}
