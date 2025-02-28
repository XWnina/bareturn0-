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
        const saveFile = await SaveFile.findOne({
            userId: req.user.id,
            saveName: req.params.saveName
        }).populate({
            path: "unlockedAchievements.achievementId", 
            select: "name method" // ✅ Ensure `name` and `method` (description) are included
        });

        if (!saveFile) {
            return res.status(404).json({ error: "Save file not found" });
        }

        // Return unlocked achievements with `name`, `method`, and `achievedDate`
        const formattedUnlocked = saveFile.unlockedAchievements.map(unlock => ({
            achievementName: unlock.achievementId.name,
            description: unlock.achievementId.method,  // ✅ Now includes method (condition)
            achievedDate: unlock.achievedDate
        }));

        res.json(formattedUnlocked);
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});


router.get("/:saveName/locked", authMiddleware, async (req, res) => {
    try {
        // Find the save file for the user
        const saveFile = await SaveFile.findOne({ userId: req.user.id, saveName: req.params.saveName });

        if (!saveFile) {
            const errorResponse = { error: "Save file not found" };
            logRequestResponse(req, res, errorResponse);
            return res.status(404).json(errorResponse);
        }

        // Get all predefined achievements
        const allAchievements = await Achievement.find({}, "name method");

        // Get unlocked achievement IDs from the save file
        const unlockedIds = saveFile.unlockedAchievements.map(a => a.achievementId.toString());

        // Filter out locked achievements
        const lockedAchievements = allAchievements.filter(a => !unlockedIds.includes(a._id.toString()));

        logRequestResponse(req, res, lockedAchievements);
        res.json(lockedAchievements);
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
