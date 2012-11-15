using System;
using System.Collections.Generic;
using System.Linq;



namespace Flip.Common.Messages
{
	public class MessageBus : IMessageBus
	{
		public MessageBus()
		{
			_subscribers = new Dictionary<object, KeyValuePair<Type, Delegate>>();
		}

		public void Publish<TMessage>(TMessage message) where TMessage : IMessage
		{
			Type messageType = typeof(TMessage);

			IEnumerable<Delegate> actions = _subscribers.Values
				.Where(pair => pair.Key.IsAssignableFrom(messageType))
				.Select(pair => pair.Value);

			foreach (Delegate action in actions)
			{
				action.DynamicInvoke(message);
			}
		}

		public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : IMessage
		{
			lock (_subscriberLock)
			{
				if (!_subscribers.ContainsKey(handler))
				{
					_subscribers.Add(handler, new KeyValuePair<Type, Delegate>(typeof(TMessage), handler));
				}
			}
		}

		public void Unsubscribe<TMessage>(Action<TMessage> handler) where TMessage : IMessage
		{
			lock (_subscriberLock)
			{
				if (_subscribers.ContainsKey(handler))
				{
					_subscribers.Remove(handler);
				}
			}
		}



		private object _subscriberLock = new object();
		private readonly Dictionary<object, KeyValuePair<Type, Delegate>> _subscribers;
	}
}
