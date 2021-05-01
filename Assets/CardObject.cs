using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    //reference to sctipable object
    public Card myCard;
    //Text for Hold Button
    [SerializeField]
    private Text text;

    public bool Hold = false;

    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        text.enabled = false;
        UpdateImage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveCard()
    {
        myCard = null;
    }

    public void SetCard(Card newCard)
    {
        myCard = newCard;
    }

    public void HoldCard()
    {
        Hold = !Hold;
        text.enabled = Hold;
    }


    public void UpdateImage()
    {
        image.sprite = myCard.CardFace;
    }
}
