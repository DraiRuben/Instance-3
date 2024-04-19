using UnityEngine;
using UnityEngine.EventSystems;

public class Target : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        RifleMinigame.Instance.HandleTargetHit(gameObject);
    }


}
