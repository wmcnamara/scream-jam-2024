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
    [SerializeField] private bool switchOpenDirection;

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
        IsOpen = !IsOpen;
        progress = 0.0f;

        if (IsOpen)
        {
            doorSource.PlayOneShot(doorOpenSfx); 
        }
        else
        {
            doorSource.PlayOneShot(doorCloseSfx);
        }
    }

    public bool CanBeInteractedWith()
    {
        return true;
    }
}
