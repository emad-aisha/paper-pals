using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TMP_Text Button;
    Vector3 OGScale;

    [SerializeField] UIAudio uiAudio;
    private void Awake()
    {
       
        OGScale = Button.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData Event)
    {
        Button.transform.localScale = OGScale * 1.1f;
        uiAudio.PlayHoverSound();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        uiAudio.PlayClickSound();
    }

    public void OnPointerExit(PointerEventData Event)
    {
        Button.transform.localScale = OGScale;
    }
}
