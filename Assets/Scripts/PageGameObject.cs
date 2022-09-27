using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class PageGameObject : MonoBehaviour
{
    public TMP_Text pageName;
    public TMP_Text currentPageRank;
    public TMP_Text linksCount;
    public Color randomColor;
    public List<LineRenderer> linksRenderers = new List<LineRenderer>();

    public List<PageGameObject> links = new List<PageGameObject>();

    private void Update()
    {
        for (int i = 0; i < linksRenderers.Count; i++)
        {
            if (i < links.Count)
            {
                //
                linksRenderers[i].startWidth = 2f / links.Count;
                linksRenderers[i].endWidth = 2f / links.Count;
                //
                linksRenderers[i].SetPosition(0, this.transform.position);
                linksRenderers[i].SetPosition(1, links[i].transform.position);
                if (!linksRenderers[i].gameObject.activeInHierarchy)
                {
                    linksRenderers[i].gameObject.SetActive(true);
                }
            }
        }
    }

    public void Reset()
    {
        pageName.text = "";
        currentPageRank.text = "1";
        linksCount.text = "0";
        randomColor = Color.white;
        for (int i = 0; i < linksRenderers.Count; i++)
        {
            linksRenderers[i].startWidth = 0.5f;
            linksRenderers[i].endWidth = 0.5f;
            // linksRenderers[i].startColor = Color.white;
            // linksRenderers[i].endColor = Color.white;
            linksRenderers[i].gameObject.SetActive(false);
        }
        links.Clear();
        this.transform.localScale = Vector3.one;
    }
}
