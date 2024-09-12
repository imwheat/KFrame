using System;
using System.Collections.Generic;
using UnityEngine;

namespace KFrame
{
    public abstract class GameMonoBehaviour : MonoBehaviour
    {
        #region 基于Mono的消息系统

        // private Dictionary<string, Action<object>> m_MsgDic = new();
        //
        // private void _registerMsg(string msgName, Action<object> msgCallBack)
        // {
        //     if (!m_MsgDic.ContainsKey(msgName))
        //     {
        //         m_MsgDic.Add(msgName, _ => { });
        //     }
        //
        //     m_MsgDic[msgName] += msgCallBack;
        // }
        //
        // private void _unRegisterMsg(string msgName, Action<object> msgCallBack)
        // {
        //     if (m_MsgDic.ContainsKey(msgName))
        //     {
        //         m_MsgDic[msgName] -= msgCallBack;
        //     }
        // }
        //
        // private void _unRegisterAllMsg(string msgName)
        // {
        //     if (m_MsgDic.ContainsKey(msgName))
        //     {
        //         m_MsgDic.Remove(msgName);
        //     }
        // }
        //
        //
        // private void _sendMsg(string msgName, object data)
        // {
        //     if (m_MsgDic.ContainsKey(msgName))
        //     {
        //         m_MsgDic[msgName](data);
        //     }
        // }

        #endregion

        #region 消息记录器

        // private static List<MsgRecord> _msgCallBackRecorder = new List<MsgRecord>(); //这个List是每个消息中所注册的方法集合
        //
        // class MsgRecord
        // {
        //     public string Name; //消息的名字
        //
        //     public Action<object> OnMsgCallBack; //消息的回调函数
        //
        //     private MsgRecord()
        //     {
        //         
        //     }
        //
        //     private static Stack<MsgRecord> _msgRecordPool = new Stack<MsgRecord>();
        //
        //     public static MsgRecord Allocate(string name, Action<object> onMsgReceived)
        //     {
        //         var receivedMsg = _msgRecordPool.Count > 0 ? _msgRecordPool.Pop() : new MsgRecord();
        //         receivedMsg.Name = name;
        //         receivedMsg.OnMsgCallBack = onMsgReceived;
        //         return receivedMsg;
        //     }
        //
        //     /// <summary>
        //     /// 回收消息记录
        //     /// </summary>
        //     public void Recycle()
        //     {
        //         this.Name = null;
        //         this.OnMsgCallBack = null;
        //         _msgRecordPool.Push(this);
        //     }
        // }

        #endregion
        
        #region API

        // /// <summary>
        // /// 注册消息
        // /// </summary>
        // /// <param name="msgName"></param>
        // /// <param name="callBack"></param>
        // public void RegisterMsg(string msgName, Action<object> callBack)
        // {
        //     _registerMsg(msgName, callBack);
        //     _msgCallBackRecorder.Add(MsgRecord.Allocate(msgName, callBack));
        // }
        //
        // public void UnRegisterAllMsg(string msgName)
        // {
        //     var recorders = _msgCallBackRecorder.FindAll(record => record.Name == msgName);
        //
        //     recorders.ForEach((record) =>
        //     {
        //         _unRegisterAllMsg(record.Name); //在消息字典中清除注册
        //         _msgCallBackRecorder.Remove(record); //在记录器中清除记录
        //         record.Recycle(); //记录器回收
        //     });
        //     recorders.Clear();
        // }
        //
        // public void UnRegisterMsg(string msgName, Action<object> callback)
        // {
        //     var recorders =
        //         _msgCallBackRecorder.FindAll(record => record.Name == msgName && record.OnMsgCallBack == callback);
        //     recorders.ForEach((record) =>
        //     {
        //         _unRegisterMsg(record.Name, record.OnMsgCallBack);
        //         _msgCallBackRecorder.Remove(record);
        //         record.Recycle();
        //     });
        //     recorders.Clear();
        // }
        //
        // public void SendMsg(string msgName, object data)
        // {
        //     _sendMsg(msgName, data);
        // }

        #endregion

        // private void OnDestroy()
        // {
        //     BeforeOnDestroy();
        //     foreach (var msgRecord in _msgCallBackRecorder)
        //     {
        //         _unRegisterMsg(msgRecord.Name, msgRecord.OnMsgCallBack);
        //         msgRecord.Recycle();
        //     }
        //
        //     _msgCallBackRecorder.Clear();
        // }
        //
        // protected abstract void BeforeOnDestroy();



        
    }
}