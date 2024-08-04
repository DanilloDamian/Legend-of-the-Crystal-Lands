using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject[] hearts;

    public void UpdateLifes(int life)
    {
        switch (life)
        {
            case 0:
                hearts[2].SetActive(false);
                hearts[1].SetActive(false);
                hearts[0].SetActive(false);
                break;
            case 1:
                hearts[2].SetActive(false);
                hearts[1].SetActive(false);
                hearts[0].SetActive(true);
                break;
            case 2:
                hearts[2].SetActive(false);
                hearts[1].SetActive(true);
                hearts[0].SetActive(true);
                break;
            case 3:
                hearts[2].SetActive(true);
                hearts[1].SetActive(true);
                hearts[0].SetActive(true);
                break;
        }
    }
}
