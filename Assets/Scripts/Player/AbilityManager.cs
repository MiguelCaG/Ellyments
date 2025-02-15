using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public enum Ability
    {
        DoubleJump,
        Fireball,
        BubbleShield,
        RockPunch,
        Dash
    }

    private Dictionary<Ability, bool> abilities = new Dictionary<Ability, bool>();

    private void Start()
    {
        foreach (Ability ability in System.Enum.GetValues(typeof(Ability)))
        {
            abilities[ability] = false;
        }
    }

    /// <summary>
    /// Desbloquea una habilidad específica.
    /// </summary>
    /// <param name="ability">Habilidad a desbloquear.</param>
    public void UnlockAbility(Ability ability)
    {
        if (abilities.ContainsKey(ability))
        {
            abilities[ability] = true;
        }
    }

    /// <summary>
    /// Comprueba si una habilidad está desbloqueada.
    /// </summary>
    /// <param name="ability">Habilidad a comprobar.</param>
    /// <returns>True si está desbloqueada, False en caso contrario.</returns>
    public bool IsAbilityUnlocked(Ability ability)
    {
        return abilities.ContainsKey(ability) && abilities[ability];
    }
}
