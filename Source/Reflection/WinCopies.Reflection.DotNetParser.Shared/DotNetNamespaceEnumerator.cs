using System;
using System.Linq;
using System.Reflection;
using System.Text;

using WinCopies;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;

namespace WinCopies.Reflection.DotNetParser
{
    public class NamespaceEnumerator : Enumerator<Type
#if CS8
            ?
#endif
            , string>
    {
        private IQueue<string> _namespaces;
        private string _current;
        private Func<bool> _moveNextGlobal;
        private PredicateIn<Type> _moveNext;
        private FuncIn<bool, Type, bool> _predicate;

        protected EnumerableHelper<string>.IEnumerableQueue Namespaces { get; private set; } = EnumerableHelper<string>.GetEnumerableQueue();

        public override bool? IsResetSupported => true;

        protected override string CurrentOverride => _current;

        public NamespaceEnumerator(in Assembly assembly, Predicate<Type>
#if CS8
            ?
#endif
            prePredicate, in Predicate<Type>
#if CS8
            ?
#endif
            postPredicate) : base(assembly.GetDefinedTypes())
        {
            _moveNext = prePredicate == null ? MoveNextOverride2 : Temp.Bool.PrependPredicateIn(prePredicate, MoveNextOverride2);

            _predicate = Temp. Bool.PrependPredicateIn(Temp.Bool.PrependPredicateNULL(type => !type.IsNested, postPredicate));

            ResetMoveNext();
        }

        protected virtual void OnTypeValidated(in string
#if CS8
            ?
#endif
            current)
        {
            if (current != null)

                Namespaces.Enqueue(_current = current);
        }

        protected virtual bool MoveNextOverride2(in Type type) => _predicate(!Namespaces.Contains(type.Namespace), type);

        protected override bool MoveNextOverride() => _moveNextGlobal();

        private bool OnInnerMoveNext()
        {
            string current = _namespaces.Dequeue();

            if (_namespaces.Count == 0)
            {
                _namespaces = null;

                ResetMoveNext();
            }

            if (Namespaces.Contains(current))

                return false;

            OnTypeValidated(current);

            return true;
        }

        protected void ResetMoveNext() => _moveNextGlobal = () =>
            {
                Type
#if CS8
            ?
#endif
            current;

                while (InnerEnumerator.MoveNext() && (current = InnerEnumerator.Current) != null)

                    if (_moveNext(current))
                    {
                        if (current.Namespace.Contains('.'))
                        {
                            _namespaces = new Collections.DotNetFix.Generic.Queue<string>();

                            string[] namespaces = current.Namespace.Split('.');

                            StringBuilder sb = new
#if !CS9
                            StringBuilder
#endif
                            ();

                            _ = sb.Append(namespaces[0]);

                            for (int i = 1; i < namespaces.Length; i++)
                            {
                                _ = sb.Append('.');
                                _ = sb.Append(namespaces[i]);

                                _namespaces.Enqueue(sb.ToString());
                            }

                            _moveNextGlobal = () =>
                            {
                                while (_namespaces != null)

                                    if (OnInnerMoveNext())

                                        return true;

                                return _moveNextGlobal();
                            };

                            if (Namespaces.Contains(namespaces[0]))

                                while (!OnInnerMoveNext())
                                {
                                    // Left empty.
                                }

                            else

                                OnTypeValidated(namespaces[0]);
                        }

                        else if (!Namespaces.Contains(current.Namespace))

                            OnTypeValidated(current.Namespace);

                        return true;
                    }

                return false;
            };

        protected override void ResetOverride2()
        {
            base.ResetOverride2();

            ResetMoveNext();
        }

        protected override void ResetCurrent()
        {
            base.ResetCurrent();

            _current = null;

            if (_namespaces != null)
            {
                _namespaces.Clear();
                _namespaces = null;
            }
        }

        protected override void DisposeManaged()
        {
            // Namespaces.Clear();

            // Namespaces is set to null only in DisposeManaged() and this method is called only once. No overriden method of this class is called after DisposeManaged(). Idem for _predicate.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Namespaces = null;

            _predicate = null;

            _moveNextGlobal = null;

            _moveNext = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            base.DisposeManaged();
        }
    }
}
