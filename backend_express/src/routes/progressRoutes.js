const express = require("express");
const router = express.Router();
const User = require("../models/User");
const SaveFile = require("../models/SaveFile");

// 获取特定存档的进度（progress）
router.get("/:username/:saveName", async (req, res) => {
    try {
        const { username, saveName } = req.params;

        // 通过 username 查找用户
        const user = await User.findOne({ username });
        if (!user) {
            return res.status(404).json({ message: "User not found" });
        }

        // 通过 saveName 查找存档
        const saveFile = await SaveFile.findOne({ userId: user._id, saveName });
        if (!saveFile) {
            return res.status(404).json({ message: "Save file not found" });
        }

        // 返回当前存档的 progress
        res.json({
            username: username,
            saveName: saveName,
            currentLevel: saveFile.progress,
        });
    } catch (error) {
        res.status(500).json({ error: "Internal server error", details: error.message });
    }
});

module.exports = router;
