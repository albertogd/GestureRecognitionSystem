using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Models;

namespace Algorithms.Audio
{
    public class SpeechRecognizer
    {
        /// <summary>
        /// Active Kinect sensor.
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        /// <summary>
        /// Speech recognition eventHandler
        /// </summary>
        private EventHandler<SpeechRecognizedEventArgs> speechRecognizedEventHandler;

        /// <summary>
        /// Speech recognition Language
        /// </summary>
        private Language language;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="speechRecognizedEvent">Speech recognition eventHandler</param>
        public SpeechRecognizer(EventHandler<SpeechRecognizedEventArgs> speechRecognizedEvent, Language lang)
        {
            speechRecognizedEventHandler = speechRecognizedEvent;
            language = lang;
        }

        public bool start()
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                try
                {
                    // Start the sensor!
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    this.sensor = null;
                    return false;
                }
            }

            if (null == this.sensor)
            {
                return false;
            }

            RecognizerInfo ri = GetKinectRecognizer(language);

            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                // Create a grammar from grammar definition XML file.
                string grammar = (language == Language.Spanish) ? Properties.Resources.SpeechGrammar_es_ES : Properties.Resources.SpeechGrammar;
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(grammar)))
                {
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);
                }

                speechEngine.SpeechRecognized += speechRecognizedEventHandler;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool close()
        {
            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();

                this.sensor.Stop();
                this.sensor = null;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= speechRecognizedEventHandler;
                this.speechEngine.RecognizeAsyncStop();
            }

            return true;
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo GetKinectRecognizer(Language language)
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string code, value;

                if (language == Language.Spanish)
                    code = "es-ES";
                else
                    code = "en-US";
                
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);

                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && code.Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }
    }
}
