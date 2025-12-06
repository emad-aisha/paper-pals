using UnityEngine;


public enum WeaponType
{
    Melee,
    Gun,
    Explosive
}

public abstract class WeaponStats : ScriptableObject
{
    // ABSTRACT METHODS
    public abstract int GetDamage();
    public abstract AudioClip GetAudio();


    // BASE INFO 
    [Header("Base Stats")]
    public WeaponType type;
    public GameObject Model;
    public ParticleSystem HitFX;


    // WEAPON BEHAVIOR 
    [Header("Behavior")]
    public bool AOE;
    public bool Throwable;
    public bool Thrown;


    // THROW SETTINGS
    [Header("Throw Settings")]
    [Range(1, 1000)]
    public int ThrowDistance;

    [SerializeField]
    public int ThrowForce;

    public float ThrowUpwardForce;

    public float ThrowSpeed;


    // AUDIO 
    [Header("Audio")]
    [Range(0, 1)]
    public float Volume;
}
