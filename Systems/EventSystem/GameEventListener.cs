using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using KFrame;

namespace KFrame.Systems
{
    public interface IMouseEvent : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler,
        IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler { }

    public class GameEventListener : MonoBehaviour, IMouseEvent
    {
        private static ObjectPoolModule poolModul = new ObjectPoolModule();

        #region 内部类、接口等

        /// <summary>
        /// 持有关键字典数据，主要用于将这个引用放入对象池中
        /// </summary>
        private class GameEventListenerData
        {
            public Dictionary<int, IGameEventListenerEventInfos> eventInfoDic =
                new Dictionary<int, GameEventListener.IGameEventListenerEventInfos>();
        }


        private interface IGameEventListenerEventInfo<T>
        {
            void TriggerEvent(T eventData);
            void Destory();
        }

        /// <summary>
        /// 某个事件中一个事件的数据包装类
        /// </summary>
        private class GameEventListenerEventInfo<T, TEventArg> : IGameEventListenerEventInfo<T>
        {
            // T：事件本身的参数（PointerEventData、Collision）
            // object[]:事件的参数
            public Action<T, TEventArg> action;
            public TEventArg arg;

            public void Init(Action<T, TEventArg> action, TEventArg args = default(TEventArg))
            {
                this.action = action;
                this.arg = args;
            }

            public void Destory()
            {
                this.action = null;
                this.arg = default(TEventArg);
                poolModul.PushObject(this);
            }

            public void TriggerEvent(T eventData)
            {
                action?.Invoke(eventData, arg);
            }
        }


        /// <summary>
        /// 某个事件中一个事件的数据包装类（无参）
        /// </summary>
        private class GameEventListenerEventInfo<T> : IGameEventListenerEventInfo<T>
        {
            // T：事件本身的参数（PointerEventData、Collision）
            // object[]:事件的参数
            public Action<T> action;

            public void Init(Action<T> action)
            {
                this.action = action;
            }

            public void Destory()
            {
                this.action = null;
                poolModul.PushObject(this);
            }

            public void TriggerEvent(T eventData)
            {
                action?.Invoke(eventData);
            }
        }


        interface IGameEventListenerEventInfos
        {
            void RemoveAll();
        }

        /// <summary>
        /// 一类事件的数据包装类型：包含多个GameEventListenerEventInfo
        /// </summary>
        private class GameEventListenerEventInfos<T> : IGameEventListenerEventInfos
        {
            // 所有的事件
            private List<IGameEventListenerEventInfo<T>> eventList = new List<IGameEventListenerEventInfo<T>>();


            /// <summary>
            /// 添加事件 无参
            /// </summary>
            public void AddListener(Action<T> action)
            {
                GameEventListenerEventInfo<T> info = poolModul.GetObject<GameEventListenerEventInfo<T>>();
                if (info == null) info = new GameEventListenerEventInfo<T>();
                info.Init(action);
                eventList.Add(info);
            }

            /// <summary>
            /// 添加事件 有参
            /// </summary>
            public void AddListener<TEventArg>(Action<T, TEventArg> action, TEventArg args = default(TEventArg))
            {
                GameEventListenerEventInfo<T, TEventArg> info =
                    poolModul.GetObject<GameEventListenerEventInfo<T, TEventArg>>();
                if (info == null) info = new GameEventListenerEventInfo<T, TEventArg>();
                info.Init(action, args);
                eventList.Add(info);
            }

            public void TriggerEvent(T evetData)
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    eventList[i].TriggerEvent(evetData);
                }
            }


            /// <summary>
            /// 移除事件（无参）
            /// 同一个函数+参数注册过多次，无论如何该方法只会移除一个事件
            /// </summary>
            public void RemoveListener(Action<T> action)
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    GameEventListenerEventInfo<T> eventInfo = eventList[i] as GameEventListenerEventInfo<T>;
                    if (eventInfo == null) continue; // 类型不符

