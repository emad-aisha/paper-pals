using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text Button;
    Vector3 OGScale;
    private void Awake()
    {
        OGScale = Button.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData Event)
    {
        Button.transform.localScale = OGScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData Event)
    {
        Button.transform.localScale = OGScale;
    }
}
