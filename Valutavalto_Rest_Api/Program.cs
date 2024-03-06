using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Valutavalto_Rest_Api
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Felhasználó által megadott HUF összeg bekérése
            Console.Write("Összeg forintban: ");
            double HUF;
            while (!double.TryParse(Console.ReadLine(), out HUF) || HUF < 0)
            {
                Console.WriteLine($"\nHibás, vagy érvénytelen érték!");
                Console.WriteLine($"Próbálja újra...");
            }

            //Forint átváltása Euro-ra
            double EURO = await AtvaltasHufEuro(HUF);

            //Eredény kiírása
            Console.WriteLine($"\n{HUF}Ft = {EURO:F2}Euro");

            Console.WriteLine("\nProgram vége...");
            Console.ReadKey();
        }

        private static async Task<double> AtvaltasHufEuro(double HUF)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://infojegyzet.hu/webszerkesztes/php/valuta/api/v1/arfolyam/");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();

                        //JSON objektum olvasása és deszerializálása
                        var valutavalto = Valutavalto.FromJson(jsonString);

                        //Ellenőrzés, hogy az átváltási arány megtalálható-e
                        if (valutavalto.Rates.ContainsKey("HUF"))
                        {
                            double NapiErtek = valutavalto.Rates["HUF"];

                            //Átváltás HUF-ról EURO-ra
                            double euro = HUF / NapiErtek;
                            return euro;
                        }
                        else
                        {
                            Console.WriteLine("Az API válasza nem tartalmazza a 'HUF' kulcsot");
                            return -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Hiba történt az API hívása során");
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hiba történt: " + ex);
                    return -1;
                }
            }
        }
    }
}