using WinCopies.Reflection.DotNetDocBuilder.SampleDll.NamespaceLevelTypes;
using WinCopies.Reflection.DotNetDocBuilder.SampleDll.NestedTypes;

namespace WinCopies.Reflection.DotNetDocBuilder.SampleDll
{
#if COMPLETE
    namespace NamespaceLevelTypes_TEMP
    {
        /// <summary>
        /// Interface <see cref="IA"/>
        /// </summary>
        public interface IA
        {

        }

        /// <summary>
        /// Interface <see cref="IB{T}"/>
        /// </summary>
        /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
        public interface IB<T> : IA
        {

        }

        /// <summary>
        /// Interface <see cref="IG"/>
        /// </summary>
        public interface IG : ParentA.IF<int>
        {

        }

        /// <summary>
        /// Interface <see cref="IM{TT, TU}"/>
        /// </summary>
        /// <typeparam name="TT">Generic Type Parameter <typeparamref name="TT"/></typeparam>
        /// <typeparam name="TU">Generic Type Parameter <typeparamref name="TU"/></typeparam>
        public interface IM<TT, TU> : IA
        {

        }

        /// <summary>
        /// Interface <see cref="IN"/>
        /// </summary>
        public interface IN : ParentB.ID
        {

        }

        /// <summary>
        /// Class <see cref="CA"/>
        /// </summary>
        public abstract class CA : IA
        {

        }

        /// <summary>
        /// Class <see cref="CB{T}"/>
        /// </summary>
        /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
        public sealed class CB<T> : CA, IB<T>
        {

        }

        /// <summary>
        /// Enum <see cref="EA"/>
        /// </summary>
        public enum EA
        {

        }

        /// <summary>
        /// Struct <see cref="SA"/>
        /// </summary>
        public struct SA : IG
        {

        }

        /// <summary>
        /// Struct <see cref="SB{T}"/>
        /// </summary>
        public struct SB<T> : IB<T>
        {

        }

        public interface IO
        {

        }

        public interface IP : IO
        {

        }

        public class CC : IO
        {

        }

#if COMPLETE
        public interface IO_TEMP
        {

        }

        public interface IP_TEMP : IO_TEMP, IO
        {

        }

        public class CC_TEMP : IO_TEMP, IO
        {

        }
#endif
    }

    namespace NestedTypes_TEMP
    {
        /// <summary>
        /// Parent Class <see cref="ParentA"/>
        /// </summary>
        public static class ParentA
        {
            /// <summary>
            /// Interface <see cref="IC"/>
            /// </summary>
            public interface IC
            {

            }

            /// <summary>
            /// Interface <see cref="IF{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IF<in T> : ParentB.II<T>
            {

            }

            /// <summary>
            /// Interface <see cref="IK{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IK<T> : ParentB.II<T>
            {

            }

            /// <summary>
            /// Interface <see cref="IL{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IL<T> : ParentB.IJ<T>
            {

            }
        }

        /// <summary>
        /// Parent Class <see cref="ParentB"/>
        /// </summary>
        public class ParentB
        {
            /// <summary>
            /// Interface <see cref="ID"/>
            /// </summary>
            public interface ID : ParentA.IC
            {

            }

            /// <summary>
            /// Interface <see cref="IE{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IE<out T> : IA
            {

            }

            /// <summary>
            /// Interface <see cref="IH"/>
            /// </summary>
            public interface IH : IG
            {

            }

            /// <summary>
            /// Interface <see cref="II{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface II<in T> : IA
            {

            }

            /// <summary>
            /// Interface <see cref="IJ{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IJ<T> : IA
            {

            }

            /// <summary>
            /// Enum <see cref="EB"/>
            /// </summary>
            public enum EB
            {

            }
        }

        /// <summary>
        /// Parent Class <see cref="ParentC"/>
        /// </summary>
        public static class ParentC
        {
            /// <summary>
            /// Struct <see cref="SC"/>
            /// </summary>
            public struct SC : ParentB.IH
            {

            }

            /// <summary>
            /// Class <see cref="CA"/>
            /// </summary>
            public class CA : NamespaceLevelTypes2.CA
            {

            }

            /// <summary>
            /// Class <see cref="CC"/>
            /// </summary>
            public class CC : CA
            {

            }

            /// <summary>
            /// Class <see cref="CD"/>
            /// </summary>
            public class CD : NamespaceLevelTypes2.CB
            {

            }
        }

