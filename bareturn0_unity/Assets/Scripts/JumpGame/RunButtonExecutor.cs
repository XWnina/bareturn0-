using UnityEngine;
using UnityEngine.UI;

namespace JumpGame
{
    public class RunButtonExecutor : MonoBehaviour
    {
        [Header("Run 按钮")]
        public Button runButton;

        [Header("代码拼接区（例如 CodeWorkspaceContent）")]
        public Transform codeWorkspace;

        [Header("玩家控制器")]
        public PlayerController player;

        void Start()
        {
            if (runButton != null)
                runButton.onClick.AddListener(RunCode);
        }

        void RunCode()
        {
            Debug.Log("▶ 执行所有 IfBlock 模块...");

            foreach (Transform child in codeWorkspace)
            {
                IfBlockExecutor ifBlock = child.GetComponent<IfBlockExecutor>();
                if (ifBlock != null)
                {
                    ifBlock.Execute(player);
                }
            }

            Debug.Log("✅ 所有逻辑执行完毕！");
        }
    }
}