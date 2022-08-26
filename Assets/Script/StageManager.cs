using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    //�Ђ�����Ԃ����Ƃ��ł����̈ʒu
    class TurnableStone
    {
        public int turnZ;
        public int turnX;
        public TurnableStone(int z, int x)
        {
            turnZ = z;
            turnX = x;
        }
    }

    public enum StoneState//�΂̏��
    {
        EMPTY,//��
        WHITE,//��
        BLACK//��
    };
    [SerializeField] GameObject firstStone;//�u������
    GameObject[,] firstStoneState = new GameObject[squareZ, squareX];//�u�����΂̍��W
    CellManager[,] stoneManagers = new CellManager[squareZ, squareX];//�΂̃V�����_�[�ƃ}�e���A���̏��
    StoneState[,] stoneState = new StoneState[squareZ, squareX];//�΂��󂩔�������

    [SerializeField] Camera mainCamera;//�J�����擾�p�ϐ�
    [SerializeField] public int whiteScore;//���̖���
    [SerializeField] public int blackScore;//���̖���

    [SerializeField] TextMeshProUGUI whiteScoreText;
    [SerializeField] TextMeshProUGUI blackScoreeText;

    const int squareX = 8;//�Տ��x(��)���W
    const int squareZ = 8;//�Տ��z(�c)���W

    StoneState turn = StoneState.BLACK;//�^�[���B�ŏ��͍�

    int _tapPointX;//�^�b�v�������W
    int _tapPointZ;//�^�b�v�������W
    int[] _turnCheckX = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };
    int[] _turnCheckZ = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };//�΂̗�8����

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

    }

    void Start()
    {
        for (int i = 0; i < squareZ; i++)
        {
            for (int j = 0; j < squareX; j++)
            {
                // �΂�64��EMPTY�Ő���
                GameObject stone = GameObject.Instantiate<GameObject>(firstStone);
                CellManager stoneManager = stone.GetComponent<CellManager>();

                stone.transform.position = new Vector3(j, 1, i);
                firstStoneState[i, j] = stone;
                stoneManagers[i, j] = stoneManager;
                stoneState[i, j] = StoneState.EMPTY;
            }
            stoneState[3, 3] = StoneState.WHITE;
            stoneState[3, 4] = StoneState.BLACK;
            stoneState[4, 3] = StoneState.BLACK;
            stoneState[4, 4] = StoneState.WHITE;
        }
        whiteScore = 2;
        blackScore = 2;
    }

    void Update()
    {
        PutStone();

        for (int i = 0; i < squareZ; i++)
        {
            for (int j = 0; j < squareX; j++)
            {
                // �΂̏�Ԃ��m�F
                stoneManagers[i, j].SetState(stoneState[i, j]);
            }
        }
    }
    /// <summary>
    /// �^�b�v�Ő΂�u������
    /// </summary>
    public void PutStone()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //�}�E�X�̃|�W�V�������擾����Ray�ɑ��
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            //�}�E�X�̃|�W�V��������Ray�𓊂��ĉ����ɓ���������hit�ɓ����
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100))
            {
                //x,z�̒l���擾
                _tapPointX = (int)hit.collider.gameObject.transform.position.x;
                _tapPointZ = (int)hit.collider.gameObject.transform.position.z;

                if (0 <= _tapPointX && _tapPointX < squareX && 0 <= _tapPointZ && _tapPointZ < squareZ &&
                    stoneState[_tapPointZ, _tapPointX] == StoneState.EMPTY && Turn(false) > 0)
                {
                    stoneState[_tapPointZ, _tapPointX] = turn;
                    Turn(true);
                    turn = ((turn == StoneState.BLACK) ? StoneState.WHITE : StoneState.BLACK);
                }
            }
        }
    }


    /// <summary>
    /// �Ђ�����Ԃ��΂��擾���鏈��
    /// </summary>
    /// <param name="isTurn"></param>
    /// <returns></returns>
    int Turn(bool isTurn)
    {
        // ����̐΂̐F
        StoneState enemyColor = ((turn == StoneState.BLACK) ? StoneState.WHITE : StoneState.BLACK);

        bool isTurnable = false;// �Ђ�����Ԃ����Ƃ��ł��邩�ǂ���
        List<TurnableStone> turnableStoneList = new List<TurnableStone>();//�Ђ�����Ԃ��΂̃��X�g
        int count = 0;
        int turnCount = 0;

        int plusX = 0, plusZ = 0;
        for (int i = 0; i < _turnCheckX.Length; i++)
        {
            int _x = _tapPointX;
            int _z = _tapPointZ;

            plusX = _turnCheckX[i];
            plusZ = _turnCheckZ[i];
            isTurnable = false;
            turnableStoneList.Clear();
            while (true)
            {
                _x += plusX;
                _z += plusZ;
                if (!(0 <= _x && _x < squareX && 0 <= _z && _z < squareZ))
                {
                    break;
                }
                if (stoneState[_z, _x] == enemyColor)
                {
                    // �Ђ�����Ԃ��Ώ�
                    turnableStoneList.Add(new TurnableStone(_z, _x));
                }
                else if (stoneState[_z, _x] == turn)
                {
                    // �Ђ�����Ԃ����Ƃ��ł���
                    isTurnable = true;
                    break;
                }
                else
                {
                    break;
                }
            }

            //�Ђ�����Ԃ�����
            if (isTurnable)
            {
                count += turnableStoneList.Count;
                if (isTurn)
                {
                    for (int j = 0; j < turnableStoneList.Count; j++)
                    {
                        TurnableStone ts = turnableStoneList[j];
                        stoneState[ts.turnZ, ts.turnX] = turn;
                        turnCount++;
                    }
                }
            }
        }
        return count;
    }
}
