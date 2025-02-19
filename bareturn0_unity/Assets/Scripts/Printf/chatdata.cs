using System.Collections.Generic;

public static class ChatData
{
    public static List<string> chatHistory = new List<string>();
    public static int currentDialogueIndex = 0;
    public static bool isChatPaused = false;
    public static Queue<string> playerLines = new Queue<string>();
    public static Queue<string> npcLines = new Queue<string>();

    public static void ResetChatData()
    {
        chatHistory.Clear();
        currentDialogueIndex = 0;
        playerLines.Clear();
        npcLines.Clear();
    }
}