        /// <summary>
        /// Parent Class <see cref="ParentD{T}"/>
        /// </summary>
        public static class ParentD<T>
        {
            /// <summary>
            /// Parent Class <see cref="ParentE{U}"/>
            /// </summary>
            public static class ParentE<U>
            {
                /// <summary>
                /// Class <see cref="CA"/>
                /// </summary>
                public static class CA
                {

                }

                /// <summary>
                /// Class <see cref="CB"/>
                /// </summary>
                public static class CB
                {

                }
            }

            /// <summary>
            /// Parent Class <see cref="ParentF{U, V}"/>
            /// </summary>
            public static class ParentF<U, V>
            {

            }
        }
    }

    namespace NamespaceLevelTypes2_TEMP
    {
        /// <summary>
        /// Class <see cref="CA"/>
        /// </summary>
        public class CA
        {

        }

        /// <summary>
        /// Class <see cref="CB"/>
        /// </summary>
        public class CB : CA
        {

        }

        /// <summary>
        /// Class <see cref="CE"/>
        /// </summary>
        public class CE : ParentC.CD
        {

        }

        /// <summary>
        /// Class <see cref="CF"/>
        /// </summary>
        public class CF : CE
        {

        }

        /// <summary>
        /// Class <see cref="CG"/>
        /// </summary>
        public class CG : CF
        {

        }
    }
#endif

    namespace NamespaceLevelTypes
    {
        /// <summary>
        /// Interface <see cref="IA"/>
        /// </summary>
        public interface IA
        {

        }

        /// <summary>
        /// Interface <see cref="IB{T}"/>
        /// </summary>
        /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
        public interface IB<T> : IA
        {

        }

        /// <summary>
        /// Interface <see cref="IG"/>
        /// </summary>
        public interface IG : ParentA.IF<int>
        {

        }

        /// <summary>
        /// Interface <see cref="IM{TT, TU}"/>
        /// </summary>
        /// <typeparam name="TT">Generic Type Parameter <typeparamref name="TT"/></typeparam>
        /// <typeparam name="TU">Generic Type Parameter <typeparamref name="TU"/></typeparam>
        public interface IM<TT, TU> : IA
        {

        }

        /// <summary>
        /// Interface <see cref="IN"/>
        /// </summary>
        public interface IN : ParentB.ID
        {

        }

        /// <summary>
        /// Class <see cref="CA"/>
        /// </summary>
        public abstract class CA : IA
        {

        }

        /// <summary>
        /// Class <see cref="CB{T}"/>
        /// </summary>
        /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
        public sealed class CB<T> : CA, IB<T>
        {

        }

        /// <summary>
        /// Enum <see cref="EA"/>
        /// </summary>
        public enum EA
        {

        }

        /// <summary>
        /// Struct <see cref="SA"/>
        /// </summary>
        public struct SA : IG
        {

        }

        /// <summary>
        /// Struct <see cref="SB{T}"/>
        /// </summary>
        public struct SB<T> : IB<T>
        {

        }

        public interface IO : ParentD<int>.IA
        {

        }

        public interface IP : ParentD<int>.ParentE<uint>.IA
        {

        }

        public interface IQ : IO, IP
        {

        }

        public struct SC : IO
        {

        }

        public struct SD : IP
        {

        }

        public struct SE : IQ
        {

        }

#if COMPLETE
        /// <summary>
        /// Interface <see cref="IA"/>
        /// </summary>
        public interface IA_TEMP
        {

        }

        /// <summary>
        /// Interface <see cref="IB{T}"/>
        /// </summary>
        /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
        public interface IB_TEMP<T> : IA
        {

        }

        /// <summary>
        /// Interface <see cref="IG"/>
        /// </summary>
        public interface IG_TEMP : ParentA.IF<int>
        {

        }

        /// <summary>
        /// Interface <see cref="IM{TT, TU}"/>
        /// </summary>
        /// <typeparam name="TT">Generic Type Parameter <typeparamref name="TT"/></typeparam>
        /// <typeparam name="TU">Generic Type Parameter <typeparamref name="TU"/></typeparam>
        public interface IMInterface_TEMP<TT, TU> : IA
        {

        }

        /// <summary>
        /// Interface <see cref="IN"/>
        /// </summary>
        public interface IN_TEMP : ParentB.ID
        {

        }

        /// <summary>
        /// Class <see cref="CA"/>
        /// </summary>
        public abstract class CA_TEMP : IA
        {

        }

