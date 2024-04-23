using System.Collections;
using UnityEngine;
public enum MedalType
{
    None,
    Bronze,
    Silver,
    Gold
}
public struct StandResults
{
    public StandResults(MedalType medal, int points)
    {
        _Medal = medal;
        _Points = points;
    }
    public MedalType _Medal;
    public int _Points;
}
public interface IInteractable
{
    public void Interact();
    public bool CanInteract();
}

public static class Utility
{
    public static Vector3 GetWorldScreenCenterPos()
    {
        Vector2 screenCenterPos = new Vector2(Screen.width/2, Screen.height/2);
        Vector3 worldScreenCenterPos = Camera.main.ScreenToWorldPoint(screenCenterPos);
        worldScreenCenterPos.z = 0;
        return worldScreenCenterPos;
    }
    public static void Invoke(this MonoBehaviour mb, System.Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }

    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}
