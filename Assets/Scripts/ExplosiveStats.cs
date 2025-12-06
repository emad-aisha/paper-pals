using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
[CreateAssetMenu(menuName = "Weapon - Explosive")]
public class ExplosiveStats : WeaponStats
{
    [Header("Explosive")]
    // public GameObject GunModel;
    [Range(1, 10)] public int Damage;
    [Range(1, 4)] public int SelfDamage;
    [Range(1, 1000)] public int BlastRadius;
    [Range(0, 1)] public float SoundVol;
    [Range(1, 10)] public int Timer;
    public AudioClip[] BeepSound;
    public AudioClip ExplosionSound;
    

    public override int GetDamage()
    {
        return Damage;  
    }

    public override AudioClip GetAudio()
    {
        return BeepSound[Random.Range(0, BeepSound.Length)];
    }
}
