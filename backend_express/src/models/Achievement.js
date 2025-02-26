const mongoose = require("mongoose");

const AchievementSchema = new mongoose.Schema({
    saveFileId: { type: mongoose.Schema.Types.ObjectId, ref: "SaveFile", required: true },
    name: { type: String, required: true }, // Name of the achievement
    achievedDate: { type: Date }, // Null if locked
    unlocked: { type: Boolean, default: false } // Default: locked
});

module.exports = mongoose.model("Achievement", AchievementSchema);
