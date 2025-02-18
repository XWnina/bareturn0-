using System.Collections.Generic;

public static class ChatData
{
    public static List<string> chatHistory = new List<string>(); // 聊天记录
    public static int currentDialogueIndex = 0; // 当前对话进度索引

    // **重置聊天数据（游戏重新运行时调用）**
    public static void ResetChatData()
    {
        chatHistory.Clear();
        currentDialogueIndex = 0;
    }
}
