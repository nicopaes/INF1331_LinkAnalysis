using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;
using TMPro;

public class PageRankManager : SerializedMonoBehaviour
{
    [TitleGroup("UnityReferences")]
    public Vector3 offsetOrigin;
    public PageGameObject pagePrefab;
    [TitleGroup("WWW")]
    public WorldWideWeb www = new WorldWideWeb();

    public List<PageGameObject> pageObjects = new List<PageGameObject>();

    private void Awake() 
    {
        Reset();
    }

    public void DrawWWW(WorldWideWeb www, bool randomizePos = false)
    {
        for (int i = 0; i < pageObjects.Count; i++)
        {
            if (i < www.pages.Count)
            {
                if (!pageObjects[i].gameObject.activeInHierarchy)
                {
                    pageObjects[i].gameObject.SetActive(true);
                }
            }
            else
            {
                if (pageObjects[i].gameObject.activeInHierarchy)
                {
                    pageObjects[i].gameObject.SetActive(false);
                }
                continue;
            }

            Page page = www.pages[i];
            PageGameObject pageObj = pageObjects[i];
            pageObj.pageName.text = page.pageName;
            Color randomColor = UnityEngine.Random.ColorHSV();            
            pageObj.currentPageRank.text = page.rank.ToString();
            // pageObj.currentPageRank.color = randomColor;
            pageObj.linksCount.text = page.links.Count.ToString();
            // pageObj.randomColor = randomColor;

            int x = i % 5;
            int y = i / 5;


            if (randomizePos)
            {
                pageObj.transform.position = www.pages.Count / 10f * Utils.GetRandomPointInAnnulusInsideBox(new Vector2(0, 0), minRadious: 10f, maxRadius: 300, minX: -260f, maxX: 280, minY: -120f, maxY: 120f);
                if(www.pages.Count >= 25)
                {
                    pageObj.transform.position = new Vector3(pageObj.transform.position.x, pageObj.transform.position.y, i * 5f);
                }
            }
            pageObj.transform.localScale = (float)(page.rank * 10f) * Vector3.one;
        }
        for (int i = 0; i < www.pages.Count; i++)
        {
            Page page = www.pages[i];
            PageGameObject pageGO = pageObjects[i];
            pageGO.links.Clear();
            for (int j = 0; j < page.listInt.Count; j++)
            {
                pageGO.links.Add(pageObjects[page.listInt[j]]);
            }
        }
    }

    [Button]
    public void DrawWWWOvertime()
    {
        DrawWWW(www, true);
        StartCoroutine(www.pageRankObj.PageRankGenerator_Corr((ranks) =>
        {
            www.SetPageRank(ranks.ToArray());
            DrawWWW(www);
        }));
    }

    [TitleGroup("WWW"), Button]
    public void Draw() => DrawWWW(www);

    [Button]
    public void Reset()
    {
        pageObjects.ForEach(pageObj => pageObj.Reset());
    }
}

[System.Serializable]
public class Page
{
    public string pageName;
    public double rank;
    public List<string> links = new List<string>();
    [ShowInInspector, ReadOnly]
    public List<int> listInt = new List<int>();

    public Page()
    {
        this.pageName = "";
        this.links = new List<string>();
        this.listInt = new List<int>();
    }

    public Page(string pageName)
    {
        this.pageName = pageName;
        this.links = new List<string>();
        this.listInt = new List<int>();
    }

    public bool IsPage(string name) => this.pageName == name;
}

[HideDuplicateReferenceBox]
public class WorldWideWeb
{
    [OnValueChanged("IndexWWW", true)]
    public List<Page> pages = new List<Page>();

    // [ShowInInspector, ReadOnly]
    public ArrayList arrayList => new ArrayList(pages.Select(page => page.listInt).ToArray());
    public PageRank pageRankObj;

    [Button]
    public void ComputePageRank() => this.SetPageRank(this.pageRankObj.ComputePageRank());

    [TitleGroup("World Wide Web")]
    [Button]
    public void IndexWWW()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            Page page = pages[i];
            //
            if (page.listInt == null) page.listInt = new List<int>();
            else page.listInt.Clear();
            //
            for (int j = 0; j < page.links.Count; j++)
            {
                // Debug.Log($"Trying to find {page.links[j]} index in the world wide web");
                for (int w = 0; w < pages.Count; w++)
                {
                    if (page.links[j] == page.pageName)
                    {
                        Debug.Log("A page should not link to itself");
                        continue;
                    }
                    if (pages[w].pageName == page.links[j])
                    {
                        // Debug.Log($"FOUND {page.links[j]} index {w} in the WWW");
                        page.listInt.Add(w);
                    }
                }
            }
        }

        pageRankObj = null;
        pageRankObj = new PageRank(this.arrayList);
        double[] results = pageRankObj.ComputePageRank();
        this.SetPageRank(results);
    }

    public void SetPageRank(double[] ranks)
    {
        Debug.Log("Set page rank");
        if (ranks.Length != pages.Count)
        {
            Debug.LogError("Not the same lenght");
            return;
        }

        for (int i = 0; i < ranks.Length; i++)
        {
            pages[i].rank = ranks[i];
        }
    }

    [TitleGroup("World Wide Web")]
    [Button]
    public void RandomWeb(int size)
    {
        pages.Clear();
        for (int i = 0; i < size; i++)
        {
            pages.Add(new Page(Guid.NewGuid().ToString("N") + ".com"));
        }

        for (int i = 0; i < size; i++)
        {
            int randomNumberOfLinks = UnityEngine.Random.Range(0, size);
            for (int j = 0; j < randomNumberOfLinks; j++)
            {
                if (i == j) continue;
                pages[i].links.Add(pages[j].pageName);
            }
        }

        IndexWWW();
    }
}
