const mongoose = require("mongoose");
const Achievement = require("../models/Achievement");
require("dotenv").config();

const predefinedAchievements = [
  // General achievements
  { name: "FirstStep", method: "Create your first save file", hidden: false },
  {
    name: "PassionOn",
    method: "Owning more than one save file",
    hidden: false,
  },

  // Level achievements
  { name: "Person You Know Who", method: "Complete level 1", hidden: false },
  { name: "Fighter", method: "Complete level 2", hidden: false },
  { name: "Live For Your Own", method: "Complete level 4", hidden: false },

  // Hidden achievements
  {
    name: "Rich Kid",
    method: "Gain more than 2000 coins",
    hidden: true,
    revealCondition: "Live For Your Own",
  },
  {
    name: "Mini Tycoon",
    method: "Gain more than 5000 coins",
    hidden: true,
    revealCondition: "Rich Kid",
  },
  {
    name: "Millionaire",
    method: "Gain more than 1000000 coins",
    hidden: true,
    revealCondition: "Rich Kid",
  },
  {
    name: "Battle Expert",
    method: "Winning more than 10 battles",
    hidden: true,
    revealCondition: "Fighter",
  },
];

async function initializeAchievements() {
  try {
    await mongoose.connect(process.env.MONGO_URI);

    for (const ach of predefinedAchievements) {
      await Achievement.findOneAndUpdate(
        { name: ach.name },
        { $setOnInsert: ach },
        { upsert: true }
      );
    }

    console.log("Achievements initialized");
    await mongoose.disconnect();
  } catch (err) {
    console.error("Initialization failed:", err);
    await mongoose.disconnect();
  }
}

initializeAchievements();
