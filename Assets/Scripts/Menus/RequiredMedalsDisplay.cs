using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequiredMedalsDisplay : MonoBehaviour
{
    public static RequiredMedalsDisplay Instance;
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
    }

    public void DisplayRequiredMedals(int bronzeMin, int silverMin, int goldMin, Sprite pointsImage)
    {
        //set points img
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = pointsImage;

        //set bronze info
        transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = bronzeMin.ToString();
        //set silver info
        transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = silverMin.ToString();
        //set gold info
        transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = goldMin.ToString();
    }
    public void StopDisplay()
    {
        gameObject.SetActive(false);
    }
}
