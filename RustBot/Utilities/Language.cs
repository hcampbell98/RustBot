using System;
using System.Collections.Generic;
using System.Text;

namespace RustBot
{
    class Language
    {
        public const string
            //Team
            Team_Error_No_Team = "You are not a member of a team. Please create one using r!createteam.",
            Team_Error_Has_Team = "This person is already a member of a team. If they wish to leave, they can run r!leaveteam.",
            Team_Error_Not_Leader = "You are not the team leader.",
            Team_Error_Notifications_Disabled = "This team has notifications disabled.",
            Team_Creation_Notifications = "Would you like to enable notifications for this team?\n\n1. Yes\n2. No\n\n**Please type the number of your answer.**",
            Team_Creation_Error_Invalid = "Please type the number of your answer. E.g: 1 for yes, 2 for no.",
            Team_Coordinates_Error_Invalid = "The input was an invalid coordinate. Examples of valid coordinated: D12, a14, c1, B28",
            Team_Leave = "You left your current team.",
            Team_Join = "You have joined the team.",
            Team_Invite_None = "You have no pending invites.",
            Team_Invite_Decline = "ou declined the team invite.",
            Team_Invite_Disabled = "This user has team invites disabled. They can re-enable them by running r!toggleinvites",
            Team_Invite_Pending = "This user already has a pending team invite. They can decline this by running r!decline",
            Team_Server_Error_Broad = "Please try a less broad search term and try again.",
            Team_Error_Wrong_Guild = "This command should be run in the server the group was created.",

            //Coinflip
            Coinflip_Error_No_Mention = "You need to @mention your opponent.",

            //CraftingInfo
            Crafting_Cannot_Craft = "This item cannot be crafted.",

            //SkinInfo
            SkinInfo_Order_By = "**1. New**\n**2. Price**\n**3. Discount**\n**4. Exclusive**\n**5. Market Price**\n**6. Name**\n**7. Item**\n\n**Please type the number of the sort type desired.**",
            SkinInfo_Order_Direction = "**1. Ascending**\n**2. Descending**\n\n *Please type the number of the sort direction desired.*",
            SkinInfo_Awaiting_Search = "**Please type your search query.**",

            //BreakInfo
            BreakInfo_Structure_Type = "1. Building Block\n2. Placeable/Door/Window\n\n**Please type the number of the structure type.**",
            BreakInfo_Attack_Type = "1. Explosive\n2. Melee\n3. Guns\n4. Throw\n\n**Please type the number of the attack type.**",
            BreakInfo_Block_Side = "1. Hard\n2. Soft\n\n**Please type the number of the attack type.**",

            //ServerInfo
            ServerInfo_Disabled = "This server has disabled the ability to search for servers.",
            ServerInfo_AddedAd = "Advertisement Added Successfully",
            ServerInfo_RemovedAd = "Advertisement Removed Successfully",

            //SendLeaderboard
            Leaderboard_Error_Not_Found = "The specified board could not be found. Please double check you've typed it correctly and try again. You can view all available boards by running `r!leaderboard`.",
            Leaderboard_Error_Premium = "This command is a Wood Tier or higher feature. If you would like access to this command, type r!premium and purchase a tier. Thank you!",

            //VerifyPremium
            Premium_Verification_Error_Failed = "It appears you haven't purchased a premium rank. Please check your transaction ID is correct and try again. If you haven't bought a rank yet, you can do so by typing r!premium.",

            //PlayerStats
            PlayerStats_Error_Private = "The profile specified may be private. If this profile is yours, please change to public and try again.";

    }
}
