
using System;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;


namespace GrillApp
{
    class GrillingAPI
    {

        /// <summary>
        /// List of menus in the JSON file
        /// </summary>
        public List<MenuItems> MenuItems;

        /// <summary>
        /// List of Foods within each item
        /// </summary>
        public List<FoodItems> FoodItems;


        /// <summary>
        /// Following method will obtain the Service URL
        /// </summary>
        /// <param name="sFeedName">Rest API URL link</param>
        /// <returns></returns>
        public string GetSwaggerUI(string sFeedName)
        {
            string sURL = "http://";
            WebRequest wr = WebRequest.Create(sFeedName);
            var getData = "";

            //Reading the json web response to a string 
            using (var webResponse = wr.GetResponse())
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                getData = reader.ReadToEnd();
            }

            var jarray = JObject.Parse(getData);



            //Rather than taking the Swagger UI link directly
            //Decided to pull the link together and make a connection
            foreach (var item in jarray)
            {
                switch (item.Key)
                {
                    case "host":
                        sURL += item.Value;
                        break;
                    case "paths":
                        {
                            JToken jToken = item.Value.First;
                            JProperty parentProp = (JProperty)jToken;
                            sURL += parentProp.Name;
                        }
                        break;
                }
            }

            return sURL;

        }

        /// <summary>
        /// Calling the swaggerUI and getting the response body of menus
        /// </summary>
        /// <param name="sSwagger">Swagger UI link that was returned in <!--GetSwaggerUI--></param>
        /// <returns>A JSON array containing all menu Items</returns>
        public JArray GetMenu(string sSwagger)
        {
            WebRequest wr2 = WebRequest.Create(sSwagger);
            string sTheMenu = string.Empty;

            using (var webResponse = wr2.GetResponse())
            using (var menuReader = new StreamReader(webResponse.GetResponseStream()))
            {
                sTheMenu = menuReader.ReadToEnd();
            }

            var joResponse = JArray.Parse(sTheMenu);

            return joResponse;
        }

        /// <summary>
        /// Parsing the file into respected Menu and Food objects
        /// </summary>
        /// <param name="values">[menu] json values</param>
        internal void LoadMenuAndFoods(JArray values)
        {
            MenuItems = new List<MenuItems>();

            //Loop through all json array and plug into our menu object
            foreach (var item in values)
            {
                MenuItems menu = new MenuItems()
                {
                    SchemaID = (int)item.SelectToken("$id")
                    ,
                    ID = (string)item.SelectToken("Id")
                    ,
                    MenuName = (string)item.SelectToken("menu")
                    ,
                    FoodItems = GetAllFoods(item.SelectToken("items"))
                };

                MenuItems.Add(menu);
            }
        }


        /// <summary>
        /// Parsing through items property tag in JSON and creating new food objects
        /// </summary>
        /// <param name="jToken">[items] tag containing food JSON </param>
        /// <returns></returns>
        private List<FoodItems> GetAllFoods(JToken? jToken)
        {
            FoodItems = new List<FoodItems>();

            //Loop through the child nodes of the parent 'items' property and get all the food objects
            foreach (var childNode in jToken)
            {
                FoodItems food = new FoodItems()
                {
                    SchemaID = (string)childNode.SelectToken("$id")
                    ,
                    ID = (string)childNode.SelectToken("Id")
                    ,
                    FoodName = (string)childNode.SelectToken("Name")
                    ,
                    Length = (int)childNode.SelectToken("Length")
                    ,
                    Width = (int)childNode.SelectToken("Width")
                    ,
                    Duration = (string)childNode.SelectToken("Duration")
                    ,
                    Quantity = (int)childNode.SelectToken("Quantity")
                };
                FoodItems.Add(food);
            }

            return FoodItems;
        }
    }
}