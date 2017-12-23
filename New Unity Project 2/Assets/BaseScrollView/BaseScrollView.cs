using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 暂时只支持6个，因为多列工具没有封装
/// 除非新需要在写
/// </summary>
public class BaseScrollView : MonoBehaviour
{
    /// <summary>
    /// 生成的prefabs
    /// </summary>
    public GameObject ViewItem;

    /// <summary>
    /// 列表数据
    /// </summary>
    public List<ViewItem> ListData { private set; get; }

    /// <summary>
    /// 管理列表
    /// </summary>
    private ScrollRect _scrollView;

    /// <summary>
    /// 排列方式
    /// </summary>
    private GridLayoutGroup _grid;

    /// <summary>
    /// 排列路径
    /// </summary>
    private const string GridPath = "Viewport/Content";

    /// <summary>
    /// 创建的列表数据
    /// </summary>
    public ArrayList objectsDataArr = null;

    private void Awake()
    {
        ListData = new List<ViewItem>();
        _scrollView = GetComponent<ScrollRect>();
        _grid = transform.Find(GridPath).GetComponent<GridLayoutGroup>();
    }

    /// <summary>
    /// 设置Grid宽高
    /// </summary>
    /// <param name="count">数据总长度</param>
    private void SetScrollView(int count)
    {
        var rectTrans = _grid.GetComponent<RectTransform>();
        var scrollTrans = _scrollView.GetComponent<RectTransform>();
        rectTrans.sizeDelta = scrollTrans.sizeDelta;

        //排列数量
        var constraint = _grid.constraintCount;
        //预制宽度
        var width = _grid.cellSize.x;
        //预制高度
        var heigth = _grid.cellSize.y;

        switch (_grid.constraint)
        {
            //纵向排列
            case GridLayoutGroup.Constraint.FixedColumnCount:
                // ReSharper disable once PossibleLossOfFraction
                rectTrans.sizeDelta = new Vector2(x: width * constraint, y: heigth * Mathf.Ceil((float)count / constraint));
                break;
            //横向排列排列
            case GridLayoutGroup.Constraint.FixedRowCount:
                // ReSharper disable once PossibleLossOfFraction
                rectTrans.sizeDelta = new Vector2(x: Mathf.Ceil((float)count / constraint), y: heigth * constraint);
                break;
        }
    }

    /// <summary>
    /// 刷新数据重新创建prefabs
    /// </summary>
    /// <param name="listData"></param>
    public void RefreshPrefabs<T>([NotNull]IEnumerable<T> listData)
    {
        if (listData == null) return;
        var enumerable = listData as T[] ?? listData.ToArray();

        SetScrollView(enumerable.Length);
        ViewNotNull();
        CreateViewItem(enumerable);
        DeleteOldViewItem(enumerable);
    }

    /// <summary>
    /// 对所有子物体发起点击事件
    /// </summary>
    /// <param name="index">点击的索引</param>
    public void SendAllViewItemClick(int index)
    {
        for (var i = 0; i < ListData.Count; i++)
        {
            var viewItem = ListData[i];
            viewItem.OnReceived(index == i);
        }
    }

    /// <summary>
    /// 检查ViewItem是否为空
    /// </summary>
    private void ViewNotNull()
    {
        if (ViewItem == null)
        {
            throw new Exception("未传递ViewItem");
        }
    }

    /// <summary>
    /// 创建列表
    /// </summary>
    private void CreateViewItem<T>(IList<T> objects)
    {
        //添加数据
        for (var i = 0; i < objects.Count; i++)
        {
            if (i < ListData.Count)
            {
                ListData[i].RefreshData(objects[i]);
            }

            else if (GetComponent<ScrollLoopController>() == null || GetComponent<ScrollLoopController>()._go.Count < GetComponent<ScrollLoopController>().GetvisibleCellsTotalCount())
            {
                var obj = InstancePrefabs(ViewItem, _grid.transform);
                if (objectsDataArr != null)
                {
                    GetComponent<ScrollLoopController>()._go.Add(obj);
                    if (GetComponent<ScrollLoopController>()._go.Count == GetComponent<ScrollLoopController>().GetvisibleCellsTotalCount())
                    {
                        GetComponent<ScrollLoopController>().initWithData(objectsDataArr);
                        objectsDataArr = null;
                    }
                }
                obj.SetActive(true);
                var itemData = obj.GetComponent<ViewItem>();

                if (itemData == null)
                {
                    throw new Exception("你的ScrollItem没有绑定脚本");
                }

                itemData.InitViewItem(i, this);
                itemData.RefreshData(objects[i]);
                ListData.Add(itemData);
            }
            else if (i == GetComponent<ScrollLoopController>().allData.Count)
            {
                GetComponent<ScrollLoopController>().allData.Add(objects[i]);
            }
        }
    }

    [Obsolete("这个方法建立不要使用很多余")]
    public  GameObject InstancePrefabs(GameObject path, Transform trans = null)
    {
        var go = Instantiate(path);
        go.transform.SetParent(trans);
        InitPrefabs(go);
        return go;
    }


    public  void InitPrefabs(GameObject gObj)
    {
        gObj.transform.localPosition = Vector3.zero;
        gObj.transform.localEulerAngles = Vector3.zero;
        gObj.transform.localScale = Vector3.one;
        gObj.name = gObj.name.Replace("(Clone)", "");
    }

    /// <summary>
    /// 移除多余溢出的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objects"></param>
    private void DeleteOldViewItem<T>(ICollection<T> objects)
    {
        //不需要移除多余的数据
        if (objects.Count >= ListData.Count) return;

        //复制老的数据
        var copyView = new List<ViewItem>();
        for (var i = objects.Count; i < ListData.Count; i++)
            copyView.Add(ListData[i]);

        //移除数据
        foreach (var item in copyView)
        {
            ListData.Remove(item);
            Destroy(item.gameObject);
        }
    }
}