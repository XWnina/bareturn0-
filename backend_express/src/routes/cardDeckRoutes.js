const express = require("express");
const CardDeck = require("../models/CardDeck");
const Card = require("../models/Card");
const SaveFile = require("../models/SaveFile");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");

const router = express.Router();

// Create a new deck for a save file
router.post("/", authMiddleware, async (req, res) => {
    try {
        const { saveFileId, name } = req.body;

        const saveFile = await SaveFile.findOne({ _id: saveFileId, userId: req.user.id });
        if (!saveFile) {
            const errorResponse = { error: "Save file not found or not authorized" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        const deck = new CardDeck({ saveFileId, name, cards: [] });
        await deck.save();

        const successResponse = { message: "Card deck created", deck };
        logRequestResponse(req, res, successResponse);
        res.status(201).json(successResponse);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

// Add card(s) to a deck
router.post("/:deckId/addCard", authMiddleware, async (req, res) => {
    try {
        const { cardId, count } = req.body;
        const deck = await CardDeck.findById(req.params.deckId);

        if (!deck) {
            const errorResponse = { error: "Deck not found" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        const saveFile = await SaveFile.findOne({ _id: deck.saveFileId, userId: req.user.id });
        if (!saveFile) {
            const errorResponse = { error: "Not authorized to modify this deck" };
            logRequestResponse(req, res, errorResponse);
            return res.status(403).json(errorResponse);
        }

        const existing = deck.cards.find(c => c.card.toString() === cardId);
        if (existing) {
            existing.count += count;
        } else {
            deck.cards.push({ card: cardId, count });
        }

        await deck.save();

        const successResponse = { message: "Card added to deck", deck };
        logRequestResponse(req, res, successResponse);
        res.json(successResponse);
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
            const errorResponse = { error: "Save file not found or not authorized" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        const decks = await CardDeck.find({ saveFileId: saveFile._id }).populate("cards.card");

        const successResponse = { decks };
        logRequestResponse(req, res, successResponse);
        res.json(successResponse);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

module.exports = router;
