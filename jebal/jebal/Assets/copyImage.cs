using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class copyImage : MonoBehaviour {
    [DllImport("user32.dll")]
    private static extern int GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern long GetWindowLong(int hwnd, int nIndex);
    [DllImport("user32.dll")]
    static extern int SetWindowLong(int hwnd, int nIndex, long dwNewLong);
    [DllImport("user32.dll")]
    public static extern bool SetWindowText(int hwnd, System.String lpString);
        
    string gestureName;
    Texture handGesture;

    Material material;

    public GameObject thumb;
    public GameObject index;
    public GameObject twoFinger;
    public GameObject threeFinger;
    public GameObject fourFinger;
    public GameObject hand;
    public GameObject grab;
    public GameObject L;
    public GameObject ILY;
    public GameObject def;
  
    void Start () {
        Screen.SetResolution(80 * 16 / 10, 80, false);
    
        int handle = GetActiveWindow();
        long lCurStyle = GetWindowLong(handle, -16);
        lCurStyle &= ~524288;
        SetWindowLong(handle, -16, lCurStyle); //시스템메뉴 삭제

        thumb = GameObject.Find("thumb") as GameObject;
        index = GameObject.Find("index") as GameObject;
        twoFinger = GameObject.Find("twoFinger") as GameObject;
        threeFinger = GameObject.Find("threeFinger") as GameObject;
        fourFinger = GameObject.Find("fourFinger") as GameObject;
        hand = GameObject.Find("hand") as GameObject;
        grab = GameObject.Find("grab") as GameObject;
        L = GameObject.Find("L") as GameObject;
        ILY = GameObject.Find("ILY") as GameObject;
        def = GameObject.Find("def") as GameObject;

    }
	

	void Update () {
         if (Input.GetKey(KeyCode.A))
        {
            appear(thumb);
         
        }
        else if (Input.GetKey(KeyCode.S))
        {
            appear(index);
          
        }
        else if (Input.GetKey(KeyCode.D))
        {
            appear(twoFinger);            
        }
        else if (Input.GetKey(KeyCode.F))
        {
            appear(threeFinger);
        }
        else if (Input.GetKey(KeyCode.G))
        {
            appear(fourFinger);
        }
        else if (Input.GetKey(KeyCode.H))
        {
            appear(hand);
        }
        else if (Input.GetKey(KeyCode.J))
        {
            appear(grab);
        }
        else if (Input.GetKey(KeyCode.K))
        {
            appear(L);
        }
        else if (Input.GetKey(KeyCode.L))
        {
            appear(ILY);
        }
        else
        {
            appear(def);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Application.Quit();
        }

    }

    public void appear(GameObject obj)
    {
        thumb.transform.position = new Vector3(0f, 0f, 2.0f);
        index.transform.position = new Vector3(0f, 0f, 2.0f);
        twoFinger.transform.position = new Vector3(0f, 0f, 2.0f);
        threeFinger.transform.position = new Vector3(0f, 0f, 2.0f);
        fourFinger.transform.position = new Vector3(0f, 0f, 2.0f);
        hand.transform.position = new Vector3(0f, 0f, 2.0f);
        grab.transform.position = new Vector3(0f, 0f, 2.0f);
        L.transform.position = new Vector3(0f, 0f, 2.0f);
        ILY.transform.position = new Vector3(0f, 0f, 2.0f);
        def.transform.position = new Vector3(0f, 0f, 2.0f);

        obj.transform.position = new Vector3(0f, 0f, 0.01f);
    }

}
