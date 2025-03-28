const mongoose = require("mongoose");

const CardsInDeckSchema = new mongoose.Schema({
    card: { type: mongoose.Schema.Types.ObjectId, ref: "Card", required: true },
    count: { type: Number, required: true, min: 1 }
}, { _id: false });

const CardDeckSchema = new mongoose.Schema({
    saveFileId: { type: mongoose.Schema.Types.ObjectId, ref: "SaveFile", required: true },
    name: { type: String, required: true },
    cards: [CardsInDeckSchema],
    createdAt: { type: Date, default: Date.now }
});

module.exports = mongoose.model("CardDeck", CardDeckSchema);
