using System;
using System.Threading;
using System.Threading.Tasks;



namespace Flip.Common.Threading
{
	public class TaskRunner : IDisposable
	{
		public TaskRunner(Func<bool> condition, Action action)
		{
			_condition = condition;
			_action = action;
		}



		public TaskRunner Start()
		{
			if (_task == null)
			{
				var tokenSource = new CancellationTokenSource();
				CancellationToken token = tokenSource.Token;
				var task = new Task(() =>
				{
					while (_condition())
					{
						if (token.IsCancellationRequested)
						{
							return;
						}
						_action();
					}
					_done = true;
				}, tokenSource.Token);

				_task = new TokenedTask(task, token, tokenSource);
				_task.Task.Start();
			}
			RemoveWaiter();
			return this;
		}

		public TaskRunner Wait()
		{
			while (!_done)
			{
				if (_task == null && _waiter == null)
				{
					_waiter = CreateWaiter();
					_waiter.Task.Wait();
				}
				else
				{
					_task.Task.Wait();
				}
			}
			return this;
		}

		public void Pause()
		{
			if (_task != null)
			{
				_task.TokenSource.Cancel();
				_task = null;
			}
		}

		public void Stop()
		{
			_done = true;
			Pause();
			RemoveWaiter();
		}



		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Stop();
			}
		}



		private TokenedTask CreateWaiter()
		{
			var tokenSource = new CancellationTokenSource();
			CancellationToken token = tokenSource.Token;
			var task = Task.Factory.StartNew(() =>
			{
				while (_condition())
				{
					if (token.IsCancellationRequested)
					{
						return;
					}
					Thread.Sleep(10);
				}
			});
			return new TokenedTask(task, token, tokenSource);
		}

		private void RemoveWaiter()
		{
			if (_waiter != null)
			{
				_waiter.TokenSource.Cancel();
				_waiter = null;
			}
		}



		void IDisposable.Dispose()
		{
			Dispose(true);
		}



		private TokenedTask _task;
		private TokenedTask _waiter;
		private readonly Func<bool> _condition;
		private readonly Action _action;
		private bool _done = false;

		private class TokenedTask
		{
			public TokenedTask(Task task, CancellationToken token, CancellationTokenSource tokenSource)
			{
				this.Task = task;
				this.Token = token;
				this.TokenSource = tokenSource;
			}



			public Task Task { get; private set; }
			public CancellationToken Token { get; private set; }
			public CancellationTokenSource TokenSource { get; private set; }
		}
	}
}
