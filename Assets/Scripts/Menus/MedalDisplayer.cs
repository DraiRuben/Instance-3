using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedalDisplayer : MonoBehaviour
{
    public Image _MoleMedal;
    public Image _FishMedal;
    public Image _CupMedal;
    public Image _RifleMedal;

    public Sprite _EmptyMedal;
    public Sprite _BronzeMedal;
    public Sprite _SilverMedal;
    public Sprite _GoldMedal;

    private void OnEnable()
    {
        if (MoleWacker.Instance)
        {
            _MoleMedal.sprite = GetMedalSprite(MoleWacker.Instance._StandResults._Medal);
        }
        if(FishManager.Instance)
        {
            _FishMedal.sprite = GetMedalSprite(FishManager.Instance._StandResults._Medal);
        }
        if(RifleMinigame.Instance)
        {
            _RifleMedal.sprite = GetMedalSprite(RifleMinigame.Instance._StandResults._Medal);
        }
        if (Cups.Instance)
        {
            _CupMedal.sprite = GetMedalSprite(Cups.Instance._StandResults._Medal);
        }
    }

    private Sprite GetMedalSprite(MedalType Medal)
    {
        switch(Medal)
        {
            case MedalType.None:
                return _EmptyMedal;
            case MedalType.Bronze: 
                return _BronzeMedal;
            case MedalType.Silver:
                return _SilverMedal;
            case MedalType.Gold:
                return _GoldMedal;
        }
        return _EmptyMedal;
    }
}
