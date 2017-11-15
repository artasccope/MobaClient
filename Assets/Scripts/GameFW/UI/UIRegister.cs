using GameFW.Core.Base;
using GameFW.ID;
using GameFW.UI.Fight;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFW.UI
{
    /// <summary>
    /// UI注册器
    /// </summary>
    public class UIRegister : MonoBehaviour
    {
        #region 注册
        private void Awake()
        {
            Regist();
        }

        public void Regist()
        {
            MgrCenter.UIMgr.RegistItem(IDCaculater.TransformIdInSceneHierachy(transform), gameObject);
        }
        #endregion

        #region UI事件直接注册

        public void AddButtonListener(UnityAction action)
        {
            if (action != null)
            {
                Button btn = transform.GetComponent<Button>();
                if (btn != null)
                    btn.onClick.AddListener(action);
            }
        }

        public void RemoveButtonListener(UnityAction action)
        {
            if (action != null)
            {
                Button btn = transform.GetComponent<Button>();
                if (btn != null)
                    btn.onClick.RemoveListener(action);
            }
        }

        public void SetJoyStickMoveListener(JoyStickTouchMove action)
        {
            if (action != null)
            {
                JoyStick stk = transform.GetComponent<JoyStick>();
                if (stk != null)
                    stk.SetJoyStickMoveListener(action);
            }
        }

        public void ClearJoyStickMoveListener()
        {
            JoyStick stk = transform.GetComponent<JoyStick>();
            if (stk != null)
                stk.ClearJoyStickMoveListener();
        }

        public void AddJoyStickMoveListener(JoyStickTouchMove action)
        {
            if (action != null)
            {
                JoyStick stk = transform.GetComponent<JoyStick>();
                if (stk != null)
                    stk.AddJoyStickMoveListener(action);
            }
        }

        public void RemoveJoyStickMoveListener(JoyStickTouchMove action)
        {
            if (action != null)
            {
                JoyStick stk = transform.GetComponent<JoyStick>();
                if (stk != null)
                    stk.RemoveJoyStickMoveListener(action);
            };
        }

        public void SetJoyStickBeginListener(JoyStickTouchBegin action)
        {
            if (action != null)
            {
                JoyStick stk = transform.GetComponent<JoyStick>();
                if (stk != null)
                    stk.SetJoyStickBeginListener(action);
            }
        }

        public void ClearJoyStickBeginListener()
        {
            JoyStick stk = transform.GetComponent<JoyStick>();
            if (stk != null)
                stk.ClearJoyStickBeginListener();
        }

        public void AddJoyStickBeginListener(JoyStickTouchBegin action)
        {
            if (action != null)
            {
                JoyStick stk = transform.GetComponent<JoyStick>();
                if (stk != null)
                    stk.AddJoyStickBeginListener(action);
            }
        }

        public void RemoveJoyStickBeginListener(JoyStickTouchBegin action)
        {
            if (action != null)
            {
                JoyStick stk = transform.GetComponent<JoyStick>();
                if (stk != null)
                    stk.RemoveJoyStickBeginListener(action);
            };
        }

        public void SetJoyStickEndListener(JoyStickTouchEnd action)
        {
            if (action != null)
            {
                JoyStick stk = transform.GetComponent<JoyStick>();
                if (stk != null)
                    stk.RemoveJoyStickEndListener(action);
            }
        }

        public void ClearJoyStickEndListener()
        {
            JoyStick stk = transform.GetComponent<JoyStick>();
            if (stk != null)
                stk.ClearJoyStickEndListener();
        }

        public void AddJoyStickEndListener(JoyStickTouchEnd action)
        {
            if (action != null)
            {
                JoyStick stk = transform.GetComponent<JoyStick>();
                if (stk != null)
                    stk.AddJoyStickEndListener(action);
            }
        }

        public void RemoveJoyStickEndListener(JoyStickTouchEnd action)
        {
            if (action != null)
            {
                JoyStick stk = transform.GetComponent<JoyStick>();
                if (stk != null)
                    stk.RemoveJoyStickEndListener(action);
            };
        }

        public void AddToggleListener(UnityAction<bool> action)
        {
            if (action != null)
            {
                Toggle tgl = transform.GetComponent<Toggle>();
                if (tgl != null)
                    tgl.onValueChanged.AddListener(action);
            }
        }

        public void RemoveToggleListener(UnityAction<bool> action)
        {
            if (action != null)
            {
                Toggle tgl = transform.GetComponent<Toggle>();
                if (tgl != null)
                    tgl.onValueChanged.RemoveListener(action);
            }
        }

        public void AddSlideListener(UnityAction<float> action)
        {
            if (action != null)
            {
                Slider sld = transform.GetComponent<Slider>();
                if (sld != null)
                    sld.onValueChanged.AddListener(action);
            }
        }
        public void RemoveSlideListener(UnityAction<float> action)
        {
            if (action != null)
            {
                Slider sld = transform.GetComponent<Slider>();
                if (sld != null)
                    sld.onValueChanged.RemoveListener(action);
            }
        }

        public void AddTextInputListener(UnityAction<string> action)
        {
            if (action != null)
            {
                InputField input = transform.GetComponent<InputField>();
                if (input != null)
                    input.onValueChanged.AddListener(action);
            }
        }

        public void RemoveTextInputListener(UnityAction<string> action)
        {
            if (action != null)
            {
                InputField input = transform.GetComponent<InputField>();
                if (input != null)
                    input.onValueChanged.RemoveListener(action);
            }
        }

        #endregion

    }
}
