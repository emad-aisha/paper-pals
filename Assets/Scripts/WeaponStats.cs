using UnityEngine;


public enum WeaponType
{
    Melee,
    Gun
}

public abstract class WeaponStats : ScriptableObject
{
    public abstract int GetDamage();

    public WeaponType type;
    public GameObject Model;
    public ParticleSystem HitFX;
    [Range(0, 1)] public float Volume;

   
}
