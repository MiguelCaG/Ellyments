using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityManager", menuName = "PlayerInfo/Ability Manager")]
public class AbilityManager : ScriptableObject
{
    public enum Ability
    {
        DoubleJump,
        Fireball,
        BubbleShield,
        Dash,
        RockPunch
    }

    [System.Serializable]
    public class AbilityState
    {
        public Ability ability;
        public bool unlocked;

        public AbilityState(Ability ability, bool unlocked)
        {
            this.ability = ability;
            this.unlocked = unlocked;
        }
    }

    public List<AbilityState> abilities = new List<AbilityState>();

    private void OnEnable()
    {
        if (abilities == null || abilities.Count == 0)
        {
            foreach (Ability ability in System.Enum.GetValues(typeof(Ability)))
            {
                abilities.Add(new AbilityState(ability, false));
            }
        }
    }

    /// <summary>
    /// Unlocks a specific ability.
    /// </summary>
    /// <param name="ability">Ability to unlock.</param>
    public void UnlockAbility(Ability ability)
    {
        var abilityState = abilities.Find(a => a.ability == ability);
        if (abilityState != null)
        {
            abilityState.unlocked = true;
            SaveState();
        }
    }

    /// <summary>
    /// Test if an ability is unlocked.
    /// </summary>
    /// <param name="ability">Ability to test.</param>
    /// <returns>True is unlocked, False if not.</returns>
    public bool IsAbilityUnlocked(Ability ability)
    {
        var abilityState = abilities.Find(a => a.ability == ability);
        return abilityState != null && abilityState.unlocked;
    }

    public void SaveState()
    {
        foreach (var abilityState in abilities)
        {
            PlayerPrefs.SetInt(abilityState.ability.ToString(), abilityState.unlocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void LoadState()
    {
        foreach (var abilityState in abilities)
        {
            abilityState.unlocked = PlayerPrefs.GetInt(abilityState.ability.ToString(), 0) == 1;
        }
    }
}
