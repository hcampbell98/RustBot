# Rust Helper
A Rust Discord Bot, developed in C#

# Commands


## Tools
```
r!break [placeable] - Sends structure breaking info. [placeable] is the object you're trying to break, such as "Stone Wall" or "Sheet Metal Door".

r!craft [amount] [item] - Sends crafting information for the specified item, multiplied by the amount.

r!furn [oreType] [amount] - Sends small furnace info such as time to smelt, total wood required and charcoal produced

r!furnl [oreType] [amount] - Sends large furnace info such as time to smelt, total wood required and charcoal produced

r!hours [steamID] - Returns the total number of hours a player has played Rust.

r!item [item] - Displays item info such as its RustLabs.com description, drop rates and crafting information. Example usage: r!item rocket

r!itemstore - Displays the current item store

r!refine [amount] - Sends small refinery info such as the time to refine, total wood required and charcoal produced

r!stats [steamID] - Displays statistics for the specified player such as K/D ratio, Headshot % and Weapon Accuracy

r!wipe - Displays the date of the next official wipe/force wipe
```

## Team Leader
```
r!createteam - Creates a team. You can invite members to your team with r!teaminvite.

r!setcoords [coords] - Sets the coordinates of your team's base.

r!teamalert [message] - Sends a message to each team member.

r!teaminvite [mention] - Invites a mentioned user to your team.

r!teamnotifications - Toggles a teams notifications on/off.

r!teamserver [search] -- Sets the server of the team. You can specify the IP address of the server or the server name. E.g r!server 127.0.0.1, r!server Generic-Server-Name
```

## Team
```
r!accept - Accepts a pending team invite.

r!decline - Declines a pending team invite.

r!leaveteam - Leaves the team you are currently in. If you own the team, it will be disbanded

r!members - Lists all the members in your current team.

r!notifications - Opts in/out of team notifications

r!raid - Notifies all members of the current team that a raid is in progress.

r!toggleinvites - Toggles on/off a users ability to be invited to a team.
```

## Fun
```
r!coinflip [opponent] - Coinflips a mentioned user.

r!randomname [input] - Randomly picks a name from a comma separated list. For example, r!randomname memes, are, cool
```

## Miscellaneous
```
r!botstats - Returns various bot statistics.

r!calculate [math] - Returns the value of the calculation specified

r!guildhelp - Sends help info useful for server owners

r!help - Sends help info

r!invite - Sends a link to invite the bot.

r!link - Links a users Discord and Steam

r!server [search] - Gets information about a specific server. You can specify the IP address of the server or the server name. E.g r!server 127.0.0.1, r!server Generic-Server-Name

r!servers - Displays the top 5 Rust servers.
```

## Server Specific
```
r!setchannel [channelMention] - Used by server owners/admins to set the bots channel. Mention a channel to set. To set back to default, run r!setchannel null

r!togglesearch - Used by server owners/admins to toggle on/off the ability to search for servers.
```
