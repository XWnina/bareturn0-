const mongoose = require("mongoose");

const AchievementSchema = new mongoose.Schema({
  name: { type: String, required: true, unique: true },
  method: { type: String, required: true },
  hidden: { type: Boolean, default: false },
  revealCondition: { type: String, default: null }
});

module.exports = mongoose.model("Achievement", AchievementSchema);