        /// <summary>
        /// Class <see cref="CB{T}"/>
        /// </summary>
        /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
        public sealed class CB_TEMP<T> : CA, IB<T>
        {

        }

        /// <summary>
        /// Enum <see cref="EA"/>
        /// </summary>
        public enum EA_TEMP
        {

        }

        /// <summary>
        /// Struct <see cref="SA"/>
        /// </summary>
        public struct SA_TEMP : IG
        {

        }

        /// <summary>
        /// Struct <see cref="SB{T}"/>
        /// </summary>
        public struct SB_TEMP<T> : IB<T>
        {

        }

        public interface IO_TEMP : ParentD<int>.IA
        {

        }

        public interface IP_TEMP : ParentD<int>.ParentE<uint>.IA
        {

        }

        public interface IQ_TEMP : IO, IP
        {

        }

        public struct SC_TEMP : IO
        {

        }

        public struct SD_TEMP : IP
        {

        }

        public struct SE_TEMP : IQ
        {

        }
#endif
    }

    namespace NestedTypes
    {
        /// <summary>
        /// Parent Class <see cref="ParentA"/>
        /// </summary>
        public static class ParentA
        {
            /// <summary>
            /// Interface <see cref="IC"/>
            /// </summary>
            public interface IC
            {

            }

            /// <summary>
            /// Interface <see cref="IF{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IF<in T> : ParentB.II<T>
            {

            }

            /// <summary>
            /// Interface <see cref="IK{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IK<T> : ParentB.II<T>
            {

            }

            /// <summary>
            /// Interface <see cref="IL{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IL<T> : ParentB.IJ<T>
            {

            }

            public struct SA : IC, IA
            {

            }
#if COMPLETE
            /// <summary>
            /// Interface <see cref="IC"/>
            /// </summary>
            public interface IC_TEMP
            {

            }

            /// <summary>
            /// Interface <see cref="IF{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IF_TEMP<in T> : ParentB.II<T>
            {

            }

            /// <summary>
            /// Interface <see cref="IK{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IK_TEMP<T> : ParentB.II<T>
            {

            }

            /// <summary>
            /// Interface <see cref="IL{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IL_TEMP<T> : ParentB.IJ<T>
            {

            }

            public struct SA_TEMP : IC, IA
            {

            }
#endif
        }

        /// <summary>
        /// Parent Class <see cref="ParentB"/>
        /// </summary>
        public class ParentB
        {
            /// <summary>
            /// Interface <see cref="ID"/>
            /// </summary>
            public interface ID : ParentA.IC
            {

            }

            /// <summary>
            /// Interface <see cref="IE{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IE<out T> : IA
            {

            }

            /// <summary>
            /// Interface <see cref="IH"/>
            /// </summary>
            public interface IH : IG
            {

            }

            /// <summary>
            /// Interface <see cref="II{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface II<in T> : IA
            {

            }

            /// <summary>
            /// Interface <see cref="IJ{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IJ<T> : IA
            {

            }

            /// <summary>
            /// Enum <see cref="EB"/>
            /// </summary>
            public enum EB
            {

            }

#if COMPLETE
            /// <summary>
            /// Interface <see cref="ID"/>
            /// </summary>
            public interface ID_TEMP : ParentA.IC
            {

            }

            /// <summary>
            /// Interface <see cref="IE{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IE_TEMP<out T> : IA
            {

            }

            /// <summary>
            /// Interface <see cref="IH"/>
            /// </summary>
            public interface IH_TEMP : IG
            {

            }

            /// <summary>
            /// Interface <see cref="II{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface II_TEMP<in T> : IA
            {

            }

            /// <summary>
            /// Interface <see cref="IJ{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IJ_TEMP<T> : IA
            {

            }

            /// <summary>
            /// Enum <see cref="EB"/>
            /// </summary>
            public enum EB_TEMP
            {

            }
#endif
        }

        /// <summary>
        /// Parent Class <see cref="ParentC"/>
        /// </summary>
        public static class ParentC
        {
            /// <summary>
            /// Struct <see cref="SC"/>
            /// </summary>
            public struct SC : ParentB.IH
            {

            }

            /// <summary>
            /// Class <see cref="CA"/>
            /// </summary>
            public class CA : NamespaceLevelTypes2.CA
            {

            }

            /// <summary>
            /// Class <see cref="CC"/>
            /// </summary>
            public class CC : CA
            {

            }

