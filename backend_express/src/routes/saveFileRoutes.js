const express = require("express");
const SaveFile = require("../models/SaveFile");
const Achievement = require("../models/Achievement");
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
    const saveFileCount = await SaveFile.countDocuments({
      userId: req.user.id,
    });

    let achievementUnlocked = null;

    if (saveFileCount === 1) {
      const freshmanAchievement = await Achievement.findOne({
        name: "Freshman",
      });

      if (freshmanAchievement) {
        newSave.unlockedAchievements.push({
          achievementId: freshmanAchievement._id,
          achievedDate: new Date(),
        });

        await newSave.save();

        achievementUnlocked = {
          message: "Freshman achievement unlocked!",
          achievement: {
            name: freshmanAchievement.name,
            method: freshmanAchievement.method,
            achievedDate: new Date(),
          },
        };
      }
    }

    const successResponse = {
      message: "Save file created successfully",
      saveFile: newSave,
      ...(achievementUnlocked && { achievementUnlocked }),
    };

    logRequestResponse(req, res, successResponse);
    res.status(201).json(successResponse);
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
      const errorResponse = { error: "Player name is required" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const save = await SaveFile.findOneAndUpdate(
      { userId: req.user.id, saveName: req.params.saveName },
      { $set: { playerName } },
      { new: true }
    );

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = {
      message: "Player name updated successfully",
      save,
    };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Update progress
router.put("/:saveName/updateProgress", authMiddleware, async (req, res) => {
  try {
    const { progress } = req.body;
    if (progress === undefined) {
      const errorResponse = { error: "Progress is required" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const save = await SaveFile.findOneAndUpdate(
      { userId: req.user.id, saveName: req.params.saveName },
      { $set: { progress } },
      { new: true }
    );

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = { message: "Progress updated successfully", save };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Update coins
router.put("/:saveName/updateCoins", authMiddleware, async (req, res) => {
  try {
    const { coins } = req.body;
    if (coins === undefined) {
      const errorResponse = { error: "Coins are required" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const save = await SaveFile.findOneAndUpdate(
      { userId: req.user.id, saveName: req.params.saveName },
      { $set: { coins } },
      { new: true }
    );

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = { message: "Coins updated successfully", save };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get all save files for the current user
router.get("/me", authMiddleware, async (req, res) => {
  try {
    const saveFiles = await SaveFile.find({ userId: req.user.id });

    logRequestResponse(req, res, { saveFiles });
    res.json(saveFiles);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get progress for a specific save file
router.get("/:saveName/progress", authMiddleware, async (req, res) => {
  try {
    const save = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = { progress: save.progress };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get coins for a specific save file
router.get("/:saveName/coins", authMiddleware, async (req, res) => {
  try {
    const save = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = { coins: save.coins };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get player name for a specific save file
router.get("/:saveName/playerName", authMiddleware, async (req, res) => {
  try {
    const save = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = { playerName: save.playerName };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

module.exports = router;

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
// Get maxHealth for a specific save file
router.get("/:saveName/maxHealth", authMiddleware, async (req, res) => {
  try {
    const save = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = { maxHealth: save.maxHealth };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Update maxHealth
router.put("/:saveName/updateMaxHealth", authMiddleware, async (req, res) => {
  try {
    const { maxHealth } = req.body;
    if (maxHealth === undefined) {
      const errorResponse = { error: "maxHealth is required" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const save = await SaveFile.findOneAndUpdate(
      { userId: req.user.id, saveName: req.params.saveName },
      { $set: { maxHealth } },
      { new: true }
    );

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = { message: "maxHealth updated successfully", save };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});
// Get speed for a specific save file
router.get("/:saveName/speed", authMiddleware, async (req, res) => {
  try {
    const save = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = { speed: save.speed };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Update speed
router.put("/:saveName/updateSpeed", authMiddleware, async (req, res) => {
  try {
    const { speed } = req.body;
    if (speed === undefined) {
      const errorResponse = { error: "speed is required" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const save = await SaveFile.findOneAndUpdate(
      { userId: req.user.id, saveName: req.params.saveName },
      { $set: { speed } },
      { new: true }
    );

    if (!save) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = { message: "speed updated successfully", save };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

module.exports = router;
