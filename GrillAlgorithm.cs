
using System.IO.Pipes;
using System.Reflection.Metadata;
using System.Runtime;

namespace GrillApp
{
    class GrillAlgorithm
    {
        /// <summary>
        /// Tracking how many cooking rounds are done on the grill.
        /// </summary>
        private int mnRoundCounter;

        /// <summary>
        /// Length of Grill
        /// </summary>
        const int mnGrill_Length = 30;

        /// <summary>
        /// Width of Grill
        /// </summary>
        const int mnGrillWidth = 20;

        /// <summary>
        /// Current length coordinate on the grill
        /// </summary>
        private int mnCurrentXAxis;

        /// <summary>
        /// Current with coordinate on the grill
        /// </summary>
        private int mnCurrentYAxis;

        /// <summary>
        /// Dictionary containing the menu as well as how many rounds were made
        /// </summary>
        public Dictionary<string, int> mdictMenuResults = new Dictionary<string, int>();

        /// <summary>
        /// Taking in each menu and performing a check for how many rounds are made
        /// </summary>
        /// <param name="mi">Menu which contains all the food items</param>
        internal void CalculateRounds(MenuItems mi)
        {
            Console.WriteLine($"Checking {mi.MenuName}");
            mdictMenuResults.Add(mi.MenuName, 0);
            mnRoundCounter = 1;
            mnCurrentXAxis = 0;
            mnCurrentYAxis = 0;

            foreach (FoodItems fi in mi.FoodItems)
            {
                Console.WriteLine($"{fi.FoodName} Quantity of: {fi.Quantity} Length: {fi.Length} Width: {fi.Width}");
                while (fi.Quantity != 0)
                {
                    //If the condition is met we know the food still fits on the grill
                    if (DoesFoodFit(mnGrillWidth, mnGrill_Length, fi.Width, fi.Length, ref mnCurrentYAxis, ref mnCurrentXAxis))
                    {
                        //Food does fit so reduce the quantity and try to add more
                        fi.Quantity -= 1;


                        if (fi.Quantity == 0)
                        {
                            //Setting here if all the food fits in the first go, update the width before going to the next food
                            if (mnCurrentYAxis == 0)
                            {
                                mnCurrentYAxis = fi.Width;
                            }
                            //Noticed issue in my algorithm where correct width was not being applied 
                            //Prep for next food by setting up the y-axis
                            else if (mnCurrentXAxis == mnGrill_Length)
                            {
                                mnCurrentYAxis += fi.Width;
                                //mnCurrentXAxis += fi.Length;
                            }
                        }
                    }
                    else
                    {
                        //The grill is full, need to add a round and clear the grill for next batch.
                        mnRoundCounter++;
                        mnCurrentXAxis = 0;
                        mnCurrentYAxis = 0;
                    }
                }
            }

            mdictMenuResults[mi.MenuName] = mnRoundCounter;
        }

        /// <summary>
        /// Method to check if the size of the food can fit on the grill.
        /// NOTE: Possible changes I could make to this if I had more time, would be the option to rotate the food to fit in the gaps.
        /// </summary>
        /// <param name="mnGrill_Width"></param>
        /// <param name="mnGrill_Length"></param>
        /// <param name="nFoodWidth"></param>
        /// <param name="nFoodLength"></param>
        /// <param name="mnCurrentYAxis">Passing by reference to keep the underlying coordinate</param>
        /// <param name="mnCurrentXAxis">Passing by reference to keep the underlying coordinate</param>
        /// <returns></returns>
        private bool DoesFoodFit(int mnGrill_Width, int mnGrill_Length, int nFoodWidth, int nFoodLength, ref int mnCurrentYAxis, ref int mnCurrentXAxis)
        {
            bool bItFits = false;

            //Cheking first if X-Axis fits in grill
            if (mnCurrentXAxis + nFoodLength <= mnGrill_Length)
            {
                //Checking next if the Y-Axis fits in grill
                if (mnCurrentYAxis + nFoodWidth <= mnGrill_Width)
                {
                    bItFits = true;
                    mnCurrentXAxis += nFoodLength;
                }
            }
            //Perhaps if we rotate the meat it may fit... Utilize majority of grill
            // else if (mnCurrentXAxis + nFoodWidth <= mnGrill_Length)
            // {
            //     if (mnCurrentYAxis + nFoodLength <= mnGrill_Width)
            //     {
            /* Within this loop, I probably would have made a boolean flag stating if the food was roatated.
            From there, I would plot new coordinates showing where the food is on the grill.
            Note for next iteration, if the current food wasnt able to fit. Possibly perform LINQ commands to acquire the next food on menu
            Go through same logic of checking all dimensions if they shall fit on the grill.*/
            //     }

            // }
            else
            {
                //no more room on X-axis anymore, go below it
                mnCurrentXAxis = 0;
                mnCurrentYAxis += nFoodWidth;

                //If this exceeds the main grill width then we know that the grill is now full
                if (mnCurrentYAxis <= mnGrill_Width)
                {
                    bItFits = true;
                    mnCurrentXAxis += nFoodLength;
                }
            }

            return bItFits;
        }
    }
}