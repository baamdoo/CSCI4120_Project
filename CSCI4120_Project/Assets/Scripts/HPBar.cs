using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    Camera _camera;
    public Slider hpBar;
    
    Status _stat;

    private void Start()
    {
        _stat = transform.parent.GetComponent<Status>();
    }

    private void Update()
    {
        transform.position = transform.parent.transform.position + Vector3.up * (transform.parent.GetComponent<Collider>().bounds.size.y);
        hpBar.value = (float)_stat.HP / (float)_stat.MaxHP;
        transform.rotation = _camera.transform.rotation;
    }
}
