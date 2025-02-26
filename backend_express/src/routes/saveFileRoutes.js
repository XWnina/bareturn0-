const express = require("express");
const SaveFile = require("../models/SaveFile");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");
const User = require("../models/User");


const router = express.Router();

// Create a new save file
router.post("/", authMiddleware, async (req, res) => {
    try {
        const { saveName, progress, coins } = req.body;

        const existingSave = await SaveFile.findOne({
            userId: req.user.id,
            saveName,
        });

        if (existingSave) {
            const errorResponse = {
                error: "Save file with this name already exists",
            };
            logRequestResponse(req, res, errorResponse);
            return res.status(400).json(errorResponse);
        }

        const newSave = new SaveFile({
            saveName,
            progress,
            coins,
            userId: req.user.id,
        });

        await newSave.save();

        const successResponse = { message: "Save file created successfully" };
        logRequestResponse(req, res, successResponse, newSave);
        res.status(201).json(newSave);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

// Update playerName
router.put("/:saveName/updatePlayerName", authMiddleware, async (req, res) => {
    try {
        const { playerName } = req.body;
        if (!playerName) {
            return res.status(400).json({ error: "Player name is required" });
        }

        const save = await SaveFile.findOneAndUpdate(
            { userId: req.user.id, saveName: req.params.saveName },
            { $set: { playerName } },
            { new: true }
        );

        if (!save) {
            return res.status(404).json({ error: "Save file not found" });
        }

        res.json({ message: "Player name updated successfully", save });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// Update progress
router.put("/:saveName/updateProgress", authMiddleware, async (req, res) => {
    try {
        const { progress } = req.body;
        if (progress === undefined) {
            return res.status(400).json({ error: "Progress is required" });
        }

        const save = await SaveFile.findOneAndUpdate(
            { userId: req.user.id, saveName: req.params.saveName },
            { $set: { progress } },
            { new: true }
        );

        if (!save) {
            return res.status(404).json({ error: "Save file not found" });
        }

        res.json({ message: "Progress updated successfully", save });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// Update coins
router.put("/:saveName/updateCoins", authMiddleware, async (req, res) => {
    try {
        const { coins } = req.body;
        if (coins === undefined) {
            return res.status(400).json({ error: "Coins are required" });
        }

        const save = await SaveFile.findOneAndUpdate(
            { userId: req.user.id, saveName: req.params.saveName },
            { $set: { coins } },
            { new: true }
        );

        if (!save) {
            return res.status(404).json({ error: "Save file not found" });
        }

        res.json({ message: "Coins updated successfully", save });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// Get all save files for the current logged-in user
router.get("/me", authMiddleware, async (req, res) => {
    try {
        const user = await User.findById(req.user.id);
        if (!user) {
            return res.status(404).json({ error: "User not found" });
        }

        const saveFiles = await SaveFile.find({ userId: user._id });
        if (!saveFiles || saveFiles.length === 0) {
            return res.status(404).json({ error: "No save files found" });
        }

        res.json(saveFiles);
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// Get progress for a specific save file
router.get("/:saveName/progress", authMiddleware, async (req, res) => {
    try {
        const saveFile = await SaveFile.findOne({
            userId: req.user.id,
            saveName: req.params.saveName,
        });

        if (!saveFile) {
            return res.status(404).json({ error: "Save file not found" });
        }

        res.json({ progress: saveFile.progress });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// Get coins for a specific save file
router.get("/:saveName/coins", authMiddleware, async (req, res) => {
    try {
        const saveFile = await SaveFile.findOne({
            userId: req.user.id,
            saveName: req.params.saveName,
        });

        if (!saveFile) {
            return res.status(404).json({ error: "Save file not found" });
        }

        res.json({ coins: saveFile.coins });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// Get playerName for a specific save file
router.get("/:saveName/playerName", authMiddleware, async (req, res) => {
    try {
        const saveFile = await SaveFile.findOne({
            userId: req.user.id,
            saveName: req.params.saveName,
        });

        if (!saveFile) {
            return res.status(404).json({ error: "Save file not found" });
        }

        res.json({ playerName: saveFile.playerName });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});


// Delete a save file
router.delete("/:saveName", authMiddleware, async (req, res) => {
    try {
        const save = await SaveFile.findOneAndDelete({
            userId: req.user.id,
            saveName: req.params.saveName,
        });

        if (!save) {
            const errorResponse = { error: "Save file not found" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        const successResponse = { message: "Save deleted successfully" };
        logRequestResponse(req, res, successResponse);
        res.json(successResponse);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

module.exports = router;
