using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollLoopController : UIBehaviour {
    public Vector2 CellSize = new Vector2(50, 50);
    public Vector2 cellOffset;

    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private int numberOfColumns = 1; 

    public List<GameObject> _go = new List<GameObject>(); 

    private int visibleCellsRowCount;
    private int visibleCellsTotalCount;

    private int preFirstVisibleIndex ;
    private int firstVisibleIndex;
    public IList allData;
    private bool initFinish;
    private int preNumberOfColumns;
    private Vector2 contentPos;
    private Vector2 initCellSize ;
    private Vector2 initCellOffset;

    private LinkedList<ViewItem> localCellsPool = new LinkedList<ViewItem>();
    private LinkedList<ViewItem> cellsInUse = new LinkedList<ViewItem>();

    private bool horizontal {
        get { return scrollRect.horizontal; }
    }

    public void initWithData(IList cellDataList) {
        if(!initFinish) {
            initCellSize = CellSize;
            initCellOffset = cellOffset;
            initData();
            SetCellsPool();
            initFinish = true;
        }
        contentPos = scrollRect.content.anchoredPosition;
        preNumberOfColumns = numberOfColumns;
        allData = cellDataList;
        SetInUseCells(0);
        int showCount = visibleCellsTotalCount > cellDataList.Count ? cellDataList.Count : visibleCellsTotalCount;
        for(int i = 0; i < showCount; i++) {
            ShowCell(i , true);
        }
    }
    public int GetvisibleCellsTotalCount()
    {
        if (visibleCellsTotalCount == 0)
        {
            if (horizontal)
            {
                visibleCellsRowCount = Mathf.CeilToInt(scrollRect.viewport.rect.width / CellSize.x);

                numberOfColumns = (int)(scrollRect.viewport.rect.height / CellSize.y);
                float cellHeight = CellSize.y;
                CellSize.y = scrollRect.viewport.rect.height / numberOfColumns;
                cellOffset.y = (CellSize.y - (cellHeight - cellOffset.y * 2)) / 2;
            }
            else
            {
                visibleCellsRowCount = Mathf.CeilToInt(scrollRect.viewport.rect.height / CellSize.y);
                numberOfColumns = (int)(scrollRect.viewport.rect.width / CellSize.x);
                float cellWidth = CellSize.x;
                CellSize.x = scrollRect.viewport.rect.width / numberOfColumns;
                //cellOffset.x = (CellSize.x - (cellWidth - cellOffset.x * 2)) / 2;
            }
            visibleCellsTotalCount = (visibleCellsRowCount + 1) * numberOfColumns;
        }
        return visibleCellsTotalCount;

    }
    void initData() {
        CellSize = initCellSize;
        cellOffset = initCellOffset;
        if(horizontal) {
            visibleCellsRowCount = Mathf.CeilToInt(scrollRect.viewport.rect.width / CellSize.x);

                numberOfColumns = (int)(scrollRect.viewport.rect.height / CellSize.y);
                float cellHeight = CellSize.y;
                CellSize.y = scrollRect.viewport.rect.height / numberOfColumns;
                cellOffset.y = (CellSize.y - (cellHeight - cellOffset.y * 2)) / 2;
        } else {
            visibleCellsRowCount = Mathf.CeilToInt(scrollRect.viewport.rect.height / CellSize.y);
                numberOfColumns = (int)(scrollRect.viewport.rect.width / CellSize.x);
                float cellWidth = CellSize.x;
                CellSize.x = scrollRect.viewport.rect.width / numberOfColumns;
                //cellOffset.x = (CellSize.x - (cellWidth - cellOffset.x * 2)) / 2;
        }
        visibleCellsTotalCount = (visibleCellsRowCount + 1) * numberOfColumns;
    }

    protected override void OnRectTransformDimensionsChange() {
        if(initFinish) {
            initData();
            SetInUseCells(visibleCellsTotalCount);
            SetCellsPool();
            if(preNumberOfColumns == numberOfColumns) {
                Refresh();
            } else {
                scrollRect.content.anchoredPosition = contentPos;
                initWithData(allData);
            }
        }
    }

    void Update () {
        if (allData != null)
        {
            CalculateCurrentIndex();
            InternalCellsUpdate();
        }
    }

    void ShowCell(int cellIndex , bool scrollingPositive) {
        ViewItem tempCell = GetCellFromPool(scrollingPositive);
        PositionCell(tempCell.gameObject, cellIndex);
        if(cellIndex < allData.Count) {
            tempCell.gameObject.SetActive(true);
            ViewItem scrollableCell = tempCell.GetComponent<ViewItem>();
            scrollableCell.RefreshData(allData[cellIndex]);
        } else {
            tempCell.gameObject.SetActive(false);
        }
    }

    void CalculateCurrentIndex() {
        if(horizontal)
            firstVisibleIndex = (int)(-scrollRect.content.anchoredPosition.x / CellSize.x);
        else
            firstVisibleIndex = (int)(scrollRect.content.anchoredPosition.y / CellSize.y);
        int limit = Mathf.CeilToInt(allData.Count / (float)numberOfColumns) - visibleCellsRowCount;
        if (firstVisibleIndex < 0 || limit <= 0)
            firstVisibleIndex = 0;
        else if(firstVisibleIndex >= limit) {
            firstVisibleIndex = limit - 1;
        }
    }

    void InternalCellsUpdate() {
        if(preFirstVisibleIndex != firstVisibleIndex) {
            bool scrollingPositive = preFirstVisibleIndex < firstVisibleIndex;
            int indexDelta = Mathf.Abs(preFirstVisibleIndex - firstVisibleIndex);

            int deltaSign = scrollingPositive ? +1 : -1;

            for(int i = 1; i <= indexDelta; i++)
                UpdateContent(preFirstVisibleIndex + i * deltaSign, scrollingPositive);

            preFirstVisibleIndex = firstVisibleIndex;
        }
    }

    void UpdateContent(int cellIndex, bool scrollingPositive) {
        int index = scrollingPositive ? ((cellIndex - 1) * numberOfColumns) + (visibleCellsTotalCount) : (cellIndex * numberOfColumns);
        for(int i = 0; i < numberOfColumns; i++) {
            FreeCell(scrollingPositive);
            ShowCell(index + i, scrollingPositive);
        }
    }

    void FreeCell(bool scrollingPositive) {
        LinkedListNode<ViewItem> cell = null;
        if(scrollingPositive) {
            cell = cellsInUse.First;
            cellsInUse.RemoveFirst();
            localCellsPool.AddLast(cell);
        } else {
            cell = cellsInUse.Last;
            cellsInUse.RemoveLast();
            localCellsPool.AddFirst(cell);
        }
     //   cell.Value.gameObject.SetActive(false);
    }

    ViewItem GetCellFromPool(bool scrollingPositive) {
        if(localCellsPool.Count == 0)
            return null;
        LinkedListNode<ViewItem> cell = localCellsPool.First;
        localCellsPool.RemoveFirst();

        if(scrollingPositive)
            cellsInUse.AddLast(cell);
        else
            cellsInUse.AddFirst(cell);
        return cell.Value;
    }

    void PositionCell(GameObject go, int index) {
        int rowMod = index % numberOfColumns;
        if(horizontal)
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(CellSize.x * (index / numberOfColumns) + cellOffset.x, -rowMod * CellSize.y + cellOffset.y);
        else
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(CellSize.x * rowMod + cellOffset.x, -(index / numberOfColumns) * CellSize.y + cellOffset.y);
    }

    void SetInUseCells(int visibleCellCount) {
        int outSideCount = cellsInUse.Count - visibleCellCount;
        while(outSideCount > 0) {
            outSideCount--;
            cellsInUse.Last.Value.gameObject.SetActive(false);
            localCellsPool.AddLast(cellsInUse.Last.Value);
            cellsInUse.RemoveLast();
        }
    }

    void SetCellsPool() {
        int outSideCount = localCellsPool.Count + cellsInUse.Count - visibleCellsTotalCount;
        print(outSideCount + "outSideCount");
        if(outSideCount > 0) {
            while(outSideCount > 0) {
                outSideCount--;
                LinkedListNode<ViewItem> cell = localCellsPool.Last;
                localCellsPool.RemoveLast();
                Destroy(cell.Value.gameObject);
            }
        } else if(outSideCount < 0) {
            for(int i=0; i< -outSideCount; i++ ) {
                localCellsPool.AddLast(_go[i].GetComponent<ViewItem>());
                _go[i].transform.SetParent(scrollRect.content.transform , false);
            }
        }
    }

    public void Refresh() {
        int allColumns = Mathf.CeilToInt(allData.Count / (float)numberOfColumns);
        int maxFirstIndex = allColumns - visibleCellsRowCount - 1;
        if(maxFirstIndex >= 0)
            firstVisibleIndex = firstVisibleIndex > maxFirstIndex ? maxFirstIndex : firstVisibleIndex;
        preFirstVisibleIndex = firstVisibleIndex;

        SetInUseCells(0);
        int needShowCount = allData.Count - firstVisibleIndex * numberOfColumns;
        needShowCount = needShowCount > visibleCellsTotalCount ? visibleCellsTotalCount : needShowCount;
        for(int i = 0; i < needShowCount; i++) {
            ShowCell(i + firstVisibleIndex * numberOfColumns, true);
        }
    }
}
