using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICharacter
{
    public static PlayerController instance;
    public int currentHealth = 50;
    public int maxHealth = 30;
    public int speed = 10;

    // 能量
    public int initialEnergy = 3;    // 第1回合的初始能量
    public int energyGainPerRound = 1; // 每回合额外获得的能量(或也可固定给 maxEnergy)
    public int currentEnergy;
    public int maxEnergy = 10;

    //护甲
    public int currentArmor = 0;

    public int sharpnessLayers = 0;
    public int poisonLayers = 0;
    public int bleedLayers = 0;
    public int burnLayers = 0;

    [SerializeField] private PlayerAnimator animatorController;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Attack()
    {
        animatorController?.PlayAttackAnimation(); // 触发攻击动画

    }

    public void Cast()
    {
        animatorController?.CastingAnimation();
    }

    public void TakeDamage(int damage)
    {
        int effectiveDamage = Mathf.Max(damage - currentArmor, 0);
        currentArmor = Mathf.Max(currentArmor - damage, 0);
        currentHealth -= effectiveDamage;
        Vector3 pos = transform.position;


        animatorController?.PlayHurtAnimation(); // 受击动画
        BattleManager.Instance.ShowFloatingValue(pos, effectiveDamage);


        Debug.Log("Player takes " + effectiveDamage + " damage. HP=" + currentHealth);
    }

    public void GainArmor(int amount)
    {
        currentArmor += amount;
        Debug.Log("Player gains " + amount + " armor. Armor=" + currentArmor);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Player heals " + amount + ". HP=" + currentHealth);
    }

    public void GainEnergy(int amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        animatorController?.PlayGainEnergyAnimation();
    }

    // 应用锐利buff
    public void ApplySharpness(int layersToAdd)
    {
        sharpnessLayers += layersToAdd;
        UpdateBuffUI();
    }

    public void ApplyPoison(int layersToAdd)
    {
        poisonLayers += layersToAdd;
        UpdateBuffUI();
    }
    public void ApplyBleed(int layersToAdd)
    {
        bleedLayers += layersToAdd;
        UpdateBuffUI();
        Debug.Log($"{name} gains {layersToAdd} layers of Bleed. Total bleed layers: {bleedLayers}");
    }

    public void ApplyBurn(int layersToAdd)
    {
        burnLayers += layersToAdd;
        UpdateBuffUI();
        Debug.Log($"{name} gains {layersToAdd} burn layers. Total burn: {burnLayers}");
    }

    public IEnumerator ProcessStartOfTurnBuffs()
    {
        if (sharpnessLayers > 0)
        {
            sharpnessLayers--;
            Debug.Log("Player's Sharpness reduced by 1, now: " + sharpnessLayers);
        }

        if (poisonLayers > 0)
        {
            int damage = poisonLayers;
            yield return new WaitForSeconds(0.5f);
            TakeDamage(damage);
            poisonLayers = Mathf.Max(poisonLayers - 1, 0);
            Debug.Log(name + " takes " + damage + " poison damage, remaining poison: " + poisonLayers);
        }

        if (burnLayers > 0)
        {
            int burnDamage = burnLayers;
            yield return new WaitForSeconds(0.5f);
            TakeDamage(burnDamage);
            burnLayers = Mathf.Max(burnLayers - 1, 0);
            Debug.Log(name + " takes " + burnDamage + " burn damage, remaining burn: " + burnLayers);
        }

        else
        {
            yield return null;
        }

        UpdateBuffUI();
    }

    public void UpdateBuffUI()
    {
        if (BattleUIManager.Instance != null)
        {
            BattleUIManager.Instance.updateBuffUI(poisonLayers, burnLayers, bleedLayers, sharpnessLayers);
        }
    }
}
