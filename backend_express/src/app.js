const express = require('express');
const app = express();

const PORT = 3000;

// 定义一个简单的 API
app.get('/', (req, res) => {
    res.send('Hello, Bareturn0 Backend!');
});

// 启动服务器
app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});
