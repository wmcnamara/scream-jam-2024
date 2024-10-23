using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour, IInteractable
{
    public bool IsOpen { get; private set; } = false;

    public float OpenSpeed = 0.5f;

    [SerializeField] private AudioClip doorOpenSfx;
    [SerializeField] private AudioClip doorCloseSfx;
    [SerializeField] private AudioClip doorUnlockSfx;
    [SerializeField] private AudioClip lockBreakSfx;
    [SerializeField] private AudioClip doorLockedInteractSfx;
    [SerializeField] private bool switchOpenDirection;
    [SerializeField] private KeyType keyType;
    [SerializeField] private bool requires3Keys = false;
    [SerializeField] private GameObject[] threeLocks;

    // New variable to determine if the door is permanently locked
    [SerializeField] private bool isPermanentlyLocked = false;

    private Vector3 doorOpenRot;
    private Vector3 doorClosedRot;
    private AudioSource doorSource;
    private float progress;

    public void Start()
    {
        doorSource = GetComponent<AudioSource>();
        doorSource.spatialBlend = 1.0f;

        doorClosedRot = transform.localRotation.eulerAngles;
        doorOpenRot = doorClosedRot;
        doorOpenRot.y += switchOpenDirection ? -90f : 90.0f;

        progress = 0.0f;
    }

    private void Update()
    {
        Vector3 lerpTarget = IsOpen ? doorOpenRot : doorClosedRot;

        progress = Mathf.Clamp01(progress + Time.deltaTime * OpenSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(lerpTarget), progress);
    }

    public void Interact(InteractData interactData)
    {
        // Check if the door is permanently locked
        if (isPermanentlyLocked)
        {
            doorSource.PlayOneShot(doorLockedInteractSfx);
            return;
        }

        if (keyType == KeyType.UNLOCKED)
        {
            ToggleDoor();
        }
        else
        {
            if (requires3Keys)
            {
                bool hasAllKeys = true;

                for (int i = (int)KeyType.LOCKED_KEY1; i <= (int)KeyType.LOCKED_KEY3; i++)
                {
                    if (!interactData.interactingPlayer.Inventory.HasKey((KeyType)i))
                    {
                        hasAllKeys = false;
                    }
                    else 
                    {
                        doorSource.PlayOneShot(lockBreakSfx, 0.5f);
                        Destroy(threeLocks[i - 1]);
                    }
                }

                if (hasAllKeys)
                {
                    UnlockDoor();
                }
            }
            else
            {
                if (interactData.interactingPlayer.Inventory.HasKey(keyType))
                {
                    UnlockDoor();
                }
                else
                {
                    doorSource.PlayOneShot(doorLockedInteractSfx);
                    return;
                }
            }
        }
    }

    private void UnlockDoor()
    {
        doorSource.PlayOneShot(doorUnlockSfx);
        keyType = KeyType.UNLOCKED;
    }

    public bool CanBeInteractedWith()
    {
        return true;
    }

    public void ToggleDoor()
    {
        IsOpen = !IsOpen;
        progress = 0.0f;
        doorSource.PlayOneShot(IsOpen ? doorOpenSfx : doorCloseSfx);
    }
}
