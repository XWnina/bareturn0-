const express = require("express");
const Card = require("../models/Card");
const { logRequestResponse } = require("../utils/logger");

const router = express.Router();

// Create a new card (for admin/dev use)
router.post("/", async (req, res) => {
    try {
        const card = new Card(req.body);
        await card.save();

        const successResponse = { message: "Card created", card };
        logRequestResponse(req, res, successResponse);
        res.status(201).json(successResponse);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

// Get all cards
router.get("/", async (req, res) => {
    try {
        const cards = await Card.find();
        logRequestResponse(req, res, { cards });
        res.json(cards);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

// Get a single card by ID
router.get("/:id", async (req, res) => {
    try {
        const card = await Card.findById(req.params.id);

        if (!card) {
            const errorResponse = { error: "Card not found" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        logRequestResponse(req, res, { card });
        res.json(card);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

module.exports = router;
