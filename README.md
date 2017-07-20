# K8Gatheriino
![alt text](https://github.com/kitsun8/K8Gatheriino/blob/master/screenshots/gatheriino1.PNG)

A bot for Discord that helps people to form pickup games (PUGs).
Originally for Overwatch, but as the team size can be changed, works with any team based game.

# What it does

0. Reads settings from appsettings.json (sample found in 'settings' in this repo)
1. Gathers X amount of people to a queue
2. Asks players to Ready up inside Y amount of time.
3. Kicks unreadied players out of the queue.
4. Decides 2 random team captains.
5. Allows captains to pick X amount of players to teams, 1 by 1.
6. Announces teams when all players have been picked.

# Details

Bot is running on Discore 2.4.0 (https://github.com/BundledSticksInkorperated/Discore)

Project is running on .NET Core 1.1 / .NET Standard 1.6

Language support added for english, default/original language was finnish. Can be changed from appsettings.json
