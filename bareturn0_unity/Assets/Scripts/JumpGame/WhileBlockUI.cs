using UnityEngine;
using UnityEngine.UI;

namespace JumpGame
{
    public class WhileBlockUI : MonoBehaviour
    {
        public Transform trueContainer;
        public Button addIfButton;
        public GameObject ifBlockPrefab;

        void Start()
        {
            if (addIfButton != null && ifBlockPrefab != null && trueContainer != null)
            {
                addIfButton.onClick.AddListener(() =>
                {
                    GameObject newIf = Instantiate(ifBlockPrefab, trueContainer);
                    newIf.transform.localScale = Vector3.one;
                    newIf.transform.SetAsLastSibling();
                    ForceRebuildAllParents(newIf.transform);
                    Debug.Log("✅ WhileBlock 中添加了嵌套 ifBlock");
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
        }
        
    }
}