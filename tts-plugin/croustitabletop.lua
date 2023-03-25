--require("vscode/console")
--require("croustitabletop")

local serverUrl = "http://localhost"
local headers = {
    ["Content-Type"] = "application/json",
    Accept = "application/json",
}

function getPlayersCustomCards(player)

    if player.getHandCount() == 0 then
        return {}
    end

    local cards = {}
    for _, object in ipairs(player.getHandObjects(1)) do
        if object.name == "CardCustom" then
            cards[#cards+1] = object
        end
    end
    
    return cards
end

function getPlayersCardsAsJson()

    local players = { }

    for _,player in ipairs(Player.getPlayers()) do

        local playersCards = {}

        for _, handObj in ipairs(getPlayersCustomCards(player)) do
            local playersCard = { guid = handObj.guid, imageUrl = handObj.getCustomObject()["face"] }
            playersCards[#playersCards+1] = playersCard
        end

        local playerAsJson = { color = player.color, cards = playersCards }
        players[#players+1] = playerAsJson

    end

    return JSON.encode_pretty({ players = players })
end

function standardizeColor(color)
    return color:sub(1,1):upper()..color:sub(2):lower()
end

function updateGameState()
    
    local endpoint = "/gamestate"
    local requestBody = getPlayersCardsAsJson()

    WebRequest.custom(serverUrl .. endpoint, "PUT", true, requestBody, headers, function(response) 
        if response.is_error then
            log("Request failed: " .. response.error)
            return
        end
    end)

end

function onLoad()

    Timer.create({
        identifier = "GameState Updater",
        function_name = "updateGameState",
        delay = 1, 
        repetitions = 0, -- infinite repititions
    })

    createQrCodes()

    highlightCards()
end

function createQrCode(player)

    local imageUrl = serverUrl .. "/qrcodes/" .. player.color .. "?" .. Time.time

    local customTileData = {
      image = imageUrl,
    }
  
    local playerSeat = player.getHandTransform()
    local spawnData = {
      type = "Custom_Tile",
      position = playerSeat.position + (playerSeat.forward * 5) + (playerSeat.right * 3),
      rotation = vector(playerSeat.rotation.x, (playerSeat.rotation.y + 180) % 360, playerSeat.rotation.z),
      scale = { 2, 2, 2 }
    }
  
    local newTile = spawnObject(spawnData)
    newTile.setCustomObject(customTileData)
end

function createQrCodes()
    for _,player in ipairs(Player.getPlayers()) do
        createQrCode(player)
    end 
end

function onObjectEnterZone(zone, object)
    updateGameState()
end

function onObjectLeaveZone(zone, object)
    object.highlightOff()
    updateGameState()
end

function highlightCards()
    local endpoint = "/gamestate/cardsToHighlight"

    WebRequest.get(serverUrl .. endpoint, function(response)
        if response.is_error then
            log("Request failed: " .. response.error)
            return
        end

        local jsonResponse = JSON.decode(response.text)

        for _, cardToHighlight in ipairs(jsonResponse) do
            local cardObject = getObjectFromGUID(cardToHighlight["guid"])
            local color = stringColorToRGB(standardizeColor(cardToHighlight["color"]))
            local highlightOn = cardToHighlight["on"]

            if highlightOn then
                cardObject.highlightOn(color)
            else
                cardObject.highlightOff()
            end
        end
    end)

    Wait.time(highlightCards, 0.1)
end