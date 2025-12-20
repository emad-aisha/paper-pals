using System.Collections;
using UnityEngine;


public class dialogueTrigger : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] string text;

    public bool isExhausted;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
    }

    private void OnTriggerEnter(Collider other) {
        if (other == GameManager.instance.player)
            SetDialogue();
    }

    public void SetDialogue() {
        GameManager.instance.disclaimerText.SetText(text);
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue() {
        GameManager.instance.disclaimerMenu.SetActive(true);
        yield return new WaitForSeconds(2);
        GameManager.instance.disclaimerMenu.SetActive(false);
    }
}
