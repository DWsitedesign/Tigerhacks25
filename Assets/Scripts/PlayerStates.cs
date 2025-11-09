using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStates : MonoBehaviour
{

    [Header("Player Health Settings")]
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private Slider healthBar;
    [Header("Player Money Settings")]
    [SerializeField] private int money = 100;
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        health = maxHealth;
        healthBar.value = health/(float)maxHealth;
        moneyText.text = "Money: $" + money;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Debug.Log("Player is dead.");
        }
        Debug.Log("Player took " + damage + " damage. Current health: " + health);
        healthBar.value = health / (float)maxHealth;
    }
    
    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        Debug.Log("Player healed " + amount + " health. Current health: " + health);
        healthBar.value = health / (float)maxHealth;
    }
}
