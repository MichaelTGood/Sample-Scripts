# Azure Functions

**Azure Functions** are a collection of scripts that can be used as a "*serverless*" server. They can be called directly, or more simply through PlayFab, which can verify the caller's authentication, as well as pass that information on to the Azure Function.

This sample of Functions contains:

#### CreateGroup.cs

CreateGroup is a hefty method that handles everything for when a player creates a group for their event. It will necessary, from verifying that the player can or should create a group, to actually creating it, to finally adding the player as well as sharable codes for other players to use and join.

#### FetchGameCustomData.cs

This function pulls all of the player's purchased events, as well as all relevent data (ie: other members of the group, displayname, UIDs), and packages it up conviently for the receiving client to store and handle.

#### RedeemMember.cs

The complementary method to CreateGroup, this will take and decypher a shared code, and then validate it against the one stored in the database. Upon verification, it will join the player to the group, and provide them with necessary invetory item and group data.

#### VerifyData.cs

This will check the player's local data file for their current event, and verify that it is the most up to date version when they launch the game. If it is not, it will return required keys needed to access the event data.

