using System;
using Xunit;



namespace Flip.Common.Messages.Tests
{
	public sealed class MessageBusTests
	{
		[Fact]
		public void MultipleSubscribersReceiveMessage()
		{
			int hitCount = 0;

			var bus = new MessageBus();
			bus.Subscribe<AnimalMessage>(animal => hitCount++);
			bus.Subscribe<AnimalMessage>(animal => hitCount++);

			bus.Publish(new AnimalMessage());
			Assert.Equal(2, hitCount);
		}

		[Fact]
		public void IncorrectMessagesAreNotReceived()
		{
			int hitCount = 0;

			var bus = new MessageBus();
			bus.Subscribe<AnimalMessage>(animal => hitCount++);
			bus.Subscribe<CatMessage>(animal => hitCount++);

			bus.Publish(new AnimalMessage());
			Assert.Equal(1, hitCount);
		}

		[Fact]
		public void InheritedMessageShouldBeReceived()
		{
			int hitCount = 0;

			var bus = new MessageBus();
			bus.Subscribe<AnimalMessage>(animal => hitCount++);
			bus.Subscribe<CatMessage>(cat => hitCount++);
			bus.Subscribe<DogMessage>(dog => hitCount++);

			bus.Publish(new CatMessage());
			Assert.Equal(2, hitCount);
		}



		private class AnimalMessage : MessageBase
		{
		}
		private class CatMessage : AnimalMessage
		{
		}
		private class DogMessage : AnimalMessage
		{
		}
	}
}
