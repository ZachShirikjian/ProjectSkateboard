using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GalleryImage : MonoBehaviour
{

    public string description;
    public Sprite imageSprite;

    //Reference to Full Size Image & Text 
    public Image fullImage;
    public TextMeshProUGUI imageText;
    public GameObject fullSizePanel;
    public GameObject closeButton;
    
    //Reference to PreviewImage
    public Image previewImage;
    // Start is called before the first frame update
    void Start()
    {
        fullSizePanel.SetActive(false);
    }
    public void ChangePreviewImage()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
        previewImage.sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
    }

    public void MaximizeImage()
    {
        fullSizePanel.SetActive(true);
        fullImage.sprite = imageSprite;
        imageText.text = description.ToString();
        EventSystem.current.SetSelectedGameObject(closeButton);
    }
}
