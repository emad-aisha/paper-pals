using UnityEngine;

public class PowerBonuses : MonoBehaviour
{
    enum PowerBonusType { health, shield }

    [SerializeField] PowerBonusType type;
    public int healingAmount;
    public int shieldDur;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

        if (type == PowerBonusType.health)
        {
            player.Heal(healingAmount);
        }
        else if (type == PowerBonusType.shield)
        {
            player.StartCoroutine(player.Shield(shieldDur));
        }
        Destroy(gameObject);
    }
}
