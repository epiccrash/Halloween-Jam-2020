using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Color normalColor;
    private Color hoverColor;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();

        normalColor = image.color;
        hoverColor = new Color(0, 0, 0, 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        image.color = normalColor;
    }

    public void LoadLevel(string levelString)
    {
        SceneManager.LoadScene(levelString);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
