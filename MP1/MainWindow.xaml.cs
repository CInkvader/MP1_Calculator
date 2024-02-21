using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Serialization;

namespace MP1
{
    public partial class MainWindow : Window
    {
        private static Color _tbHistoryColor = (Color)ColorConverter.ConvertFromString("#FF5A5A5A");
        private static SolidColorBrush _tbHistoryColorBrush = new SolidColorBrush(_tbHistoryColor);

        private static Color _selectedModeColor = (Color)ColorConverter.ConvertFromString("#FF91EBDC");
        private static SolidColorBrush _selectedModeColorBrush = new SolidColorBrush(_selectedModeColor);

        private Button[] _btnNumpad = [];
        private Button[] _btnHexpad = [];

        private enum Modes
        {
            Hexadecimal,
            Decimal,
            Octal,
            Binary
        }

        private bool[] _modeStatus = [];
        private Rectangle[] _modeIndicator = []; 
        private int _num1 = 0;
        private int _num2 = 0;
        private string[] _convertedInput = [];

        private int _operation = -1;
        private int _modeSelected = 1;
        private string[] _history = [];

        private bool _inputStart = false;
        private bool _num2Input = false;

        public MainWindow()
        {
            InitializeComponent();
            _btnNumpad = [btnNum0, btnNum1, btnNum2, btnNum3, btnNum4, btnNum5, btnNum6, btnNum7, btnNum8, btnNum9];
            _btnHexpad = [btnHexA, btnHexB, btnHexC, btnHexD, btnHexE, btnHexF];
            _modeIndicator = [rctHEX, rctDEC, rctOCT, rctBIN];
            _modeStatus = [false, false, false, false];
            _history = new string[4];
            _convertedInput = new string[4];

            SetModeSelected((int)Modes.Decimal);
        }

        private void NumpadAdd(int number, string letter = "")
        {
            string input = number.ToString();
            if (!_num2Input)
            {
                tbHistory.Text = string.Empty;
            }
            if (!_inputStart)
            {
                _inputStart = true;
                tbInput.Text = string.Empty;
            }
            if (letter != "")
            {
                input = letter;
            }

            switch (_modeSelected)
            {
                case 0:
                    NumpadAddHexadecimal(input);
                    break;
                case 1:
                    NumpadAddDecimal(number);
                    break;
                case 2:
                    NumpadAddOctal(input);
                    break;
                case 3:
                    NumpadAddBinary(input);
                    break;
            }
            OutputUpdateAll(_num1);
        }
        private void SetConvertedInput(int num)
        {
            _convertedInput[0] = OutputHexadecimal(num);
            _convertedInput[1] = OutputDecimal(num);
            _convertedInput[2] = OutputOctal(num);
            _convertedInput[3] = OutputBinary(num);

            tbInput.Text = _convertedInput[_modeSelected];
        }
        private void NumpadAddDecimal(int number)
        {
            string currentInput = tbInput.Text;
            string newInput = currentInput + number.ToString();

            if (newInput.Length > 11)
                newInput = newInput[1..];
            if (newInput.StartsWith(','))
                newInput = newInput[1..];

            int pureInt = int.Parse(newInput.Replace(",", ""));
            if (_operation == -1)
                _num1 = pureInt;
            else
                _num2 = pureInt;

            if (newInput.StartsWith("0"))
            {
                _inputStart = false;
                newInput = "0";
            }

            SetConvertedInput(pureInt);
            tbInput.Text = OutputDecimal(pureInt);
        }
        private void NumpadAddHexadecimal(string input)
        {
            string currentInput = tbInput.Text;
            string newInput = currentInput + input;

            if (_operation == -1)
            {
                _num1 = Converter.HexadecimalToDecimal(newInput);
                SetConvertedInput(_num1);
            }
            else
            {
                _num2 = Converter.HexadecimalToDecimal(newInput);
                SetConvertedInput(_num2);
            }
        }
        private void NumpadAddBinary(string input)
        {
            string currentInput = tbInput.Text;
            string newInput = currentInput + input;

            if (_operation == -1)
            {
                _num1 = Converter.BinaryToDecimal(newInput);
                SetConvertedInput(_num1);
            }
            else
            {
                _num2 = Converter.BinaryToDecimal(newInput);
                SetConvertedInput(_num2);
            }
        }
        private void NumpadAddOctal(string input)
        {
            string currentInput = tbInput.Text;
            string newInput = currentInput + input;

            if (_operation == -1)
            {
                _num1 = Converter.OctalToDecimal(newInput);
                SetConvertedInput(_num1);
            }
            else
            {
                _num2 = Converter.OctalToDecimal(newInput);
                SetConvertedInput(_num2);
            }
        }

