const express = require("express");
const dotenv = require("dotenv");
const connectDB = require("./config/db");

// DB Connection
dotenv.config();
connectDB();

// App setup
const app = express();
app.use(express.json());

// Routes
app.use("/users", require("./routes/userRoutes")); // User routes
app.use("/savefiles", require("./routes/saveFileRoutes")); // Save file routes
//app.use("/progress", require("./routes/progressRoutes")); // 更新的进度查询 API
app.use("/achievements", require("./routes/achievementRoutes")); // Achievement routes

// Backend API Testing
app.get("/", (req, res) => {
    res.send("Hello, Bareturn0 Backend! 🚀 API is working!");
});

// Server Testing
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`\nServer starts successfully, running on http://localhost:${PORT}`);
    console.log("Success! MongoDB Connected!");
    console.log("\n=====Request & Response Log (set on backend_express/src/utils/logger.js)=====");
});

