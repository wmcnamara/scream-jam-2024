using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private AudioClip doorLockedInteractSfx;
    [SerializeField] private bool switchOpenDirection;
    [SerializeField] private KeyType keyType;

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
        if (keyType == KeyType.UNLOCKED)
        {
            ToggleDoor();
        }
        else
        {
            if (interactData.interactingPlayer.Inventory.HasKey(keyType))
            {
                doorSource.PlayOneShot(doorUnlockSfx);
                UnlockDoor();
            }
            else
            {
                doorSource.PlayOneShot(doorLockedInteractSfx);
                return;
            }
        }
    }
    
    private void UnlockDoor()
    {
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
