using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InteractData
{
    public Player interactingPlayer;
}

public interface IInteractable
{
    void Interact(InteractData interactData);
    bool CanBeInteractedWith();
}
