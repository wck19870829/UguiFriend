using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UguiDepthTest : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        var cr=GetComponent<RawImage>().canvasRenderer;
        for (var i= 0;i< cr.materialCount;i++)
        {
            Debug.Log(cr.GetMaterial(i).renderQueue);
        }
	}
}
