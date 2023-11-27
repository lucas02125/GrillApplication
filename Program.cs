using System.Drawing;
using System.Net;
using Newtonsoft.Json.Linq;

namespace GrillApp
{
    class Program
    {
        /// <summary>
        /// REST API Link that was provided in the documentation
        /// </summary>
        private const string FeedName = @"http://isol-grillassessment.azurewebsites.net/swagger/docs/v1";

        /// <summary>
        /// Total sum of all rounds for all menus
        /// </summary>
        private static int TotalRounds;

        /// <summary>
        /// Grilling API object that will obtain all the data information of the menus
        /// </summary>
        private static GrillingAPI Cooking = new GrillingAPI();

        /// <summary>
        /// Object which contains all algorithm info for grilling rounds
        /// </summary>
        private static GrillAlgorithm MenuAlg = new GrillAlgorithm();


        static void Main()
        {
            //Do all the REST API stuff
            string sSwaggerLink = Cooking.GetSwaggerUI(FeedName);
            JArray menuJson = Cooking.GetMenu(sSwaggerLink);
            Cooking.LoadMenuAndFoods(menuJson);

            //*** UNIT TESTS made in comments below:
            // MenuAlg.CalculateRounds(Cooking.menuItems[8]);
            //MenuAlg.CalculateRounds(Cooking.menuItems[0]);

            foreach (MenuItems mi in Cooking.MenuItems)
            {
                MenuAlg.CalculateRounds(mi);
            }

            //Going through all menus and outputting rounds
            foreach (KeyValuePair<string, int> kvp in MenuAlg.mdictMenuResults)
            {
                TotalRounds += kvp.Value;
                Console.WriteLine($"{kvp.Key}: {kvp.Value} rounds");
            }

            Console.WriteLine($"Total: {TotalRounds}");
        }
    }
}