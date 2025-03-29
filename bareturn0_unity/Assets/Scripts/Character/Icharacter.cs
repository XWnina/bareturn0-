public interface ICharacter
{
    // 接口方法：
    void TakeDamage(int damage);
    void Attack();
    void Heal(int amount);

    void GainArmor(int amount);

    void Cast();
}
