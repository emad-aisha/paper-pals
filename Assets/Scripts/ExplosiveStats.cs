using UnityEngine;
[CreateAssetMenu(menuName = "Weapon - Explosive")]
public class ExplosiveStats : WeaponStats
{
    [Header("Explosive")]
    // public GameObject GunModel;
    [Range(1, 10)] public int Damage;
    [Range(5, 1000)] public int BlastRadius;
    [Range(0, 1)] public float SoundVol;
    [Range(1, 10)] public int Timer;
    public AudioClip[] WeaponSound;
    

    public override int GetDamage()
    {
        return Damage;  
    }

    public override AudioClip GetAudio()
    {
        return WeaponSound[Random.Range(0, WeaponSound.Length)];
    }
}
