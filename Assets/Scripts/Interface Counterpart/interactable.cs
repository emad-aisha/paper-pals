using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable {
    enum InterfaceType {
        HealingTape,
        Ammo,
        Currency,
        Trophy,
        Weapon,
        Flashlight,
        Keys,
        DoubleJump,
        Dash
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
        // TODO: change into a switch
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
            GameManager.instance.WinTrophy(0);
        }
        else if (type == InterfaceType.Weapon) {
            Player.GetWeaponStats(weaponStats);
            Destroy(gameObject);

            if (weaponStats.type == WeaponType.Gun)
            {
                GunStats Gun = (GunStats)weaponStats;
                Gun.AmmoCurr = Gun.AmmoMax;
                GameManager.instance.CurrAmmo.text = Gun.AmmoCurr.ToString();
                GameManager.instance.TotalAmmo.text = Gun.AmmoMax.ToString();

            }
        }
        else if (type == InterfaceType.Flashlight){
            GameManager.instance.hasFlashlight = true;
            GameManager.instance.ShowFlashlightHint();
        }
        else if (type == InterfaceType.Keys) {
            GameManager.instance.ownedKeys += 1;
            GameManager.instance.KeyCheck();
        }
        else if (type == InterfaceType.DoubleJump) {
            GameManager.instance.hasDoubleJump = true;
        }
        else if(type == InterfaceType.Dash){
           GameManager.instance.hasDash = true;
        }

        if (type != InterfaceType.Trophy) Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == "Player" && type == InterfaceType.Currency) {
            GameManager.instance.UpdateCoinCount(amount);
            Destroy(this.gameObject);
        }
    }

    public bool SetTape() {
        return GameManager.instance.TapeImage.activeSelf;
    }
}
