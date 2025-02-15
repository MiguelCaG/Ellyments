using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerActionsData", menuName = "PlayerActions/Player Actions Data")]
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
}
