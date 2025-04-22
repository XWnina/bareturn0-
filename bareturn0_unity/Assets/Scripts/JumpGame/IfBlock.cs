using UnityEngine;
using UnityEngine.UI;

namespace JumpGame
{
    public class IfBlock : MonoBehaviour
    {
        [Header("Slots")]
        public Transform conditionSlot;
        public Transform trueBodySlot;
        public Transform elseBodySlot;

        [Header("UI Elements")]
        public GameObject elseContainer;
        public Button addElseButton;
        public Button deleteElseButton;

        void Awake()
        {
            conditionSlot = transform.Find("HeaderRow/ConditionSlot");
            trueBodySlot = transform.Find("TrueBodySlot");

            elseContainer = transform.Find("ElseContainer")?.gameObject;
            elseBodySlot = elseContainer?.transform.Find("ElseBodySlot");

            addElseButton = transform.Find("AddElseButton")?.GetComponent<Button>();
            deleteElseButton = elseContainer?.transform.Find("DeleteElseButton")?.GetComponent<Button>();
        }

        void Start()
        {
            if (elseContainer != null)
                elseContainer.SetActive(false);

            if (addElseButton != null)
            {
                addElseButton.onClick.AddListener(() =>
                {
                    elseContainer?.SetActive(true);
                    addElseButton.gameObject.SetActive(false);
                });
            }

            if (deleteElseButton != null)
            {
                deleteElseButton.onClick.AddListener(() =>
                {
                    elseContainer?.SetActive(false);
                    addElseButton.gameObject.SetActive(true);

                    // 可选：清空 ElseBodySlot 内部已拖的模块
                    foreach (Transform child in elseBodySlot)
                    {
                        Destroy(child.gameObject);
                    }
                });
            }
        }
    }
}