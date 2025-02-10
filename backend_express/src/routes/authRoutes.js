const express = require("express");
const User = require("../models/User");

const router = express.Router();

// API for creating account
router.post("/register", async (req, res) => {
    try {
        const { username, password } = req.body;

        // Check for duplicate username
        const existingUser = await User.findOne({ username });
        if (existingUser) {
            return res.status(400).json({ error: "Username already exists" });
        }

        // Create a new user account
        const newUser = new User({ username, password });
        await newUser.save();

        res.json({ message: "User registered successfully!" });
    } catch (error) {
        res.status(500).json({ error: "Internal server error" });
    }
});

module.exports = router;
