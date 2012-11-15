using System;



namespace Flip.Common.Messages
{
	public interface IMessageBus
	{
		void Publish<TMessage>(TMessage message) where TMessage : IMessage;
		void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : IMessage;
		void Unsubscribe<TMessage>(Action<TMessage> handler) where TMessage : IMessage;
	}
}
