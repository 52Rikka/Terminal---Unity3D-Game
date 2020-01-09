using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Panel : MonoBehaviour {


    public GameObject []panel;
    private Color Selected = new Color(0, 0, 0, 1);
    private Color UnSelected = new Color(0, 0, 0, 100/255f);
    private Image se;
    public bool EnActive;
    void Start () {

        EnActive = false;
        Image a = panel[0].transform.GetChild(0).GetComponent<Image>();
        a.color = Selected;
        for (int i=1;i<4;i++)
        {
            a = panel[i].transform.GetChild(0).GetComponent<Image>();
            a.color = UnSelected;
        }
        se = panel[1].transform.GetChild(0).GetComponent<Image>();
        se.color = Selected;
        //  GameObject.Find("Canvas").SetActive(true);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }
    private int index;
    // Update is called once per frame
    void Update () {

        if (!EnActive)
            return;
        if (Input.GetKeyDown(KeyCode.S))
        {

            if (index < 3)
            {
                index++;
                change();
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {

            if (index > 1)
            {
                index--;
                change();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {

            if (panel[index].name == "Back")
            {
                OpenOrClosePanel();
            }
            else if (panel[index].name == "Quit")
            {
                SceneManager.LoadScene("Menu");
            }
            else if (panel[index].name == "Restart")
            {
                SceneManager.LoadScene("LoadMap");
            }
        }

    }

    void change()
    {
        se.color = UnSelected;
        se = panel[index].transform.GetChild(0).GetComponent<Image>();
        se.color = Selected;
    }

    public void OpenOrClosePanel()
    {
        if (!EnActive)       //如果按下esc键
        {
            EnActive = true;
            gameObject.GetComponent<CanvasGroup>().alpha = (EnActive ? 1 : 0);
            index = 1;
        }
        else
        {
            EnActive = false;
            gameObject.GetComponent<CanvasGroup>().alpha = (EnActive ? 1 : 0);
        }
    }
}
