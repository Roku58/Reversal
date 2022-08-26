using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    //ひっくり返すことができる駒の位置
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

    public enum StoneState//石の状態
    {
        EMPTY,//空
        WHITE,//白
        BLACK//黒
    };
    [SerializeField] GameObject firstStone;//置いた石
    GameObject[,] firstStoneState = new GameObject[squareZ, squareX];//置いた石の座標
    CellManager[,] stoneManagers = new CellManager[squareZ, squareX];//石のシリンダーとマテリアルの状態
    StoneState[,] stoneState = new StoneState[squareZ, squareX];//石が空か白か黒か

    [SerializeField] Camera mainCamera;//カメラ取得用変数
    [SerializeField] public int whiteScore;//白の枚数
    [SerializeField] public int blackScore;//黒の枚数

    [SerializeField] TextMeshProUGUI whiteScoreText;
    [SerializeField] TextMeshProUGUI blackScoreeText;

    const int squareX = 8;//盤上のx(横)座標
    const int squareZ = 8;//盤上のz(縦)座標

    StoneState turn = StoneState.BLACK;//ターン。最初は黒

    int _tapPointX;//タップした座標
    int _tapPointZ;//タップした座標
    int[] _turnCheckX = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };
    int[] _turnCheckZ = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };//石の隣8方向

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
                // 石を64枚EMPTYで生成
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
                // 石の状態を確認
                stoneManagers[i, j].SetState(stoneState[i, j]);
            }
        }
    }
    /// <summary>
    /// タップで石を置く処理
    /// </summary>
    public void PutStone()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //マウスのポジションを取得してRayに代入
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            //マウスのポジションからRayを投げて何かに当たったらhitに入れる
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100))
            {
                //x,zの値を取得
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
    /// ひっくり返す石を取得する処理
    /// </summary>
    /// <param name="isTurn"></param>
    /// <returns></returns>
    int Turn(bool isTurn)
    {
        // 相手の石の色
        StoneState enemyColor = ((turn == StoneState.BLACK) ? StoneState.WHITE : StoneState.BLACK);

        bool isTurnable = false;// ひっくり返すことができるかどうか
        List<TurnableStone> turnableStoneList = new List<TurnableStone>();//ひっくり返す石のリスト
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
                    // ひっくり返す対象
                    turnableStoneList.Add(new TurnableStone(_z, _x));
                }
                else if (stoneState[_z, _x] == turn)
                {
                    // ひっくり返すことができる
                    isTurnable = true;
                    break;
                }
                else
                {
                    break;
                }
            }

            //ひっくり返す処理
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