            /// <summary>
            /// Class <see cref="CD"/>
            /// </summary>
            public class CD : NamespaceLevelTypes2.CB
            {

            }

#if COMPLETE
            /// <summary>
            /// Struct <see cref="SC"/>
            /// </summary>
            public struct SC_TEMP : ParentB.IH
            {

            }

            /// <summary>
            /// Class <see cref="CA"/>
            /// </summary>
            public class CA_TEMP : NamespaceLevelTypes2.CA
            {

            }

            /// <summary>
            /// Class <see cref="CC"/>
            /// </summary>
            public class CC_TEMP : CA
            {

            }

            /// <summary>
            /// Class <see cref="CD"/>
            /// </summary>
            public class CD_TEMP : NamespaceLevelTypes2.CB
            {

            }
#endif
        }

        /// <summary>
        /// Parent Class <see cref="ParentD{T}"/>
        /// </summary>
        public static class ParentD<T>
        {
            /// <summary>
            /// Parent Class <see cref="ParentE{U}"/>
            /// </summary>
            public static class ParentE<U>
            {
                /// <summary>
                /// Class <see cref="CA"/>
                /// </summary>
                public static class CA
                {

                }

                /// <summary>
                /// Class <see cref="CB"/>
                /// </summary>
                public static class CB
                {

                }

                public struct SA : ParentA.IC, NamespaceLevelTypes.IA
                {

                }

                public interface IA : ParentA.IC, NamespaceLevelTypes.IA
                {

                }

                public interface IB
                {

                }

                public struct SB : IA, IB
                {

                }
#if COMPLETE
                /// <summary>
                /// Class <see cref="CA"/>
                /// </summary>
                public static class CA_TEMP
                {

                }

                /// <summary>
                /// Class <see cref="CB"/>
                /// </summary>
                public static class CB_TEMP
                {

                }

                public struct SA_TEMP : ParentA.IC, NamespaceLevelTypes.IA
                {

                }

                public interface IA_TEMP : ParentA.IC, NamespaceLevelTypes.IA
                {

                }

                public interface IB_TEMP
                {

                }

                public struct SB_TEMP : IA, IB
                {

                }
#endif
            }

            /// <summary>
            /// Parent Class <see cref="ParentF{U, V}"/>
            /// </summary>
            public static class ParentF<U, V>
            {
                /// <summary>
                /// Class <see cref="CA"/>
                /// </summary>
                public static class CA
                {

                }

                /// <summary>
                /// Class <see cref="CB"/>
                /// </summary>
                public static class CB
                {

                }

#if COMPLETE
                /// <summary>
                /// Class <see cref="CA"/>
                /// </summary>
                public static class CA_TEMP
                {

                }

                /// <summary>
                /// Class <see cref="CB"/>
                /// </summary>
                public static class CB_TEMP
                {

                }
#endif
            }

            public struct SA : ParentE<T>.IA
            {

            }

            public struct SB : ParentE<T>.IB
            {

            }

            public interface IA : ParentE<T>.IA
            {

            }

#if COMPLETE
            /// <summary>
            /// Parent Class <see cref="ParentE{U}"/>
            /// </summary>
            public static class ParentE_TEMP<U>
            {
                /// <summary>
                /// Class <see cref="CA"/>
                /// </summary>
                public static class CA
                {

                }

                /// <summary>
                /// Class <see cref="CB"/>
                /// </summary>
                public static class CB
                {

                }
            }

            /// <summary>
            /// Parent Class <see cref="ParentF{U, V}"/>
            /// </summary>
            public static class ParentF_TEMP<U, V>
            {
                /// <summary>
                /// Class <see cref="CA"/>
                /// </summary>
                public static class CA
                {

                }

                /// <summary>
                /// Class <see cref="CB"/>
                /// </summary>
                public static class CB
                {

                }

#if COMPLETE
                /// <summary>
                /// Class <see cref="CA"/>
                /// </summary>
                public static class CA_TEMP
                {

                }

                /// <summary>
                /// Class <see cref="CB"/>
                /// </summary>
                public static class CB_TEMP
                {

                }
#endif
            }

            public struct SA_TEMP : ParentE<T>.IA
            {

            }

            public struct SB_TEMP : ParentE<T>.IB
            {

            }

            public struct SA_TEMP2 : ParentE<T>.IA_TEMP
            {

            }

            public struct SB_TEMP2 : ParentE<T>.IB_TEMP
            {

            }

            public interface IA_TEMP : ParentE<T>.IA
            {

            }

