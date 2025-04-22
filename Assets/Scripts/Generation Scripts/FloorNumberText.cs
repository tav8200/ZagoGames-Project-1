using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloorNumberText : MonoBehaviour
{
    private int floorNum;

    public TextMeshProUGUI floorNumber;

    private string floorNumberText;

    private void Start()
    {
        floorNumberText = PlayerPrefs.GetInt("floorNumber", floorNum).ToString();
        floorNumber.text = "Floor:" + floorNumberText;
    }
}
