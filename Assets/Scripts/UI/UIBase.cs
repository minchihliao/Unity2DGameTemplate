using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class UIBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

     public virtual UniTask OnOpen()
    {
        return default;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
