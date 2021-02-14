using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dissonance;
using Dissonance.Audio.Capture;
using NAudio.Wave;

namespace PheggMod
{
	class DissonanceShit : IMicrophoneSubscriber
	{
		// Create a buffer to hold unprocessed data. A `Queue` is not ideal,
		// in a real world situation you should use something like a
		// ring buffer.
		Queue<float> _transferBuffer = new Queue<float>(8192);

		// Store the format of data that we are expecting from Dissonance.
		WaveFormat _format = null;

		// Set a flag indicating that a `Reset` occurred
		bool _reset = false;

		// This will be called automatically by Dissonance.
		// This is **not** called on the main thread! Be careful what you do in this method.
		void IMicrophoneSubscriber.Reset()
		{
			// The audio pipeline is being reset. Throw away any buffered data
			// that has not been processed yet. This is all done inside a lock
			// to ensure thread safety. In your code you MUST ensure that any
			// locks are held for the shortest time possible!
			lock (_transferBuffer)
			{
				_transferBuffer.Clear();
				_format = null;
				_reset = true;
			}
		}

		// This will be called automatically by Dissonance.
		// This is **not** called on the main thread! Be careful what you do in this method.
		void IMicrophoneSubscriber.ReceiveMicrophoneData(
			ArraySegment<float> buffer,
			WaveFormat format
		)
		{
			lock (_transferBuffer)
			{
				// If the format is null just store the current format
				if (_format == null)
					_format = format;

				// Otherwise, check that the format has not changed. A change
				// here indicates a bug!
				else if (!_format.Equals(format))
					throw new InvalidOperationException(
						"Format was changed " +
						"without a call to reset!"
					);

				// Copy all of the data into the _transferBuffer.
				for (var i = 0; i < buffer.Count; i++)
					_transferBuffer.Enqueue(buffer.Array[buffer.Offset + i]);
			}
		}

		// Create another buffer of data to process on the main thread.
		float[] _readyForProcessing = new float[1024];

		// This is the normal unity `Update` method
		void Update()
		{
			var reset = false;

			// Copy data out of the `_transferBuffer` to the
			// `_readyForProcessing` buffer
			lock (_transferBuffer)
			{
				if (_transferBuffer.Count >= _readyForProcessing.Length)
				{
					for (var i = 0; i < _readyForProcessing.Length; i++)
						_readyForProcessing[i] = _transferBuffer.Dequeue();
				}

				// Copy the reset flag
				reset = _reset;
			}

			// A reset occurred on the other thread. Whatever is processing the
			// audio should be reset as well. For example if you are writing to
			// a file, the file handle shold be flushed and closed here.
			if (reset)
				ResetAudioProcessing();

			// `_readyForProcessing` now holds 1024 samples of audio data. You
			// can do anything you want with this data.
			ProcessAudio(_readyForProcessing);
		}

		void ResetAudioProcessing()
		{
			//todo: implement this to do whatever you want!
		}

		void ProcessAudio(float[] data)
		{
			//todo: implement this to do whatever you want!
		}
	}
}
