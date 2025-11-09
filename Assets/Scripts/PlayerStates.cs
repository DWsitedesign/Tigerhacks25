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
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private int ammo = 0;
    [SerializeField] private int healthPotion = 0;
    [SerializeField] private TextMeshProUGUI healthPotionUI;
    [SerializeField] private TextMeshProUGUI ammoUI;

    private void Start()
    {
        health = maxHealth;
        healthBar.value = health/(float)maxHealth;
        moneyText.text = "Money: $" + money;
        ammoUI.text = ammo.ToString();
        healthPotionUI.text = healthPotion.ToString();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Debug.Log("Player is dead.");
            gameObject.GetComponent<SideScrollerController>().EnableUI();
            deathMenu.SetActive(true);
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
    public void PickUpAmmo(int amount)
    {
        ammo += amount;
        ammoUI.text = ammo.ToString();
    }
    public bool UseAmmo(int amount)
    {
        if (ammo >= amount)
        {
            ammo -= amount;
            ammoUI.text = ammo.ToString();
            return true;
        }
        else
        {
            Debug.Log("Not enough ammo!");
            return false;
        }
    }
    public void UseHealthPotion(int amount)
    {
        if( healthPotion < amount)
        {
            Debug.Log("Not enough health potions!");
            return;
        }
        Heal(amount * 20); // Each potion heals 20 health
        healthPotion -= amount;
        healthPotionUI.text = healthPotion.ToString();
    }
}
