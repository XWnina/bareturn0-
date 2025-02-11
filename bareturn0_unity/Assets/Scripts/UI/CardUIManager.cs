using System.Collections.Generic;
using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [Header("Card UI References")]
    public GameObject cardViewPrefab;
    public Transform handPanel;
    public List<CardView> handCardViews = new List<CardView>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnDrawCard(CardData cardData)
    {
        //检查参数不为空 
        if (cardViewPrefab == null)
        {
            Debug.LogError("CardUIManager: cardViewPrefab is not assigned!");
            return;
        }

        if (handPanel == null)
        {
            Debug.LogError("CardUIManager: handPanel is not assigned!");
            return;
        }

        // 在handPanel下生成一个CardView对象
        GameObject cardObj = Instantiate(cardViewPrefab, handPanel);

        // 初始化UI组件
        var cv = cardObj.GetComponent<CardView>();
        cv.SetCard(cardData);

        handCardViews.Add(cv);

        // 其它UI设置（可选），如缩放、位置动画等
        cardObj.transform.localScale = Vector3.one;
    }

    public void DestroyAllCardViews()
    {
        foreach (var cv in handCardViews)
        {
            Destroy(cv.gameObject);
        }
        handCardViews.Clear();
    }

    public void RemoveCardView(CardData cardData)
    {
        CardView cardToRemove = handCardViews.Find(cv => cv.GetCardData() == cardData);

        if (cardToRemove != null)
        {
            handCardViews.Remove(cardToRemove);
            Destroy(cardToRemove.gameObject);
        }
    }

}