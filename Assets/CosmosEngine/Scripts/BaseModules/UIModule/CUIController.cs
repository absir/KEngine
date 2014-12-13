﻿//------------------------------------------------------------------------------
//
//      CosmosEngine - The Lightweight Unity3D Game Develop Framework
// 
//                     Version 0.8 (20140904)
//                     Copyright © 2011-2014
//                   MrKelly <23110388@qq.com>
//              https://github.com/mr-kelly/CosmosEngine
//
//------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Abstract class of all UI Script
/// </summary>
public abstract class CUIController : MonoBehaviour
{
    public string UITemplateName = "";
    public string UIName = "";
    public string UITitle = "(未设置)"; // UI的标题
    public bool HasBackBtn = true; // 是否有返回按钮

    public virtual void OnInit() { }

    /// <summary>
    /// Hook, if false, block "Open" action
    /// </summary>
    /// <param name="doOpenAction"></param>
    /// <returns></returns>
    public virtual bool OnBeforeOpenHook(Action doOpenAction)
    {
        return true;
    }

    public virtual void OnOpen(params object[] args)
    {
    }

    public virtual bool OnBeforeCloseHook(Action doCloseAction)
    {
        return true;
    }
    public virtual void OnClose() { }
    

    /// <summary>
    /// 输入uri搜寻控件
    /// findTrans默认参数null时使用this.transform
    /// </summary>
    public T GetControl<T>(string uri, Transform findTrans = null, bool isLog = true) where T : UnityEngine.Object
    {
        return (T)GetControl(typeof(T), uri, findTrans, isLog);
    }

    public object GetControl(Type type, string uri, Transform findTrans = null, bool isLog = true)
    {
        if (findTrans == null)
            findTrans = transform;

        Transform trans = findTrans.Find(uri);
        if (trans == null)
        {
            if (isLog)
                CDebug.LogError("Get UI Control Error: " + uri);
            return null;
        }

        if (type == typeof(GameObject))
            return trans.gameObject;

        return trans.GetComponent(type);
    }

    /// <summary>
    /// 默认在当前transfrom下根据Name查找子控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T FindControl<T>(string name) where T : Component
    {
        GameObject obj = DFSFindObject(transform, name);
        if (obj == null)
        {
            CDebug.LogError("Find UI Control Error: " + name);
            return null;
        }

        return obj.GetComponent<T>();
    }

    public GameObject FindGameObject(string name)
    {
        GameObject obj = DFSFindObject(transform, name);
        if (obj == null)
        {
            CDebug.LogError("Find GemeObject Error: " + name);
            return null;
        }

        return obj;
    }
    /// <summary>
    /// 从parent下根据Name查找
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject DFSFindObject(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform node = parent.GetChild(i);
            if (node.name == name)
                return node.gameObject;

            GameObject target = DFSFindObject(node, name);
            if (target != null)
                return target;
        }

        return null;
    }

    /// <summary>
    /// 清除一个GameObject下面所有的孩子
    /// </summary>
    /// <param name="go"></param>
    public void DestroyGameObjectChildren(GameObject go)
    {
        CTool.DestroyGameObjectChildren(go);
    }

    /// <summary>
    /// 模仿 NGUISelectionTool的同名方法，将位置旋转缩放清零
    /// </summary>
    /// <param name="t"></param>
    public void ResetLocalTransform(Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    /// <summary>
    /// 传入指定数量， 对UIGrid里指定数量项SetActive(true)/或创建, 其余的SetActive(false)
    /// 常用于UIGrid下的对象动态增长
    /// </summary>
    public void ResizeUIGridGameObjects(UIGrid uiGrid, int resizeCount, GameObject templateForNew)
    {
        _ResizeUIWidgetContainerGameObjects(uiGrid.transform, resizeCount, templateForNew);
        uiGrid.Reposition();
    }

    public void ResizeUITableGameObjects(UITable uiTable, int resizeCount, GameObject templateForNew)
    {
        _ResizeUIWidgetContainerGameObjects(uiTable.transform, resizeCount, templateForNew);
        uiTable.Reposition();
    }

    public void ResizeCUITableGridGameObjects(CUITableGrid uiTable, int resizeCount, GameObject templateForNew)
    {
        _ResizeUIWidgetContainerGameObjects(uiTable.transform, resizeCount, templateForNew);
        uiTable.Reposition();
    }
    public void _ResizeUIWidgetContainerGameObjects(Transform transf, int resizeCount, GameObject templateForNew)
    {
        if (templateForNew == null)
            templateForNew = default(GameObject);

        for (int i = 0; i < resizeCount; i++)
        {
            GameObject newTemplate = null;
            if (i >= transf.childCount)
            {
                newTemplate = Instantiate(templateForNew) as GameObject;
                newTemplate.transform.parent = transf;
                ResetLocalTransform(newTemplate.transform);

                //gameObjList.Add(newTemplate);
            }
            newTemplate = transf.GetChild(i).gameObject;
            if (!newTemplate.activeSelf)
                newTemplate.SetActive(true);
        }

        for (int i = resizeCount; i < transf.childCount; ++i)
        {
            GameObject newTemplate = transf.GetChild(i).gameObject;
            if (newTemplate.activeSelf)
                newTemplate.SetActive(false);
        }

    }

    /// <summary>
    /// Shortcuts for UIModule's Open Window
    /// </summary>
    protected void OpenWindow(string uiName, params object[] args)
    {
        CUIModule.Instance.OpenWindow(uiName, args);
    }

    /// <summary>
    /// Shortcuts for UIModule's Close Window
    /// </summary>
    /// <param name="uiName"></param>
    protected void CloseWindow(string uiName = null)
    {
        CUIModule.Instance.CloseWindow(uiName == null ? UIName : uiName);
    }


    /// <summary>
    /// 从数组获取参数，并且不报错，返回null, 一般用于OnOpen, OnClose的可变参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="openArgs"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected T GetFromArgs<T>(object[] openArgs, int offset, bool isLog = true)
    {
        return openArgs.Get<T>(offset, isLog);
    }

}
