# Crousti-Tabletop
This tabletop-simulator plugin allows you to play with your friends on the same instance of the game while having a hidden hand.  It achieves that by using a local server and a web app on which the clients will see their hands.

## Setup

### Run the local server

    cd server/CroustiAPI
    dotnet watch run

**Note**: *You might want to edit the appsettings.json file*

### Load the plugin in tabletop-simulator

Go in the steam workshop and search for crousti-tabletop


## Local dev

### Run the client

    cd client/crousti-client
    npm install
    npm run  dev  --  --host

### Run the server

    cd server/CroustiAPI
    dotnet watch run

### Run the plugin
You will need to install the vscode or atom plugin to load the files in the game.  So, when tabletop-simulator is loaded, in vscode or atom, do a Get Lua Scripts, add you changes and do a Save and Play.