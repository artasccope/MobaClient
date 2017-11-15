using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuaUIBehaviour : MonoBehaviour {

	// Use this for initialization
	protected void Awake () {
        CallMethod("LUIManager", "RegistGameObject", gameObject);
	}

    public void AddButtonListerner(LuaFunction action) {
        Button btn = transform.GetComponent<Button>();

        if (btn != null) {
            btn.onClick.AddListener(delegate() {
                action.Call(gameObject);
            });
        }
    }

    public void AddToggleListener(LuaFunction action)
    {
        if (action != null)
        {
            Toggle tgl = transform.GetComponent<Toggle>();
            if (tgl != null)
                tgl.onValueChanged.AddListener(delegate(bool selected) {
                    action.Call(selected);
                });
        }
    }

    public void AddSlideListener(LuaFunction action)
    {
        if (action != null)
        {
            Slider sld = transform.GetComponent<Slider>();
            if (sld != null)
                sld.onValueChanged.AddListener(delegate(float value) {
                    action.Call(value);
                });
        }
    }

    public void AddInputListener(LuaFunction action)
    {
        if (action != null)
        {
            InputField input = transform.GetComponent<InputField>();
            if (input != null)
                input.onValueChanged.AddListener(delegate(string str) {
                    action.Call(str);
                });
        }
    }

    public RectTransform GetTransform() {
        RectTransform trans = GetComponent<RectTransform>();
        if (trans == null) {
            trans = gameObject.AddComponent<RectTransform>();
        }

        return trans;
    }

    public Image SetImageSprite(Sprite sprite) {
        Image img = GetComponent<Image>();
        if (img == null)
            img = gameObject.AddComponent<Image>();

        img.sprite = sprite;
        return img;
    }

    /// <summary>
    /// 执行lua方法
    /// </summary>
    /// <param name="module"></param>
    /// <param name="func"></param>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    private int CallMethod(string module, string func, GameObject gameObject)
    {
        string funcName = module + '.' + func;
        return LuaClient.Instance.CallFuncWithGameObject(funcName, gameObject);
    }

    protected void OnDestroy()
    {
        Debug.Log("~" + name + " was destroy!");
    }
}
