using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirusController : MonoBehaviour
{
    public static VirusController Instance;
    int currentHP;
    int currentAttack;

    float hpRatio, attackRatio;
    const int maxHP = 150, maxAttack = 45;

    [SerializeField] Image hpBar, attackBar;
    [SerializeField] TMP_Text hpText, attackText;

    [SerializeField] int[] hpByLevel = new int[7];
    [SerializeField] int[] attackByLevel = new int[7];
    [Range(1, 7)][SerializeField] int level;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ResetToStart();
    }

    public void ResetToStart()
    {
        currentHP = hpByLevel[level - 1];
        currentAttack = attackByLevel[level - 1];
        UpdateBars();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) currentHP = 0;
        UpdateBars();
        if (currentHP == 0) HackWindowController.Instance.FailLock();
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        UpdateBars();
    }

    public void RecalculateAttack()
    {
        currentAttack = attackByLevel[level - 1] - HackWindowController.Instance.GetGridController().GetPacifierDebuff();
        if (currentAttack <= 10) currentAttack = 10;
        UpdateBars();
    }

    public int GetCurrentAttack()
    {
        return currentAttack;
    }

    public void UpdateBars()
    {
        hpRatio = (float)currentHP / maxHP;
        attackRatio = (float) currentAttack / maxAttack;

        hpText.text = $"{currentHP}";
        attackText.text = $"{currentAttack}";
    }

    private void Update()
    {
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hpRatio, 0.2f);
        attackBar.fillAmount = Mathf.Lerp(attackBar.fillAmount, attackRatio, 0.2f);
    }
}
