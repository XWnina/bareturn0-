const express = require("express");
const SaveFile = require("../models/SaveFile");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");

const router = express.Router();

// Add a new talent
router.post("/:saveName/addTalent", authMiddleware, async (req, res) => {
  try {
    const { talentName, count } = req.body;

    const saveFile = await SaveFile.findOne({ userId: req.user.id, saveName: req.params.saveName });
    if (!saveFile) {
      const error = { error: "Save file not found" };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    const existing = saveFile.talents.find(t => t.talentName === talentName);
    if (existing) {
      existing.count += count;
    } else {
      saveFile.talents.push({ talentName, count });
    }

    await saveFile.save();
    const response = { message: "Talent added", talents: saveFile.talents };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
  }
});

// Update count of a talent
router.put("/:saveName/updateTalent", authMiddleware, async (req, res) => {
  try {
    const { talentName, count } = req.body;

    const saveFile = await SaveFile.findOne({ userId: req.user.id, saveName: req.params.saveName });
    if (!saveFile) {
      const error = { error: "Save file not found" };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    const talent = saveFile.talents.find(t => t.talentName === talentName);
    if (!talent) {
      const error = { error: "Talent not found" };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    talent.count = count;

    await saveFile.save();
    const response = { message: "Talent updated", talents: saveFile.talents };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
  }
});

// Get all talents
router.get("/:saveName/talents", authMiddleware, async (req, res) => {
  try {
    const saveFile = await SaveFile.findOne({ userId: req.user.id, saveName: req.params.saveName });
    if (!saveFile) {
      const error = { error: "Save file not found" };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    const response = { talents: saveFile.talents };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
  }
});

module.exports = router;
