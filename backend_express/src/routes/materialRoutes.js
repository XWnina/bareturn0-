const express = require("express");
const router = express.Router();
const SaveFile = require("../models/SaveFile");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");

// Create a new material (error if already exists)
router.post("/create/:saveName", authMiddleware, async (req, res) => {
  const { name, count } = req.body;

  if (typeof name !== "string" || typeof count !== "number" || count <= 0) {
    const error = { error: "Invalid name or count (must be > 0)" };
    logRequestResponse(req, res, error);
    return res.status(400).json(error);
  }

  try {
    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!saveFile) {
      const error = { error: "Save file not found" };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    const existing = saveFile.materials.find((mat) => mat.name === name);
    if (existing) {
      const error = { error: `Material '${name}' already exists` };
      logRequestResponse(req, res, error);
      return res.status(400).json(error);
    }

    saveFile.materials.push({ name, count });
    await saveFile.save();

    const response = {
      message: "Material added",
      materials: saveFile.materials,
    };
    logRequestResponse(req, res, response);
    res.status(201).json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
  }
});

// Update material by name (increase or decrease count, remove if <= 0)
router.put("/update/:saveName/:materialName", authMiddleware, async (req, res) => {
  const { count } = req.body;

  if (typeof count !== "number") {
    const error = { error: "Count must be a number" };
    logRequestResponse(req, res, error);
    return res.status(400).json(error);
  }

  try {
    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!saveFile) {
      const error = { error: "Save file not found" };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    const index = saveFile.materials.findIndex(
      (mat) => mat.name === req.params.materialName
    );

    if (index === -1) {
      const error = { error: `Material '${req.params.materialName}' not found` };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    saveFile.materials[index].count = count;

    if (saveFile.materials[index].count <= 0) {
      saveFile.materials.splice(index, 1);
    }

    await saveFile.save();

    const response = {
      message: "Material updated",
      materials: saveFile.materials,
    };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
  }
});

// Get all materials
router.get("/all/:saveName", authMiddleware, async (req, res) => {
  try {
    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!saveFile) {
      const error = { error: "Save file not found" };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    const response = {
      materials: saveFile.materials,
    };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
  }
});

// Get a specific material count by name
router.get("/count/:saveName/:materialName", authMiddleware, async (req, res) => {
  try {
    const saveFile = await SaveFile.findOne({
      userId: req.user.id,
      saveName: req.params.saveName,
    });

    if (!saveFile) {
      const error = { error: "Save file not found" };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    const material = saveFile.materials.find(
      (mat) => mat.name === req.params.materialName
    );

    if (!material) {
      const error = { error: `Material '${req.params.materialName}' not found` };
      logRequestResponse(req, res, error);
      return res.status(404).json(error);
    }

    const response = {
      count: material.count,
    };
    logRequestResponse(req, res, response);
    res.json(response);
  } catch (err) {
    const error = { error: err.message };
    logRequestResponse(req, res, error);
    res.status(500).json(error);
  }
});

module.exports = router;
