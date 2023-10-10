using System;
using System.Collections.Generic;
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

        object _scissorLock = new object();
        object _rotationLock = new object();

        public bool IsScissorOpen
        {
            get
            {
                // you probably don't need to lock a bool - but let's be on the safe side
                bool scissorClosed = _scissorClosed;
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
                bool scissorClosed = _scissorClosed;
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
