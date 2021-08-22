using System;

namespace WinCopies.Console
{
    public class ProgressBar : ControlElement
    {
        private uint _size;
        private uint _min;
        private uint _max;
        private uint _value;

        public uint Size { get => _size; set => UpdateProperty(ref _size, value >= 2 ? value : throw new ArgumentOutOfRangeException(nameof(value), "The size must be greater or equal to 2.")); }

        public uint Minimum { get => _min; set => UpdateProperty(ref _min, Between(value, _value, _max) ? value : throw GetArgumentOutOfRangeException()); }

        public uint Value { get => _value; set => UpdateProperty(ref _value, Between(_min, value, _max) ? value : throw GetArgumentOutOfRangeException()); }

        public uint Maximum { get => _max; set => UpdateProperty(ref _max, Between(_min, _value, value) ? value : throw GetArgumentOutOfRangeException()); }

        private static bool Between(in uint x, in uint value, in uint y) => value.Between(x, y, true, true);

        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException() => new
#if !CS9
            ArgumentOutOfRangeException
#endif
            ("value", "The minimum value must be less than or equal to the current progress value and the maximum value.");

        protected override string RenderOverride2()
        {
            uint progress;
            float _progress;

            if (_max == 0)
            {
                progress = 0;
                _progress = 0;
            }

            else
            {
                _progress = (float)Value / (float)Maximum;

                progress = _progress == 0u ? 0u : (uint)((float)Size * _progress);
            }

            return $"[{new string((char)9608, (int)progress)}{new string(' ', (int)(Size - progress))}] {System.Math.Round(((double)_progress) * 100, 2)} %";
        }
    }
}
