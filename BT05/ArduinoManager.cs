using screens;
using shared;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT05
{
    /// <summary>
    /// This connects to the Serial Port on an Arduino and reads the rotation state and scissor state
    /// </summary>
    public sealed class ArduinoManager
    {
        private static readonly ArduinoManager _instance = new ArduinoManager();

        public static ArduinoManager Instance { get {  return _instance; } }

        bool _scissorClosed = false;
        Language _language = Language.english;

        object _scissorLock = new object();
        object _rotationLock = new object();
        object _languageLock = new object();

        public Language Lang
        {
            get { 
                Language lang = Language.none;
                lock(_languageLock)
                {
                    lang = _language;
                }

                return lang; 
            
            }

            set {
                lock (_languageLock)
                {
                    bool hasChanged = false;
                    Language newValue = value;

                    if (_language != newValue)
                    {
                        hasChanged = true;
                        _language = newValue;
                    }

                    if (hasChanged)
                    {
                        GameManager.Instance.LanguageChangedByArduino(newValue);
                    }
                }
            }
        }

        public bool IsScissorOpen
        {
            get
            {
                // you probably don't need to lock a bool - but let's be on the safe side
                bool scissorClosed = false;
                lock (_scissorLock)
                {
                    scissorClosed = _scissorClosed;
                }

                return !scissorClosed;
            }
        }

        public bool IsScissorClosed
        {
            get
            {
                // you probably don't need to lock a bool - but let's be on the safe side
                bool scissorClosed = false;
                lock (_scissorLock)
                {
                    scissorClosed = _scissorClosed;
                }

                return scissorClosed;
            }
        }

        bool ScissorClosed
        {
            set
            {
                lock (_scissorLock)
                {
                    _scissorClosed = value;
                }
            }
        }


        /// <summary>
        /// This tells you which way we've gone since the last read
        /// </summary>
        /// <returns></returns>
        public int GetLatestRotation
        {
            get
            {
                int rotationChange = 0;
                
                lock (_rotationLock)
                {
                    rotationChange = _rotationTicks - _lastRotationRead;
                    _lastRotationRead = _rotationTicks;
                }

                return Math.Clamp(rotationChange, -MAX_ROTATION, MAX_ROTATION);
            }
        }

        int MAX_ROTATION = 50;
        int _rotationTicks;
        int _lastRotationRead = 0;

        int RotationTick
        {
            set
            {
                lock (_rotationLock)
                { 
                    _rotationTicks = value;
                }
            }
        }

        SerialPort _mySerialPort;

        public void OpenFirstSerialPort()
        {
            var portNames = SerialPort.GetPortNames();
            if (portNames.Length==1)
            {
                OpenSerialPort(portNames[0]);
            }
            else if (portNames.Length==0) 
            {
                // error - not ports found
            }
            else
            {
                string match = "COM4";
                foreach(var portName in portNames)
                {
                    if (portName.Contains(match))
                    {
                        OpenSerialPort(portName);
                    }
                }

            }
        }

        void OpenSerialPort(string portName)
        {
            try
            {
                _mySerialPort = new SerialPort(portName);

                //SerialPort mySerialPort = new SerialPort("COM1");

                _mySerialPort.BaudRate = 9600;
                _mySerialPort.Parity = Parity.None;
                _mySerialPort.StopBits = StopBits.One;
                _mySerialPort.DataBits = 8;
                _mySerialPort.Handshake = Handshake.None;
                _mySerialPort.RtsEnable = true;

                _mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                _mySerialPort.Open();
                //_mySerialPort.Write("M"); // initiate the query
            }
            catch(Exception ex)
            {
                DebugOutput.Instance.WriteError("Unable to open arduino serial port: " +ex.Message);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            var indata = sp.ReadLine();


            var localMessage = indata.TrimEnd('\r', '\n');

            // messages are: SCISSOR|OPEN / CLOSED
            //               ROTATE|integer

            try
            {
                var sections = localMessage.Trim().Split("|");
                if (sections.Length == 2)
                {
                    string command = sections[0];
                    string parameter = sections[1];

                    if (string.Compare(command, "SCISSOR", true) == 0)
                    {
                        if (string.Compare(parameter, "OPEN", true) == 0)
                        {
                            ScissorClosed = false;
                        }

                        if (string.Compare(parameter, "CLOSE", true) == 0)
                        {
                            ScissorClosed = true;
                        }
                    }

                    if (string.Compare(command, "ROTATE", true) == 0)
                    {
                        if (Int32.TryParse(parameter, out int newRotation))
                        {
                            RotationTick = newRotation;
                        }
                    }

                    if (string.Compare(command, "WAVE", true) == 0)
                    {
                        if (MyScreenManager.Instance.CurrentScreen != null)
                        {
                            MyScreenManager.Instance.CurrentScreen.WaveHappened();
                        }
                    }

                    if (string.Compare(command, "LANGUAGE", true) == 0)
                    {
                        if (string.Compare(parameter, "ENGLISH", true) == 0)
                        {
                            Language lang = Language.english;
                            Lang = lang;
                        }

                        if (string.Compare(parameter, "HINDI", true) == 0)
                        {
                            Language lang = Language.hindi;
                            Lang = lang;
                        }
                    }
                }
                else
                {
                    string error = "Error Processing Message (EXPECTING 2 PARTS): " + indata;
                }
            }
            catch (Exception ex)
            {
                string error = "Error Processing Message (EXCEPTION): " + indata +"\n" + ex.Message;
            }
        }

        public void CloseSerialPort()
        {
            if (_mySerialPort!= null)
            {
                _mySerialPort.Close();
            }
        }
    }
}