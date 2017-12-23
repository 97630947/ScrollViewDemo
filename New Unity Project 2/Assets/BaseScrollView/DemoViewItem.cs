using UnityEngine;

using UnityEngine.UI;

/// <summary>
/// 我是测试列表数据
/// </summary>
public class ViewDemeItem
    {
        public string Title;
        public string Text;
    }

/// <summary>
/// demoe viewitem
/// </summary>
public class DemoViewItem : ViewItem
{
    public Text m_Text;

    public override void OnReceived(bool isMine)
    {

    }

    protected override void OnStart()
    {

    }

    protected override void OnInit()
    {

    }

    protected override void OnRemove()
    {

    }

    protected override void RefreshChildData(object data)
    {
        Listdata _data = (Listdata)data;
        m_Text.text = _data.txt; ;
    }

    private void OnBtnTextClick(GameObject go)
    {
        SendAllViewItem();
    }
}
