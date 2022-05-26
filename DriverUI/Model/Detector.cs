using BindingEnums;
using DriverUI.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DetectorUI.Model
{
    /// <summary>
    /// ASCII PROTOCOL
    ///The RS485 serial port can be configured for ASCII protocol, which is intended for applications that
    ///don’t require custom software on the host side.Off the shelf terminal emulation software can be used
    ///to receive messages from the device. Percent LEL and sensor readings are sent once per second and
    ///user prompt messages are sent during the calibration process to guide the user at each step.Default
    ///serial settings are 9600 baud, 1 stop bit, and no parity.Protocol and serial parameters should be
    ///selected with the HART handheld communicator
    /// </summary>
    /*
        ASCII PROTOCOL
        The RS485 serial port can be configured for ASCII protocol, which is intended for applications that
        don’t require custom software on the host side. Off the shelf terminal emulation software can be used
        to receive messages from the device. Percent LEL and sensor readings are sent once per second and
        user prompt messages are sent during the calibration process to guide the user at each step. Default
        serial settings are 9600 baud, 1 stop bit, and no parity. Protocol and serial parameters should be
        selected with the HART handheld communicator
     */

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum BaudRate
    {
        [Description ("1200 baudrate/second")]
        Baud_Rate_1200 = 0,
        [Description("2400 baudrate/second")]
        Baud_Rate_2400 = 1,
        [Description("4800 baudrate/second")]
        Baud_Rate_4800 = 2,
        [Description("9600 baudrate/second")]
        Baud_Rate_9600 = 3,
        [Description("19200 baudrate/second")]
        Baud_Rate_19200 = 4
    }
    public enum Party
    {
        None = 0,
        Even = 1,
        Odd = 2
    }
    public enum GasType
    {
        Methane = 0,
        Ethane = 1,
        Propane = 2,
        Ethylene = 3,
        Propylene = 4,
        Butane = 5,
        Special = 9
    }
    public enum CalibrationGasType
    {
        SameAsMeasured = 0,
        Methane = 1,
        Propane = 2
    }
    public enum CalibrationMethod
    {
        Standart = 0,
        Cuvette = 1
    }
    public enum AnalogFaultCode
    {
        Eclipse = 0,
        PIR_9400 = 1,
        UserDefined = 2
    }
    public enum CalibrationStep
    {
        WaitingToStart = 0,
        WaitingForZero = 1,
        WaitingForSignal = 2,
        WaitingForGas = 3,
        WaitingForSpan = 4,
        WaitingForEnd = 5,
        CalibrationTerminated = 6,
        CalibrationComplete = 7
    }
    public enum AlarmLatchConfiguration
    {
        NonLatching = 0,
        Latching = 1
    }
    
    [INotifyPropertyChanged]
    partial class Detector 
    {
       
       
        #region Private Field
        [ObservableProperty] private string _name;
        
        // factory constants
        [ObservableProperty] private int _deviceType;        //4001
        [ObservableProperty] private int _firmwareVersion;   //4003
        [ObservableProperty] private int _serialNumberLSW;   //4004
        [ObservableProperty] private int _serialNumberMSW;   //4005
        //private readonly ulong _serialNumber;
        [ObservableProperty] private int _manufactureYear;   //4006
        [ObservableProperty] private int _manufactureMonth;  //4007
        [ObservableProperty] private int _manufactureDay;    //4008

        // device configuration read/write fields
        [ObservableProperty] private int _modbusPollingAddress;          //40101
        [ObservableProperty] private BaudRate _baudRateCode;             //40102
        [ObservableProperty] private Party _parityCode;                  //40103
        [ObservableProperty] private GasType _gasType;                   //40104
        [ObservableProperty] private CalibrationGasType _calibrationGasType;            //40105
        [ObservableProperty] private CalibrationMethod _calibrationMethod;             //40106
        [ObservableProperty] private int _calibrationCuvetteLengthLSW;   //40107
        [ObservableProperty] private int _calibrationCuvetteLengthMSW;   //40108
        //private readonly float _calibrationCuvetteLength;    //1.0 to 150.0 mm
        [ObservableProperty] private AnalogFaultCode _analogFaultCode;   //40109
        [ObservableProperty] private int _f4_to_20_RangeLSW;              //40110
        [ObservableProperty] private int _f4_to_20_RangeMSW;              //40110
        //private readonly float _4_to_20_Range;               //20 to 100% LEL
        [ObservableProperty] private int _calibrationGasConcentrationLSW;//40112
        [ObservableProperty] private int _calibrationGasConcentrationMSW;//40112
        //private readonly float _calibrationGasConcentration; //20 to 100% LEL
        [ObservableProperty] private int _warmUpFaultLevelLSW;            //40114
        [ObservableProperty] private int _warmUpFaultLevelMSW;            //40115
        //private readonly float _warmUpFaultLevel;             //4-20 mA
        [ObservableProperty] private int _blockedOpticsFaultLevelLSW;     //40116
        [ObservableProperty] private int _blockedOpticsFaultLevelMSW;     //40117
        //private readonly float _blockedOpticsFaultLevel;      //4-20 mA
        [ObservableProperty] private int _calibrationCurrentLevelLSW;     //40118
        [ObservableProperty] private int _calibrationCurrentLevelMSW;     //40119
        //private readonly float _calibrationCurrentLevel;      //4-20 mA
        [ObservableProperty] private int _generalFaultCurrentLevelLSW;    //40120
        [ObservableProperty] private int _generalFaultCurrentLevelMSW;    //40121
        //private readonly float _generalFaultCurrentLevel;     //4-20 mA
        [ObservableProperty] private int _volume_at_LELLSW;              //40122
        [ObservableProperty] private int _volume_at_LELMSW;              //40123
        //private readonly float _volume_at_LEL;
        [ObservableProperty] private int _gasCoefficient_A_LSW;          //40124
        [ObservableProperty] private int _gasCoefficient_A_MSW;          //40125
        //private readonly float _gasCoefficient_A;            //Special Gas Type
        [ObservableProperty] private int _gasCoefficient_B_LSW;          //40126
        [ObservableProperty] private int _gasCoefficient_B_MSW;          //40127
        //private readonly float _gasCoefficient_B;            //Special Gas Type
        [ObservableProperty] private int _gasCoefficient_C_LSW;          //40128
        [ObservableProperty] private int _gasCoefficient_C_MSW;          //40129
        //private readonly float _gasCoefficient_C;            //Special Gas Type
        [ObservableProperty] private int _gasCoefficient_D_LSW;          //40130
        [ObservableProperty] private int _gasCoefficient_D_MSW;          //40131
        //private readonly float _gasCoefficient_D;            //Special Gas Type
        [ObservableProperty] private int _gasCoefficient_E_LSW;          //40132
        [ObservableProperty] private int _gasCoefficient_E_MSW;          //40133
        //private readonly float _gasCoefficient_E;            //Special Gas Type
        [ObservableProperty] private int _lowAlarmLevelLSW;              //40134
        [ObservableProperty] private int _lowAlarmLevelMSW;              //40135
        //private readonly float _lowAlarmLevel;
        [ObservableProperty] private int _highAlarmLevelLSW;             //40136
        [ObservableProperty] private int _highAlarmLevelMSW;             //40137
        //private readonly float _highAlarmLevel;
        [ObservableProperty] private int _lowAlarmLatch;                 //40138
        [ObservableProperty] private int _highAlarmLatch;                //40139

        // device status read only
        /*
         * Device Fault (any fault)     0
         * Calibration Active           1
         * Warm up Mode                 2
         * Low Alarm Active             3
         * High Alarm Active            4
         * Output Current Fixed         5
         * Modbus Write Protect         6
         * Calibration Input Active     7
         * Magnetic Switch Active       8
         * Hart Initiated Self Test     9
         * Reserved                     10
         * Response Test Active         11
         * Manual Self Test Active      12  
         */
        [ObservableProperty] private int _generalStatusBits;             //40201

        /*
         * Calibration Fault            0
         * Dirty Optics                 1
         * Open Lamp                    2
         * Cal Active at start          3
         * EE Error 1                   4
         * EE Error 2                   5
         * Ref ADC Saturated            6
         * Active ADC Saturated         7
         * Bad 24 volts                 8
         * Bad 12 volts                 9
         * Bad 5 volts                  10
         * Zero Drift                   11
         * Flash CRC Error              12
         * Ram Error                    13
         */

        [ObservableProperty] private int _faultStatusBits;               //40202
        [ObservableProperty] private int _gasLevel_in_LELLSW;            //40203
        [ObservableProperty] private int _gasLevel_in_LELMSW;            //40204
        //private readonly float _gasLevel_in_LEL;
        [ObservableProperty] private CalibrationStep _calibrationStep;               //40205
        [ObservableProperty] private int _activeSensorSignalLSW;         //40206
        [ObservableProperty] private int _activeSensorSignalMSW;         //40207
        //private readonly float _activeSensorSignal;
        [ObservableProperty] private int _referenceSensorSignalLSW;      //40208
        [ObservableProperty] private int _referenceSensorSignalMSW;      //40209
        //private readonly float _referenceSensorSignal;
        [ObservableProperty] private int _sensorRatioLSW;                //40210
        [ObservableProperty] private int _sensorRatioMSW;                //40211
        //private readonly float _sensorRatio;
        [ObservableProperty] private int _sensorAbsorptionSW;            //40212
        [ObservableProperty] private int _sensorAbsorptionMSW;           //40213
        //private readonly float _sensorAbsorption;
        [ObservableProperty] private int _temperatureLSW;                //40214
        [ObservableProperty] private int _temperatureMSW;                //40215
        //private readonly float _temperature;
        [ObservableProperty] private int _hourMeterLSW;                    //40216
        [ObservableProperty] private int _hourMeterMSW;                    //40217
        //private readonly uint _hourMeter;
        [ObservableProperty] private int _maxTemperatureLSW;               //40218
        [ObservableProperty] private int _maxTemperatureMSW;               //40219
        //private readonly float _maxTemperature;
        [ObservableProperty] private int _maxTemperatureHourLSW;           //40220
        [ObservableProperty] private int _maxTemperatureHourMSW;           //40221
        //private readonly ulong _maxTemperatureHour;
        [ObservableProperty] private int _maxTemperatureSinceResetLSW;     //40222
        [ObservableProperty] private int _maxTemperatureSinceResetMSW;     //40223
        //private readonly float _maxTemperatureSinceReset;
        [ObservableProperty] private int _maxTemperatureHourSinceResetLSW; //40224
        [ObservableProperty] private int _maxTemperatureHourSinceResetMSW; //40225
        //private readonly ulong _maxTemperatureHourSinceReset;
        [ObservableProperty] private uint _ramErrorCode;              //40226
        [ObservableProperty] private int _volume_at_LELLSW_ro;        //40227
        [ObservableProperty] private int _volume_at_LELMSW_ro;        //40228
        //private readonly float _volume_at_LEL_ro;
        [ObservableProperty] private int _gasCoefficient_A_LSW_ro;    //40229
        [ObservableProperty] private int _gasCoefficient_A_MSW_ro;    //40230
        //private readonly float _gasCoefficient_A_ro;               //Special Gas Type
        [ObservableProperty] private int _gasCoefficient_B_LSW_ro;    //40231
        [ObservableProperty] private int _gasCoefficient_B_MSW_ro;    //40232
        //private readonly float _gasCoefficient_B_ro;               //Special Gas Type
        [ObservableProperty] private int _gasCoefficient_C_LSW_ro;    //40233
        [ObservableProperty] private int _gasCoefficient_C_MSW_ro;    //40234
        //private readonly float _gasCoefficient_C_ro;               //Special Gas Type
        [ObservableProperty] private int _gasCoefficient_D_LSW_ro;    //40235
        [ObservableProperty] private int _gasCoefficient_D_MSW_ro;    //40236
        //private readonly float _gasCoefficient_D_ro;               //Special Gas Type
        [ObservableProperty] private int _gasCoefficient_E_LSW_ro;    //40237
        [ObservableProperty] private int _gasCoefficient_E_MSW_ro;    //40238
        //private readonly float _gasCoefficient_E_ro;               //Special Gas Type
        [ObservableProperty] private int _minTemperatureLSW;          //40239
        [ObservableProperty] private int _minTemperatureMSW;          //40240
        //private readonly float _minTemperature;
        [ObservableProperty] private int _minTemperatureHourLSW;      //40241
        [ObservableProperty] private int _minTemperatureHourMSW;      //40242
        //private readonly ulong _minTemperatureHour;
        [ObservableProperty] private int _minTemperatureSinceResetLSW;     //40243
        [ObservableProperty] private int _minTemperatureSinceResetMSW;     //40244
        //private readonly float _minTemperatureSinceReset;
        [ObservableProperty] private int _minTemperatureHourSinceResetLSW; //40245
        [ObservableProperty] private int _minTemperatureHourSinceResetMSW; //40246
        //private readonly ulong _minTemperatureHourSinceReset;
        [ObservableProperty] private int _fixed_4_to_20_RangeLSW;        //40247
        [ObservableProperty] private int _fixed_4_to_20_RangeMSW;        //40248
        //private readonly float _fixed_4_to_20_Range;
        [ObservableProperty] private int _zeroRatioLSW;                  //40253
        [ObservableProperty] private int _zeroRatioMSW;                  //40254
        //private readonly float _zeroRatio;
        [ObservableProperty] private int _spanFactorLSW;                 //40255
        [ObservableProperty] private int _spanFactorMSW;                 //40256
        //private readonly float _spanFactor;
        [ObservableProperty] private int _f5_Volt_Power_Supply_ValueLSW;  //40257
        [ObservableProperty] private int _f5_Volt_Power_Supply_ValueMSW;  //40258
        //private readonly float _5_Volt_Power_Supply_Value;
        [ObservableProperty] private int _f12_Volt_Power_Supply_ValueLSW; //40259
        [ObservableProperty] private int _f12_Volt_Power_Supply_ValueMSW; //40260
        //private readonly float _12_Volt_Power_Supply_Value;
        [ObservableProperty] private int _f24_Volt_Power_Supply_ValueLSW; //40261
        [ObservableProperty] private int _f24_Volt_Power_Supply_ValueMSW; //40262
        //private readonly float _24_Volt_Power_Supply_Value;

        // control words
        /* 
         * Start Calibration            0
         * Abort Calibration            1
         * Warm up Mode                 2
         * Low Alarm Active             3
         * High Alarm Active            4
         * Output Current Fixed         5
         * Modbus Write Protect         6
         * Calibration Input Active     7
         * Magnetic Switch Active       8
         * Hart Initiated Self Test     9
         * Reserved                     10
         * Response Test Active         11
         * Manual Self Test Active      12
         * End Response Test            13
         * Reserved                     14
         * Start Manual Self Test       15        
         */
        [ObservableProperty] private int[] _commandWord = new int[1]; //40301 
        
        [ObservableProperty] private BitArray _generalStatus;
        [ObservableProperty] private BitArray _faultStatus;
        [ObservableProperty] private BitArray _command;
        
        #endregion
       
        public Detector()
        {
           // StartCommand = new RelayCommand( StartExecuted, CanExecuted );
            //StopCommand = new RelayCommand( StopExecuted, CanExecuted );
            //QuickStopCommand = new RelayCommand( QuickStopExecuted, CanExecuted );
            //ForwardCommand = new RelayCommand( ForwardExecuted, CanExecuted );
            //ReverseCommand = new RelayCommand( ReverseExecuted, CanExecuted );
            //ResetCommand = new RelayCommand( ResetExecuted, CanExecuted );
        }
        #region Property Field

        public ulong SerialNumber => ReadModbusUlongValue(SerialNumberLSW, SerialNumberMSW);
        public float CalibrationCuvetteLength => ReadModbusFloatingPointValue(lsw: CalibrationCuvetteLengthLSW, msw: CalibrationCuvetteLengthMSW);
        public float mARange => ReadModbusFloatingPointValue(lsw: F4_to_20_RangeLSW, msw: F4_to_20_RangeMSW);
        public float CalibrationGasConcentration => ReadModbusFloatingPointValue(lsw: CalibrationGasConcentrationLSW, msw: CalibrationGasConcentrationMSW);
        public float WarmUpFaultLevel => ReadModbusFloatingPointValue(lsw: WarmUpFaultLevelLSW, msw: WarmUpFaultLevelMSW);
        public float BlockedOpticsFaultLevel => ReadModbusFloatingPointValue(lsw: BlockedOpticsFaultLevelLSW, msw: BlockedOpticsFaultLevelMSW);
        public float CalibrationCurrentLevel => ReadModbusFloatingPointValue(lsw: CalibrationCurrentLevelLSW, msw: CalibrationCurrentLevelMSW);
        public float GeneralFaultCurrentLevel => ReadModbusFloatingPointValue(lsw: GeneralFaultCurrentLevelLSW, msw: GeneralFaultCurrentLevelMSW);
        public float Volume_at_LEL => ReadModbusFloatingPointValue(lsw: Volume_at_LELLSW, msw: Volume_at_LELMSW);
        public float GasCoefficient_A => ReadModbusFloatingPointValue(lsw: GasCoefficient_A_LSW, msw: GasCoefficient_A_MSW);
        public float GasCoefficient_B => ReadModbusFloatingPointValue(lsw: GasCoefficient_B_LSW, msw: GasCoefficient_B_MSW);
        public float GasCoefficient_C => ReadModbusFloatingPointValue(lsw: GasCoefficient_C_LSW, msw: GasCoefficient_C_MSW);
        public float GasCoefficient_D => ReadModbusFloatingPointValue(lsw: GasCoefficient_D_LSW, msw: GasCoefficient_D_MSW);
        public float GasCoefficient_E => ReadModbusFloatingPointValue(lsw: GasCoefficient_E_LSW, msw: GasCoefficient_E_MSW);
        public float LowAlarmLevel => ReadModbusFloatingPointValue(lsw: LowAlarmLevelLSW, msw: LowAlarmLevelMSW);
        public float HighAlarmLevel => ReadModbusFloatingPointValue(lsw: HighAlarmLevelLSW, msw: HighAlarmLevelMSW);
        public float GasLevel_in_LEL => ReadModbusFloatingPointValue(lsw: GasLevel_in_LELLSW, msw: GasLevel_in_LELMSW);
        public float ActiveSensorSignal => ReadModbusFloatingPointValue(lsw: ActiveSensorSignalLSW, msw: ActiveSensorSignalMSW);
        public float ReferenceSensorSignal => ReadModbusFloatingPointValue(lsw: ReferenceSensorSignalLSW, msw: ReferenceSensorSignalMSW);
        public float SensorRatio => ReadModbusFloatingPointValue(lsw: SensorRatioLSW, msw: SensorRatioMSW);
        public float SensorAbsorption => ReadModbusFloatingPointValue(lsw: SensorAbsorptionSW, msw: SensorAbsorptionMSW);
        public float Temperature => ReadModbusFloatingPointValue(lsw: TemperatureLSW, msw: TemperatureMSW);
        public uint HourMeter => ReadModbusUintValue(lsw: HourMeterLSW, msw: HourMeterMSW);
        public float MaxTemperature => ReadModbusFloatingPointValue(lsw: MaxTemperatureLSW, msw: MaxTemperatureMSW);
        public ulong MaxTemperatureHour => ReadModbusUlongValue(lsw: MaxTemperatureHourLSW, msw: MaxTemperatureHourMSW);
        public float MaxTemperatureSinceReset => ReadModbusFloatingPointValue(lsw: MaxTemperatureSinceResetLSW, msw: MaxTemperatureSinceResetMSW);
        public ulong MaxTemperatureHourSinceReset => ReadModbusUlongValue(lsw: MaxTemperatureHourSinceResetLSW, msw: MaxTemperatureHourSinceResetMSW);
        public float Volume_at_LEL_ro => ReadModbusFloatingPointValue(lsw: Volume_at_LELLSW_ro, msw: Volume_at_LELMSW_ro);
        public float GasCoefficient_A_ro => ReadModbusFloatingPointValue(lsw: GasCoefficient_A_LSW_ro, msw: GasCoefficient_A_MSW_ro);
        public float GasCoefficient_B_ro => ReadModbusFloatingPointValue(lsw: GasCoefficient_B_LSW_ro, msw: GasCoefficient_B_MSW_ro);
        public float GasCoefficient_C_ro => ReadModbusFloatingPointValue(lsw: GasCoefficient_C_LSW_ro, msw: GasCoefficient_C_MSW_ro);
        public float GasCoefficient_D_ro => ReadModbusFloatingPointValue(lsw: GasCoefficient_D_LSW_ro, msw: GasCoefficient_D_MSW_ro);
        public float GasCoefficient_E_ro => ReadModbusFloatingPointValue(lsw: GasCoefficient_E_LSW_ro, msw: GasCoefficient_E_MSW_ro);
        public float MinTemperature => ReadModbusFloatingPointValue(lsw: MinTemperatureLSW, msw: MinTemperatureMSW);      
        public ulong MinTemperatureHour => ReadModbusUlongValue(lsw: MinTemperatureHourLSW, msw: MinTemperatureHourMSW);       
        public float MinTemperatureSinceReset => ReadModbusFloatingPointValue(lsw: MinTemperatureSinceResetLSW, msw: MinTemperatureSinceResetMSW);
        public ulong MinTemperatureHourSinceReset => ReadModbusUlongValue(lsw: MinTemperatureHourSinceResetLSW, msw: MinTemperatureHourSinceResetMSW);
        public float Fixed_4_to_20_Range => ReadModbusFloatingPointValue(lsw: Fixed_4_to_20_RangeLSW, msw: Fixed_4_to_20_RangeMSW);
        public float ZeroRatio => ReadModbusFloatingPointValue(lsw: ZeroRatioLSW, msw: ZeroRatioMSW);
        public float SpanFactor => ReadModbusFloatingPointValue(lsw: SpanFactorLSW, msw: SpanFactorMSW);
        public float Five_Volt_Power_Supply_Value => ReadModbusFloatingPointValue(lsw: F5_Volt_Power_Supply_ValueLSW, msw: F5_Volt_Power_Supply_ValueMSW);
        public float Twelve_Volt_Power_Supply_Value => ReadModbusFloatingPointValue(lsw: F12_Volt_Power_Supply_ValueLSW, msw: F12_Volt_Power_Supply_ValueMSW);
        public float TwentyFour_Volt_Power_Supply_Value => ReadModbusFloatingPointValue(lsw: F24_Volt_Power_Supply_ValueLSW, msw: F24_Volt_Power_Supply_ValueMSW);
       

        #endregion
        #region Method Field
        public float ReadModbusFloatingPointValue(int lsw, int msw)
        {
            uint a = (uint)lsw, b = (uint)msw;
            return BitConverter.ToSingle(BitConverter.GetBytes(b << 16 | a), 0);
        }
        public ulong ReadModbusUlongValue(int lsw, int msw)
        {
            uint a = (uint)lsw;
            ulong b = (uint)msw;
            return b << 32 | a;
        }
        public uint ReadModbusUintValue(int lsw, int msw)
        {
            uint a = (uint)lsw;
            uint b = (uint)msw;
            return b << 32 | a;
        }
        public void IntToBoolean()
        {
            _generalStatus = new BitArray(GeneralStatusBits);
            _faultStatus = new BitArray(FaultStatusBits);
        }
        public void BolleanToInt()
        {
            _command.CopyTo(CommandWord, 0);
        }
        public string Ok { get; init; }

        private bool CanExecuted( object parametr ) => true;
        [ICommand]
        public void Start(object parametr) => Start = true;
        [ICommand]
        private void Stop(object parametr) => _stop = true;
        [ICommand]
        private void QuickStop(object parametr) => _quickStop = true;
        [ICommand]
        private void Reset(object parametr) => _reset = true;
        [ICommand]
        private void Forward(object parametr) => _forward = true;
        [ICommand]
        private void Reverse(object parametr) => _reverse = true;

        //public void ToStart()
        //{
        //    if ( _start )
        //    {
        //        VFDStatus c = ( VFDStatus ) _statusWord;
        //        switch ( c )
        //        {
        //            case VFDStatus.switchOnDisabled & ( VFDStatus ) 0xff:
        //                ControlWord = ( int ) VFDControl.shotDown;
        //                goto case VFDStatus.readyToSwitchOn & ( VFDStatus ) 0xff;
        //            case VFDStatus.readyToSwitchOn & ( VFDStatus ) 0xff:
        //                ControlWord = ( int ) VFDControl.switchOn;
        //                break;
        //        }
        //        _start = false;
        //    }
        //}
        //public void ToStop()
        //{
        //    if ( _stop )
        //    {
        //        if ( StatusWord != (int) ( VFDStatus.readyToSwitchOn & ( VFDStatus ) 0xff ) )
        //            ControlWord = ( int ) VFDControl.shotDown;
        //        _stop = false;
        //    }
        //}
        //public void ToQuickStop()
        //{
        //    if ( _quickStop )
        //    {
        //        if ( StatusWord != (int)( VFDStatus.quickStopActive & ( VFDStatus ) 0xff ) )
        //            ControlWord = ( int ) VFDControl.quickStop;
        //        _quickStop = false;
        //    }
        //}
        //public void Moving()
        //{
        //    if ( _forward )
        //    {
        //        VFDStatus c = ( VFDStatus ) _statusWord;
        //        switch ( c )
        //        {
        //            case VFDStatus.switchedOn & ( VFDStatus ) 0xff:
        //            case VFDStatus.reverseOperationEnabled & ( VFDStatus )(-3841):
        //                ControlWord = ( int ) VFDControl.forward;
        //                break;
        //        }
        //        _forward = false;
        //    }
        //    else if ( _reverse )
        //    {
        //        VFDStatus c = ( VFDStatus ) _statusWord;
        //        switch ( c )
        //        {
        //            case VFDStatus.switchedOn & ( VFDStatus ) 0xff:
        //            case VFDStatus.forwardOperationEnabled & ( VFDStatus ) 0xff:
        //                ControlWord = ( int ) VFDControl.reverse;
        //                break;
        //        }
        //        _reverse = false;
        //    }
        //}
        //public void ToReset()
        //{
        //    if ( _reset )
        //    {
        //        if ( StatusWord == (int) ( VFDStatus.fault & ( VFDStatus ) 0xf ) )
        //            ControlWord = ( int ) VFDControl.faultReset;
        //        _reset = false;
        //    }
        //}
        #endregion
    }
}
