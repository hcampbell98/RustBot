using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Collections.Generic;
using Discord;
using Discord.Addons.Interactive;
using System.Linq;
using System.Text;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Craft : InteractiveBase
{
    [Command("craft", RunMode = RunMode.Async)]
    [Summary("Sends crafting info")]
    public async Task SendCraftingInfo(int number, [Remainder] string item)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        //Stops people spamming characters as to lag the bot
        if (item.Length > 100) { await ReplyAsync("Please use a shorter item name."); return; }
        if (item.Length < 3) { await ReplyAsync("Please use a longer item name."); return; }
        if (number < 0) { await ReplyAsync("Input must be larger than 0."); return; }

        //Grabbing item info
        var itemInfo = await Utilities.SearchForItem(item);

        //If item not found
        if (itemInfo == null) { await ReplyAsync($"Item `{item}` not found."); await Utilities.StatusMessage("item", Context); return; }
        //If item found
        else
        {
            //If there's more than one item found
            if (itemInfo.Count > 1)
            {
                StringBuilder si = new StringBuilder();
                int count = 0;

                foreach (SearchItem s in itemInfo)
                {
                    count++;
                    si.Append($"{count}. {s.ItemName}\n");
                }

                si.Append("\n**Please type the number of the item you are looking for.**");

                await ReplyAsync("", false, Utilities.GetEmbedMessage("Search Results", "Multiple Results", si.ToString(), Context.Message.Author, Color.Red));


                var response = await NextMessageAsync();

                if (response != null)
                {
                    Item i = await Utilities.GetItemInfo(itemInfo[Convert.ToInt32(response.Content) - 1]);

                    //If the item isn't craftable, send an error message and return
                    if (i.Ingredients.Count < 1) { await ReplyAsync("", false, Utilities.GetEmbedMessage($"{i.ItemName}", "Not Craftable", "This item cannot be crafted.", Context.Message.Author, Color.Red)); return; }

                    await ReplyAsync("", false, GenMessage(i, number));
                }

            }
            //If one item found
            else
            {
                Item i = await Utilities.GetItemInfo(itemInfo[0]);

                //If the item isn't craftable, send an error message and return
                if (i.Ingredients.Count < 1) { await ReplyAsync("", false, Utilities.GetEmbedMessage($"{i.ItemName}", "Not Craftable", "This item cannot be crafted.", Context.Message.Author, Color.Red)); return; }

                await ReplyAsync("", false, GenMessage(i, number));
            }

            await Utilities.StatusMessage("craft", Context);
        }

    }

    private Embed GenMessage(Item i, int amount)
    {
        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithThumbnailUrl(i.Icon);
        eb.WithUrl(i.URL);
        eb.WithTitle($"{i.ItemName} x{amount}");

        //Crafting info builder
        if (i.Ingredients.Count != 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"__*Ingredient - Amount*__\n");

            foreach (var d in i.Ingredients)
            {
                if (Int32.TryParse(d.IngredientAmount.Replace("�", ""), out int defAmount))
                {
                    sb.Append($"{d.IngredientName} �{((Int64)defAmount * amount).ToString("#,##0")}\n");
                }
                else
                {
                    sb.Append($"{d.IngredientName} �{amount.ToString("#,##0")}\n");
                }
            }

            eb.AddField("Ingredients", $"{sb.ToString()}", false);
        }

        eb.WithColor(Color.Red);
        eb.WithFooter(fb);

        return eb.Build();
    }
}