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
        get { return text.activeSelf; }//���낢�����������B�ϐ��łȂ�
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
        //���łɊJ���Ă��� or �t���O�������Ă���
        if (text.activeSelf || null != ObjFlag) return false;

        //�e�L�X�g�\��
        text.SetActive(true);

        //�J���[�ݒ�
        cube.GetComponent<Renderer>().material.color =
            new Color32(215, 185, 153, 255);

        //���e
        if(IsMine)
        {
            cube.GetComponent<Renderer>().material.color = Color.red;
        }
        else if(0 < MineCount)
        {
            //�����\��
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
