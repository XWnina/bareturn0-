using System.Collections;
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
                runButton.onClick.AddListener(() => StartCoroutine(RunCode()));
        }
        IEnumerator RunCode()
        {
            player.interrupted = false; // ✅ 清除上次的中断状态
            Debug.Log("▶ 执行所有模块...");
            player.reachedGoal = false; 

            foreach (Transform child in codeWorkspace)
            {
                IfBlockExecutor ifBlock = child.GetComponent<IfBlockExecutor>();
                WhileBlockExecutor whileBlock = child.GetComponent<WhileBlockExecutor>();

                if (ifBlock != null)
                {
                    Debug.Log($"🧩 执行 IfBlock：{child.name}");
                    yield return StartCoroutine(ifBlock.Execute(player));
                }
                else if (whileBlock != null)
                {
                    Debug.Log($"🔁 执行 WhileBlock：{child.name}");
                    yield return StartCoroutine(whileBlock.Execute(player));
                }
                else
                {
                    Debug.LogWarning($"⚠ 未知模块类型：{child.name}");
                }
            }

            Debug.Log("✅ 所有逻辑执行完毕！");
        }
    }
}