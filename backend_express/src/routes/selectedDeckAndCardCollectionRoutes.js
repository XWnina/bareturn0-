const express = require("express");
const SaveFile = require("../models/SaveFile");
const CardDeck = require("../models/CardDeck");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");

const router = express.Router();

// Set selected deck
router.put("/:saveName/setSelectedDeck", authMiddleware, async (req, res) => {
  try {
    const { deckId } = req.body;

    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });
    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const deck = await CardDeck.findOne({
      _id: deckId,
      saveFileId: saveFile._id,
    });
    if (!deck) {
      const errorResponse = { error: "Deck not found or does not belong to save file" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    saveFile.selectedDeck = deckId;
    await saveFile.save();

    const response = { message: "Selected deck updated", saveFile };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Add card to cardCollection
router.post("/:saveName/addCardToCollection", authMiddleware, async (req, res) => {
  try {
    const { cardName, count } = req.body;

    const saveFile = await SaveFile.findOne({ userId: req.user.id, saveName: req.params.saveName });
    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const existingCard = saveFile.cardCollection.cards.find(c => c.cardName === cardName);
    if (existingCard) {
      existingCard.count += count;
    } else {
      saveFile.cardCollection.cards.push({ cardName, count });
    }

    await saveFile.save();
    const response = { message: "Card added to collection", cardCollection: saveFile.cardCollection };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Remove card from cardCollection
router.post("/:saveName/removeCardFromCollection", authMiddleware, async (req, res) => {
  try {
    const { cardName, count } = req.body;

    const saveFile = await SaveFile.findOne({ userId: req.user.id, saveName: req.params.saveName });
    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const card = saveFile.cardCollection.cards.find(c => c.cardName === cardName);
    if (!card || card.count < count) {
      const errorResponse = { error: "Not enough cards to remove" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    card.count -= count;
    if (card.count === 0) {
      saveFile.cardCollection.cards = saveFile.cardCollection.cards.filter(c => c.cardName !== cardName);
    }

    await saveFile.save();
    const response = { message: "Card removed from collection", cardCollection: saveFile.cardCollection };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

module.exports = router;
