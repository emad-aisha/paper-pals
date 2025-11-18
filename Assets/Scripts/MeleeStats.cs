using UnityEngine;

[CreateAssetMenu(menuName = "Weapon - Melee")]
public class MeleeStats : WeaponStats
{
    [Header("Melee")]
    [SerializeField] Animator Anim;
    [Range(1, 25)] public int Damage;
    [Range(0.1f, 1f)]
    public float SwingSpeed;
    [Range(0, 5)] public int TickDamage;
    
    public bool DamageOverTime;

    //test

    public override int GetDamage()
    {
        return Damage;
    }
}
