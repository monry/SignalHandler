using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SignalHandler
{
    [UsedImplicitly]
    public abstract class SignalBase<TSignal> : ISignal, IEquatable<SignalBase<TSignal>>
        where TSignal : SignalBase<TSignal>, new()
    {
        [UsedImplicitly]
        public static TSignal Create()
        {
            return new TSignal();
        }

        bool ISignalTerminator.IsTerminator => false;

        private string Name { get; } = typeof(TSignal).FullName;

        bool IEquatable<SignalBase<TSignal>>.Equals(SignalBase<TSignal> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name;
        }

        bool IEquatable<ISignal>.Equals(ISignal other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && ((IEquatable<SignalBase<TSignal>>) this).Equals((SignalBase<TSignal>) obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public static bool operator ==(SignalBase<TSignal> left, SignalBase<TSignal> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SignalBase<TSignal> left, SignalBase<TSignal> right)
        {
            return !Equals(left, right);
        }
    }

    [UsedImplicitly]
    public abstract class SignalBase<TSignal, TParameter> : SignalBase<TSignal>, ISignal<TParameter>, IEquatable<SignalBase<TSignal, TParameter>>
        where TSignal : SignalBase<TSignal, TParameter>, new()
    {
        private TParameter parameter;

        protected SignalBase()
        {
            parameter = default;
        }

        protected SignalBase(TParameter parameter)
        {
            this.parameter = parameter;
        }

        [UsedImplicitly]
        public static TSignal Create(TParameter parameter)
        {
            return new TSignal
            {
                parameter = parameter
            };
        }

        private string Name { get; } = $"{typeof(TSignal).FullName}:{typeof(TParameter).FullName}";

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public TParameter Parameter => parameter;

        bool IEquatable<SignalBase<TSignal, TParameter>>.Equals(SignalBase<TSignal, TParameter> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return base.Equals(other) && Name == other.Name && EqualityComparer<TParameter>.Default.Equals(Parameter, other.Parameter);
        }

        bool IEquatable<ISignal<TParameter>>.Equals(ISignal<TParameter> other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && ((IEquatable<SignalBase<TSignal, TParameter>>) this).Equals((SignalBase<TSignal, TParameter>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ EqualityComparer<TParameter>.Default.GetHashCode(Parameter);
                return hashCode;
            }
        }

        public static bool operator ==(SignalBase<TSignal, TParameter> left, SignalBase<TSignal, TParameter> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SignalBase<TSignal, TParameter> left, SignalBase<TSignal, TParameter> right)
        {
            return !Equals(left, right);
        }
    }
}