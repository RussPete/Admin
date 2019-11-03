using System;
using System.Configuration;

namespace Utils
{
	/// <summary>
	/// Summary description for WebConfig.
	/// </summary>
	public class WebConfig : IDisposable
	{
		AppSettingsReader AppSettings = null;

		public WebConfig()
		{
			AppSettings = new AppSettingsReader();
		}

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~WebConfig() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public string Str(string Key)
		{
			string Value = string.Empty;
			try
			{
				Value = (string)AppSettings.GetValue(Key, typeof(string));
			}
			catch (InvalidOperationException)
			{
			}

			return Value;
		}

		public long Long(String Key)
		{
			return (long)AppSettings.GetValue(Key, typeof(long));
		}

		public bool Bool(String Key)
		{
			try
			{
				return (bool)AppSettings.GetValue(Key, typeof(bool));
			}
			catch (Exception)
			{
				return false;
			}
		}
    }
}
