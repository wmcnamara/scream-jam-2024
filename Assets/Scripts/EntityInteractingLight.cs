using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityInteractingLight : MonoBehaviour
{
    [SerializeField] private Light controllingLight;
    private bool shouldFlicker = false;
    private float timeToNextChange = 0.0f;

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TheEntity>() != null)
        {
            shouldFlicker = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<TheEntity>() != null)
        {
            shouldFlicker = false;
            controllingLight.enabled = true;
        }
    }

    private void Update()
    {
        if (shouldFlicker)
        {
            timeToNextChange -= Time.deltaTime;

            if (timeToNextChange <= 0.0f)
            {
                controllingLight.enabled = !controllingLight.enabled;
                timeToNextChange = Random.Range(0.1f, .5f);
            }
        }
    }
}
