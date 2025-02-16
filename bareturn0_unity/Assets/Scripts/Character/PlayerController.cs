using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int currentHealth = 50;
    public int maxHealth = 50;

    // 能量
    public int initialEnergy = 3;    // 第1回合的初始能量
    public int energyGainPerRound = 1; // 每回合额外获得的能量(或也可固定给 maxEnergy)
    public int currentEnergy;

    // 速度
    public int speed = 10;

    //护甲
    public int currentArmor = 0;

    [SerializeField] private PlayerAnimator animatorController;




    public void Attack()
    {
        animatorController?.PlayAttackAnimation(); // 触发攻击动画

    }

    public void TakeDamage(int damage)
    {
        int effectiveDamage = Mathf.Max(damage - currentArmor, 0);
        currentArmor = Mathf.Max(currentArmor - damage, 0);
        currentHealth -= effectiveDamage;

        animatorController?.PlayHurtAnimation(); // 受击动画

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
        animatorController?.PlayGainEnergyAnimation();
    }
}
