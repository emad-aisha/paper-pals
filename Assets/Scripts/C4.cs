using System.Collections;
using UnityEngine;

public class C4 : MonoBehaviour
{
    ExplosiveStats Stats;
    SphereCollider Collider;
    MeshRenderer C4Model;
    ParticleSystem C4ParticleSystem;

    [SerializeField] GameObject RedLight;


    public void OnThrow(ExplosiveStats NewStats)
    {
        Stats = NewStats;
        Collider = this.GetComponent<SphereCollider>();
        C4Model = GetComponentInChildren<MeshRenderer>();
        C4ParticleSystem = NewStats.HitFX;

        if (Collider == null)
        {
            Debug.Log("um why no lcllider");
        }

        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        StartCoroutine(Light());
        yield return new WaitForSeconds(Stats.Timer);

        // "shockwave" stuff
        C4Model.enabled = false;
        Instantiate(C4ParticleSystem, C4Model.transform.position, Quaternion.identity);
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

        Destroy(gameObject);
        
        
    }

    IEnumerator Light()
    {
        while (C4Model.enabled)
        {
            RedLight.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            RedLight.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        RedLight.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamage Target = other.GetComponent<IDamage>();
            Target.TakeDamage(Stats.Damage);
        }
        else if (other.CompareTag("Player"))
        {
            IDamage Target = other.GetComponent<IDamage>();
            Target.TakeDamage(2);
        }
    }
}
