const express = require("express");
const SaveFile = require("../models/SaveFile");
const Achievement = require("../models/Achievement");
const CardDeck = require("../models/CardDeck");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");
const User = require("../models/User");

const router = express.Router();

// Create a new save file
router.post("/", authMiddleware, async (req, res) => {
  try {
    const { saveName } = req.body;

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

    // Set default values for the card collection
    const initialCardPool = [
      { cardName: "HeavyAttack", count: 2 },
      { cardName: "QuickRecharge", count: 2 },
      { cardName: "Shield", count: 2 },
      { cardName: "Slash", count: 2 },
      { cardName: "SmallShield", count: 2 },
      { cardName: "CheatAttack", count: 2 },
    ];
    const collectionCards = initialCardPool.map((card) => ({ ...card }));
    const deckCards = initialCardPool.map((card) => ({ ...card }));

    // Set default values for the materials
    const initialMaterials = [
      { name: "if", count: 1 },
      { name: "while", count: 1 },
      { name: "math", count: 1 },
      { name: "BlankCard", count: 10 },
    ];

    const newSave = new SaveFile({
      saveName,
      cardCollection: {
        name: "Card Collection",
        cards: collectionCards,
      },
      materials: initialMaterials,
      userId: req.user.id,
    });

    // Create CardDeck copy of cardCollection and link it as selectedDeck
    const defaultSelectDeck = new CardDeck({
      saveFileId: newSave._id,
      name: "Default Deck",
      cards: deckCards,
    });
    await defaultSelectDeck.save();

    newSave.selectedDeck = defaultSelectDeck._id;

    await newSave.save();
    const saveFileCount = await SaveFile.countDocuments({
      userId: req.user.id,
    });
    console.log("Save file count after save:", saveFileCount);

    let achievementUnlocked = null;

    if (saveFileCount === 1) {
      const firstStep = await Achievement.findOne({ name: "First Step" });

      if (firstStep) {
        const unlockTime = new Date();

        newSave.unlockedAchievements.push({
          name: firstStep.name,
          achievedDate: unlockTime,
        });

        await newSave.save();

        achievementUnlocked = {
          message: "First Step achievement unlocked!",
          achievement: {
            name: firstStep.name,
            method: firstStep.method,
            achievedDate: unlockTime,
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

/* Getters */
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

// Get save file ID by saveName
router.get("/:saveName/id", authMiddleware, async (req, res) => {
  try {
    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const response = { saveFileId: saveFile._id };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
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

// Get selected deck content for a save file
router.get("/:saveName/selectedDeck", authMiddleware, async (req, res) => {
  try {
    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });
    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const selectedDeck = await CardDeck.findById(saveFile.selectedDeck);
    if (!selectedDeck) {
      const errorResponse = { error: "Selected deck not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const response = { selectedDeck };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get card collection content for a save file
router.get("/:saveName/cardCollection", authMiddleware, async (req, res) => {
  try {
    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });
    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const response = { cardCollection: saveFile.cardCollection };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get full savefile object by saveName
router.get("/:saveName", authMiddleware, async (req, res) => {
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

    const response = {
      saveName: save.saveName,
      playerName: save.playerName,
      progress: save.progress,
      coins: save.coins,
      maxHealth: save.maxHealth,
      speed: save.speed,
      createdAt: save.createdAt,
    };

    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
  }
});

/* Setters */
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
router.get("/:saveName/selectedDeckName", authMiddleware, async (req, res) => {
  try {
    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const selectedDeck = await CardDeck.findById(saveFile.selectedDeck);
    const selectedDeckName = selectedDeck?.name ?? "NULL";

    const response = { selectedDeckName };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
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

router.get("/:saveName/profileInfo", authMiddleware, async (req, res) => {
  try {
    const user = await User.findById(req.user.id);
    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!user || !saveFile) {
      const errorResponse = { error: "User or save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const selectedDeck = await CardDeck.findById(saveFile.selectedDeck);

    const message = `CurrentUserName:${user.username}\nCurrentSelectedDeck:${
      selectedDeck?.name ?? "NULL"
    }`;
    const successResponse = { profileInfo: message };

    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    const errorResponse = { error: err.message };
    logRequestResponse(req, res, errorResponse);
    res.status(500).json(errorResponse);
  }
});

router.get("/:saveName/minigamesStatus", authMiddleware, async (req, res) => {
  try {
    const saveFile = await SaveFile.findOne({
      saveName: req.params.saveName,
      userId: req.user.id,
    });

    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const rawStatus = saveFile.minigameStatus || ["", "", ""];
    let statusString = "";

    for (let i = 0; i < rawStatus.length; i++) {
      const s = rawStatus[i];
      if (s === "0" || s === "1") {
        statusString += s;
      } else {
        statusString += " ";
      }
    }

    const successResponse = { status: statusString };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    const errorResponse = { error: err.message };
    logRequestResponse(req, res, errorResponse);
    res.status(500).json(errorResponse);
  }
});

router.post("/:saveName/updateMinigames", authMiddleware, async (req, res) => {
  try {
    const { index, value } = req.body;

    if (index < 0 || index > 2) {
      const errorResponse = { error: "Index must be 0, 1, or 2" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    if (value !== "0" && value !== "1") {
      const errorResponse = { error: "Status must be '0' or '1'" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const saveFile = await SaveFile.findOne({
      saveName: req.params.saveName,
      userId: req.user.id,
    });

    if (!saveFile) {
      const errorResponse = { error: "Save file not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    let statusArray = saveFile.minigameStatus || ["", "", ""];

    for (let i = 0; i < 3; i++) {
      if (statusArray[i] !== "0" && statusArray[i] !== "1") {
        statusArray[i] = "";
      }
    }

    const currentStatus = statusArray[index];


    if (currentStatus === "0" && value === "0") {
      let statusString = "";
      for (let i = 0; i < statusArray.length; i++) {
        const s = statusArray[i];
        statusString += s === "0" || s === "1" ? s : " ";
      }
      const infoResponse = {
        message: "Already unlocked",
        status: statusString,
      };
      logRequestResponse(req, res, infoResponse);
      return res.status(200).json(infoResponse);
    }

    statusArray[index] = value;
    saveFile.minigameStatus = statusArray;
    await saveFile.save();

    let updatedStatus = "";
    for (let i = 0; i < statusArray.length; i++) {
      const s = statusArray[i];
      updatedStatus += s === "0" || s === "1" ? s : " ";
    }

    const successResponse = {
      message: "Minigame status updated",
      status: updatedStatus,
    };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    const errorResponse = { error: err.message };
    logRequestResponse(req, res, errorResponse);
    res.status(500).json(errorResponse);
  }
});

module.exports = router;
