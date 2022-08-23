using System;

namespace WinCopies.Console
{
    public class ProgressBar : ControlElement
    {
        private uint _size;
        private double _min;
        private double _max;
        private double _value;

        public const char PROGRESS_CHAR = (char)9608;

        public uint ActualProgressLength { get; private set; }
        public float ActualPercentProgress { get; private set; }

        public uint Size { get => _size; set => UpdateProperty(ref _size, value >= 2 ? value : throw new ArgumentOutOfRangeException(nameof(value), "The size must be greater or equal to 2.")); }

        public double Minimum { get => _min; set => UpdateProperty(ref _min, Between(value, _value, _max) ? value : throw GetArgumentOutOfRangeException()); }

        public double Value
        {
            get => _value;

            set => UtilHelpers.UpdateValue(ref _value, Between(_min, value, _max) ? value : throw GetArgumentOutOfRangeException(), () =>
            {
                int progress = GetProgress2(out float _progress);

                if (progress == ActualProgressLength)

                    return;

                CursorPosition cursorPosition = CursorPosition;
                int actualProgress;

                void _write(in int left, in string text)
                {
                    System.Console.SetCursorPosition(left, cursorPosition.Top);

                    System.Console.Write(text);
                }

                void write( int left, in char c)
                {
                    left = cursorPosition.Left + left;
                    _write(left + 1, new string(c, actualProgress));

                    left = cursorPosition.Left + (int)Size;
                    _write(left + 3, new string(' ', (int)Math.GetLength((ulong)(ActualPercentProgress * 100)) + 5));
                    _write(left + 1, GetEnding(_progress));
                }

                if (progress > ActualProgressLength)
                {
                    actualProgress = progress - (int)ActualProgressLength;

                    write((int)ActualProgressLength, PROGRESS_CHAR);
                }

                else
                {
                    actualProgress = (int)ActualProgressLength - progress;

                    write(progress, ' ');
                }

                ActualProgressLength = (uint)progress;
                ActualPercentProgress = _progress;

                ResetCursorPosition();
            });
        }

        public double Maximum { get => _max; set => UpdateProperty(ref _max, Between(_min, _value, value) ? value : throw GetArgumentOutOfRangeException()); }

        private static bool Between(in double x, in double value, in double y) => value.Between(x, y, true, true);

        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException() => new
#if !CS9
            ArgumentOutOfRangeException
#endif
            ("value", "The minimum value must be less than or equal to the current progress value and the maximum value.");

        private static string GetEnding(float progress) => $"] {(progress = (float)System.Math.Round(progress * 100, 2)).ToString($"F2")} %";

        protected double GetProgress(out float progress)
        {
            if (_max == 0)
            {
                progress = 0;

                return 0;
            }

            else
            {
                double tmp = (double)Value / (double)Maximum;

                double result = tmp == 0u ? 0u : (double)(Size * tmp);

                progress = (float)tmp;

                return result;
            }
        }

        protected int GetProgress2(out float progress) => (int)GetProgress(out progress);

        protected override string RenderOverride2()
        {
            int progress = GetProgress2(out float _progress);

            return $"[{new string(PROGRESS_CHAR, progress)}{new string(' ', (int)Size - progress)}{GetEnding(_progress)}";
        }
    }
}
