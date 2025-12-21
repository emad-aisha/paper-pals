using System.Collections;
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
        Dash,
        Checkpoint
    };

    PlayerController Player;
    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    [SerializeField] InterfaceType type;
    [SerializeField] int amount;
    public WeaponStats weaponStats;
    [SerializeField] GameObject pointLight;
    [SerializeField] GameObject pointLight2;

    public void Interact()
    {
        switch (type)
        {
            case InterfaceType.HealingTape:
                if (SetTape())
                {
                    GameManager.instance.ShowTapeHint();
                    return;
                }
                GameManager.instance.TapeImage.SetActive(true);
                SetTape();
                Destroy(gameObject);
                break;

            case InterfaceType.Ammo:
                GameManager.instance.UpdateAmmoCount(amount);
                Destroy(gameObject);
                break;

            case InterfaceType.Currency:
                GameManager.instance.UpdateCoinCount(amount);
                Destroy(gameObject);
                break;

            case InterfaceType.Trophy:
                GameManager.instance.WinTrophy();
                break;

            case InterfaceType.Weapon:
                Player.GetWeaponStats(weaponStats);
                Destroy(gameObject);

                if (weaponStats.type == WeaponType.Gun)
                {
                    GunStats gun = (GunStats)weaponStats;
                    gun.AmmoCurr = gun.AmmoMax;
                    GameManager.instance.CurrAmmo.text = gun.AmmoCurr.ToString();
                    GameManager.instance.TotalAmmo.text = gun.AmmoMax.ToString();
                }
                break;

            case InterfaceType.Flashlight:
                GameManager.instance.hasFlashlight = true;
                GameManager.instance.ShowFlashlightHint();
                Destroy(gameObject);
                break;

            case InterfaceType.Keys:
                GameManager.instance.ownedKeys += 1;
                GameManager.instance.KeyCheck();
                Destroy(gameObject);
                break;

            case InterfaceType.DoubleJump:
                GameManager.instance.hasDoubleJump = true;
                GameManager.instance.ShowDoubleJumpHint();
                Destroy(gameObject);
                break;

            case InterfaceType.Dash:
                GameManager.instance.hasDash = true;
                GameManager.instance.ShowDashHint();
                Destroy(gameObject);
                break;

            case InterfaceType.Checkpoint:
                ActivateCheckpoint();
                break;
        }
    }

    void ActivateCheckpoint()
    {
        StartCoroutine(Remidner());
        if (GameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
        }
    }

    IEnumerator Remidner() {
        pointLight.SetActive(true);
        pointLight2.SetActive(true);
        GameManager.instance.reminderText.text = "Checkpoint Got!";
        GameManager.instance.reminderMenu.SetActive(true);
        yield return new WaitForSeconds(2);
        GameManager.instance.reminderMenu.SetActive(false);
        pointLight.SetActive(false);
        pointLight2.SetActive(false);
    }

    // for money
    private void OnTriggerEnter(Collider other)
    {
        // Currency auto-pickup ONLY
        if (other.CompareTag("Player") && type == InterfaceType.Currency)
        {
            GameManager.instance.UpdateCoinCount(amount);
            Destroy(gameObject);
        }
    }

    public bool SetTape()
    {
        return GameManager.instance.TapeImage.activeSelf;
    }
}
