using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class NewBehaviourScript : MonoBehaviour {
    public Button m_TestButton;
	// Use this for initialization
	void Start () {
        m_TestButton.onClick.AddListener(delegate() { Debug.Log("点击"); });   

	}
	
	// Update is called once per frame
	void Update () {
	
	}

   void Test(System.Action fun)
    {

    }
    void Test1()
   {

   }
}
