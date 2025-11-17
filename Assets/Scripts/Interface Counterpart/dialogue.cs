using UnityEngine;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour, IDialogue
{
    [Header("Menus")]
    [SerializeField] string characterName;
    [SerializeField] List<string> text = new List<string>();

    int textIndexMax;
    int textIndex;

    public bool isExhausted;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textIndexMax = text.Count;
    }

    public bool IsExhausted() {
        return isExhausted;
    }

    public void SetDialogue() {        
        if (textIndex == textIndexMax) isExhausted = true;
        if (isExhausted) { 
            GameManager.instance.EndDialogue();
            textIndex--;
            isExhausted = false;
            return;
        }

        GameManager.instance.characterName.SetText(characterName.ToUpper());
        GameManager.instance.characterText.SetText(text[textIndex]);
        GameManager.instance.Dialogue();

        if (textIndex < textIndexMax) textIndex++;
    }
}
