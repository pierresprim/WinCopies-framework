using WinCopies.Data.SQL;
using WinCopies.Reflection.DotNetParser;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        public void UpdateEnums()
        {
#if CS8
            static
#endif
                string getUnderlyingTypeCSName(in DotNetEnum @enum) => @enum.UnderlyingType.ToCSName();

            using
#if !CS8
                (
#endif
                var utColl = new DBEntityCollection<UnderlyingType>(Connection)
#if CS8
                ;
#else
                )
            {
#endif
            UnderlyingType getUnderlyingType(DotNetEnum @enum) => new
#if !CS9
                UnderlyingType
#endif
                (utColl)
            { Name = getUnderlyingTypeCSName(@enum) };

            UpdateItems("Enum", false, @enum => true, null, null, item => item.Type, GetAllEnumsInPackages, (@enum, type, enums) => new
#if !CS9
                Enum
#endif
                (enums)
            {
                UnderlyingType = getUnderlyingType(@enum),

                Type = type
            },
            (DotNetEnum type, Enum @enum, DBEntityCollection<Enum> enums) => @enum.UnderlyingType = getUnderlyingType(type), CreateEnumFile, null);
#if !CS8
            }
#endif
        }
    }
}
