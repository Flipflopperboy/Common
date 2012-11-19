using System;
using System.Threading;
using System.Threading.Tasks;



namespace Flip.Common.Threading
{
	public abstract class TaskRunner : IDisposable
	{
		public TaskRunner Start()
		{
			if (_task == null)
			{
				var tokenSource = new CancellationTokenSource();
				CancellationToken token = tokenSource.Token;
				var task = new Task(() =>
				{
					while (LoopCondition())
					{
						if (token.IsCancellationRequested)
						{
							return;
						}
						LoopAction();
					}
					_done = true;
				}, tokenSource.Token);

				if(_first)
				{
					OnFirstStarting();
					_first = false;
				}

				OnStarting();
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
					if (_pausing && LoopCondition())
					{
						OnPaused();
					}
					_pausing = false;
				}
			}
			return this;
		}

		public void Pause()
		{
			_pausing = true;
			CancelTask();
		}

		public void Stop()
		{
			Dispose(false);
		}



		protected abstract bool LoopCondition();
		protected abstract void LoopAction();
		protected virtual void OnFirstStarting()
		{
		}
		protected virtual void OnStarting()
		{
		}
		protected virtual void OnPaused()
		{
		}
		protected virtual void Dispose(bool disposing)
		{
			_done = true;
			CancelTask();
			RemoveWaiter();
		}



		private void CancelTask()
		{
			if (_task != null)
			{
				_task.TokenSource.Cancel();
				_task = null;
			}
		}
		
		private TokenedTask CreateWaiter()
		{
			var tokenSource = new CancellationTokenSource();
			CancellationToken token = tokenSource.Token;
			var task = Task.Factory.StartNew(() =>
			{
				while (LoopCondition())
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
		private bool _done = false;
		private bool _pausing = false;
		private bool _first = true;

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
