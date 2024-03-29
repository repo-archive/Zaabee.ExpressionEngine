﻿namespace Zaabee.ExpressionEngine
{
    internal sealed class OperationBuilder
    {
        public static T SymbolBuild<T>(string code) where T : Operation, new()
        {
            var reader = BuildingContext.Current.ExpressionReader;

            var codeLength = code.Length;

            var c = reader.Peek(codeLength);

            if (c == code)
            {
                var nextPeek = reader.PeekAt(codeLength);

                if (nextPeek != -1)
                {
                    var next = (char) nextPeek;

                    if (!(next == ' ' || next == ',' || next == ';'
                          || next == '(' || next == ')' || next == '[' || next == ']'
                          || char.IsDigit(next) || char.IsLetter(next)))
                    {
                        return null;
                    }
                }

                reader.Read(codeLength);
                return new T();
            }

            return null;
        }

        public static T LetterBuild<T>(string code) where T : Operation, new()
        {
            var reader = BuildingContext.Current.ExpressionReader;

            var codeLength = code.Length;

            if (reader.Peek(codeLength) == code && !reader.IsLetterOrDigit(codeLength))
            {
                reader.Read(codeLength);
                return new T();
            }

            return null;
        }

        public static T FuncBuild<T>(string code) where T : Operation, new()
        {
            var reader = BuildingContext.Current.ExpressionReader;

            code += "(";

            var codeLength = code.Length;

            if (reader.Peek(codeLength) == code)
            {
                reader.Read(codeLength);
                return new T();
            }

            return null;
        }

        public static T FixedBuild<T>(string code) where T : Operation, new()
        {
            var reader = BuildingContext.Current.ExpressionReader;

            var codeLength = code.Length;

            if (reader.Peek(codeLength) == code)
            {
                reader.Read(codeLength);
                return new T();
            }

            return null;
        }
    }
}