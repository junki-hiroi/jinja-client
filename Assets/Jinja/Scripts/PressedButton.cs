using UnityEngine;
using UnityEngine.EventSystems;

namespace Jinja.Scripts
{
public class PressedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
    }
}
}
