using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string s;
    public void OnPointerClick(PointerEventData eventData)
    {
       // _text.text = "点击态";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //_text.text = "悬浮态";
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
       // _text.text = s;

    }

    private void Start()
    {

    }
}
