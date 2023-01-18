using System.Collections.Generic;
using System;

namespace VelvieR
{
    //usage var i = new VListener<SomeType>();
    // i.value += () => { Console.WriteLine("changed!"); };
    public class VListener<T>
    {
        private T _value;

        public Action value;

        public T Value
        {
            get => _value;

            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged();
                }
            }
        }

        protected virtual void OnValueChanged() => value?.Invoke();
    }
}