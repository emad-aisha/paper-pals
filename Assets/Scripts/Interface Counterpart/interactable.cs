using UnityEngine;

public class interactable : MonoBehaviour, IInteractable {
    enum InterfaceType {
        HealingTape,
        Ammo,
        Currency,
        Trophy
    };

    [SerializeField] InterfaceType type;
    [SerializeField] int amount;

    public void Interact() {
        Debug.Log("Interact");
        if (type == InterfaceType.HealingTape && SetTape() == false) {
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

        if (type != InterfaceType.Trophy) Destroy(this.gameObject);
    }

    public bool SetTape() {
        return GameManager.instance.TapeImage.activeSelf;
    }
}
