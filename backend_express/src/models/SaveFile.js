const mongoose = require("mongoose");

const SaveFileSchema = new mongoose.Schema(
  {
    userId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "User",
      required: true,
    }, // Reference to the User model

    saveName: { type: String, required: true },
    playerName: { type: String, default: "|" },
    progress: { type: Number, default: 0 },
    coins: { type: Number, default: 0 },
    maxHealth: { type: Number, default: 20 },
    speed: { type: Number, default: 1 },

    minigameStatus: {
      type: [String],
      default: [" ", " ", " "],
    },

    unlockedAchievements: [
      {
        name: { type: String, required: true },
        achievedDate: { type: Date, default: null },
      },
    ], // Reference to the Achievement model

    selectedDeck: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "CardDeck",
      default: null,
    },

    cardCollection: {
      name: { type: String, default: "Card Collection" },
      cards: {
        type: [
          {
            cardName: String,
            count: Number,
          },
        ],
        default: [],
      },
    },

    materials: {
      type: [
        {
          name: { type: String, required: true },
          count: { type: Number, required: true },
        },
      ],
    },
  },
  { timestamps: true }
);

module.exports = mongoose.model("SaveFile", SaveFileSchema);
