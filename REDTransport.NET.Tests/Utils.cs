using System;
using System.Collections;

namespace REDTransport.NET.Tests
{
    public static class Utils
    {
        public static bool CompareObjects(object expectInput, object actualInput)
        {
            var expectInputType = expectInput.GetType();
            var actualInputType = actualInput.GetType();
            
            // If T is primitive type.
            if (expectInputType.IsPrimitive)
            {
                if (expectInput.Equals(actualInput))
                {
                    return true;
                }

                return false;
            }

            if (typeof(IEquatable<>).IsAssignableFrom(expectInputType))
            {
                if (expectInput.Equals(actualInput))
                {
                    return true;
                }

                return false;
            }

            if (typeof(IComparable).IsAssignableFrom(expectInputType))
            {
                if (((IComparable) expectInput).CompareTo(actualInput) == 0)
                {
                    return true;
                }

                return false;
            }

            // If T is implement IEnumerable.
            if (expectInput is IEnumerable)
            {
                if (!(actualInput is IEnumerable))
                {
                    return false;
                }
                
                var expectEnumerator = ((IEnumerable) expectInput).GetEnumerator();
                var actualEnumerator = ((IEnumerable) actualInput).GetEnumerator();

                var canGetExpectMember = expectEnumerator.MoveNext();
                var canGetActualMember = actualEnumerator.MoveNext();

                while (canGetExpectMember && canGetActualMember && true)
                {
                    var currentType = expectEnumerator.Current.GetType();
                    object isEqual = typeof(Utils).GetMethod("CompareObjects").MakeGenericMethod(currentType)
                        .Invoke(null, new object[] {expectEnumerator.Current, actualEnumerator.Current});

                    if ((bool) isEqual == false)
                    {
                        return false;
                    }

                    canGetExpectMember = expectEnumerator.MoveNext();
                    canGetActualMember = actualEnumerator.MoveNext();
                }

                if (canGetExpectMember != canGetActualMember)
                {
                    return false;
                }

                return true;
            }

            // If T is class.
            var properties = expectInputType.GetProperties();
            foreach (var property in properties)
            {
                var expectValueProperty = expectInputType.GetProperty(property.Name);
                var actualValueProperty = actualInputType.GetProperty(property.Name);

                if (actualValueProperty == null)
                {
                    return false;
                }

                var expectValue = expectValueProperty.GetValue(expectInput);
                var actualValue = actualValueProperty.GetValue(actualInput);
                
                if (expectValue == null || actualValue == null)
                {
                    if (expectValue == null && actualValue == null)
                    {
                        continue;
                    }

                    return false;
                }

                object isEqual = typeof(Utils).GetMethod(nameof(CompareObjects)).MakeGenericMethod(property.PropertyType)
                    .Invoke(null, new object[] {expectValue, actualValue});

                if ((bool) isEqual == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}