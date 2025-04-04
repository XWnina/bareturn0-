const express = require("express");
const Achievement = require("../models/Achievement");
const SaveFile = require("../models/SaveFile");
const authMiddleware = require("../middlewares/authMiddleware");

const router = express.Router();

/**
 * GET /api/achievements/:saveName/all-status
 * Returns the status of all achievements for the given save file.
 * Includes: name, method, unlocked, hidden, achievedDate
 * Handles "revealCondition" logic for hidden achievements.
 */
router.get("/:saveName/all-status", authMiddleware, async (req, res) => {
  try {
    const { saveName } = req.params;

    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName
    });

    if (!saveFile) {
      return res.status(404).json({ error: "Save file not found" });
    }

    const allAchievements = await Achievement.find({});
    const unlockedMap = new Map(
      saveFile.unlockedAchievements.map(a => [a.name, a.achievedDate])
    );

    const statusList = allAchievements.map(a => {
      const isUnlocked = unlockedMap.has(a.name);

      // Reveal condition: if this achievement has a "revealCondition",
      // it will only be visible if the condition is already unlocked.
      const revealConditionUnlocked =
        !a.revealCondition || unlockedMap.has(a.revealCondition);

      // Determine visibility for this saveFile
      const isVisible = isUnlocked || (!a.hidden || revealConditionUnlocked);

      return {
        name: isVisible ? a.name : "???",
        method: isVisible ? a.method : "This achievement is hidden.",
        unlocked: isUnlocked,
        hidden: a.hidden || false,
        achievedDate: unlockedMap.get(a.name) || null
      };
    });

    res.json(statusList);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

/**
 * PUT /api/achievements/:saveName/unlock
 * Unlocks an achievement for a given save file.
 * Body: { "achievementName": "Rich Kid" }
 */
router.put("/:saveName/unlock", authMiddleware, async (req, res) => {
  try {
    const { achievementName } = req.body;
    const { saveName } = req.params;

    const achievement = await Achievement.findOne({ name: achievementName });
    if (!achievement) {
      return res.status(404).json({ error: "Achievement not found" });
    }

    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName
    });

    if (!saveFile) {
      return res.status(404).json({ error: "Save file not found" });
    }

    const alreadyUnlocked = saveFile.unlockedAchievements.some(
      (a) => a.name === achievementName
    );

    if (alreadyUnlocked) {
      return res.status(400).json({ error: "Achievement already unlocked" });
    }

    const unlockTime = new Date();

    // Add to saveFile's unlockedAchievements
    saveFile.unlockedAchievements.push({
      name: achievementName,
      achievedDate: unlockTime
    });

    await saveFile.save();

    res.json({
      message: "Achievement unlocked",
      name: achievementName,
      achievedDate: unlockTime
    });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

module.exports = router;
