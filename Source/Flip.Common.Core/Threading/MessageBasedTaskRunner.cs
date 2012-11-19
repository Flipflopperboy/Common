using Flip.Common.Messages;



namespace Flip.Common.Threading
{
	public abstract class MessageBasedTaskRunner<TStartedMessage, TPausedMessage, TStoppedMessage> : TaskRunner
		where TStartedMessage : IMessage
		where TPausedMessage : IMessage
		where TStoppedMessage : IMessage
	{
		public MessageBasedTaskRunner(IMessageBus messageBus)
		{
			_messageBus = messageBus;
			_messageBus.Subscribe<TStartedMessage>(OnStartedMessage);
			_messageBus.Subscribe<TPausedMessage>(OnPausedMessage);
			_messageBus.Subscribe<TStoppedMessage>(OnStoppedMessage);
		}



		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_messageBus.Unsubscribe<TStartedMessage>(OnStartedMessage);
				_messageBus.Unsubscribe<TPausedMessage>(OnPausedMessage);
				_messageBus.Unsubscribe<TStoppedMessage>(OnStoppedMessage);
			}
			base.Dispose(disposing);
		}



		private void OnStartedMessage(TStartedMessage message)
		{
			Start();
		}

		private void OnPausedMessage(TPausedMessage message)
		{
			Pause();
		}

		private void OnStoppedMessage(TStoppedMessage message)
		{
			Stop();
		}



		protected readonly IMessageBus _messageBus;
	}
}
