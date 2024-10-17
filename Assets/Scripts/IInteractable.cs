using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InteractData
{
    PlayerMovement interactingPlayer;
}

public interface IInteractable
{
    void Interact(InteractData interactData);
    bool CanBeInteractedWith();
}