        private void Calculate(int num, int operation) 
        {
            switch (operation)
            {
                case 0:
                    _num1 += num;
                    break;
                case 1:
                    _num1 -= num;
                    break;
                case 2:
                    _num1 *= num;
                    break;
                case 3:
                    _num1 /= num;
                    break;
                case 4:
                    _num1 %= num;
                    break;
            }
        }
        private void ClearCalculator()
        {
            _num1 = 0;
            _num2 = 0;
            _operation = -1;
            _num2Input = false;
            _inputStart = false;
            _history = new string[4];
            _convertedInput = new string[4];
        }

        private void SetHistory()
        {
            if (_operation == -1 || _num2Input)
            {
                _history[0] += _convertedInput[0];
                _history[1] += _convertedInput[1];
                _history[2] += _convertedInput[2];
                _history[3] += _convertedInput[3];
            }
            else
            {
                _history[0] += _convertedInput[0];
                _history[1] += _convertedInput[1];
                _history[2] += _convertedInput[2];
                _history[3] += _convertedInput[3];
            }

            for (int i = 0; i < 4; i++)
            {
                switch (_operation)
                {
                    case 0:
                        _history[i] += " + ";
                        break;
                    case 1:
                        _history[i] += " - ";
                        break;
                    case 2:
                        _history[i] += " x ";
                        break;
                    case 3:
                        _history[i] += " / ";
                        break;
                    case 4:
                        _history[i] += " % ";
                        break;
                    case 5:
                        _history[i] += " = ";
                        break;
                }
            }

            tbHistory.Text = _history[_modeSelected];
        }
        private void SetOperation(int operation)
        {
            if (!_inputStart)
            {
                return;
            }
            if (operation != -1)
            {
                Calculate(_num2, _operation);
                _operation = operation;
                SetHistory();
                SetConvertedInput(_num1);
                OutputUpdateAll(_num1);
            }
            else
            {
                SetHistory();
            }
            _inputStart = false;

            _num2Input = true;
        }

        #region OutputUpdate
        private void CheckInputLength()
        {
            if (tbInput.Text.Length > 17)
            {
                tbInput.FontSize = 30;

                if (tbInput.Text.Length > 40)
                {
                    tbInput.Text = tbInput.Text.Insert(40, "\n");
                }
            }
            else
            {
                tbInput.FontSize = 40;
            }
        }
        private void OutputUpdateAll(int result)
        {
            if (result == 0)
            {
                tbHEX.Text = "0";
                tbDEC.Text = "0";
                tbOCT.Text = "0";
                tbBIN.Text = "0";
                return;
            }
            if (_operation == 5)
            {
                SetConvertedInput(_num1);
            }
            tbHEX.Text = _convertedInput[0];
            tbDEC.Text = _convertedInput[1];
            tbOCT.Text = _convertedInput[2];
            tbBIN.Text = _convertedInput[3];

            CheckInputLength();
        }
        private string OutputDecimal(int num)
        {
            string output = num.ToString();

            int limit = num < 0 ? 1 : 0;
            for (int i = output.Length - 3; i > limit; i -= 3)
            {
                output = output.Insert(i, ",");
            }

            return output;
        }
        private string OutputBinary(int num)
        {
            string output = string.Empty;
            int[] result = Converter.DecimalToBinary(num);
            bool firstOneFound = false;

            for (int i = 0; i < result.Length; ++i)
            {
                if (result[i] == 1 || firstOneFound)
                {
                    output += result[i];
                    firstOneFound = true;
                }
            }

            while (output.Length % 4 != 0)
            {
                output = "0" + output;
            }
            for (int i = output.Length - 4; i > 0; i -= 4)
            {
                output = output.Insert(i, " ");
            }

            if (output.Length > 29)
            {
                tbBIN.FontSize = 13;
                tbBIN.Height = 36;

                if (output.Length > 39)
                {
                    output = output.Insert(40, "\n");
                }
            }
            else
            {
                tbBIN.FontSize = 18;
                tbBIN.Height = 26;
            }
            return output;
        }
        private string OutputOctal(int num)
        {
            string output = Converter.DecimalToOctal(num);

            for (int i = output.Length - 3; i > 0; i -= 3)
            {
                output = output.Insert(i, " ");
            }
            return output;
        }
        private string OutputHexadecimal(int num)
        {
            string output = Converter.DecimalToHexadecimal(num);

            for (int i = output.Length - 4; i > 0; i -= 4)
            {
                output = output.Insert(i, " ");
            }
            return output;
        } 
        #endregion

