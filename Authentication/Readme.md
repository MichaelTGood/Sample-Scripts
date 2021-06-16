# Authentication 

These scripts handle a lot of the player authentication for my current project. 

#### Auth.cs 

Contains the static fields that store the player's profile data, as well their team's data. This class also handles the logic for all calls pertaining to authentication, online data or profile updates.

#### Azure.cs

This is a static class with various static methods for communicating with the projects backend server in Microsoft Azure. It organizes and simplifies server calls for the other classes handling logic.

#### AzureModels.cs

A collection of data types used for handling data to and from the server.

