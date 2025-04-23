using UnityEngine;
using UnityEngine.UI;

namespace JumpGame
{
    public class IfBlockUI : MonoBehaviour
    {
        public GameObject elseContainer;
        public Button addElseButton;
        public Button deleteElseButton;

        void Start()
        {
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
        }
    }
}