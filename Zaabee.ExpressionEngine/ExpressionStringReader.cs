using System;

namespace Zaabee.ExpressionEngine
{
    internal sealed class ExpressionStringReader
    {
        private readonly string _source;
        private int _currentIndex = 0;

        public ExpressionStringReader(string source) => _source = source;

        public int Peek()
        {
            if (_currentIndex >= _source.Length)
                return -1;
            return _source[_currentIndex];
        }

        public int PeekAt(int position)
        {
            var idx = _currentIndex + position;
            if (idx >= _source.Length || idx < 0)
                return -1;
            return _source[idx];
        }

        public string Peek(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException();

            return _currentIndex + count > _source.Length ? null : _source.Substring(_currentIndex, count);
        }

        public int Read()
        {
            if (_currentIndex >= _source.Length)
                return -1;
            return _source[_currentIndex++];
        }

        public string Read(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException();

            if (_currentIndex + count > _source.Length)
                return null;
            var r = _source.Substring(_currentIndex, count);
            _currentIndex += count;
            return r;
        }

        public bool IsLetterOrDigit(int position)
        {
            var idx = _currentIndex + position;

            if (idx >= _source.Length || idx < 0)
                return false;

            var c = _source[idx];

            return char.IsLetter(c) || char.IsDigit(c);
        }
    }
}