        #region ModeSelection
        private void SetModeSelected(int mode)
        {
            for (int i = 0; i < _modeStatus.Length; i++)
            {
                _modeStatus[i] = false;
                _modeIndicator[i].Fill = Brushes.Transparent;
            }
            _modeStatus[mode] = true;
            _modeIndicator[mode].Fill = _selectedModeColorBrush;

            switch (mode)
            {
                case 0:
                    _modeSelected = 0;
                    SetHexMode();
                    break;
                case 1:
                    _modeSelected = 1;
                    SetDecMode();
                    break;
                case 2:
                    _modeSelected = 2;
                    SetOctMode();
                    break;
                case 3:
                    _modeSelected = 3;
                    SetBinMode();
                    break;
            }
            tbInput.Text = _convertedInput[mode];
            CheckInputLength();

            tbHistory.Text = _history[mode];
        }

        private void SetHexMode()
        {
            for (int i = 0; i < _btnNumpad.Length; i++)
            {
                _btnNumpad[i].IsEnabled = true;
            }
            for (int i = 0; i < _btnHexpad.Length; i++)
            {
                _btnHexpad[i].IsEnabled = true;
            }
        }
        private void SetDecMode()
        {
            for (int i = 0; i < _btnNumpad.Length; i++)
            {
                _btnNumpad[i].IsEnabled = true;
            }
            for (int i = 0; i < _btnHexpad.Length; i++)
            {
                _btnHexpad[i].IsEnabled = false;
            }
        }
        private void SetOctMode()
        {
            for (int i = 0; i < _btnNumpad.Length; i++)
            {
                _btnNumpad[i].IsEnabled = true;
            }

            _btnNumpad[8].IsEnabled = false;
            _btnNumpad[9].IsEnabled = false;

            for (int i = 0; i < _btnHexpad.Length; i++)
            {
                _btnHexpad[i].IsEnabled = false;
            }
        }
        private void SetBinMode()
        {
            for (int i = 0; i < _btnNumpad.Length; i++)
            {
                _btnNumpad[i].IsEnabled = false;
            }

            for (int i = 0; i < _btnHexpad.Length; i++)
            {
                _btnHexpad[i].IsEnabled = false;
            }

            _btnNumpad[0].IsEnabled = true;
            _btnNumpad[1].IsEnabled = true;
        }

        private void btnHEX_Click(object sender, RoutedEventArgs e)
        {
            SetModeSelected((int)Modes.Hexadecimal);
        }
        private void btnDEC_Click(object sender, RoutedEventArgs e)
        {
            SetModeSelected((int)Modes.Decimal);
        }
        private void btnOCT_Click(object sender, RoutedEventArgs e)
        {
            SetModeSelected((int)Modes.Octal);
        }
        private void btnBIN_Click(object sender, RoutedEventArgs e)
        {
            SetModeSelected((int)Modes.Binary);
        }
        #endregion

