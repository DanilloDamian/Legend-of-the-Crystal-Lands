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
    public int itemID;
    public GameObject textPriceObject;

    void Start()
    {
        _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        myButton = this.GetComponent<Button>();
        Text textPrice = textPriceObject.GetComponent<Text>();
        if (textPrice != null)
        {
            textPrice.text = priceItem.ToString();
        }

    }

    void Update()
    {

        if (priceItem > _gameManager.diamonds)
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

    public void BuyItem()
    {
        switch (itemID)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                _gameManager.BuyExtraLife(priceItem);
                break;
            case 3:
                _gameManager.BuyBonusDamage(priceItem);
                break;
        }
    }

}
