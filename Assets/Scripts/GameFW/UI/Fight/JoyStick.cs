using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFW.UI.Fight
{
    /// <summary>
    /// 定义触摸开始事件委托
    /// </summary>
    public delegate void JoyStickTouchBegin(Vector2 vec);
    /// <summary>
    /// 定义触摸过程事件委托
    /// </summary>
    /// <param name="vec">虚拟摇杆的移动方向</param>
    public delegate void JoyStickTouchMove(Vector2 vec);
    /// <summary>
    /// 定义触摸结束事件委托
    /// </summary>
    public delegate void JoyStickTouchEnd();

    /// <summary>
    /// 摇杆
    /// </summary>
    public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        /// <summary>
        /// 可触摸触发摇杆的区域
        /// </summary>
        private RectTransform touchArea;
        /// <summary>
        /// 摇杆
        /// </summary>
        private RectTransform stick;
        /// <summary>
        /// 摇杆可活动区域
        /// </summary>
        private RectTransform stickBed;
        /// <summary>
        /// 摇杆最大半径
        /// 以像素为单位
        /// </summary>
        public float JoyStickRadius = 170;
        /// <summary>
        /// 是否触摸了虚拟摇杆
        /// </summary>
        private bool isTouched = false;
        /// <summary>
        /// 虚拟摇杆的默认位置
        /// </summary>
        private Vector2 originPosition;
        /// <summary>
        /// 虚拟摇杆的移动方向
        /// </summary>
        private Vector2 touchedAxis;
        public Vector2 TouchedAxis
        {
            get
            {
                if (touchedAxis.magnitude < JoyStickRadius * 0.1f)
                    return Vector2.zero;
                return touchedAxis.normalized;
            }
        }

        #region 事件声明
        /// <summary>
        /// 注册触摸开始事件
        /// </summary>
        public event JoyStickTouchBegin OnJoyStickTouchBegin;
        /// <summary>
        /// 注册触摸过程事件
        /// </summary>
        public event JoyStickTouchMove OnJoyStickTouchMove;
        /// <summary>
        /// 注册触摸结束事件
        /// </summary>
        public event JoyStickTouchEnd OnJoyStickTouchEnd;

        #endregion

        #region 初始化
        void Start()
        {
            //初始化虚拟摇杆的默认方向
            touchArea = this.GetComponent<RectTransform>();
            stickBed = transform.GetChild(0).GetComponent<RectTransform>();
            stick = transform.GetChild(1).GetComponent<RectTransform>();
            originPosition = stickBed.anchoredPosition;
        }

        #endregion

        #region 触发事件:按下、抬起、拖动
        /// <summary>
        /// 按下时触发
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            touchedAxis = GetJoyStickAxis(eventData);
            isTouched = true;
            if (this.OnJoyStickTouchBegin != null)
                this.OnJoyStickTouchBegin(TouchedAxis);
        }


        /// <summary>
        /// 抬起时触发
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            isTouched = false;
            stick.anchoredPosition = originPosition;
            stickBed.anchoredPosition = originPosition;

            touchedAxis = Vector2.zero;
            if (this.OnJoyStickTouchEnd != null)
                this.OnJoyStickTouchEnd();
        }

        /// <summary>
        /// 拖动时触发
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            touchedAxis = GetJoyStickAxis(eventData);
            if (this.OnJoyStickTouchMove != null)
                this.OnJoyStickTouchMove(TouchedAxis);
        }

        #endregion

        #region 摇杆偏移量更新
        /// <summary>
        /// 返回虚拟摇杆的偏移量
        /// </summary>
        /// <returns>The joy stick axis.</returns>
        /// <param name="eventData">Event data.</param>
        private Vector2 GetJoyStickAxis(PointerEventData eventData)
        {
            //获取手指位置的世界坐标
            Vector3 worldPosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(touchArea,
                     eventData.position, eventData.pressEventCamera, out worldPosition))
            {
                stick.position = worldPosition;
                if (!isTouched)
                {
                    stickBed.position = worldPosition;
                }
            }
            //获取摇杆的偏移量
            Vector2 touchAxis = stick.anchoredPosition - stickBed.anchoredPosition;
            //摇杆偏移量限制
            if (touchAxis.magnitude >= JoyStickRadius)
            {
                touchAxis = touchAxis.normalized * JoyStickRadius;
                stick.anchoredPosition = touchAxis + stickBed.anchoredPosition;
            }
            return touchAxis;
        }

        #endregion


        #region 注册、注销
        public void AddJoyStickBeginListener(JoyStickTouchBegin jsBegin)
        {
            if (jsBegin != null)
            {
                this.OnJoyStickTouchBegin += jsBegin;
            }
        }

        public void SetJoyStickBeginListener(JoyStickTouchBegin jsBegin)
        {
            if (jsBegin != null)
            {
                this.OnJoyStickTouchBegin = jsBegin;
            }
        }

        public void RemoveJoyStickBeginListener(JoyStickTouchBegin jsBegin)
        {
            if (jsBegin != null)
            {
                this.OnJoyStickTouchBegin -= jsBegin;
            }
        }

        public void ClearJoyStickBeginListener()
        {
            this.OnJoyStickTouchBegin = null;
        }

        public void AddJoyStickMoveListener(JoyStickTouchMove jsMove)
        {
            if (jsMove != null)
            {
                this.OnJoyStickTouchMove += jsMove;
            }
        }

        public void SetJoyStickMoveListener(JoyStickTouchMove jsMove)
        {
            if (jsMove != null)
            {
                this.OnJoyStickTouchMove = jsMove;
            }
        }

        public void RemoveJoyStickMoveListener(JoyStickTouchMove jsMove)
        {
            if (jsMove != null)
            {
                this.OnJoyStickTouchMove -= jsMove;
            }
        }

        public void ClearJoyStickMoveListener()
        {
            this.OnJoyStickTouchMove = null;
        }

        public void AddJoyStickEndListener(JoyStickTouchEnd jsEnd)
        {
            if (jsEnd != null)
            {
                this.OnJoyStickTouchEnd += jsEnd;
            }
        }

        public void SetJoyStickEndListener(JoyStickTouchEnd jsEnd)
        {
            if (jsEnd != null)
            {
                this.OnJoyStickTouchEnd = jsEnd;
            }
        }

        public void RemoveJoyStickEndListener(JoyStickTouchEnd jsEnd)
        {
            if (jsEnd != null)
            {
                this.OnJoyStickTouchEnd -= jsEnd;
            }
        }

        public void ClearJoyStickEndListener()
        {
            this.OnJoyStickTouchEnd = null;
        }

        #endregion
    }
}
