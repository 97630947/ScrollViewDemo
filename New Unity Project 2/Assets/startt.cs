using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Listdata
{
    public string txt;

    public Listdata(string _txt)
    {
        txt = _txt;
    }
  
}

public class startt : MonoBehaviour {



    public GameObject _BaseScrollViewPrefab;
    // Use this for initialization
    void Start () {

        List<Listdata> _data = new List<Listdata>();
        _data.Add(new Listdata("0"));
        _data.Add(new Listdata("1"));
        _data.Add(new Listdata("2"));
        _data.Add(new Listdata("3"));
        _data.Add(new Listdata("4"));
        _data.Add(new Listdata("5"));
        _data.Add(new Listdata("6"));
        _data.Add(new Listdata("7"));
        _data.Add(new Listdata("8"));
        _data.Add(new Listdata("9"));
        _data.Add(new Listdata("10"));
        _data.Add(new Listdata("11"));
        _data.Add(new Listdata("12"));
        _data.Add(new Listdata("13"));
        _data.Add(new Listdata("00"));
        _data.Add(new Listdata("01"));
        _data.Add(new Listdata("02"));
        _data.Add(new Listdata("03"));
        _data.Add(new Listdata("04"));
        _data.Add(new Listdata("05"));
        _data.Add(new Listdata("06"));
        _data.Add(new Listdata("07"));
        _data.Add(new Listdata("08"));
        _data.Add(new Listdata("09"));
        _data.Add(new Listdata("010"));
        _data.Add(new Listdata("011"));
        _data.Add(new Listdata("012"));
        _data.Add(new Listdata("013"));
        GameObject m_BaseScrollViewPrefab = Instantiate<GameObject>(_BaseScrollViewPrefab);
        m_BaseScrollViewPrefab.transform.SetParent(transform);
        m_BaseScrollViewPrefab.transform.localPosition = Vector3.zero;
        BaseScrollView _BaseScrollView = m_BaseScrollViewPrefab.GetComponent<BaseScrollView>();
        _BaseScrollView.objectsDataArr = new ArrayList(_data);
        _BaseScrollView.RefreshPrefabs(_data);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
