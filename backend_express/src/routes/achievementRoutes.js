const express = require("express");
const Achievement = require("../models/Achievement");
const SaveFile = require("../models/SaveFile");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");

const router = express.Router();

// Get all predefined achievements
router.get("/all", async (req, res) => {
    try {
        const achievements = await Achievement.find({});
        logRequestResponse(req, res, achievements);
        res.json(achievements);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

// Get unlocked achievements for a save file
router.get("/:saveName/unlocked", authMiddleware, async (req, res) => {
    try {
        const saveFile = await SaveFile.findOne({ userId: req.user.id, saveName: req.params.saveName })
            .populate("unlockedAchievements.achievementId");

        if (!saveFile) {
            const errorResponse = { error: "Save file not found" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        logRequestResponse(req, res, saveFile.unlockedAchievements);
        res.json(saveFile.unlockedAchievements);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});

router.put("/:saveName/unlock", authMiddleware, async (req, res) => {
    try {
        const { achievementName } = req.body;

        // Find the achievement by name
        const achievement = await Achievement.findOne({ name: achievementName });

        if (!achievement) {
            const errorResponse = { error: "Achievement not found" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        // Find the save file
        const saveFile = await SaveFile.findOne({ userId: req.user.id, saveName: req.params.saveName });

        if (!saveFile) {
            const errorResponse = { error: "Save file not found" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        // Check if the achievement is already unlocked in the save file
        if (saveFile.unlockedAchievements.some(a => a.achievementId.equals(achievement._id))) {
            const errorResponse = { error: "Achievement already unlocked" };
            logRequestResponse(req, res, errorResponse);
            return res.status(400).json(errorResponse);
        }

        // Store the unlock time
        const unlockTime = new Date();

        // Add the achievement to the save file's unlocked list
        saveFile.unlockedAchievements.push({
            achievementId: achievement._id,
            achievedDate: unlockTime
        });

        await saveFile.save();

        // ✅ Update the Achievement record to mark it as unlocked
        achievement.unlocked = true;
        achievement.achievedDate = unlockTime;
        await achievement.save();

        const successResponse = { 
            message: "Achievement unlocked", 
            unlockedAchievement: {
                _id: achievement._id,
                name: achievement.name,
                method: achievement.method,
                unlocked: achievement.unlocked,
                achievedDate: achievement.achievedDate
            }
        };

        logRequestResponse(req, res, successResponse);
        res.json(successResponse);
    } catch (err) {
        logRequestResponse(req, res, { error: err.message });
        res.status(500).json({ error: err.message });
    }
});


module.exports = router;
