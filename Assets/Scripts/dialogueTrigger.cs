using System.Collections;
using TMPro;
using UnityEngine;


public class dialogueTrigger : MonoBehaviour
{

    GameObject disclaimerMenu;
    TMP_Text disclaimerText;

    private void Start() {
        disclaimerMenu = GameManager.instance.reminderMenu;
        disclaimerText = GameManager.instance.reminderText;
    }

    private void OnTriggerEnter(Collider other) {
        //awful way to do this but idc :grin:
        if (other.name == "Player") {
            StartCoroutine(ShowDialogue());
        }
    }

    void Set() {
        disclaimerText.text = "This'll send you to the shop";
    }

    IEnumerator ShowDialogue() {
        Set();
        disclaimerMenu.SetActive(true);
        yield return new WaitForSeconds(2);
        disclaimerMenu.SetActive(false);
    }
}
