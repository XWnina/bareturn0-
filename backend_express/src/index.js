require("dotenv").config();
const express = require("express");
const cors = require("cors");
const connectDB = require("./config/db");
const authRoutes = require("./routes/authRoutes");

// Express Server
const app = express();
app.use(express.json()); 
app.use(cors());

const PORT = process.env.PORT || 3000;

// MongoDB Connection
connectDB();

// API Routes
app.use("/api/auth", authRoutes);

// Server
app.listen(PORT, () => console.log(`Success! Server running on http://localhost:${PORT}`));
