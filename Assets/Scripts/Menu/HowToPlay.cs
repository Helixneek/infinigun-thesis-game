using System.Collections.Generic;
using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;
    private int index = 0;

    public void OpenPrevious()
    {
        if(index - 1 <= -1) { return; }

        pages[index].SetActive(false);
        index--;
        pages[index].SetActive(true);
    }

    public void OpenNext()
    {
        if (index + 1 >= pages.Count) { return; }

        pages[index].SetActive(false);
        index++;
        pages[index].SetActive(true);
    }
}
