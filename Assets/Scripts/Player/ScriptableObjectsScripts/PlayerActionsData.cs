using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerActionsData", menuName = "PlayerInfo/Player Actions Data")]
public class PlayerActionsData : ScriptableObject
{
    /// <summary>
    /// Represents the player's attack distance preference.
    /// - Higher values indicate long-range attacks (e.g., "Fireball").
    /// - Lower values indicate close-range attacks (e.g., "Rock Punch").
    /// </summary>
    public float attackingDistance = 0f;

    /// <summary>
    /// Represents the player's tendency to attack rather than flee or heal.
    /// </summary>
    public float aggressiveness = 0f;

    /// <summary>
    /// Indicates how likely the player is to dodge Zephyros' Aerial Rush attack in a specific direction.
    /// (Upwards, Middle, Downwards)
    /// </summary>
    public Vector3 dodgeZephyros = new Vector3(0.25f, 0.5f, 0f);

    /// <summary>
    /// Reflects the player's ability to evade or take hits from Zephyros' attacks.
    /// </summary>
    public float hitByZephyros = 0f;

    public void SaveState()
    {
        PlayerPrefs.SetFloat("AttackingDistance", attackingDistance);
        PlayerPrefs.SetFloat("Aggressiveness", aggressiveness);
        PlayerPrefs.SetFloat("DodgeZephyros_Up", dodgeZephyros.x);
        PlayerPrefs.SetFloat("DodgeZephyros_Mid", dodgeZephyros.y);
        PlayerPrefs.SetFloat("DodgeZephyros_Down", dodgeZephyros.z);
        PlayerPrefs.SetFloat("HitByZephyros", hitByZephyros);

        PlayerPrefs.Save();
    }

    public void LoadState()
    {
        attackingDistance = PlayerPrefs.GetFloat("AttackingDistance", 0f);
        aggressiveness = PlayerPrefs.GetFloat("Aggressiveness", 0f);
        float x = PlayerPrefs.GetFloat("DodgeZephyros_Up", 0.25f);
        float y = PlayerPrefs.GetFloat("DodgeZephyros_Mid", 0.5f);
        float z = PlayerPrefs.GetFloat("DodgeZephyros_Down", 0f);
        dodgeZephyros = new Vector3(x, y, z);
        hitByZephyros = PlayerPrefs.GetFloat("HitByZephyros", 0f);
    }
}
