namespace SignalHandler
{
    public class Signal : SignalBase<Signal>
    {
    }

    public class SignalWithStructParameter : SignalBase<SignalWithStructParameter, SignalWithStructParameter.ComplexParameter>
    {
        public struct ComplexParameter
        {
            public bool BoolValue { get; }
            public int IntValue { get; }

            public ComplexParameter(bool boolValue, int intValue)
            {
                BoolValue = boolValue;
                IntValue = intValue;
            }
        }
    }

    public class SignalWithValueTupleParameter : SignalBase<SignalWithValueTupleParameter, (bool, float)>
    {
    }
}