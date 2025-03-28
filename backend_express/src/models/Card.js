const mongoose = require("mongoose");

const CardSchema = new mongoose.Schema({
    name: { type: String, required: true },
    type: { type: String, enum: ['Attack', 'Defense', 'Recovery', 'Others'], required: true },
    description: String,
    cost: Number,
    attack: Number,
    defense: Number,
    recovery: Number,
    targetingType: { type: String, enum: ['Manual', 'Self', 'Ally', 'FirstEnemy', 'LowestHPEnemy'], required: true },
    rarity: { type: String, enum: ['common','rare', 'epic'] },
    staminaCost: Number,
    updateCost: Number
});

module.exports = mongoose.model("Card", CardSchema);
