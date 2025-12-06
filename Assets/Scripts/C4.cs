using System.Collections;
using UnityEngine;

public class C4 : MonoBehaviour
{
    ExplosiveStats Stats;
    SphereCollider Collider;
    MeshRenderer C4Model;
    ParticleSystem C4ParticleSystem;
    bool Exploded;


    [SerializeField] GameObject RedLight;
    AudioSource aud;

    public void OnThrow(ExplosiveStats NewStats)
    {
        aud = GetComponentInChildren<AudioSource>();
        Exploded = false;
        Stats = NewStats;
        Collider = this.GetComponent<SphereCollider>();
        C4Model = GetComponentInChildren<MeshRenderer>();
        C4ParticleSystem = NewStats.HitFX;

        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        StartCoroutine(Light());
        yield return new WaitForSeconds(Stats.Timer);

        // "shockwave" stuff
        Exploded = true;
        C4Model.enabled = false;
        Instantiate(C4ParticleSystem, C4Model.transform.position, C4Model.transform.rotation);
        float Duration = 0.25f;
        float StartRadius = 0.1f;
        float EndRadius = Stats.BlastRadius;
        float CurrTime = 0f;

        Collider.isTrigger = true;
        Collider.enabled = true;

        while (CurrTime < Duration)
        {
            CurrTime += Time.deltaTime;
            float lerp = CurrTime / Duration;
            Collider.radius = Mathf.Lerp(StartRadius, EndRadius, lerp);
            yield return null;
        }
        yield return new WaitForSeconds(.7f);
        Destroy(gameObject);
        
        
    }

    IEnumerator Light()
    {
        while (C4Model.enabled)
        {
            aud.pitch = 5;
            // TODO: this is commented for now cuz it shows a bug
            // TODO: put a sound in the serialized field for this
            // aud.PlayOneShot(Stats.GetAudio(), Stats.SoundVol);

            RedLight.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            RedLight.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        RedLight.SetActive(false);
        aud.pitch = 1;
        aud.PlayOneShot(Stats.ExplosionSound, Stats.SoundVol);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (Exploded)
        {
            if (other.CompareTag("Enemy"))
            {
                IDamage Target = other.GetComponentInParent<IDamage>();
                float dist = Vector3.Distance(other.transform.position, transform.position);

                if (dist <= Stats.BlastRadius)
                {
                    Target.TakeDamage(Stats.Damage);
                }
            }
            else if (other.CompareTag("Player"))
            {
               
                IDamage Target = other.GetComponentInParent<IDamage>();
                float dist = Vector3.Distance(other.transform.position, transform.position);

                if (dist <= Stats.BlastRadius)
                {
                    Target.TakeDamage(Stats.SelfDamage);
                }
            }
        }
    }
    // shows explosion for debugging only in scene view
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && Exploded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Collider.radius);

            // final blast radius for comparison
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Stats.BlastRadius);
        }
    }
}
