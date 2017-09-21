using System;

namespace TestModels
{
    public class OverloadedClass
    {
        public bool Invert(bool b) => !b;
        public int Invert(int i) => -i;
        private Guid Correlation() => Guid.NewGuid();
        public void Print() => Console.WriteLine("Hello World!");
    }

    public class TestId
    {
        private readonly int _value;

        private TestId(int value)
        {
            if (value < 0)
                throw new ArgumentException($"{value} is not a valid {nameof(TestId)}, it must be greater than zero");
            _value = value;
        }

        public static implicit operator int(TestId c) => c._value;
        public static implicit operator TestId(int i) => new TestId(i);

        public override string ToString() => _value.ToString();

        public static bool operator ==(TestId first, object second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }
            if ((object)first == null || second == null)
            {
                return false;
            }
            return first.Equals(second);
        }

        public static bool operator !=(TestId first, object second)
        {
            return !(first == second);
        }

        private bool Equals(TestId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as TestId;
            if (other != null)
                return Equals(other);
            if (obj is int)
            {
                return _value.Equals(obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }

}
