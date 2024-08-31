using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField]
    private int priceItem;
    [SerializeField]
    private Image imageDiamond;
    private Button myButton;


    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        myButton = this.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(priceItem > _gameManager.diamonds)
        {      

            myButton.interactable = false;           
            Text text = myButton.GetComponentInChildren<Text>();

            imageDiamond.color = new Color(imageDiamond.color.r, imageDiamond.color.g, imageDiamond.color.b, 0.5f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.5f);

        }
        else
        {
            myButton.interactable = true;
        }

    }
}
