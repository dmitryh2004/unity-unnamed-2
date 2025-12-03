using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JewerlyTable : Interactable
{
    [SerializeField] List<JewerlyTableCraft> crafts = new();
    [SerializeField] PlayerInput playerInput;
    [SerializeField] JewerlyTableUIController jewerlyTableUIController;
    [Range(1, 7)]
    [SerializeField] int level = 1;

    public override void Interact()
    {
        jewerlyTableUIController.ShowWindow();
    }

    public int GetLevel() => level;
    public void SetLevel(int level) => this.level = level;
}