        #region btnNumpad
        private void btnNum0_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(0);
        }
        private void btnNum1_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(1);
        }
        private void btnNum2_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(2);
        }
        private void btnNum3_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(3);
        }
        private void btnNum4_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(4);
        }
        private void btnNum5_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(5);
        }
        private void btnNum6_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(6);
        }
        private void btnNum7_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(7);
        }
        private void btnNum8_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(8);
        }
        private void btnNum9_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(9);
        }
        #endregion

        #region Operations
        private void btnEqual_Click(object sender, RoutedEventArgs e)
        {
            if (_operation == -1 || !_inputStart)
            {
                return;
            }
            Calculate(_num2, _operation);

            _operation = 5;
            tbInput.Text = _num1.ToString();

            SetHistory();
            OutputUpdateAll(_num1);

            _history = new string[4];

            _operation = -1;

            SetConvertedInput(_num1);
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SetOperation(0);
        }
        private void btnSubtract_Click(object sender, RoutedEventArgs e)
        {
            SetOperation(1);
        }
        private void btnMultiply_Click(object sender, RoutedEventArgs e)
        {
            SetOperation(2);
        }
        private void btnDivide_Click(object sender, RoutedEventArgs e)
        {
            SetOperation(3);
        }
        private void btnModulus_Click(object sender, RoutedEventArgs e)
        {
            SetOperation(4);
        }
        #endregion

        #region HexInput
        private void btnHexA_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(0, "A");
        }
        private void btnHexB_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(0, "B");
        }
        private void btnHexC_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(0, "C");
        }
        private void btnHexD_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(0, "D");
        }
        private void btnHexE_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(0, "E");
        }
        private void btnHexF_Click(object sender, RoutedEventArgs e)
        {
            NumpadAdd(0, "F");
        }
        #endregion

        private static class Converter
        {
            public static int[] Binary2sComplement(int decimalNumber)
            {
                int[] bits = new int[64];
                int carry = 1;
                decimalNumber *= -1;

                for (int i = 0; i < 64; i++)
                {
                    bits[i] = decimalNumber % 2;
                    decimalNumber /= 2;
                }
                for (int i = 0; i < 64; i++)
                {
                    if (bits[i] == 0)
                    {
                        bits[i] = 1;
                    }
                    else
                    {
                        bits[i] = 0;
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    if (bits[i] == 1 && carry >= 1)
                    {
                        bits[i] = 0;
                    }
                    else if (bits[i] == 0 && carry >= 1)
                    {
                        bits[i] = 1;
                        --carry;
                    }
                }
                Array.Reverse(bits);
                return bits;
            }
            public static int[] DecimalToBinary(int decimalNumber)
            {
                int[] bits = new int[64];

                if (decimalNumber >= 0)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        bits[i] = decimalNumber % 2;
                        decimalNumber /= 2;
                    }

                    Array.Reverse(bits);
                    return bits;
                }
                return Binary2sComplement(decimalNumber);
            }
            public static int HexadecimalToDecimal(string hexadecimal)
            {
                string hexadecimalNoSpace = hexadecimal.Replace(" ", "");
                if (hexadecimalNoSpace.Contains('-'))
                {
                    hexadecimalNoSpace = hexadecimalNoSpace.Replace("-", "");
                    return Convert.ToInt32(hexadecimalNoSpace, 16) * -1;
                }
                return Convert.ToInt32(hexadecimalNoSpace, 16);
            }
            public static int BinaryToDecimal(string binary)
            {
                string binaryNoSpace = binary.Replace(" ", "");
                binaryNoSpace = binaryNoSpace.Replace("\n", "");

                if (binaryNoSpace.Contains('-'))
                {
                    binaryNoSpace = binaryNoSpace.Replace("-", "");
                    return Convert.ToInt32(binaryNoSpace, 2) * -1;
                }
                return (int)Convert.ToInt64(binaryNoSpace, 2);
            }
            public static int OctalToDecimal(string octal)
            {
                string octalNoSpace = octal.Replace(" ", "");
                if (octalNoSpace.Contains('-'))
                {
                    octalNoSpace = octalNoSpace.Replace("-", "");
                    return Convert.ToInt32(octalNoSpace, 8) * -1;
                }
                return Convert.ToInt32(octalNoSpace, 8);
            }
            public static string DecimalToOctal(int decimalNumber)
            {
                string octalString = Convert.ToString(Math.Abs(decimalNumber), 8).ToUpper();
                if (decimalNumber < 0)
                    octalString = "-" + octalString;

                return octalString;
            }
            public static string DecimalToHexadecimal(int decimalNumber)
            {
                string hexString = Convert.ToString(Math.Abs(decimalNumber), 16).ToUpper();
                if (decimalNumber < 0)
                    hexString = "-" + hexString;
                return hexString;
            }
        }

        private void ClearInputs()
        {
            if (!_inputStart)
            {
                return;
            }

            _inputStart = false;
            if (!_num2Input)
            {
                _num1 = 0;
            }
            else
            {
                _num2 = 0;
            }
            OutputUpdateAll(0);
            SetConvertedInput(0);
        }

        private void btnClearEntry_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();

            ClearCalculator();
            SetHistory();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (tbInput.Text.Length == 0)
            {
                return;
            }
            bool isNegative = false;
            if (_convertedInput[1].StartsWith("-"))
            {
                isNegative = true;
            }

            for (int i = 0; i < 4; ++i)
            {
                _convertedInput[i] = _convertedInput[i].Replace(",", "");
                _convertedInput[i] = _convertedInput[i].Replace(" ", "");
                _convertedInput[i] = _convertedInput[i].Replace("-", "");
            }

            string newInput = _convertedInput[_modeSelected];
            newInput = newInput.Remove(newInput.Length - 1);

            int newNum = 1;
            if (newInput.Length == 0)
            {
                newInput = "0";
                _inputStart = false;
            }
            if (isNegative)
            {
                newNum = -1;
            }
            switch (_modeSelected)
            {
                case 0:
                    newNum *= Converter.HexadecimalToDecimal(newInput);
                    break;
                case 1:
                    newNum *= int.Parse(newInput);
                    break;
                case 2:
                    newNum *= Converter.OctalToDecimal(newInput);
                    break;
                case 3:
                    newNum *= Converter.BinaryToDecimal(newInput);
                    break;
            }

            _num1 = newNum;
            SetConvertedInput(newNum);
            OutputUpdateAll(newNum);
        }
        private void btnSign_Click(object sender, RoutedEventArgs e)
        {
            if (!_inputStart)
            {
                return;
            }

            if (_operation == -1)
            {
                _num1 *= -1;
                SetConvertedInput(_num1);
                OutputUpdateAll(_num1);
                
            }
            else
            {
                _num2 *= -1;
                SetConvertedInput(_num2);
                OutputUpdateAll(_num2);
            }
        }
    }
}