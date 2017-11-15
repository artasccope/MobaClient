using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefTest : MonoBehaviour {
    int i;

	// Use this for initialization
	void Start () {
        int j = 7;

        TestRef(ref j);

        Debug.Log(i);

        Debug.Log(Vector3.forward);//Z方向forward为正前方
        Debug.Log(Vector3.right);//右边为x正方向
	}

    void TestRef(ref int outI) {
        
        i = outI;

    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(i);
	}
}
