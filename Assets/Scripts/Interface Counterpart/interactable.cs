using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable {
    enum InterfaceType {
        HealingTape,
        Ammo,
        Currency,
        Trophy,
        Weapon,
        Flashlight
    };

    PlayerController Player;
    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    [SerializeField] InterfaceType type;
    [SerializeField] int amount;
    public WeaponStats weaponStats;

    public void Interact() {
        Debug.Log("Interact");
        if (type == InterfaceType.HealingTape && !SetTape()) {
            GameManager.instance.TapeImage.SetActive(true);
            SetTape();
        }
        else if (type == InterfaceType.Ammo) {
            GameManager.instance.UpdateAmmoCount(amount);
        }
        else if (type == InterfaceType.Currency) {
            GameManager.instance.UpdateCoinCount(amount);
        }
        else if (type == InterfaceType.Trophy) {
            GameManager.instance.WinTrophy(amount);
        }
        else if (type == InterfaceType.Weapon) {
            Player.GetWeaponStats(weaponStats);
            Destroy(gameObject);
        }
        else if (type == InterfaceType.Flashlight) {
            GameManager.instance.hasFlashlight = true;
        }

        if (type != InterfaceType.Trophy) Destroy(this.gameObject);
    }

    public bool SetTape() {
        return GameManager.instance.TapeImage.activeSelf;
    }
}
