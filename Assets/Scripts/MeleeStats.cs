using UnityEngine;

[CreateAssetMenu(menuName = "Weapon - Melee")]
public class MeleeStats : WeaponStats
{
    [Header("Melee")]
    [SerializeField] Animator Anim;
    [Range(1, 25)] public int Damage;
    [Range(0.1f, 1f)] public float SwingSpeed;
    [Range(0.1f, 30)] public float MeleeRange;
    [Range(0, 5)] public int TickDamage;
    public AudioClip[] SwingSound;
    [Range(0, 1)] public float ShootSoundVol;

    public bool DamageOverTime;

    //test

    public override int GetDamage()
    {
        return Damage;
    }

    public override AudioClip GetAudio()
    {
        return SwingSound[Random.Range(0, SwingSound.Length)];
    }

}
