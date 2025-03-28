// routes/cardDeckRoutes.js
const express = require("express");
const CardDeck = require("../models/CardDeck");
const SaveFile = require("../models/SaveFile");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");

const router = express.Router();

// Create a new card deck
router.post("/create", authMiddleware, async (req, res) => {
  try {
    const { saveFileId, name } = req.body;
    const saveFile = await SaveFile.findOne({ _id: saveFileId, userId: req.user.id });
    if (!saveFile) {
      const errorResponse = { error: "Save file not found or unauthorized" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const deck = new CardDeck({ saveFileId, name, cards: [] });
    await deck.save();

    const response = { message: "Card deck created", deck };
    logRequestResponse(req, res, response);
    res.status(201).json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Add card(s) to a deck
router.post("/:deckId/addCard", authMiddleware, async (req, res) => {
  try {
    const { cardName, count } = req.body;
    const deck = await CardDeck.findById(req.params.deckId);
    if (!deck) {
      const errorResponse = { error: "Deck not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const saveFile = await SaveFile.findOne({ _id: deck.saveFileId, userId: req.user.id });
    if (!saveFile) {
      const errorResponse = { error: "Unauthorized" };
      logRequestResponse(req, res, errorResponse);
      return res.status(403).json(errorResponse);
    }

    const existing = deck.cards.find(c => c.cardName === cardName);
    if (existing) existing.count += count;
    else deck.cards.push({ cardName, count });

    await deck.save();
    const response = { message: "Card added", deck };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get all decks for a save file
router.get("/save/:saveFileId", authMiddleware, async (req, res) => {
  try {
    const saveFile = await SaveFile.findOne({ _id: req.params.saveFileId, userId: req.user.id });
    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const decks = await CardDeck.find({ saveFileId: req.params.saveFileId });
    const response = { decks };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get a specific deck by ID
router.get("/:deckId", authMiddleware, async (req, res) => {
  try {
    const deck = await CardDeck.findById(req.params.deckId);
    if (!deck) {
      const errorResponse = { error: "Deck not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const saveFile = await SaveFile.findOne({ _id: deck.saveFileId, userId: req.user.id });
    if (!saveFile) {
      const errorResponse = { error: "Unauthorized" };
      logRequestResponse(req, res, errorResponse);
      return res.status(403).json(errorResponse);
    }

    const cardNameCountPairs = deck.cards.map(card => [card.cardName, card.count]);
    const response = { deck, cardNameCountPairs };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Set selected deck on save file
router.put("/select", authMiddleware, async (req, res) => {
  try {
    const { saveFileId, deckId } = req.body;

    const saveFile = await SaveFile.findOneAndUpdate(
      { _id: saveFileId, userId: req.user.id },
      { selectedDeck: deckId },
      { new: true }
    );

    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const response = { message: "Selected deck set", saveFile };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Delete a card deck
router.delete("/:deckId", authMiddleware, async (req, res) => {
  try {
    const deck = await CardDeck.findById(req.params.deckId);
    if (!deck) {
      const errorResponse = { error: "Deck not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const saveFile = await SaveFile.findOne({ _id: deck.saveFileId, userId: req.user.id });
    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(403).json(errorResponse);
    }

    await deck.deleteOne();
    const response = { message: "Deck deleted" };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

module.exports = router;
