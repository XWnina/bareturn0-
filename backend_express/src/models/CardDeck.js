const mongoose = require("mongoose");

const CardsInDeckSchema = new mongoose.Schema({
  cardName: { type: String, required: true },
  count: { type: Number, required: true, min: 1 }
}, { _id: false });

const CardDeckSchema = new mongoose.Schema({
  saveFileId: { type: mongoose.Schema.Types.ObjectId, ref: "SaveFile", required: true },
  name: { type: String, required: true },
  cards: [CardsInDeckSchema]
});

module.exports = mongoose.model("CardDeck", CardDeckSchema);
