using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementManager : MonoBehaviour
{
    private GameObject elementMenu;

    private void Start()
    {
        elementMenu = gameObject.transform.GetChild(0).gameObject;

        PlayerBehaviour.OpenElements += Open;
        PlayerBehaviour.CloseElements += Close;
    }

    private void OnDestroy()
    {
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
