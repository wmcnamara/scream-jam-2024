using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartGameIndicatorText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "Find all 3 keys to escape!!!";
        Destroy(gameObject, 4.0f);
    }
}
