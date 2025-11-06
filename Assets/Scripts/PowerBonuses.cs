using UnityEngine;
using System.Collections;

public class PowerBonuses : MonoBehaviour
{
    enum powerBonusType { health, shield }

    [SerializeField] powerBonusType type;
    public int healingAmount;
    public int shieldDur;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
            return;

        if (type == powerBonusType.health)
        {
            player.Heal(healingAmount);
        }
        else if (type == powerBonusType.shield)
        {
            player.StartCoroutine(player.Shield(shieldDur));
        }
        Destroy(gameObject);
    }
}
