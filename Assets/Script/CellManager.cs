using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    [SerializeField] Material _material = null;
    [SerializeField] MeshRenderer _topCylinder = null;
    [SerializeField] MeshRenderer _backCylinder = null;

    //後からアニメション適応
    Material _topMaterial = null;//上部
    Material _bottomMaterial = null;//下部


    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    public void SetState(StageManager.StoneState state)
    {
        bool isActive = (state != StageManager.StoneState.EMPTY);
        {
            _topCylinder.gameObject.SetActive(isActive);
            _backCylinder.gameObject.SetActive(isActive);
        }
        SetColor(state == StageManager.StoneState.WHITE);

    }

    /// <summary>
    /// 色を変える処理
    /// </summary>
    /// <param name="isWHITE"></param>
    public void SetColor(bool isWHITE)
    {
        if (_topMaterial == null)
        {
            _topMaterial = GameObject.Instantiate<Material>(_material);
            _bottomMaterial = GameObject.Instantiate<Material>(_material);
            _topCylinder.material = _topMaterial;
            _backCylinder.material = _bottomMaterial;
        }
        _topMaterial.color = isWHITE ? Color.white : Color.black;
        _bottomMaterial.color = isWHITE ? Color.black : Color.white;
    }
}
