using UnityEngine;
[CreateAssetMenu(menuName = "Weapon - Gun")]
public class GunStats : WeaponStats
{
    [Header("Gun")]
    public GameObject GunModel;
    [Range(1, 10)] public int Damage;
    [Range(15, 1000)] public int ShootDistance;
    [Range(0.1f, 2)] public int ShootRate;
    public int AmmoCur;
    [Range(5, 50)] public int AmmoMax;

    public AudioClip[] ShootSound;
    [Range(0, 1)] public float ShootSoundVol;

    public override int GetDamage()
    {
        return Damage;
    }
}
