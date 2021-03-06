﻿using UnityEngine;
using System.Collections;

namespace M8.NGUI {
    /// <summary>
    /// Allow InputManager to handle input for NGUI.  Put this component along with ui modal manager. Use in conjuction with UIButtonKeys for each item.
    /// </summary>
    [AddComponentMenu("M8/NGUI/ModalInput")]
    public class ModalInput : MonoBehaviour {
        public int player = 0;

        public int axisX = InputManager.ActionInvalid;
        public int axisY = InputManager.ActionInvalid;

        public float axisDelay = 0.25f;
        public float axisThreshold = 0.75f;

        public int enter = InputManager.ActionInvalid;
        public int enterAlt = InputManager.ActionInvalid;
        public int cancel = InputManager.ActionInvalid;
        public int cancelAlt = InputManager.ActionInvalid;

        private enum AxisState {
            Up,
            Down
        }

        private AxisState mAxisXState = AxisState.Up;
        private AxisState mAxisYState = AxisState.Up;

        private UICamera.MouseOrTouch mController = new UICamera.MouseOrTouch();
        private bool mInputActive = false;
        private float mNextTime = 0.0f;

        void OnDestroy() {
            ONUIModalSetActive(false);
        }

        void Awake() {
            Camera cam = NGUITools.FindCameraForLayer(gameObject.layer);
            if(cam != null) {
                UICamera uiCam = cam.GetComponent<UICamera>();
                if(uiCam != null) {
                    uiCam.useKeyboard = false; //NGUI shouldn't have hardcoded input logic...bad!
                }
            }

            M8.UIModal.Manager.instance.activeCallback += ONUIModalSetActive;
        }

        void OnInputEnter(InputManager.Info data) {
            bool pressed = data.state == InputManager.State.Pressed;
            bool released = data.state == InputManager.State.Released;

            if(pressed || released) {
                UICamera.currentTouchID = -666;
                UICamera.currentTouch = mController;
                UICamera.currentTouch.current = UICamera.selectedObject;
                UICamera.eventHandler.ProcessTouch(pressed, released);
                UICamera.currentTouch.current = null;

                UICamera.currentTouch = null;
            }

            //UICamera.current
            //mController
            /* if(data.state == InputManager.State.Pressed && UICamera.selectedObject != null) {
                 UICamera.Notify(UICamera.selectedObject, "OnClick", null);
             }*/
        }

        void OnInputCancel(InputManager.Info data) {
            if(data.state == InputManager.State.Pressed && UICamera.selectedObject != null) {
                UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.Escape);
            }
        }

        void Update() {
            if(mInputActive) {
                if(UICamera.selectedObject != null) {
                    float time = Time.realtimeSinceStartup;
                    float delta = time - mNextTime;

                    InputManager input = InputManager.instance;

                    if(axisX != InputManager.ActionInvalid) {
                        float x = input.GetAxis(player, axisX);
                        if(x < -axisThreshold) {
                            if(delta >= axisDelay || mAxisXState == AxisState.Up) {
                                mNextTime = time;
                                UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.LeftArrow);
                            }

                            mAxisXState = AxisState.Down;
                        }
                        else if(x > axisThreshold) {
                            if(delta >= axisDelay || mAxisXState == AxisState.Up) {
                                mNextTime = time;
                                UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.RightArrow);
                            }

                            mAxisXState = AxisState.Down;
                        }
                        else {
                            mAxisXState = AxisState.Up;
                        }
                    }

                    if(axisY != InputManager.ActionInvalid) {
                        float y = input.GetAxis(player, axisY);
                        if(y < -axisThreshold) {
                            if(delta >= axisDelay || mAxisYState == AxisState.Up) {
                                mNextTime = time;
                                UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.DownArrow);
                            }

                            mAxisYState = AxisState.Down;
                        }
                        else if(y > axisThreshold) {
                            if(delta >= axisDelay || mAxisYState == AxisState.Up) {
                                mNextTime = time;
                                UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.UpArrow);
                            }

                            mAxisYState = AxisState.Down;
                        }
                        else {
                            mAxisYState = AxisState.Up;
                        }
                    }
                }
            }
        }

        void ONUIModalSetActive(bool b) {
            if(b) {
                if(!mInputActive) {
                    //bind callbacks
                    InputManager input = InputManager.instance;

                    if(enter != InputManager.ActionInvalid)
                        input.AddButtonCall(player, enter, OnInputEnter);

                    if(enterAlt != InputManager.ActionInvalid)
                        input.AddButtonCall(player, enterAlt, OnInputEnter);

                    if(cancel != InputManager.ActionInvalid)
                        input.AddButtonCall(player, cancel, OnInputCancel);

                    if(cancelAlt != InputManager.ActionInvalid)
                        input.AddButtonCall(player, cancelAlt, OnInputCancel);

                    mInputActive = true;

                    mAxisXState = AxisState.Up;
                    mAxisYState = AxisState.Up;
                    mNextTime = Time.realtimeSinceStartup;
                }
            }
            else {
                if(mInputActive) {
                    //unbind callbacks
                    InputManager input = InputManager.instance;

                    if(input != null) {
                        if(enter != InputManager.ActionInvalid)
                            input.RemoveButtonCall(player, enter, OnInputEnter);

                        if(enterAlt != InputManager.ActionInvalid)
                            input.RemoveButtonCall(player, enterAlt, OnInputEnter);

                        if(cancel != InputManager.ActionInvalid)
                            input.RemoveButtonCall(player, cancel, OnInputCancel);

                        if(cancelAlt != InputManager.ActionInvalid)
                            input.RemoveButtonCall(player, cancelAlt, OnInputCancel);
                    }

                    mInputActive = false;
                }
            }
        }
    }
}