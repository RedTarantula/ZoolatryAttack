using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerStatusPanel : MonoBehaviour
{
    public Text t;
   public void SetStatus(string status)
    {
        t.text = status + "...";
    }
}
