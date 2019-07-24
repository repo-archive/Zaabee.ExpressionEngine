using System;

namespace Zaabee.ExpressionEngine
{
    internal sealed class ExpressionStringReader
    {
        private string source;
        private int _currentIndex;

        public ExpressionStringReader(string source)
        {
            this.source = source;
        }

        public int Peek()
        {
            if (_currentIndex >= source.Length)
            {
                return -1;
            }

            return source[_currentIndex];
        }

        public int PeekAt(int position)
        {
            var idx = _currentIndex + position;

            if (idx >= source.Length || idx < 0)
            {
                return -1;
            }

            return source[idx];
        }

        public string Peek(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException();

            if (_currentIndex + count > source.Length)
            {
                return null;
            }

            return source.Substring(_currentIndex, count);

        }

        public int Read()
        {
            if (_currentIndex >= source.Length)
            {
                return -1;
            }

            return source[_currentIndex++];
        }

        public string Read(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException();

            if (_currentIndex + count > source.Length)
            {
                return null;
            }

            var r = source.Substring(_currentIndex, count);
            _currentIndex += count;
            return r;
        }

        public bool IsLetterOrDigit(int position)
        {
            var idx = _currentIndex + position;

            if (idx >= source.Length || idx < 0)
            {
                return false;
            }

            var c = source[idx];

            return char.IsLetter(c) || char.IsDigit(c);
        }
    }
}