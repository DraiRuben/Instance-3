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
        gameObject.SetActive(false);
    }

    public void DisplayRequiredMedals(MedalRequirements requirements, Sprite pointsImage)
    {
        gameObject.SetActive(true);
        //set points img
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = pointsImage;

        //set bronze info
        transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = requirements.MinRequiredForMedal[MedalType.Bronze].ToString();
        //set silver info
        transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = requirements.MinRequiredForMedal[MedalType.Silver].ToString();
        //set gold info
        transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = requirements.MinRequiredForMedal[MedalType.Gold].ToString();
    }
    public void StopDisplay()
    {
        gameObject.SetActive(false);
    }
}
