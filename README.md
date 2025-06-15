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
打开 Unity Hub，下载安装 Unity 6(editor version选择6000.0.34f1)。

### 3️⃣ 在 Unity 中打开项目
1. 打开 **Unity Hub**
2. 点击 `"Add"` 选择`"Add project from disk"`,选择项目根目录
3. editor version选择6000.0.34f1选择对的时候editor version右侧不会出现⚠️

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

# 🛠 前后项目结构
## 前端unity

本项目遵循标准的 Unity 目录结构，以保持文件的整洁和组织性。

### 📁 Assets
`Assets/` 目录是 Unity 项目的主资源存放地。它包含以下子目录：

#### 📂 Animations
存放 **动画** 相关文件，包括：
- `.anim`（动画剪辑）
- `.controller`（动画状态机）
- `.overrideController`（动画重写控制器）

---

#### 📂 Audio
存放 **音频资源**，进一步分为：
- 📂 `Music` - 背景音乐文件（`*.mp3`, `*.wav` 等）
- 📂 `SFX` - 音效文件，如按钮点击、环境音等

---

#### 📂 Editor
存放 **Unity 编辑器扩展脚本**。此文件夹中的代码 **仅在编辑器中运行**，不会影响游戏运行时。

---

#### 📂 Fonts
存放 **字体文件**（`.ttf`, `.otf`）。

---

#### 📂 Materials
存放 **材质（Material）**，用于定义游戏对象的外观。

---

#### 📂 Models
存放 **3D 模型**，进一步分为：
- 📂 `Characters` - 角色模型
- 📂 `Props` - 道具和场景物件

支持的格式包括：
- `.fbx`
- `.obj`
- `.blend`

---

#### 📂 Plugins
存放 **第三方插件** 或 **原生插件（Native Plugins）**。

---

#### 📂 Prefabs
存放 **预制体（Prefab）**，用于重复使用的游戏对象，如：
- 角色
- UI 界面组件
- 物品/道具

---

#### 📂 Resources
存放 **代码动态加载的资源**，例如：
- 通过 `Resources.Load()` 加载的音频、图片、JSON 数据等

⚠️ **注意**：`Resources` 目录下的资源会被打包到最终的构建文件中，使用时要谨慎，以免影响性能。

---

#### 📂 Scenes
存放 **Unity 场景（`.unity` 文件）**，如：
- `MainMenu.unity` - 主菜单
- `Level1.unity` - 关卡 1
- `Level2.unity` - 关卡 2

---

#### 📂 Scripts
存放 **C# 脚本**，进一步分为：
- 📂 `Managers` - 游戏管理器（如 `GameManager.cs`）
- 📂 `Player` - 玩家控制相关脚本
- 📂 `UI` - UI 交互相关脚本

---

#### 📂 StreamingAssets
存放 **流式资源（Streaming Assets）**，如：
- 视频（`*.mp4`）
- 配置文件（`*.json`）
- 其他需要在运行时直接访问的文件

💡 **注意**：`StreamingAssets` 中的文件不会被 Unity 压缩，会原样存储到最终构建文件中。

---

#### 📂 Textures
存放 **图片纹理**（`*.png`, `*.jpg`, `*.tga` 等）。

---

#### 📂 ThirdParty (第三方插件)
存放 **外部插件和 SDK**（如 AdMob, Firebase, Photon 等）。

---

### 📁 Packages
Unity **Package Manager（UPM）** 依赖项存放位置，不建议手动修改。

---

### 📁 ProjectSettings
存放 **Unity 项目的全局设置**，如：
- `InputManager.asset`（输入映射）
- `Physics2DSettings.asset`（物理设置）
- `TagManager.asset`（标签和图层）

---

### 💡 额外建议
✅ **请保持目录结构清晰**，避免文件混乱  
✅ **使用 `.gitignore` 忽略 `.DS_Store`、`Library`、`Logs` 等不必要的文件  
✅ **资源命名遵循统一格式**（如 `Player_Run.anim`, `BG_Music.mp3`）  
✅ **定期清理未使用的资源**，优化项目大小  

---

## 后端 express(node.js)（只是个例子大概说一下每个文件夹是干嘛的，目前src下面的文件夹基本是空的）
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


## License

This project is licensed under the MIT License - see the [](LICENSE) file for details.

Other deployment insturction might be provided with further updates.
