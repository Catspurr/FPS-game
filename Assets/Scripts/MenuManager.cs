using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    [SerializeField] private Menu[] menus;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        foreach (var menu in menus)
        {
            if (menu.menuName == menuName)
            {
                menu.Open();
            }
            else if (menu.open)
            {
                CloseMenu(menu);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        foreach (var vMenu in menus)
        {
            if (vMenu.open)
            {
                CloseMenu(vMenu);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}