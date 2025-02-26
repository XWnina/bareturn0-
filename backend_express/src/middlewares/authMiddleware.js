const jwt = require("jsonwebtoken");

const authMiddleware = (req, res, next) => {
    const authHeader = req.header("Authorization");

    // Check if the authorization header exists and is correctly formatted
    if (!authHeader || !authHeader.startsWith("Bearer ")) {
        return res.status(401).json({ error: "No token provided or token format is invalid" });
    }

    const token = authHeader.split(" ")[1]; // Extract the token after "Bearer"

    try {
        // Verify token
        const decoded = jwt.verify(token, process.env.JWT_SECRET);
        req.user = decoded; // Attach user data (id, etc.) from the token payload to the request
        next(); // Proceed to the next middleware or route handler
    } catch (err) {
        // Handle invalid or expired token
        res.status(401).json({ error: "Invalid or expired token" });
    }
};

module.exports = authMiddleware;
