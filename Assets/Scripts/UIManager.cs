using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mapOptionsMenu;
    public GameObject hideMapOptionsBtn;
    public GameObject showMapOptionsBtn;

    public GameObject searchOptionsMenu;
    public GameObject hideSearchOptionsBtn;
    public GameObject showSearchOptionsBtn;

    public GameObject shortcutMenu;
    public GameObject hideShortcutBtn;
    public GameObject showShortcutBtn;
    public GameObject shortCutMenuHeader;

    public Text seedText;

    public void HideMapOptions()
    {
        mapOptionsMenu.SetActive(false);
        hideMapOptionsBtn.SetActive(false);
        showMapOptionsBtn.SetActive(true);
    }

    public void ShowMapOptions()
    {
        mapOptionsMenu.SetActive(true);
        hideMapOptionsBtn.SetActive(true);
        showMapOptionsBtn.SetActive(false);
    }

    public void HideSearchOptions()
    {
        searchOptionsMenu.SetActive(false);
        hideSearchOptionsBtn.SetActive(false);
        showSearchOptionsBtn.SetActive(true);
    }

    public void ShowSearchOptions()
    {
        searchOptionsMenu.SetActive(true);
        hideSearchOptionsBtn.SetActive(true);
        showSearchOptionsBtn.SetActive(false);
    }

    public void HideShortcutsMenu()
    {
        shortcutMenu.SetActive(false);
        hideShortcutBtn.SetActive(false);
        showShortcutBtn.SetActive(true);

        shortCutMenuHeader.transform.localPosition = new Vector3(0f, -87.5f); // since Shortcuts menu is at the bottom, when collapsed we want the header to go down so it looks better.
    }

    public void ShowShortcutsMenu()
    {
        shortcutMenu.SetActive(true);
        hideShortcutBtn.SetActive(true);
        showShortcutBtn.SetActive(false);

        shortCutMenuHeader.transform.localPosition = new Vector3(0f, 87.5f);
    }


    public void CopySeed()
    {
        string seedString = seedText.text;
        seedString = seedString.Substring(6);
        GUIUtility.systemCopyBuffer = seedString; // copies seed to clipboard.
    }
}
