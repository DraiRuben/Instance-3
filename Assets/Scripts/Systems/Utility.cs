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
}
public static class Utility
{
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