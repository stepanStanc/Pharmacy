using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Pharmacy.Views;

namespace Pharmacy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool editing = false; //pokud je editován lék
        private Medication edited; // gloální uložení editovaného léku
        private Utils utils = new Utils(); //načtení univerzálníhc metod
        public MainWindow()
        {
            InitializeComponent();

            //výpis dat z DB do všemožných ListView
            showListIng(0);
            showListMed();
            showListPat();
            showListSold();
        }

        
        private static ItemDatabase _database;
        //instance databáze
        public static ItemDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    var fileHelper = new FileHelper();
                    _database = new ItemDatabase(fileHelper.GetLocalFilePath("SQLite.db3"));
                }
                return _database;
            }
        }

        /// <summary>
        /// Přidávání nových ingrediencí
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addAl_Click(object sender, RoutedEventArgs e)
        {
            if (!(String.IsNullOrEmpty(newAl.Text))) //pokud je v názvu nové ingredience text
            { 
                Ingredient al = new Ingredient();
                al.Name = newAl.Text;
                Database.SaveItemAsync(al); //uložení
            }

            newAl.Text = ""; //vyčistí textové pole

            showListIng(21); //vypíše znovy všechny listy s ingrediencemi
        }
        /// <summary>
        /// vypíše znovy všechny listy s ingrediencemi
        /// </summary>
        public async void showListIng(int delay)
        {
            //načítání dat z DB odděleno protože jinak jsou listy provázané
            await Task.Delay(delay); //opozdí funkci aby se styihly nové údaje uložit
            var itemsFromDb = Database.GetItemsAsync<Ingredient>().Result; //načte list všech ingrediencí v DB
            listEditAl.ItemsSource = itemsFromDb;
            var itemsFromDb1 = Database.GetItemsAsync<Ingredient>().Result; //načte list všech ingrediencí v DB
            medAllIng.ItemsSource = itemsFromDb1;
            medNewIng.Items.Clear();
            var itemsFromDb2 = Database.GetItemsAsync<Ingredient>().Result; //načte list všech ingrediencí v DB
            patAllAl.ItemsSource = itemsFromDb2;
            patNewAl.Items.Clear();
        }
        /// <summary>
        /// Vypíše všechny léky
        /// </summary>
        public async void showListMed()
        {
            await Task.Delay(21); //opozdí funkci aby se styihly nové údaje uložit
            //listy se vzájemně ovlviňujéí pokud mají stejný zdroj
            List<MedicationView> medV = new List<MedicationView>(); // zapotřebí pro zobrazení léků s jejichingrediencemi
            List<MedicationView> medV1 = new List<MedicationView>(); // zapotřebí pro zobrazení léků s jejichingrediencemi

            var itemsFromDb = Database.GetItemsAsync<Medication>().Result; 
            //pro každý - Medication - v listu
            foreach(Medication med in itemsFromDb)
            {
                var thisIngL = Database.GetAssociatedL<Ingredient>(med.ID,"Ingredients","MedicationID","IngredientID").Result; //všechny ingredience získané pomocí vazební tabulky přes join
                MedicationView mV = new MedicationView(); //mV - prozobrazení informací o léku
                mV.ID = med.ID; //id medikace pro možnost smazání podle zobrazovací třídy
                //další data medikace
                mV.Name = med.Name; 
                mV.Price = med.Price;
                mV.Ingredients = ""; //- vytvoření stringu
                //pro každou ingredienci patřící k medikaci
                foreach (Ingredient p in thisIngL)
                {
                    mV.Ingredients += p.Name + ", "; // s epřidá do stringu
                }
                mV.Ingredients = (mV.Ingredients.Length > 0) ? mV.Ingredients.Remove(mV.Ingredients.Length - 2) : ""; //maže psolední čárku
                medV.Add(mV); //přidá do instance MedicationView pro zobrazování
                medV1.Add(mV);
            }
            //načte všechny léky 
            mainMeds.ItemsSource = medV;
            sellAllMeds.ItemsSource = medV1;
            sellNewMeds.Items.Clear();
        }
        /// <summary>
        /// výpis všech pacientů
        /// </summary>
        public async void showListPat()
        {
            await Task.Delay(21);

            List<PatientView> patV = new List<PatientView>();
            List<PatientView> patV1 = new List<PatientView>();

            var itemsFromDB = Database.GetItemsAsync<Patient>().Result;

            foreach(Patient pat in itemsFromDB)
            {
                var thisAlL = Database.GetAssociatedL<Ingredient>(pat.ID, "Allergies", "PatientID", "AllergenID").Result; //všechny ingredience získané pomocí vazební tabulky přes join

                PatientView pV = new PatientView();
                pV.ID = pat.ID;
                pV.Name = pat.Name;
                pV.Surname = pat.Surname;
                var today = DateTime.Today;
                pV.Age = today.Year - pat.YearBirth.Year;
                pV.Allergies = "";

                foreach (Ingredient p in thisAlL)
                {
                    pV.Allergies += p.Name + ", "; // s epřidá do stringu
                }
                pV.Allergies = (pV.Allergies.Length > 0) ? pV.Allergies.Remove(pV.Allergies.Length - 2) : ""; //maže psolední čárku
                patV.Add(pV); //přidá do instance MedicationView pro zobrazování
                patV1.Add(pV);
            }

            allPats.ItemsSource = patV;
            sellAllPats.ItemsSource = patV1;
        }
        /// <summary>
        /// výpis všech prodejů
        /// </summary>
        private async void showListSold()
        {
            await Task.Delay(21);

            List<SoldView> soldV = new List<SoldView>();

            var itemsFromDB = Database.GetItemsAsync<Sold>().Result;

            foreach (Sold sld in itemsFromDB)
            {
                var thisMedL = Database.GetAssociatedL<Medication>(sld.ID, "SoldMeds", "SoldID", "MedicationID").Result;
                List<Patient> pat = Database.GetItemsByColumnValue<Patient>(sld.PatientID,"ID").Result;

                SoldView sV = new SoldView();
                sV.FullName = pat[0].Name + " " + pat[0].Surname;
                sV.Price = sld.Price;
                sV.Date = sld.Date.ToString("dd/MM/yyyy");
                sV.Medications = "";

                foreach (Medication m in thisMedL)
                {
                    sV.Medications += m.Name + ", "; // s epřidá do stringu
                }
                sV.Medications = (sV.Medications.Length > 0) ? sV.Medications.Remove(sV.Medications.Length - 2) : ""; //maže psolední čárku
                soldV.Add(sV); //přidá do instance MedicationView pro zobrazování
            }
            soldV.Reverse();
            allSold.ItemsSource = soldV;
        }

        /// <summary>
        /// Mazání alergenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delAl_Click(object sender, RoutedEventArgs e)
        {
            var al = (listEditAl.SelectedItem as Ingredient);
            Database.DeleteItemAsync(al); //získá jí z listu a smaže tento Item z Databáze

            showListIng(21);
        }
        /// <summary>
        /// Přidání nového léku - obsahuje funkce pro editci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addMed_Click(object sender, RoutedEventArgs e)
        {
            //pokud je v editačním ListView nějaký Item - Ingredience
            if(medNewIng.Items.Count > 0)
            {
                int pom = 0;
                int.TryParse(NewMedPrc.Text, out pom); // získá int pro cenu
                if (!(String.IsNullOrEmpty(NewMedName.Text)) && pom > 0)//pokud lék měl určenou cenu a pokud je v textovém poli text
                {
                    Medication med = new Medication();
                    med.Name = NewMedName.Text;
                    med.Price = pom;
                    
                    if (editing)
                    {
                        List<Ingredients> ingsL = Database.GetItemsByColumnValue<Ingredients>(edited.ID,"MedicationID").Result;
                        foreach (Ingredients ings in ingsL)
                        {
                            Database.DeleteItemAsync<Ingredients>(ings);
                        }

                        med.ID = edited.ID;
                    }

                    Database.SaveItemAsync(med);
                    //načte všechny z ListView
                    foreach (Ingredient ing in medNewIng.Items)
                    {
                        Ingredients ings = new Ingredients();
                        ings.IngredientID = ing.ID;
                        //získá psolední použité ID z DB
                        List<Medication> pom1 = (!editing) ? Database.GetLastID<Medication>().Result : null;
                        ings.MedicationID = (!editing) ? pom1[0].ID++ : edited.ID;//určí ID a přičte 1 - předpověď ID vytvářeného léku
                        Database.SaveItemAsync(ings);
                    }
                    // vše vyčistí a načte nové údaje
                    showListMed();
                    NewMedPrc.Text = "";
                    NewMedName.Text = "";
                    showListIng(0);
                    medNewIng.Items.Clear();
                    editing = false;
                    addMed.Content = "Add";
                }
            }                      
        }

        /// <summary>
        /// Přidávání pacienta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addPat_Click(object sender, RoutedEventArgs e)
        {
            if (patNewAl.Items.Count > 0)
            {
                var date = patBirth.SelectedDate;
                if ((!(String.IsNullOrEmpty(patName.Text))) && (!(String.IsNullOrEmpty(patSurname.Text))) && date != null){
                    Patient pati = new Patient();
                    pati.Name = patName.Text;
                    pati.Surname = patSurname.Text;
                    pati.YearBirth = date.Value;

                    Database.SaveItemAsync(pati);

                    foreach (Ingredient al in patNewAl.Items)
                    {
                        Allergies als = new Allergies();
                        als.AllergenID = al.ID;
                        //získá psolední použité ID z DB
                        List<Patient> pom1 =  Database.GetLastID<Patient>().Result;
                        als.PatientID = pom1[0].ID++ ;//určí ID a přičte 1 - předpověď ID vytvářeného léku
                        Database.SaveItemAsync(als);
                    }
                    showListPat();
                    patSurname.Text = "";
                    patName.Text = "";
                    patBirth.SelectedDate = null;
                    showListIng(0);
                    patNewAl.Items.Clear();
                    pat.IsSelected = true;
                }              
            }

        }

        /// Přehazují Ingredience mezi poli

        private void medAllIngClick(object sender, SelectionChangedEventArgs e)
        {
            utils.manageSwicthClick<Ingredient>(medAllIng, medNewIng,false);
        }             
        private void medNewIngClick(object sender, SelectionChangedEventArgs e)
        {
            utils.manageSwicthClick<Ingredient>(medAllIng, medNewIng, true);
        }

        private void patAllAlClick(object sender, SelectionChangedEventArgs e)
        {
            utils.manageSwicthClick<Ingredient>(patAllAl, patNewAl, false);
        }
        private void patNewAlClick(object sender, SelectionChangedEventArgs e)
        {
            utils.manageSwicthClick<Ingredient>(patAllAl, patNewAl, true);
        }
        //přehazování léků přes medicationView
        private void sellAllMedsClick(object sender, SelectionChangedEventArgs e)
        {            
            utils.manageSwicthClick<MedicationView>(sellAllMeds, sellNewMeds, false);
            loadTotalPrice();
            checkAllergy();
        }       
        private void sellNewMedsClick(object sender, SelectionChangedEventArgs e)
        {
            utils.manageSwicthClick<MedicationView>(sellAllMeds, sellNewMeds, true);
            loadTotalPrice();
            checkAllergy();
        }
        //kliknutí na pacianty při prodeji
        private void sellAllPatsClick(object sender, SelectionChangedEventArgs e)
        {
            checkAllergy();
        }
        //získa aktuální cenu všech léků
        private void loadTotalPrice()
        {
            if(sellNewMeds.Items != null)
            {
                int i = 0;
                foreach (MedicationView mV in sellNewMeds.Items.Cast<MedicationView>().ToList() as List<MedicationView>)
                {
                    i += mV.Price;
                }
                totalPrc.Text = i.ToString();
            }          
        }
        /// <summary>
        /// Provádí kontrolu zda je pacient alergický na léky
        /// </summary>
        private void checkAllergy()
        {
            if (sellAllPats.SelectedItem != null) //pokud jsou zvolyny nějaké léky
            {
                var pat = (sellAllPats.SelectedItem as PatientView); //zvolí pacienta
                //listy pro získání léky a ingrediency na které má pacient alergie
                List<string> badAls = new List<string>(); 
                List<string> badMeds = new List<string>();
                //načtení alergií pacienta
                List<Allergies> alsL = Database.GetItemsByColumnValue<Allergies>(pat.ID, "PatientID").Result;
                //pro každý lék
                foreach (MedicationView mV in sellNewMeds.Items.Cast<MedicationView>().ToList() as List<MedicationView>)
                {
                    //načte ingredience léku
                    List<Ingredients> ingsL = Database.GetItemsByColumnValue<Ingredients>(mV.ID, "MedicationID").Result;
                    //pro každou ingredienci
                    foreach (Ingredients ings in ingsL)
                    {
                        //pro každou alergii pacienta 
                        //porovná jestli se shodují pokud ano, načte je z databáze a získá jejich jména
                        foreach (Allergies als in alsL)
                        {
                            if (als.AllergenID == ings.IngredientID)
                            {
                                var al = Database.GetItemsByColumnValue<Ingredient>(als.AllergenID,"ID").Result;
                                var med = Database.GetItemsByColumnValue<Medication>(mV.ID,"ID").Result;
                                badAls.Add(al[0].Name);
                                badMeds.Add(med[0].Name);
                            }
                        }
                    }
                }
                //odtraní duplikáty
                badAls = badAls.Distinct().ToList();
                badMeds = badMeds.Distinct().ToList();
                string badMed = "";
                string badAl = "";
                //pokud byly nalezeny alergie
                //vytvoří stringy
                if(badAls.Any() && badMeds.Any())
                {
                    foreach(string a in badAls)
                    {
                        badAl += a + ", ";
                    }
                    foreach (string m in badMeds)
                    {
                        badMed += m + ", ";
                    }
                    badMed = (badMed.Length > 0) ? badMed.Remove(badMed.Length - 2) : ""; //maže psolední čárku
                    badAl = (badAl.Length > 0) ? badAl.Remove(badAl.Length - 2) : ""; //maže psolední čárku

                    //vypíše varování a zablokuje prodej dokud se nedostraí alergie
                    alWarning.Text = string.Format("Patient has allergy \n to: {0} \n in: {1}",badAl,badMed);
                    alWarning.Visibility = Visibility.Visible;
                    sellBtn.IsEnabled = false;
                }
                else
                {
                    alWarning.Visibility = Visibility.Hidden;
                    sellBtn.IsEnabled = true;
                }
                //
            }
        }

        /// <summary>
        /// Maže léky a k nim provázané ingredience
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delMed_Click(object sender, RoutedEventArgs e)
        {
            if(mainMeds.SelectedItem != null)
            {
                var med = (mainMeds.SelectedItem as MedicationView);
                Database.DeleteItemAsyncByID<Medication>(med.ID);

                showListMed();
                //samže každou k leku patřici vazebni tabulky ingrediencí
                List<Ingredients> ingsL = Database.GetItemsByColumnValue<Ingredients>(med.ID,"MedicationID").Result;
                foreach (Ingredients ings in ingsL)
                {
                    Database.DeleteItemAsync<Ingredients>(ings);
                }
            }           
        }
        /// <summary>
        /// maže pacienty a k nim provázané alergie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delPat_Click(object sender, RoutedEventArgs e)
        {
            if (allPats.SelectedItem != null)
            {
                var pat = (allPats.SelectedItem as PatientView);
                Database.DeleteItemAsyncByID<Patient>(pat.ID);

                showListPat();
                //samže každou k leku patřici vazebni tabulky ingrediencí
                List<Allergies> alsL = Database.GetItemsByColumnValue<Allergies>(pat.ID, "PatientID").Result;
                foreach (Allergies als in alsL)
                {
                    Database.DeleteItemAsync<Allergies>(als);
                }
            }
        }

        /// <summary>
        /// spuštění editace léku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edMed_CLick(object sender, RoutedEventArgs e)
        {
            if (mainMeds.SelectedItem != null)
            {
                var med = (mainMeds.SelectedItem as MedicationView);
                var pom = Database.GetItemsByColumnValue<Medication>(med.ID,"ID").Result;
                edited = pom[0];

                NewMedName.Text = edited.Name;
                NewMedPrc.Text = edited.Price.ToString();

                List<Ingredients> ingsL = Database.GetItemsByColumnValue<Ingredients>(med.ID, "MedicationID").Result;
                showListIng(0); //kdyby byl task opožděn další funkce by s litem nemohli pracovat
                medNewIng.Items.Clear();

                foreach (Ingredients ings in ingsL)
                {
                    //načíst INgredience podle ings.ID, smazat je z jednoho listu a přidat do druhého
                    var pom1 = Database.GetItemsByColumnValue<Ingredient>(ings.IngredientID,"ID").Result;
                    medNewIng.Items.Add(pom1[0]); //Přehodí do druhého lsitu
                    var source = medAllIng.ItemsSource as List<Ingredient>; //získá všechny z aktuálního listView
                    var pom2 = source.Find(n => n.ID == pom1[0].ID);
                    source.Remove(pom2);
                    medAllIng.Items.Refresh(); // obnoví se změnami

                }

                editing = true;
                addMed.Content = "Save";
            }
        }
        /// <summary>
        /// Prodej léků podle pacienta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sellBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sellAllPats.SelectedItem != null) //pokud je zvolen pacient
            {
                if (sellNewMeds.Items.Count > 0) //pokud jsou zvoleny nějaké léky
                {
                    Sold sold = new Sold();
                    PatientView pat = (sellAllPats.SelectedItem as PatientView);
                    sold.PatientID = pat.ID;
                    sold.Price = Convert.ToInt32(totalPrc.Text); 
                    sold.Date = DateTime.Now;

                    Database.SaveItemAsync(sold);
                    //ukládá všechny léky
                    foreach (MedicationView med in sellNewMeds.Items)
                    {
                        SoldMeds sm = new SoldMeds();
                        sm.MedicationID = med.ID;
                        //získá psolední použité ID z DB
                        List<Sold> pom1 =  Database.GetLastID<Sold>().Result;
                        sm.SoldID = pom1[0].ID++ ;//určí ID a přičte 1 - předpověď ID vytvářeného léku
                        Database.SaveItemAsync(sm);
                    }

                    showListMed();
                    showListSold();
                    totalPrc.Text = "0";
                    sellNewMeds.Items.Clear();
                }
            }
            
        }
    }
}