            public interface IA_TEMP2 : ParentE<T>.IA_TEMP
            {

            }
#endif
        }

#if COMPLETE
        /// <summary>
        /// Parent Class <see cref="ParentA"/>
        /// </summary>
        public static class ParentA_TEMP
        {
            /// <summary>
            /// Interface <see cref="IC"/>
            /// </summary>
            public interface IC
            {

            }

            /// <summary>
            /// Interface <see cref="IF{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IF<in T> : ParentB.II<T>
            {

            }

            /// <summary>
            /// Interface <see cref="IK{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IK<T> : ParentB.II<T>
            {

            }

            /// <summary>
            /// Interface <see cref="IL{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IL<T> : ParentB.IJ<T>
            {

            }
        }

        /// <summary>
        /// Parent Class <see cref="ParentB"/>
        /// </summary>
        public class ParentB_TEMP
        {
            /// <summary>
            /// Interface <see cref="ID"/>
            /// </summary>
            public interface ID : ParentA.IC
            {

            }

            /// <summary>
            /// Interface <see cref="IE{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IE<out T> : IA
            {

            }

            /// <summary>
            /// Interface <see cref="IH"/>
            /// </summary>
            public interface IH : IG
            {

            }

            /// <summary>
            /// Interface <see cref="II{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface II<in T> : IA
            {

            }

            /// <summary>
            /// Interface <see cref="IJ{T}"/>
            /// </summary>
            /// <typeparam name="T">Generic Type Parameter <typeparamref name="T"/></typeparam>
            public interface IJ<T> : IA
            {

            }

            /// <summary>
            /// Enum <see cref="EB"/>
            /// </summary>
            public enum EB
            {

            }
        }

        /// <summary>
        /// Parent Class <see cref="ParentC"/>
        /// </summary>
        public static class ParentC_TEMP
        {
            /// <summary>
            /// Struct <see cref="SC"/>
            /// </summary>
            public struct SC : ParentB.IH
            {

            }

            /// <summary>
            /// Class <see cref="CA"/>
            /// </summary>
            public class CA : NamespaceLevelTypes2.CA
            {

            }

            /// <summary>
            /// Class <see cref="CC"/>
            /// </summary>
            public class CC : CA
            {

            }

            /// <summary>
            /// Class <see cref="CD"/>
            /// </summary>
            public class CD : NamespaceLevelTypes2.CB
            {

            }
        }

        /// <summary>
        /// Parent Class <see cref="ParentD{T}"/>
        /// </summary>
        public static class ParentD_TEMP<T>
        {
            /// <summary>
            /// Parent Class <see cref="ParentE{U}"/>
            /// </summary>
            public static class ParentE<U>
            {
                /// <summary>
                /// Class <see cref="CA"/>
                /// </summary>
                public static class CA
                {

                }

                /// <summary>
                /// Class <see cref="CB"/>
                /// </summary>
                public static class CB
                {

                }
            }

            /// <summary>
            /// Parent Class <see cref="ParentF{U, V}"/>
            /// </summary>
            public static class ParentF<U, V>
            {

            }
        }
#endif
    }

    namespace NamespaceLevelTypes2
    {
        /// <summary>
        /// Class <see cref="CA"/>
        /// </summary>
        public class CA
        {

        }

        /// <summary>
        /// Class <see cref="CB"/>
        /// </summary>
        public class CB : CA
        {

        }

        /// <summary>
        /// Class <see cref="CE"/>
        /// </summary>
        public class CE : ParentC.CD
        {

        }

        /// <summary>
        /// Class <see cref="CF"/>
        /// </summary>
        public class CF : CE
        {

        }

        /// <summary>
        /// Class <see cref="CG"/>
        /// </summary>
        public class CG : CF
        {

        }

#if COMPLETE
        /// <summary>
        /// Class <see cref="CA"/>
        /// </summary>
        public class CA_TEMP
        {

        }

        /// <summary>
        /// Class <see cref="CB"/>
        /// </summary>
        public class CB_TEMP : CA
        {

        }

        /// <summary>
        /// Class <see cref="CE"/>
        /// </summary>
        public class CE_TEMP : ParentC.CD
        {

        }

        /// <summary>
        /// Class <see cref="CF"/>
        /// </summary>
        public class CF_TEMP : CE
        {

        }

        /// <summary>
        /// Class <see cref="CG"/>
        /// </summary>
        public class CG_TEMP : CF
        {

        }
#endif
    }
}
