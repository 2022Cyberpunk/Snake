using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.EventManager
{
    /// <summary>
    /// �¼�����
    /// </summary>
    public class EventController
    {
        /// <summary>
        /// �����Ե���Ϣ����Cleanup��ʱ����Щ��Ϣ����Ӧ�ǲ��ᱻ����ġ�  
        /// </summary>
        private readonly List<string> m_PermanentEvents = new List<string>();

        /// <summary>
        /// �¼�·��    
        /// </summary>
        private readonly Dictionary<string, Delegate> m_Router = new Dictionary<string, Delegate>();

        /// <summary>
        /// �����ֶλ�ȡ�¼�·��
        /// </summary>
        /// <value>The router.</value>
        public Dictionary<string, Delegate> theRouter => this.m_Router;

        /// <summary>
        /// ����¼�������������
        /// </summary>
        /// <param name="eventType">
        /// �¼�����
        /// </param>
        /// <param name="handle">
        /// �¼���������
        /// </param>
        public void AddEventListener(string eventType, Action handle)
        {
            this.OnListenerAdding(eventType, handle);
            this.m_Router[eventType] = (Action)Delegate.Combine((Action)this.m_Router[eventType], handle);
        }

        /// <summary>
        /// ����¼�����һ��������
        /// </summary>
        /// <typeparam name="T">�����</typeparam>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        public void AddEventListener<T>(string eventType, Action<T> handler)
        {
            this.OnListenerAdding(eventType, handler);
            this.m_Router[eventType] = (Action<T>)Delegate.Combine((Action<T>)this.m_Router[eventType], handler);
        }

        /// <summary>
        /// ɾ���¼�������������
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="handle">
        /// The handle.
        /// </param>
        public void RemoveEventListener(string eventType, Action handle)
        {
            if (this.OnListenerRemoving(eventType, handle))
            {
                this.m_Router[eventType] = (Action)Delegate.Remove((Action)this.m_Router[eventType], handle);
            }

            this.OnListenerRemoved(eventType);
        }

        /// <summary>
        /// ɾ���¼�����һ��������
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <typeparam name="T">
        /// �����
        /// </typeparam>
        public void RemoveEventListener<T>(string eventType, Action<T> handle)
        {
            if (this.OnListenerRemoving(eventType, handle))
            {
                this.m_Router[eventType] = (Action)Delegate.Remove((Action)this.m_Router[eventType], handle);
            }
        
            this.OnListenerRemoved(eventType);
        }

        /// <summary>
        /// �����¼�������������
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        public void TriggerEvent(string eventType)
        {
            if (this.m_Router.TryGetValue(eventType, out var delegateHandle))
            {
                var invocationList = delegateHandle.GetInvocationList();
                for (int invocationIndex = 0; invocationIndex < invocationList.Length; invocationIndex++)
                {
                    var action = invocationList[invocationIndex] as Action;
                    if (action == null)
                    {
                        throw new EventException($"TriggerEvent {eventType} error: types of parameters are not match.");
                    }

                    try
                    {
                        action();
                    }
                    catch (Exception exception)
                    {   
                        Debug.LogError($"msg:{exception.Message} \nstacktrace:{exception.StackTrace}");
                    }
                }
            }
        }

        /// <summary>
        /// �����¼�����һ��������
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="args1">
        /// The args 1.
        /// </param>
        /// <typeparam name="T">
        /// ��
        /// </typeparam>
        public void TriggerEvent<T>(string eventType, T args1)
        {
            if (this.m_Router.TryGetValue(eventType, out var delegateHandle))
            {
                var invocationList = delegateHandle.GetInvocationList();
                for (int invocationIndex = 0; invocationIndex < invocationList.Length; invocationIndex++)
                {
                    var action = invocationList[invocationIndex] as Action<T>;
                    if (action == null)
                    {
                        throw new EventException($"TriggerEvent {eventType} error: types of parameters are not match.");
                    }

                    try
                    {
                        action(args1);
                    }
                    catch (Exception exception) 
                    {
                        Debug.LogError($"msg:{exception.Message} \nstacktrace:{exception.StackTrace}");
                    }
                }
            }
        }

        /// <summary>
        /// �����¼�
        /// </summary>
        public void CleanUp()
        {
            List<string> list = new List<string>();
            foreach (var pair in this.m_Router)
            {
                var flag = false;
                foreach (var str in this.m_PermanentEvents)
                {
                    if (pair.Key == str)
                    {
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    list.Add(pair.Key);
                }
            }

            foreach (var str in list)
            {
                this.m_Router.Remove(str);
            }
        }

        /// <summary>
        /// Marks as permanent.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        public void MarkAsPermanent(string eventType)
        {
            this.m_PermanentEvents.Add(eventType);
        }

        /// <summary>
        /// ����¼�
        /// </summary>
        /// <param name="eventType">
        /// �¼�����
        /// </param>
        /// <param name="listenerBeingAdded">
        /// The listener Being Added.
        /// </param>
        private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)    
        {
            if (!m_Router.ContainsKey(eventType))
            {
                m_Router.Add(eventType, null);
            }

            var delegateHandle = this.m_Router[eventType];
            if (delegateHandle != null && delegateHandle.GetType() != listenerBeingAdded.GetType())
            {
                throw new EventException(
                    $"Try to add not correct event {eventType}. Current type is {delegateHandle.GetType().Name}, adding type is {listenerBeingAdded.GetType().Name}.");
            }
        }

        /// <summary>
        /// ɾ���¼�
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="listenerBeingRemoved">
        /// The listener being removed.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="EventException">
        /// �쳣��Ϣ
        /// </exception>
        private bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (!this.m_Router.ContainsKey(eventType))
            {
                return false;
            }

            var delegateHandle = this.m_Router[eventType];
            if (delegateHandle != null && delegateHandle.GetType() != listenerBeingRemoved.GetType())
            {
                throw new EventException(
                    $"Remove listener {eventType}\" failed, Current type is {delegateHandle.GetType()}, adding type is {listenerBeingRemoved.GetType()}.");
            }

            return true;
        }

        /// <summary>
        /// �Ƴ������͵������¼�
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        private void OnListenerRemoved(string eventType)
        {
            if (this.m_Router.ContainsKey(eventType) && this.m_Router[eventType] == null)
            {
                this.m_Router.Remove(eventType);
            }
        }
    }
}
