const mongoose = require("mongoose");
const Achievement = require("../models/Achievement");
require("dotenv").config();

const predefinedAchievements = [
    { name: "Person You Know Who", method: "Complete the first level and gain your own player name" },
    { name: "Freshman", method: "Login to the game and create a new savefile for the first time" },
    { name: "Survivor", method: "Complete the second level" },
    { name: "Master Explorer", method: "Find all hidden secrets in a level" }
];

async function initializeAchievements() {
    try {
        await mongoose.connect(process.env.MONGO_URI, {
            useNewUrlParser: true,
            useUnifiedTopology: true
        });

        console.log("Connected to MongoDB");

        for (const ach of predefinedAchievements) {
            await Achievement.findOneAndUpdate(
                { name: ach.name }, // If already exists, skip insertion
                { method: ach.method, preset: true },
                { upsert: true }
            );
        }

        console.log("Predefined achievements initialized");

        mongoose.disconnect();
    } catch (err) {
        console.error("Error initializing achievements:", err);
        mongoose.disconnect();
    }
}

initializeAchievements();
