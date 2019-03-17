using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dm_bot.Contexts;
using dm_bot.Models;
using dm_bot.Services;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace dm_bot.Commands
{
    public class InventoryCommand : ModuleBase<SocketCommandContext>
    {
        private readonly DMContext _db;
        private readonly IConfiguration _configuration;
        private readonly TradeService _tradeService;

        public InventoryCommand(DMContext context, IConfiguration configuration, TradeService tradeService)
        {
            this._db = context;
            this._configuration = configuration;
            this._tradeService = tradeService;
        }

        [Command("inventory")]
        public async Task InventoryAsync(string command = "list", int itemId = -1, int quantity = 1, [Remainder] string message = null)
        {
            // find the player
            var user = _db.Players.FirstOrDefault(p => p.DiscordMention == Context.User.Mention);

            // If we didn't find one, redirect them to staff to add them
            if (user == null)
            {
                var helpRole = _configuration.GetValue("helpRoleName", "Staff");
                await ReplyAsync($"Sorry, you don't appear to be in our system. Please talk to somone in {Context.Guild.Roles.First(role => role.Name == helpRole).Mention}");
                return;
            }

            switch (command.ToLower())
            {
                case "list": // $inventory list
                    await ShowUserInventory(user);
                    break;
                case "add": // $inventory add <Item Id> <Quantity>
                    await AddItemToInventory(user, itemId, quantity);
                    break;
                case "remove": // $inventory remove <Item Id>
                    await RemoveItemFromInventory(user, itemId);
                    break;
                default:
                    await ShowHelpMessage();
                    return;
            }
        }

        private async Task RemoveItemFromInventory(Player user, int itemId)
        {
            if (!user.PlayerInventory.Any(item => item.Item.Id == itemId))
            {
                return;
            }

            user.PlayerInventory.Remove(user.PlayerInventory.First(item => item.Item.Id == itemId));

            _db.Players.Update(user);

            await _db.SaveChangesAsync();
        }

        private async Task AddItemToInventory(Player user, int itemId, int quantity)
        {
            if (user.PlayerInventory.Any(inventory => inventory.Item.Id == itemId))
            {
                user.PlayerInventory.First(inventory => inventory.Item.Id == itemId).Quantity += quantity;
            }

            var item = await _db.Items.FindAsync(new { ID = itemId });

            if (item == null)
            {
                await ReplyAsync("");
                return;
            }

            user.PlayerInventory.Add(new InventoryItem() { Item = item, Quantity = quantity });

            _db.Update(user);
            await _db.SaveChangesAsync();
        }

        private async Task ShowUserInventory(Player user)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < user.PlayerInventory.Count; i++)
            {
                var item = user.PlayerInventory.ElementAt(i);

                sb.AppendLine($"{item.Item.Id}) {item.Item.Name} ({item.Item.DisplayValue})");
            }

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithDescription(sb.ToString());
            await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());
        }

        private Task ShowHelpMessage()
        {
            throw new NotImplementedException();
        }
    }
}