const LOGGING_ENABLED = true;

const logRequestResponse = (req, res, responseBody) => {
    if (!LOGGING_ENABLED) return; // Skip logging if disabled
    console.log(`[${req.method}] ${req.originalUrl}`);

    // Log request body for all request types
    if (Object.keys(req.body).length > 0) {
        console.log("Request Body:", req.body);
    }

    console.log("Response:", responseBody);
    console.log("");
};

const setLogging = (enabled) => {
    global.LOGGING_ENABLED = enabled;
};

module.exports = { logRequestResponse, setLogging };
