using UnityEngine;
using UnityEngine.UI;

namespace JumpGame
{
    public class IfBlockUI : MonoBehaviour
    {
        [Header("Else 分支控制")]
        public GameObject elseContainer;
        public Button addElseButton;
        public Button deleteElseButton;

        [Header("If 嵌套控制")]
        public Transform trueContainer;         // 👉 TrueContainer 区域
        public Button addIfButton;              // 👉 “Add If” 按钮
        public GameObject ifBlockPrefab;        // 👉 可嵌套的 IfBlock 预制体

        void Start()
        {
            // Else 控制
            if (elseContainer != null)
                elseContainer.SetActive(false);

            if (addElseButton != null)
            {
                addElseButton.onClick.AddListener(() =>
                {
                    elseContainer.SetActive(true);
                    addElseButton.gameObject.SetActive(false);
                });
            }

            if (deleteElseButton != null)
            {
                deleteElseButton.onClick.AddListener(() =>
                {
                    elseContainer.SetActive(false);
                    addElseButton.gameObject.SetActive(true);
                    
                });
            }
            void ForceRebuildAllParents(Transform target)
            {
                while (target != null)
                {
                    RectTransform rt = target as RectTransform;
                    if (rt != null)
                        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

                    target = target.parent;
                }
            }


            // AddIf 按钮功能 ✅
            if (addIfButton != null && ifBlockPrefab != null && trueContainer != null)
            {
                addIfButton.onClick.AddListener(() =>
                {
                    GameObject newIfBlock = Instantiate(ifBlockPrefab, trueContainer);
                    newIfBlock.transform.localScale = Vector3.one;
                    newIfBlock.transform.SetAsLastSibling(); // 插入到容器最底部
                    Debug.Log("✅ 新 IfBlock 嵌套成功！");
                    ForceRebuildAllParents(newIfBlock.transform);
                });
            }
        }
    }
}