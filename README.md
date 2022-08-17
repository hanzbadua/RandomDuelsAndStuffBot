# League Themed RPG Bot

Archival note: no longer being worked on, will remain unfinished - making an RPG in Discord by yourself is hard

----

Player data is saved/loaded in a json file called `playerData.json` adjacent to the executable

The bot token is loaded from a file called `token.txt` adjacent to the executable, of course the client won't work if there's no token

Item/skill data is all saved in the `LeagueThemedRPGBot.Refs.*` namespace(s) - all items and skills are derived from the abstract classes in `LeagueThemedRPGBot.Items` and `LeagueThemedRPGBot.Skills` namespaces if you want examples

To add new items and skills, create a class extending the appropriate abstract class with proper override, and register a new id for your item/skill within the ItemId/SkillId enums to use with the custom `RegisterSkill` and `RegisterItem` attributes. Then apply the appropriate atrribute to your new class with your new id that you added. 

IMPORTANT: make sure the 'None' enums are ALWAYS 0, and if making a new Item, making sure your new id is within the appropriate bounds depending on your new item type (see comments in ItemId regarding register bounds per type)
