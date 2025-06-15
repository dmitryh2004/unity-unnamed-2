using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockController : Interactable
{
    [SerializeField] int startDifficulty;
    int difficulty;
    bool active = true;
    [SerializeField] GameObject screen;
    MeshRenderer screenRenderer;
    Rigidbody rb;
    [SerializeField] TMP_Text difficultyText;
    [SerializeField] List<LockController> linkedLocks = new();
    [SerializeField] List<Lockable> lockables = new();

    private void Start()
    {
        difficulty = startDifficulty;
        rb = GetComponent<Rigidbody>();
        screenRenderer = screen.GetComponent<MeshRenderer>();
        screenRenderer.material = new Material(screenRenderer.material);
        UpdateDifficultyScreen();
    }

    public int GetDifficulty()
    {
        return difficulty;
    }
    
    public override void Interact()
    {
        if (IsActive() && IsHackable())
        {
            HackWindowController.Instance.OpenHackWindow(this);
        }
    }

    public void SetDifficulty(int diff, bool updateLinked = true)
    {
        difficulty = diff;
        StartCoroutine(ChangeDifficultyOnScreenCoroutine(diff));
        if (updateLinked)
        {
            foreach (LockController linked in linkedLocks)
            {
                if (linked.IsActive()) linked.SetDifficulty(diff, updateLinked: false);
            }
        }
    }

    public void IncreaseDifficulty(int diff, bool updateLinked = true)
    {
        difficulty += diff;
        StartCoroutine(ChangeDifficultyOnScreenCoroutine(diff));
        if (updateLinked)
        {
            foreach (LockController linked in linkedLocks)
            {
                if (linked.IsActive()) linked.IncreaseDifficulty(diff, updateLinked: false);
            }
        }
    }

    IEnumerator ChangeDifficultyOnScreenCoroutine(int diff)
    {
        screen.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        UpdateDifficultyScreen();
    }

    public bool IsActive()
    {
        return active;
    }

    public bool IsHackable()
    {
        return difficulty < 11;
    }

    public void UpdateDifficultyScreen()
    {
        screen.SetActive(active);
        if (!active) return;

        if (IsHackable()) 
        { 
            difficultyText.text = $"C: {difficulty}";
            difficultyText.color = new Color(Mathf.Clamp01(-0.25f + difficulty * 0.25f), Mathf.Clamp01(2.5f - difficulty * 0.25f), 0f);
        }
        else
        {
            difficultyText.text = $"Locked";
            difficultyText.color = new Color(1f, 0f, 0f);

            Debug.Log(screenRenderer.material.color);
            screenRenderer.material.color = new Color(.5f, 0f, 0f);
            screenRenderer.material.SetColor("_EmissionColor", new Color(.25f, 0f, 0f));
        }
    }

    public void DisableLock(bool updateLinked = true)
    {
        active = false;
        rb.useGravity = true;
        UpdateDifficultyScreen();
        foreach (LockController linked in linkedLocks)
        {
            if (linked.IsActive()) linked.DisableLock(updateLinked: false);
        }
        foreach (Lockable l in lockables)
        {
            l.UpdateLocked();
        }
    }
}
