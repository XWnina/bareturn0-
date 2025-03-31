const mongoose = require("mongoose");
require("dotenv").config();

// Load models
const User = require("../models/User");
const SaveFile = require("../models/SaveFile");
const Achievement = require("../models/Achievement");
const CardDeck = require("../models/CardDeck");

async function cleanupDatabase() {
    try {
        // Connect to MongoDB
        await mongoose.connect(process.env.MONGO_URI);
        console.log("✅ Connected to MongoDB");

        // === Instead of dropping the database, delete all records ===
        await User.deleteMany({});
        await SaveFile.deleteMany({});
        await Achievement.deleteMany({});
        await CardDeck.deleteMany({});
        console.log("🗑️ Cleared all records from Users, SaveFiles, and Achievements!");

        // Close the connection
        await mongoose.disconnect();
        console.log("✅ Database cleanup completed. Connection closed.");
    } catch (err) {
        console.error("❌ Error cleaning up database:", err);
        await mongoose.disconnect();
    }
}

// Run cleanup
cleanupDatabase();
