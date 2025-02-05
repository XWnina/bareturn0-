# 🚀 **bareturn0: 基于 Unity 前端 + Node.js + Express + MongoDB 的后端 API**

## 📦 技术栈

| **技术**       | **版本**     |
|--------------|------------|
| **Unity**   | `6`        |
| **Node.js** | `v22.13.1` |
| **Express.js** | `^4.21.2` |
| **MongoDB**  | `未知`     |

---

## 📌 bareturn0_unity

### 1️⃣ 下载 Unity Hub
🔗 [Unity Hub 下载地址](https://unity.com/download)

### 2️⃣ 下载 Unity 6
打开 Unity Hub，下载安装 Unity 6。

### 3️⃣ 在 Unity 中打开项目
1. 打开 **Unity Hub**
2. 点击 `"Open"` 选择项目目录

### 4️⃣ 选择 Unity 代码编辑器
建议使用：
- **JetBrains Rider Editor**
- **Visual Studio Community**
  *⚠️ 注意：Visual Studio Community 不是 VS Code (Visual Studio Code)*
### 🛠 设置外部代码编辑器（重要！）

如果你的项目使用 **VS Code** 或 **Rider** 或**Visual Studio Community**，但打开 C# 文件时 Unity 仍然使用默认的 **MonoDevelop** 或错误的 IDE，需要进行以下设置：

#### mac:
1. 进入 **unity → setting → External Tools**  
2. 在 **External Script Editor** 里选择：
   - **Visual Studio Code (code)**
   - **JetBrains Rider**
   - **Visual Studio (Community/Professional)**

#### Windows:
1. 进入 **Edit → Preferences → External Tools**  
2. 在 **External Script Editor** 里选择：
   - **Visual Studio Code (code)**
   - **JetBrains Rider**
   - **Visual Studio (Community/Professional)**
3. 勾选 **Generate .csproj files for:**  
   ✅ Embedded packages  
   ✅ Local packages  
   ✅ Registry packages  
   ✅ Git packages  
   ✅ Built-in packages  

这样，Unity 才能正确生成 **.csproj** 文件，确保代码补全正常。 ✅


---

## 📌 bareturn0-backend

### 1️⃣ 选择 Express.js（Node.js）代码编辑器
推荐使用：
- **Visual Studio Code (VS Code)**

### 2️⃣ 下载 Node.js
🔗 [Node.js 下载地址](https://nodejs.org/en)
请下载 **v22.13.1**（Node.js LTS 版本）

### 3️⃣ 安装依赖
下载完 Node.js 后，进入后端根目录：
```bash
cd backend_express
```
安装依赖：
```bash
npm install
```
### 4️⃣ 测试后端是否正常运行
在backend_express文件夹下运行
```bash
npm start
```
然后会显示
```bash
> backend_express@1.0.0 start
> node src/app.js
Server is running on http://localhost:3000
```
打开浏览器输入: http://localhost:3000
网页显示：Hello, Bareturn0 Backend!说明后端运行正常

# 🛠 后端代码结构（只是个例子大概说一下每个文件夹是干嘛的，目前src下面的文件夹基本是空的）

```bash
bareturn0-backend/
│── node_modules/         # npm 依赖
│── src/
│   │── app.js           # 入口文件（Express 服务器）
│   │── config/          # 配置相关（数据库、环境变量）
│   │   │── db.js        # MongoDB 连接
│   │── routes/          # 路由（API 端点）
│   │   │── userRoutes.js # 用户相关路由
│   │── controllers/     # 业务逻辑
│   │   │── userController.js # 处理用户逻辑
│   │── models/          # 数据模型（MongoDB Schema）
│   │   │── User.js      # 用户模型
│   │── middlewares/     # 中间件（身份验证、错误处理等）
│   │   │── authMiddleware.js # JWT 认证
│   │── utils/           # 工具函数（加密、日志等）
│── .gitignore           # 忽略文件（node_modules、环境变量等）
│── package.json         # npm 配置文件
│── package-lock.json    # 依赖锁文件
│── README.md            # 项目说明文档
```
