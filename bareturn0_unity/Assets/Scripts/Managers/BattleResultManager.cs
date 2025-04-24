using UnityEngine;

public class BattleResultManager : MonoBehaviour
{
    public static BattleResultManager Instance;
    public bool passed = false;
    public bool isPefectPassed = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // ±‹√‚÷ÿ∏¥
        }
    }

    public void CheckPerfectPass()
    {
        if (BattleManager.Instance.CurrentRoundNumber < 5)
        {
            isPefectPassed = true;
        }
        else
        {
            isPefectPassed = false;
        }
    }
}