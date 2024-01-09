using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public GameObject text;
    public GameObject cube;

    public GameObject ObjFlag;

    public int MineCount;
    public bool IsMine;
    public bool IsOpen
    {
        get { return text.activeSelf; }//いろいろ説明がある。変数でなく
    }
    public Vector2Int Pos;



    // Start is called before the first frame update
    void Start()
    {
        text.SetActive(false);





    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Open()
    {
        //すでに開いている or フラグが立っている
        if (text.activeSelf || null != ObjFlag) return false;

        //テキスト表示
        text.SetActive(true);

        //カラー設定
        cube.GetComponent<Renderer>().material.color =
            new Color32(215, 185, 153, 255);

        //爆弾
        if(IsMine)
        {
            cube.GetComponent<Renderer>().material.color = Color.red;
        }
        else if(0 < MineCount)
        {
            //数字表示
            text.GetComponent<TextMeshPro>().text = "" + MineCount;
            text.GetComponent<TextMeshPro>().color = Color.blue;
            if (2 < MineCount)
            {
                cube.GetComponent<Renderer>().material.color = Color.red;
            }
            else if (1 < MineCount)
            {
                cube.GetComponent<Renderer>().material.color = Color.green;
            }
        }

        return true;

    }


}
