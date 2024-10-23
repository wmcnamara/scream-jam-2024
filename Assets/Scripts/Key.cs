using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyType
{
    UNLOCKED,
    LOCKED_KEY1,
    LOCKED_KEY2,
    LOCKED_KEY3,
    LOCKED_KEY4,
    LOCKED_KEY5,
}

//Actual monobehaviour prop class for the key
[RequireComponent(typeof(AudioSource))]
public class Key : MonoBehaviour, IInteractable
{
    [SerializeField] private KeyType keyType;
    [SerializeField] private AudioClip onKeyCollectedSfx;

    private AudioSource source;
    private bool hasBeenPickedUp = false;

    public bool CanBeInteractedWith()
    {
        return !hasBeenPickedUp;
    }

    public void Interact(InteractData interactData)
    {    
        interactData.interactingPlayer.Inventory.AddKey(keyType);

        //This isnt the best way. We should instantiate a new object and play the sound on it but this is faster
        hasBeenPickedUp = true;
        GetComponentInChildren<Renderer>().enabled = false;
        Invoke(nameof(DestroySelf), 3.0f);
    }

    private void Awake()
    {
        Debug.Assert(keyType != KeyType.UNLOCKED, "The keytype on the key is set to unlocked. This makes the key useless");
        source = GetComponent<AudioSource>();
    }
    
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
