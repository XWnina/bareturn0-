const mongoose = require("mongoose");

const SaveFileSchema = new mongoose.Schema({
    userId: { type: mongoose.Schema.Types.ObjectId, ref: "User", required: true },
    saveName: { type: String, required: true },
    playerName: { type: String, default: "|" },
    progress: { type: Number, default: 0 },
    coins: { type: Number, default: 0 },
    timeStamp: { type: Date, default: Date.now },
}, { timestamps: true });

module.exports = mongoose.model("SaveFile", SaveFileSchema);
