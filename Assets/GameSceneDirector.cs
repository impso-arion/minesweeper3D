using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;//UI���g���̂ŕK�{�A�����A�E�N���b�N�ł�����Ȃ�

public class GameSceneDirector : MonoBehaviour
{
    const int CELL_X = 5;
    const int CELL_Y = 5;
    int mineCount = 3;

    public GameObject prefabCell;
    public GameObject prefabFlag;
    public GameObject result;

    public GameObject prefabOpenEx;
    public GameObject prefabMineEx;




    public Text txtTimer;
    public Text txtFlag;

    public Text txtResult;

    CellController[,] cells;//�Z���𑀍삷�邽�߂̓񎟌��z��
    int flagCount;
    float timer;


    // Start is called before the first frame update
    void Awake()
    {
        
        //�q�I�u�W�F�N�g�擾
        GameObject child = result.transform.Find("TextResult").gameObject;
        txtResult = child.GetComponent<Text>();

        //�Z���̐ݒ�
        cells = new CellController[CELL_X, CELL_Y];

        for(int i = 0; i < CELL_X; i++)
        {
            for(int j = 0; j < CELL_Y; j++)
            {
                float x = i - (CELL_X / 2);
                float y = j - (CELL_Y / 2);

                Vector3 pos = new Vector3(x, 0, y);
                GameObject obj = Instantiate(prefabCell, pos, Quaternion.identity);

                cells[i,j] = obj.GetComponent<CellController>();
                cells[i,j].Pos = new Vector2Int(i,j);

            }
        }

        //���e�̐ݒ�
        int cnt = mineCount;
        while(0 <cnt)
        {
            int x = Random.Range(0, cells.GetLength(0));
            int y = Random.Range(0, cells.GetLength(1));

            //���e���Z�b�g����Ă��Ȃ����
            if (!cells[x, y].IsMine)
            {
                cells[x,y].IsMine = true;

                //����̃Z���̃J�E���g�����
                foreach(var v in GetRoundCell(cells[x,y]))
                {
                    v.MineCount++;
                }
                cnt--;
            }

        }
        flagCount = mineCount;
        txtFlag.text = "" + flagCount;
    }

    // Update is called once per frame
    void Update()
    {
        //�Q�[���N���A���\������Ă����珈�������Ȃ�
        if (result.activeSelf) return;

        //�Q�[���N���A����
        if(isClear() )
        {
            txtResult.text = "���߂łƂ��I";
            result.SetActive(true);
            return;
        }

        //�^�C�}�[
        timer += Time.deltaTime;
        if (999 < timer) timer = 999;
        txtTimer.text = "" + (int)timer;

        //�I�������Ɠ���
        CellController cell = null;
        bool flag = false;

        //�Z���������ꂽ���̏���(�E�N���b�N�ƍ��N���b�N�œ���)
        if(Input.GetMouseButtonUp(0)||Input.GetMouseButtonUp(1))
        {//�N���b�N�œ���ꏊ
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                //�z��ԍ��ɕϊ�
                GameObject obj = hit.collider.gameObject;
                int x = (int)obj.transform.position.x + (CELL_X / 2);
                int y = (int)obj.transform.position.z + (CELL_Y / 2);

                cell = cells[x, y];
                //�t���O�Z�b�g �E�N���b�N��
                if (Input.GetMouseButtonUp(1))
                {
                    Debug.Log("�E�N���b�N���ꂽ");
                    flag = true;
                }

            }

            
        }
        //�t���O�Z�b�g
        if ( flag )
        {
            //���łɃt���O�������Ă���������Ă����
            if (null != cell.ObjFlag)
            {
                Destroy(cell.ObjFlag);
                flagCount++;
                txtFlag.text = "" + flagCount;
                return;
            }
            //�V�����t���O�����Ă�
            if (1 > flagCount) return;
            Vector3 pos = cell.gameObject.transform.position;
            pos.y += 1.0f;

            cell.ObjFlag = Instantiate(prefabFlag, pos, Quaternion.identity);
            flagCount--;
            txtFlag.text = "" + flagCount;
        }
        //�I�[�v��
        else if( null != cell)
        {
            //�I�[�v����
            if (!cell.Open()) return;

            //�n��
            if(cell.IsMine)
            {
                //�n���������Ƃ��̉��o
                foreach(var v in cells)
                {
                    Vector3 force = new Vector3(
                        Random.Range(-1000, 1000),
                        Random.Range(-1000, 1000),
                        Random.Range(-1000, 1000));
                    v.gameObject.AddComponent<Rigidbody>().AddForce(force);
                    if(null != v.ObjFlag)
                    {
                        v.ObjFlag.AddComponent<Rigidbody>().AddForce(force);
                    }



                    //���j���o
                    GameObject obj = 
                        Instantiate(prefabMineEx, v.gameObject.transform.position, Quaternion.identity);
                    Destroy(obj, 1.95f);

                    //�Q�[���I�[�o�[
                    txtResult.text = "����˂�I";
                    result.SetActive(true);

                }
            }
            //������0�ȏゾ������I��
            if (0 < cell.MineCount) return;

            //����̃Z����������
            List<CellController> roundcells = GetRoundCell(cell);
            while(0< roundcells.Count)
            {
                CellController tgt = roundcells[0];

                //�I�[�v��
                if (tgt.Open())
                {
                    //0�Ȃ炻�̎���̃Z�����ǉ�����B�ꏏ�ɂ�����
                    if(0 == tgt.MineCount)
                    {
                        roundcells.AddRange(GetRoundCell(tgt));
                    }
                    //�J�����o
                    Vector3 pos = tgt.gameObject.transform.position;
                    pos.y += 1.0f;
                    GameObject obj =
                        Instantiate(prefabOpenEx, pos, Quaternion.identity);
                    Destroy(obj, 1.5f);
                }
                //���X�g����폜
                roundcells.Remove(tgt);
            }



        }

    }

    //�N���A����
    bool isClear()
    {
        //�I�[�v������Ă��Ȃ��Z�������邩�ǂ���
        foreach (var v in cells)
        {
            if (!v.IsOpen && !v.IsMine) return false;
        }
        return true;
    }

    List<CellController>GetRoundCell(CellController center)
    {
        List<CellController> ret = new List<CellController>();

        //����
        List<Vector2Int> dir = new List<Vector2Int>()
        {
            new Vector2Int(-1,0),
            new Vector2Int(-1,+1),
            new Vector2Int(0,+1),
            new Vector2Int(+1,+1),
            new Vector2Int(+1,0),
            new Vector2Int(+1,-1),
            new Vector2Int(0,-1),
            new Vector2Int(-1,-1),
        };

        int x = center.Pos.x;
        int y = center.Pos.y;

        foreach(var v in dir)
        {
            CellController cell = getCell(x + v.x , y + v.y);

            if(null == cell) continue;
            ret.Add(cell);
        }

        return ret;
    }

    //�w��̏ꏊ�̃Z����Ԃ�
    
    CellController getCell(int x,int y) 
    {
        //�z��I�[�o�[�̃`�F�b�N
        if(  x < 0 || cells.GetLength(0) <= x
           ||y < 0 || cells.GetLength(1) <= y)
        {
            return null;
        }
        return cells[x,y];
    }

    //���g���C
    public void Retry()
    {

        SceneManager.LoadScene("GameScene");

    }
   




}
