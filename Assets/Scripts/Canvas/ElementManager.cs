using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AbilityManager;

public class ElementManager : MonoBehaviour
{
    [SerializeField] private AbilityManager aM;

    private GameObject elementMenu;

    [SerializeField] private GameObject[] elements;
    [SerializeField] private Sprite[] elemSprites;

    [SerializeField] private Image elemSelected;

    private Ability[] abilities = { Ability.Fireball, Ability.BubbleShield, Ability.Dash, Ability.RockPunch };

    private void Start()
    {
        elementMenu = gameObject.transform.GetChild(0).gameObject;

        PlayerBehaviour.ElemChanged += ElementChanged;
        PlayerBehaviour.OpenElements += Open;
        PlayerBehaviour.CloseElements += Close;
    }

    private void Update()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].GetComponent<Button>().interactable = aM.IsAbilityUnlocked(abilities[i]);
            elements[i].GetComponent<Image>().color = aM.IsAbilityUnlocked(abilities[i]) ? Color.white : Color.black;
        }
    }

    private void ElementChanged(PlayerBehaviour.Element element)
    {
        if (element == PlayerBehaviour.Element.None)
        {
            elemSelected.enabled = false;
            return;
        }

        elemSelected.enabled = true;
        elemSelected.sprite = elemSprites[(int)element]; ;
    }

    private void OnDestroy()
    {
        PlayerBehaviour.ElemChanged -= ElementChanged;
        PlayerBehaviour.OpenElements -= Open;
        PlayerBehaviour.CloseElements -= Close;
    }

    private void Open()
    {
        if (!elementMenu.activeSelf)
        {
            elementMenu.SetActive(true);
            Time.timeScale = 0.5f;
        }
    }
    private void Close()
    {
        elementMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
