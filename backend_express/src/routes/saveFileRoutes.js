const express = require("express");
const SaveFile = require("../models/SaveFile");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");

const router = express.Router();

// Get all save files
router.get("/", authMiddleware, async (req, res) => {
    try {
        const saves = await SaveFile.find({ userId: req.user.id });
        logRequestResponse(req, res, saves);
        res.json(saves);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

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

// Update a save file
router.put("/:saveName", authMiddleware, async (req, res) => {
    try {
        const save = await SaveFile.findOneAndUpdate(
            { userId: req.user.id, saveName: req.params.saveName },
            { $set: req.body },
            { new: true }
        );

        if (!save) {
            const errorResponse = { error: "Save file not found" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        logRequestResponse(req, res, save);
        res.json(save);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

// Get the save file for the current logged-in user
router.get("/me", authMiddleware, async (req, res) => {
    try {
        const user = await User.findById(req.user.id);
        if (!user) {
            return res.status(404).json({ error: "User not found" });
        }

        const saveFile = await SaveFile.findOne({ userId: user._id });
        if (!saveFile) {
            return res.status(404).json({ error: "Save file not found" });
        }

        res.json(saveFile);
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

module.exports = router;
