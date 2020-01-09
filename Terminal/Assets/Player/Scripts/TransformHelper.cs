using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelper{
    
    /// <summary>
    /// 深度优先搜索子物体
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="targetName"></param>
    /// <returns></returns>
    public static Transform DeepFind(this Transform parent, string targetName)
    {
        Transform tempTrans = null;
        foreach (Transform child in parent)
        {
            if (child.name == targetName)
            {
                return child;
            }
            else
            {
                tempTrans = DeepFind(child, targetName);
                if (tempTrans != null)
                {
                    return tempTrans;
                }
            }
        }
        return tempTrans;
    }
}
