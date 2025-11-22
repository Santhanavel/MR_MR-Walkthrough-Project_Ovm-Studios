using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuPanelManager : MonoBehaviour
{
    public GameObject _menuButtonGO;
    public GameObject _menuGO;
    public GameObject _setupSelectionGO;
    public GameObject _setupButtonGO;
    public GameObject _viewButtonGO;

    private void Awake()
    {
        CloseMenu();
        OpenSetUpSelection();
    }

    #region Menu Control
    public void OpenMenu()
    {
        _menuButtonGO.SetActive(false);
        _menuGO.SetActive(true);
    }
    public void CloseMenu()
    {
        _menuButtonGO.SetActive(true);
        _menuGO.SetActive(false);
    }

    public void OpenSetUp()
    {
        _setupSelectionGO.SetActive(false);
        _viewButtonGO.SetActive(false);
        _setupButtonGO.SetActive(true);

    }
    public void OpenView()
    {
        _setupSelectionGO.SetActive(false);
        _viewButtonGO.SetActive(true);
        _setupButtonGO.SetActive(false);

    }

    void OpenSetUpSelection()
    {
        _setupSelectionGO.SetActive(true);
        _viewButtonGO.SetActive(false);
        _setupButtonGO.SetActive(false);
    }

    #endregion
}
