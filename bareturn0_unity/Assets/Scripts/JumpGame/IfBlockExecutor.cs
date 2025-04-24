using System.Collections;
using UnityEngine;
using TMPro;

namespace JumpGame
{
    public class IfBlockExecutor : MonoBehaviour
    {
        public TMP_Dropdown conditionDropdown;
        public TMP_Dropdown trueDropdown;
        public GameObject elseContainer;
        public TMP_Dropdown elseDropdown;

        public Transform trueContainer; // ✅ 你需要绑定 TrueContainer
        public Transform elseContent;   // ✅ 可选：绑定 ElseContainer 作为 Transform 以支持嵌套

        public IEnumerator Execute(PlayerController player)
        {
            string condition = conditionDropdown.options[conditionDropdown.value].text;
            string trueAction = trueDropdown != null ? trueDropdown.options[trueDropdown.value].text : null;
            string elseAction = (elseContainer.activeSelf && elseDropdown != null)
                ? elseDropdown.options[elseDropdown.value].text
                : null;

            bool conditionMet = false;
            yield return player.EvaluateConditionAsync(condition, result => conditionMet = result);
            Debug.Log($"▶ 判断：{condition} = {conditionMet}");

            if (conditionMet)
            {
                if (!string.IsNullOrEmpty(trueAction))
                    yield return player.ExecuteActionAsync(trueAction);

                Debug.Log("📥 正在尝试执行 TrueContainer 中的嵌套 if blocks");
                foreach (Transform child in trueContainer)
                {
                    IfBlockExecutor nested = child.GetComponent<IfBlockExecutor>();
                    if (nested != null)
                    {
                        Debug.Log($"🧩 执行嵌套 if：{child.name}");
                        yield return nested.Execute(player);
                    }
                    else
                    {
                        Debug.LogWarning($"⚠ 子物体 {child.name} 没有挂 IfBlockExecutor");
                    }
                }

            }
            else if (!string.IsNullOrEmpty(elseAction))
            {
                yield return player.ExecuteActionAsync(elseAction);

                if (elseContent != null)
                {
                    foreach (Transform child in elseContent)
                    {
                        IfBlockExecutor nested = child.GetComponent<IfBlockExecutor>();
                        if (nested != null)
                            yield return nested.Execute(player);
                    }
                }
            }
        }
    }
}
