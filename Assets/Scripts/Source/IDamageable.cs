public interface IDamageable
{
    void Die();
}

public interface IDamageable<TDamage> : IDamageable where TDamage : struct
{
    void TakeDamage(TDamage damage);
}

