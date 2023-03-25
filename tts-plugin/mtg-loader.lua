----#include !/CardGames/Load/Common
function createCard(count, name, cardFace, cardBack, player, deckIndex)
    local customCardData = {
      face = cardFace,
      back = cardBack
    }
  
    local playerSeat = player.getHandTransform()
    local spawnData = {
      type = "CardCustom",
      position = playerSeat.position + (playerSeat.forward * 5) + (playerSeat.right * 3 * deckIndex),
      rotation = vector(playerSeat.rotation.x, (playerSeat.rotation.y + 180) % 360, playerSeat.rotation.z)
    }
  
    for i=1, count do
      local newCard = spawnObject(spawnData)
      newCard.setName(name)
      newCard.setCustomObject(customCardData)
    end
  end
  
  ----#include !/CardGames/Load/Common
  ----#include !/CardGames/Load/MTG
  ----#include !/Core/StringUtils
  StringUtils = {
    trim = function(s) return (string.gsub(s, "^%s*(.-)%s*$", "%1")) end
  }
  
  ----#include !/Core/StringUtils
  
  magicBack = "http://cloud-3.steamusercontent.com/ugc/1044218919659567154/72AEBC61B3958199DE4389B0A934D68CE53D030B/"
  
  function readContent(content, player)
    local parsedContent = parseContent(content[1], player)
    fetchCardData(1, parsedContent, {}, player)
  end
  
  function parseContent(content, player)
    printToColor("Parsing deck list", player.color)
    local parsedData = {}
    local i = 1
  
    for line in string.gmatch(content, '[^\r\n]+') do
      local toParse = StringUtils.trim(line)
      local cardInfo = {}
  
      local startSet
      local endSet
  
      -- count
      local nonDigit,_ = string.find(toParse, "%D")
      -- If nonDigit < 2, the first character is not a digit. Bad format
      -- If nonDigit is nil then it's not a good line either
      if not nonDigit or nonDigit < 2 then
        goto continue
      end
      cardInfo["count"] = tonumber(string.sub(toParse, 1, nonDigit-1))
      toParse = StringUtils.trim(string.sub(toParse, nonDigit))
  
      -- set
      startSet, _ = string.find(toParse, "%(")
      endSet, _ = string.find(toParse, "%)")
      -- If first and last exist, we assume there is a set
      if startSet and endSet then
        local setBlock = string.sub(toParse, startSet + 1, endSet - 1)
        toParse = StringUtils.trim(string.sub(toParse, 1, startSet - 1))
  
        local setSplit
        setSplit, _ = string.find(setBlock, "%#")
        -- If setSplit exist, there is a collector's number
        if setSplit then
          cardInfo["set"] = string.lower(string.sub(setBlock, 1, setSplit - 1))
          collectorBlock = string.sub(setBlock, setSplit + 1)
  
          local langSplit
          langSplit, _ = string.find(collectorBlock, "%#")
          -- If langSplit exist, there is a language code
          if langSplit then
            cardInfo["collector"] = string.sub(collectorBlock, 1, langSplit - 1)
            cardInfo["lang"] = string.lower(string.sub(collectorBlock, langSplit + 1))
          else
            cardInfo["collector"] = collectorBlock
          end
        else
          cardInfo["set"] = string.lower(setBlock)
        end
      end
  
      -- card name
      cardInfo["name"] = toParse
  
      -- add to list
      parsedData[i] = cardInfo
      i = i + 1
  
      ::continue::
    end
    return parsedData
  end
  
  function fetchCardData(index, parsedCards, relatedCards, player)
    printToColor("Creating card " .. index .. "/" .. #parsedCards, player.color)
    local url = "https://api.scryfall.com/cards/"
    if parsedCards[index]["set"] and parsedCards[index]["collector"] then
      -- Use set and number to find
      url = url .. parsedCards[index]["set"] .."/" .. parsedCards[index]["collector"]
      if parsedCards[index]["lang"] then
        url = url .. "/" .. parsedCards[index]["lang"]
      end
    else
      -- Use name and maybe set to find
      url = url .. "named?exact=" .. parsedCards[index]["name"]
      if parsedCards[index]["set"] then
        url = url .. "&set=" .. parsedCards[index]["set"]
      end
    end
  
    WebRequest.get(url, function(data) cardDataReceived(data, index, parsedCards, relatedCards, player) end)
  end
  
  function cardDataReceived(data, index, parsedCards, relatedCards, player)
    local cardData = JSON.decode(data.text)
  
    -- Handle timeout error
    if data.is_error then
      if cardData["status"] == 429 then
        printToColor("Too many request, retrying in one second", player.color, "Red")
        Wait.time(function() fetchCardData(index, parsedCards, relatedCards, player) end, 1)
        return
      else
        local error = "Unhandled error: " .. parsedCards[index]["name"]
        if (parsedCards[index]["set"]) then
          error = error .. " (" .. parsedCards[index]["set"] .. ")"
        end
        printToColor(error, player.color, "Red")
      end
    -- Handle card not found
    elseif cardData["status"] == 404 then
      local error = "Card Not Found: " .. parsedCards[index]["name"]
      if (parsedCards[index]["set"]) then
        error = error .. " (" .. parsedCards[index]["set"] .. ")"
      end
      if (parsedCards[index]["lang"]) then
        error = error .. " [" .. parsedCards[index]["lang"] .. "]"
      end
      printToColor(error, player.color, "Red")
    -- Handle card found
    else
      if cardData["all_parts"] then
        for _,part in ipairs(cardData["all_parts"]) do
          if part["component"] == "token" or part["component"] == "meld_result" then
            relatedCards[part["id"]] = true
          end
          if part["component"] == "combo_piece" and string.find(part["type_line"], "Emblem", 1, true) then
            relatedCards[part["id"]] = true
          end
        end
      end
  
      local cardFront
      if cardData["layout"] == "transform" or cardData["layout"] == "modal_dfc" or cardData["layout"] == "double_faced_token" then
        cardFront = cardData["card_faces"][1]["image_uris"]["normal"]
        relatedCards[cardData["id"]] = true
      else
        cardFront = cardData["image_uris"]["normal"]
      end
  
      createCard(parsedCards[index]["count"], cardData["name"], cardFront, magicBack, player, 0)
    end
  
    if index >= #parsedCards then
      local relatedList = {}
      local i = 1
      for related,_ in pairs(relatedCards) do
        relatedList[i] = related
        i = i + 1
      end
      if #relatedList > 0 then
        Wait.time(function() fetchRelatedCardData(1, relatedList, player) end, 0.1)
      end
      return
    end
  
    Wait.time(function() fetchCardData(index+1, parsedCards, relatedCards, player) end, 0.1)
  end
  
  function fetchRelatedCardData(index, relatedCards, player)
    printToColor("Creating related card " .. index .. "/" .. #relatedCards, player.color)
    local url = "https://api.scryfall.com/cards/" .. relatedCards[index]
    WebRequest.get(url, function(data) relatedCardDataReceived(data, index, relatedCards, player) end)
  end
  
  function relatedCardDataReceived(data, index, relatedCards, player)
    local cardData = JSON.decode(data.text)
  
    if data.is_error then
      if cardData["status"] == 429 then
        printToColor("Too many request, retrying in one second", player.color, "Red")
        Wait.time(function() fetchRelatedCardData(index, relatedCards, player) end, 1)
        return
      end
    else
      if cardData["layout"] == "transform" or cardData["layout"] == "double_faced_token" or cardData["layout"] == "modal_dfc" then
        createCard(1, cardData["card_faces"][2]["name"], cardData["card_faces"][2]["image_uris"]["normal"], magicBack, player, 1)
        if cardData["layout"] == "double_faced_token" then
          createCard(1, cardData["card_faces"][1]["name"], cardData["card_faces"][1]["image_uris"]["normal"], magicBack, player, 1)
        end
      else
        createCard(1, cardData["name"], cardData["image_uris"]["normal"], magicBack, player, 1)
      end
    end
  
    if index >= #relatedCards then
      return
    end
  
    Wait.time(function() fetchRelatedCardData(index+1, relatedCards, player) end, 0.1)
  end
  
  ----#include !/CardGames/Load/MTG
  ----#include !/CardGames/Load/UIConstructed
  -- UI element names
  function getWindowName(player)
    return "load_" .. player.color
  end
  
  function getInputName(player)
    return "input_" .. player.color
  end
  
  ----#include !/CardGames/Load/UIConstructed
  ----#include !/CardGames/Load/UI
  loadInputCount = 1
  
  -- UI callbacks
  function onOpenLoadDialog(player)
    UI.show(getWindowName(player))
  end
  
  function onCancelLoad(player)
    UI.hide(getWindowName(player))
  end
  
  function onLoadDeck(player)
    UI.hide(getWindowName(player))
  
    local content = {}
    for i=1, loadInputCount do
      content[i] = UI.getAttribute(getInputName(player) .. i, "text")
    end
    readContent(content, player)
  end
  
  function onEndEditDeck(_, text, input)
    UI.setAttribute(input, "text", text)
  end
  
  ----#include !/CardGames/Load/UI