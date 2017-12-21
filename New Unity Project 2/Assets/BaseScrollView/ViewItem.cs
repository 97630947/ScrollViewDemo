using UnityEngine;


public abstract class ViewItem : MonoBehaviour
{
    /// <summary>
    /// 这个item的索引
    /// </summary>
    public int ViewItemIndex { private set; get; }


    /// <summary>
    /// 这个组件被点击
    /// </summary>
    /// <param name="isMine">是否是我自己发的消息</param>
    public abstract void OnReceived(bool isMine);

    protected abstract void OnStart();
    protected abstract void OnInit();
    protected abstract void OnRemove();

    /// <summary>
    /// 刷新数据
    /// </summary>
    /// <param name="data">具体数据</param>
    protected abstract void RefreshChildData(object data);

    /// <summary>
    /// 管理对象
    /// </summary>
    private BaseScrollView _baseScrollView;

    /// <summary>
    /// 初始化item属性
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="view">管理对象</param>
    public void InitViewItem(int index, BaseScrollView view)
    {
        ViewItemIndex = index;
        _baseScrollView = view;
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    /// <param name="data"></param>
    public void RefreshData(object data)
    {
        RefreshChildData(data);
    }

    private void Start()
    {
        OnStart();
        OnInit();
    }

    private void OnDestroy()
    {
        OnRemove();
    }

    /// <summary>
    /// 发送消息给其他同级item
    /// </summary>
    protected void SendAllViewItem()
    {
        _baseScrollView.SendAllViewItemClick(ViewItemIndex);
    }
}