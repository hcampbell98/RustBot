using SSRPBalanceBot;
using System;
using System.Collections.Generic;
using System.Text;

namespace RustBot.Aliases
{
    class AliasManager
    {
        public static List<ItemAlias> itemAliases = Utilities.FillList<ItemAlias>("Aliases/itemAliases.json");

        public static string GetItemName(string alias)
        {
            foreach(ItemAlias item in itemAliases)
            {
                foreach(string a in item.Aliases)
                {
                    if (a == alias.ToLower()) { return item.ItemName; }
                }
            }

            return null;
        }
    }

    class ItemAlias
    {
        public string ItemName { get; set; }
        public string[] Aliases { get; set; }
    }
}
