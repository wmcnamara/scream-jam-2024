using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Image crosshair;
    [SerializeField] private TextMeshProUGUI keyIndicator;

    public TextMeshProUGUI KeyIndicator 
    {
        get { return keyIndicator; }
    }

    public Image Crosshair
    {
        get
        {
            return crosshair;
        }

        private set
        {
            crosshair = value;
        }
    }
}
