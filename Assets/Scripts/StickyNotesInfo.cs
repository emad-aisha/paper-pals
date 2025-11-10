using UnityEngine;
using TMPro;

public class StickyNotesInfo : MonoBehaviour
{
    [SerializeField] TMP_Text noteText;
    [SerializeField] TMP_Text bodytext;

    public void SetNoteText(string title, string body)
    {
        if(noteText != null)
            noteText.text = title;
        if (bodytext != null)
            bodytext.text = body;
    }

}
