using System;



namespace Flip.Common.Messages
{
	public abstract class MessageBase : IMessage
	{
		public MessageBase()
		{
			this.Id = Guid.NewGuid();
		}



		public Guid Id { get; private set; }
	}
}
