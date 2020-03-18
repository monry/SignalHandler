using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SignalHandler.Application.Interface;
using UnityEngine;

namespace SignalHandler.Application.Signal
{
    [UsedImplicitly]
    public abstract class ScriptableObjectSignalBase<TSignal> : ScriptableObject, ISignal, IEquatable<ScriptableObjectSignalBase<TSignal>>
        where TSignal : ScriptableObjectSignalBase<TSignal>
    {
        [UsedImplicitly]
        public static TSignal Create()
        {
            return CreateInstance<TSignal>();
        }

        bool ISignalTerminator.IsTerminator => false;

        private string Name { get; } = typeof(TSignal).FullName;

        bool IEquatable<ScriptableObjectSignalBase<TSignal>>.Equals(ScriptableObjectSignalBase<TSignal> other)
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

            return obj.GetType() == GetType() && Equals((ScriptableObjectSignalBase<TSignal>) obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public static bool operator ==(ScriptableObjectSignalBase<TSignal> left, ScriptableObjectSignalBase<TSignal> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScriptableObjectSignalBase<TSignal> left, ScriptableObjectSignalBase<TSignal> right)
        {
            return !Equals(left, right);
        }
    }

    [UsedImplicitly]
    public abstract class ScriptableObjectSignalBase<TSignal, TParameter> : ScriptableObjectSignalBase<TSignal>, ISignal<TParameter>, IEquatable<ScriptableObjectSignalBase<TSignal, TParameter>>
        where TSignal : ScriptableObjectSignalBase<TSignal, TParameter>
    {
        protected ScriptableObjectSignalBase()
        {
            parameter = default;
        }

        protected ScriptableObjectSignalBase(TParameter parameter)
        {
            this.parameter = parameter;
        }

        [UsedImplicitly]
        public static TSignal Create(TParameter parameter)
        {
            var signal = CreateInstance<TSignal>();
            signal.parameter = parameter;
            return signal;
        }

        [SerializeField] private TParameter parameter;

        private string Name { get; } = $"{typeof(TSignal).FullName}:{typeof(TParameter).FullName}";

        public TParameter Parameter => parameter;

        bool IEquatable<ScriptableObjectSignalBase<TSignal, TParameter>>.Equals(ScriptableObjectSignalBase<TSignal, TParameter> other)
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

            return obj.GetType() == GetType() && Equals((ScriptableObjectSignalBase<TSignal, TParameter>) obj);
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

        public static bool operator ==(ScriptableObjectSignalBase<TSignal, TParameter> left, ScriptableObjectSignalBase<TSignal, TParameter> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScriptableObjectSignalBase<TSignal, TParameter> left, ScriptableObjectSignalBase<TSignal, TParameter> right)
        {
            return !Equals(left, right);
        }
    }
}