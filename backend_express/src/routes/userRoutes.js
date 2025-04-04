const express = require("express");
const bcrypt = require("bcryptjs");
const jwt = require("jsonwebtoken");
const User = require("../models/User");
const authMiddleware = require("../middlewares/authMiddleware");
const { logRequestResponse } = require("../utils/logger");

const router = express.Router();

// User Registration with Input Validation
router.post("/register", async (req, res) => {
  try {
    const { username, password } = req.body;

    // Check if username and password are provided
    if (
      !username ||
      typeof username !== "string" ||
      username.trim().length === 0
    ) {
      const errorResponse = {
        error: "Username is required and cannot be empty",
      };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    if (!password || typeof password !== "string" || password.length < 6) {
      const errorResponse = {
        error: "Password is required and must be at least 6 characters long",
      };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const existingUser = await User.findOne({ username });
    if (existingUser) {
      const errorResponse = { error: "Username already exists" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    // Hash the password before storing it
    const hashedPassword = await bcrypt.hash(password, 10);
    const newUser = new User({ username, password: hashedPassword });

    await newUser.save();
    const successResponse = { message: "User registered successfully" };
    logRequestResponse(req, res, successResponse);
    res.status(201).json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// User Login
router.post("/login", async (req, res) => {
  try {
    const { username, password } = req.body;
    if (!username || !password) {
      const errorResponse = { error: "Username and password are required" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const user = await User.findOne({ username });
    if (!user) {
      const errorResponse = { error: "User does not exist" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    // if (user.logStatus) {
    //     const errorResponse = { error: "User is already logged in" };
    //     logRequestResponse(req, res, errorResponse);
    //     return res.status(400).json(errorResponse);
    // }

    const isMatch = await bcrypt.compare(password, user.password);
    if (!isMatch) {
      const errorResponse = { error: "Incorrect password" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    user.logStatus = true;
    await user.save();

    const token = jwt.sign({ id: user._id }, process.env.JWT_SECRET, {
      expiresIn: "7d",
    });

    const successResponse = { message: "Login successful", token };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get current logged-in user
router.get("/me", authMiddleware, async (req, res) => {
  try {
    const user = await User.findById(req.user.id).select("-password");
    logRequestResponse(req, res, user);
    res.json(user);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Get all users (Including Passwords)
router.get("/all", async (req, res) => {
  try {
    const users = await User.find({}); // Retrieve all users including passwords
    logRequestResponse(req, res, users);
    res.json(users);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

// Delete a user
router.delete("/:username", async (req, res) => {
  try {
    const { username } = req.params;

    const user = await User.findOneAndDelete({ username });
    if (!user) {
      const errorResponse = { error: "User not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const successResponse = {
      message: `User '${username}' deleted successfully`,
    };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

router.post("/logout", async (req, res) => {
  try {
    const token = req.headers.authorization?.split(" ")[1]; // Extract token from header
    if (!token) {
      const errorResponse = { error: "Unauthorized: No token provided" };
      logRequestResponse(req, res, errorResponse);
      return res.status(401).json(errorResponse);
    }

    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    const userId = decoded.id;

    const user = await User.findById(userId);
    if (!user) {
      const errorResponse = { error: "User does not exist" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    // Set logStatus to false
    user.logStatus = false;
    await user.save();

    const successResponse = { message: "Logout successful" };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    const errorResponse = { error: err.message };
    logRequestResponse(req, res, errorResponse);
    res.status(500).json(errorResponse);
  }
});

router.put("/updateUsername", authMiddleware, async (req, res) => {
  try {
    const { newUsername } = req.body;

    if (!newUsername || newUsername.trim() === "") {
      const errorResponse = { error: "New username is required" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const existingUser = await User.findOne({ username: newUsername });
    if (existingUser) {
      const errorResponse = { error: "Username already taken" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const updatedUser = await User.findByIdAndUpdate(
      req.user.id,
      { $set: { username: newUsername } },
      { new: true }
    );

    const successResponse = {
      message: "Username updated successfully",
      newUsername: updatedUser.username,
    };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

router.put("/updatePassword", authMiddleware, async (req, res) => {
  try {
    const { oldPassword, newPassword } = req.body;

    if (!oldPassword || !newPassword || newPassword.length < 6) {
      const errorResponse = {
        error:
          "Both passwords required. New password must be at least 6 characters.",
      };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const user = await User.findById(req.user.id);
    if (!user) {
      const errorResponse = { error: "User not found" };
      logRequestResponse(req, res, errorResponse);
      return res.status(404).json(errorResponse);
    }

    const isMatch = await bcrypt.compare(oldPassword, user.password);
    if (!isMatch) {
      const errorResponse = { error: "Old password is incorrect" };
      logRequestResponse(req, res, errorResponse);
      return res.status(400).json(errorResponse);
    }

    const hashedNewPassword = await bcrypt.hash(newPassword, 10);
    user.password = hashedNewPassword;
    await user.save();

    const successResponse = { message: "Password updated successfully" };
    logRequestResponse(req, res, successResponse);
    res.json(successResponse);
  } catch (err) {
    logRequestResponse(req, res, { error: err.message });
    res.status(500).json({ error: err.message });
  }
});

module.exports = router;
