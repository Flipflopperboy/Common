using System;
using Flip.Common.Messages;
namespace Flip.Common.Threading
{
	public class MessageBasedTaskRunner<TStartedMessage, TPausedMessage, TStoppedMessage> : TaskRunner
		where TStartedMessage : IMessage
		where TPausedMessage : IMessage
		where TStoppedMessage  : IMessage
	{
		public MessageBasedTaskRunner(Func<bool> condition, Action action, IMessageBus messageBus)
			: base(condition, action)
		{
			_messageBus = messageBus;
			_messageBus.Subscribe<TStartedMessage>(OnStarted);
			_messageBus.Subscribe<TPausedMessage>(OnPaused);
			_messageBus.Subscribe<TStoppedMessage>(OnStopped);
		}



		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_messageBus.Unsubscribe<TStartedMessage>(OnStarted);
				_messageBus.Unsubscribe<TPausedMessage>(OnPaused);
				_messageBus.Unsubscribe<TStoppedMessage>(OnStopped);
			}
			base.Dispose(disposing);
		}



		private void OnStarted(TStartedMessage message)
		{
			Start();
		}

		private void OnPaused(TPausedMessage message)
		{
			Pause();
		}

		private void OnStopped(TStoppedMessage message)
		{
			Stop();
		}



		private readonly IMessageBus _messageBus;
	}
}
