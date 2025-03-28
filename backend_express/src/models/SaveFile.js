const mongoose = require("mongoose");

const SaveFileSchema = new mongoose.Schema(
  {
    userId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "User",
      required: true,
    }, // Reference to the User model
    saveName: { type: String, required: true },
    playerName: { type: String, default: "|" }, // Default player name is "|"
    progress: { type: Number, default: 0 },
    coins: { type: Number, default: 0 },
    unlockedAchievements: [
      {
        achievementId: {
          type: mongoose.Schema.Types.ObjectId,
          ref: "Achievement",
        },
        achievedDate: { type: Date, default: null },
      },
    ], // Reference to the Achievement model

    maxHealth: { type: Number, default: 20 }, // Default max health is 20
    speed: { type: Number, default: 1 } // Default speed is 1
  },
  { timestamps: true }
);

module.exports = mongoose.model("SaveFile", SaveFileSchema);
