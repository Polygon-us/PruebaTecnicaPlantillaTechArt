using System;

[Serializable]
public struct Damage
{
    public float damageAmount;
    public ElementalDamageType damageElement;

    public Damage(float damageAmount, ElementalDamageType damageElement)
    {
        this.damageAmount = damageAmount;
        this.damageElement = damageElement;
    }
}
