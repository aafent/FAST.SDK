using FAST.Logging;
using System.Text;

namespace FAST.Core
{
    /// <summary>
    /// Utility to help reading bytes and strings of a <see cref="Stream"/>
    /// </summary>
    public static class streamHelper
	{
		/// <summary>
		/// Read a line from the stream.
		/// A line is interpreted as all the bytes read until a CRLF or LF is encountered.<br/>
		/// CRLF pair or LF is not included in the string.
		/// </summary>
		/// <param name="stream">The stream from which the line is to be read</param>
		/// <returns>A line read from the stream returned as a byte array or <see langword="null"/> if no bytes were readable from the stream</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="stream"/> is <see langword="null"/></exception>
		public static byte[] readLineAsBytes(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			using (MemoryStream memoryStream = new MemoryStream())
			{
				while (true)
				{
					int justRead = stream.ReadByte();
					if (justRead == -1 && memoryStream.Length > 0)
						break;

					// Check if we started at the end of the stream we read from
					// and we have not read anything from it yet
					if (justRead == -1 && memoryStream.Length == 0)
						return null;

					char readChar = (char)justRead;

					// Do not write \r or \n
					if (readChar != '\r' && readChar != '\n')
						memoryStream.WriteByte((byte)justRead);

					// Last point in CRLF pair
					if (readChar == '\n')
						break;
				}

				return memoryStream.ToArray();
			}
		}

		/// <summary>
		/// Read a line from the stream. <see cref="readLineAsBytes"/> for more documentation.
		/// </summary>
		/// <param name="stream">The stream to read from</param>
		/// <returns>A line read from the stream or <see langword="null"/> if nothing could be read from the stream</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="stream"/> is <see langword="null"/></exception>
		public static string readLineAsAscii(Stream stream)
		{
			byte[] readFromStream = readLineAsBytes(stream);
			return readFromStream != null ? Encoding.ASCII.GetString(readFromStream) : null;
		}


		/// <summary>
		/// Converts a string to a steam.
		/// use always with a using statment eg: using(var stream =  streamHelper.streamFromString("xxxx") ){}
		/// </summary>
		/// <param name="input">the string to pass to the stream</param>
		/// <returns></returns>
		public static Stream streamFromString(string input)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(input);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}


        /// <summary>
        /// Copy a stream to a file Asynchronus 
        /// </summary>
        /// <param name="streamWithFileContent">Stream with the content of the file</param>
        /// <param name="fileName">The full file path</param>
        /// <returns>Task (void)</returns>
        public static async Task streamToFileAsync(Stream streamWithFileContent, string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await streamWithFileContent.CopyToAsync(fileStream);
            }
        }

        /// <summary>
        /// Download a url to a stream
        /// </summary>
        /// <param name="url">Url to download</param>
        /// <returns>Memory Stream</returns>
        public static async Task<Stream> downloadToStreamAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var fileData = await response.Content.ReadAsByteArrayAsync();
                //this.lastDownloadFileName = Path.GetFileName(url);

                return new MemoryStream(fileData);
                //return File(fileData, "application/octet-stream", fileName);
            }
        }

        /// <summary>
        /// Stream to string
        /// </summary>
        /// <param name="stream">The input Stream</param>
        /// <param name="encoding">The encoding, optional, default is UTF8</param>
        /// <returns></returns>
        public static string streamToString(Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8; // Default to UTF8 if no encoding is provided
            }

            using (var reader = new StreamReader(stream, encoding))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// Convert memory stream to file stream
        /// </summary>
        /// <param name="sourceStream">Source Input stream</param>
        /// <param name="destinationStream">Destination Output Stream</param>
        /// <param name="hideException">Hide or not the exceptions</param>
        /// <param name="positionReset">True:reset the position of the input and output position to 0, False:do not, default is true</param>
        /// <returns>File Stream or null if error with hidden exceptions</returns>
        public static void streamToStream<TSource,TDestination>(TSource sourceStream, TDestination destinationStream, bool hideException, bool positionReset = true)
                                                            where TSource : Stream  
                                                            where TDestination : Stream
        {
            // Ensure the memory stream is positioned at the beginning.
            if (positionReset) sourceStream.Position = 0;

            try
            {
                // Copy the memory stream's contents to the file stream.
                sourceStream.CopyTo(destinationStream);

                // Reset the file stream position to the beginning if needed.
                if (positionReset) destinationStream.Position = 0;

                return;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the copy operation.
                fastLogger.error($"Error converting SourceStream to DestinationStream");
                fastLogger.error(ex);

                // Close and dispose of the file stream to release resources.
                destinationStream.Dispose();
                if (!hideException) throw;

                return; // Or throw the exception, depending on your error handling strategy.
            }

        }

        /// <summary>
        /// Convert memory stream to file stream. The Destination Stream is by ref
        /// </summary>
        /// <param name="sourceStream">Source Input stream</param>
        /// <param name="destinationStream">byref, Destination Output Stream</param>
        /// <param name="hideException">Hide or not the exceptions</param>
        /// <param name="positionReset">True:reset the position of the input and output position to 0, False:do not, default is true</param>
        /// <returns>File Stream or null if error with hidden exceptions</returns>
        public static void streamToStreamByRef<TSource, TDestination>(TSource sourceStream, ref TDestination destinationStream, bool hideException, bool positionReset = true)
                                                            where TSource : Stream
                                                            where TDestination : Stream
        {
            // Ensure the memory stream is positioned at the beginning.
            if (positionReset) sourceStream.Position = 0;

            try
            {
                // Copy the memory stream's contents to the file stream.
                sourceStream.CopyTo(destinationStream);

                // Reset the file stream position to the beginning if needed.
                if (positionReset) destinationStream.Position = 0;

                return;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the copy operation.
                fastLogger.error($"Error converting SourceStream to DestinationStream");
                fastLogger.error(ex);

                // Close and dispose of the file stream to release resources.
                destinationStream.Dispose();
                if (!hideException) throw;

                return; // Or throw the exception, depending on your error handling strategy.
            }

        }


    }


}