                    // 找到这个事件，查看是否相等
                    if (eventInfo.action.Equals(action))
                    {
                        // 移除
                        eventInfo.Destory();
                        eventList.RemoveAt(i);
                        return;
                    }
                }
            }

            /// <summary>
            /// 移除事件（有参）
            /// 同一个函数+参数注册过多次，无论如何该方法只会移除一个事件
            /// </summary>
            public void RemoveListener<TEventArg>(Action<T, TEventArg> action, TEventArg args = default(TEventArg))
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    GameEventListenerEventInfo<T, TEventArg> eventInfo =
                        eventList[i] as GameEventListenerEventInfo<T, TEventArg>;
                    if (eventInfo == null) continue; // 类型不符

                    // 找到这个事件，查看是否相等
                    if (eventInfo.action.Equals(action))
                    {
                        // 移除
                        eventInfo.Destory();
                        eventList.RemoveAt(i);
                        return;
                    }
                }
            }

            /// <summary>
            /// 移除全部，全部放进对象池
            /// </summary>
            public void RemoveAll()
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    eventList[i].Destory();
                }

                eventList.Clear();
                poolModul.PushObject(this);
            }
        }

        #endregion

        private GameEventListenerData data;

        private GameEventListenerData Data
        {
            get
            {
                if (data == null)
                {
                    data = poolModul.GetObject<GameEventListenerData>();
                    if (data == null) data = new GameEventListenerData();
                }

                return data;
            }
        }

        #region 外部的访问

        /// <summary>
        /// 添加无参事件 
        /// </summary>
        public void AddListener<T>(int eventTypeInt, Action<T> action)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IGameEventListenerEventInfos info))
            {
                ((GameEventListenerEventInfos<T>)info).AddListener(action);
            }
            else
            {
                GameEventListenerEventInfos<T> infos = poolModul.GetObject<GameEventListenerEventInfos<T>>();
                if (infos == null) infos = new GameEventListenerEventInfos<T>();
                infos.AddListener(action);
                Data.eventInfoDic.Add(eventTypeInt, infos);
            }
        }

        /// <summary>
        /// 添加事件（有参）
        /// </summary>
        public void AddListener<T, TEventArg>(int eventTypeInt, Action<T, TEventArg> action, TEventArg args)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IGameEventListenerEventInfos info))
            {
                ((GameEventListenerEventInfos<T>)info).AddListener(action, args);
            }
            else
            {
                GameEventListenerEventInfos<T> infos = poolModul.GetObject<GameEventListenerEventInfos<T>>();
                if (infos == null) infos = new GameEventListenerEventInfos<T>();
                infos.AddListener(action, args);
                Data.eventInfoDic.Add(eventTypeInt, infos);
            }
        }


        /// <summary>
        /// 添加事件（无参）
        /// </summary>
        public void AddListener<T>(GameEventType eventType, Action<T> action)
        {
            AddListener((int)eventType, action);
        }

        /// <summary>
        /// 添加事件（有参）
        /// </summary>
        public void AddListener<T, TEventArg>(GameEventType eventType, Action<T, TEventArg> action, TEventArg args)
        {
            AddListener((int)eventType, action, args);
        }


        /// <summary>
        /// 移除事件（无参）
        /// </summary>
        public void RemoveListener<T>(int eventTypeInt, Action<T> action)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IGameEventListenerEventInfos info))
            {
                ((GameEventListenerEventInfos<T>)info).RemoveListener(action);
            }
        }

        /// <summary>
        /// 移除事件（无参）
        /// </summary>
        public void RemoveListener<T>(GameEventType eventType, Action<T> action)
        {
            RemoveListener((int)eventType, action);
        }


        /// <summary>
        /// 移除事件（有参）
        /// </summary>
        public void RemoveListener<T, TEventArg>(int eventTypeInt, Action<T, TEventArg> action)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IGameEventListenerEventInfos info))
            {
                ((GameEventListenerEventInfos<T>)info).RemoveListener(action);
            }
        }

        /// <summary>
        /// 移除事件（有参）
        /// </summary>
        public void RemoveListener<T, TEventArg>(GameEventType eventType, Action<T, TEventArg> action)
        {
            RemoveListener((int)eventType, action);
        }

        /// <summary>
        /// 移除某一个事件类型下的全部事件
        /// </summary>
        public void RemoveAllListener(GameEventType eventType)
        {
            RemoveAllListener((int)eventType);
        }

        /// <summary>
        /// 移除某一个事件类型下的全部事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        public void RemoveAllListener(int eventType)
        {
            if (Data.eventInfoDic.TryGetValue(eventType, out IGameEventListenerEventInfos infos))
            {
                infos.RemoveAll();
                Data.eventInfoDic.Remove(eventType);
            }
        }

        /// <summary>
        /// 移除全部事件
        /// </summary>
        public void RemoveAllListener()
        {
            foreach (IGameEventListenerEventInfos infos in Data.eventInfoDic.Values)
            {
                infos.RemoveAll();
            }

            data.eventInfoDic.Clear();
            // 将整个数据容器放入对象池
            poolModul.PushObject(data);
            data = null;
        }

        #endregion

        /// <summary>
        /// 触发事件
        /// </summary>
        public void TriggerAction<T>(int eventTypeInt, T eventData)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IGameEventListenerEventInfos infos))
            {
                (infos as GameEventListenerEventInfos<T>)?.TriggerEvent(eventData);
            }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        public void TriggerAction<T>(GameEventType eventType, T eventData)
        {
            TriggerAction<T>((int)eventType, eventData);
        }

        #region 鼠标事件

        public void OnPointerEnter(PointerEventData eventData)
        {
            TriggerAction(GameEventType.OnMouseEnter, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TriggerAction(GameEventType.OnMouseExit, eventData);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            TriggerAction(GameEventType.OnBeginDrag, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            TriggerAction(GameEventType.OnDrag, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            TriggerAction(GameEventType.OnEndDrag, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TriggerAction(GameEventType.OnClick, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            TriggerAction(GameEventType.OnClickDown, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TriggerAction(GameEventType.OnClickUp, eventData);
        }

        #endregion

        #region 碰撞事件

        private void OnCollisionEnter(Collision collision)
        {
            TriggerAction(GameEventType.OnCollisionEnter, collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            TriggerAction(GameEventType.OnCollisionStay, collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            TriggerAction(GameEventType.OnCollisionExit, collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TriggerAction(GameEventType.OnCollisionEnter2D, collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TriggerAction(GameEventType.OnCollisionStay2D, collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            TriggerAction(GameEventType.OnCollisionExit2D, collision);
        }

        #endregion

        #region 触发事件

        private void OnTriggerEnter(Collider other)
        {
            TriggerAction(GameEventType.OnTriggerEnter, other);
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerAction(GameEventType.OnTriggerStay, other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerAction(GameEventType.OnTriggerExit, other);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            TriggerAction(GameEventType.OnTriggerEnter2D, collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            TriggerAction(GameEventType.OnTriggerStay2D, collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            TriggerAction(GameEventType.OnTriggerExit2D, collision);
        }

        #endregion

        #region 销毁和失活事件

        private void OnDisable()
        {
            TriggerAction(GameEventType.OnDisable, gameObject);


            // // 销毁所有数据，并将一些数据放回对象池中
            // RemoveAllListener();
        }

        private void OnDestroy()
        {
            TriggerAction(GameEventType.OnReleaseAddressableAsset, gameObject);
            TriggerAction(GameEventType.OnDestroy, gameObject);

            // 销毁所有数据，并将一些数据放回对象池中
            RemoveAllListener();
        }

        #endregion
    }
}