using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GalleryImage : MonoBehaviour
{

    public string description;
    public Sprite imageSprite;

    //Reference to Full Size Image & Text 
    public Image fullImage;
    public TextMeshProUGUI imageText;
    public GameObject fullSizePanel;
    // Start is called before the first frame update
    void Start()
    {
        fullSizePanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MaximizeImage()
    {
        fullSizePanel.SetActive(true);
        fullImage.sprite = imageSprite;
        imageText.text = description.ToString();

    }
}
