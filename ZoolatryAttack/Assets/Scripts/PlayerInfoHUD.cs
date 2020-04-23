using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoHUD : MonoBehaviour
{
    public Text magazineTxt, ammoTxt, hpTxt;
    public Image magazineBar, hpBar;

    public void UpdateHudAmmo(int currentMagazine,int maxMagazine,int ammoInv)
    {
        float ammof = (float)currentMagazine/(float)maxMagazine;
        magazineTxt.text = $"{currentMagazine}/{maxMagazine}";
        magazineBar.fillAmount = ammof;
        ammoTxt.text = ammoInv.ToString();
    }

    public void UpdateHudHP(float currentHP, float maxHP)
    {
        float hpf = currentHP/maxHP;
        int hpc = (int)currentHP;
        int hpm = (int)maxHP;

        hpBar.fillAmount = hpf;
        hpTxt.text = $"{hpc}/{hpm}";
    }
}
