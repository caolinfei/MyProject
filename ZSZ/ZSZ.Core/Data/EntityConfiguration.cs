using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Humanizer;

namespace ZSZ.Core.Data
{
   public class EntityConfiguration<TEntity> where TEntity:BaseEntity
   {
       private readonly IDictionary<string, PropertyConfiguration> _properties;

       private readonly IList<PropertyInfo> propertyInfos = typeof(TEntity).GetProperties(BindingFlags.GetProperty|BindingFlags.Instance|BindingFlags.Public|BindingFlags.SetProperty).Where(m=>m.CanRead&&m.CanWrite&& _databaseTypes.Contains(m.PropertyType)).ToList();
       private readonly IList<string> _ignoredFields = new List<string>();
       private string _tableName = typeof(TEntity).Name.Pluralize();

       protected EntityConfiguration()
       {
           _properties = propertyInfos.ToDictionary((s) => { return s.Name;}, s => { return  new PropertyConfiguration(s);});
       }
       private static readonly Type[] _databaseTypes = new[]
       {
           typeof (int), typeof (long), typeof (byte), typeof (bool), typeof (short), typeof (string),typeof(decimal),
           typeof (int?), typeof (long?), typeof (byte?), typeof (bool?), typeof (short?),typeof(decimal?),
           typeof (DateTime),
           typeof (DateTime?)
       };
    }

    public class PropertyConfiguration
    {
        internal PropertyConfiguration(PropertyInfo propertyInfo)
        {
            Property = new Property()
            {
                PropertyInfo = propertyInfo,
                Name = propertyInfo.Name,
                PropertyType = propertyInfo.PropertyType,
                PropertyTypeCode = GetTypeCode(propertyInfo.PropertyType),
                Required = propertyInfo.GetCustomAttribute<RequiredAttribute>() != null,
                IsKey = propertyInfo.GetCustomAttribute<KeyAttribute>() != null,
                Option = DatabaseGeneratedOption.Identity,
                DbType = mapping[propertyInfo.PropertyType],

            };
            if (propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>() != null)
            {
                Property.Option = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>().DatabaseGeneratedOption;
            }
            if (propertyInfo.GetCustomAttribute<MaxLengthAttribute>() != null)
            {
                Property.MaxLength = propertyInfo.GetCustomAttribute<MaxLengthAttribute>().Length;
            }
            if (propertyInfo.GetCustomAttribute<DefaultValueAttribute>() != null)
            {
                Property.DefaultValue = propertyInfo.GetCustomAttribute<DefaultValueAttribute>().Value;
            }
        }
        /// <summary>
        /// 标识该属性非空
        /// 注：大部分字段都应该为非空
        /// </summary>
        /// <param name="defaultValue">非空时的默认值</param>
        /// <returns></returns>
        public PropertyConfiguration Required(object defaultValue = null)
        {
            Property.Required = true;
            Property.DefaultValue = defaultValue;
            return this;
        }
        /// <summary>
        /// 标识该属性的详细描述
        /// </summary>
        /// <param name="comment">描述</param>
        /// <returns></returns>
        public PropertyConfiguration HasComment(string comment)
        {
            Property.Comment = comment;
            return this;
        }
        /// <summary>
        /// 标识该属性的最大长度（在<see cref="String"/>类型时有效）
        /// 注：nvarchar 最大长度为4000，如 过不指定MaxLength或者超过4000，则认为是nvarchar(max)
        /// </summary>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public PropertyConfiguration HasMaxLength(int maxLength)
        {
            Property.MaxLength = maxLength;
            return this;
        }
        /// <summary>
        /// 将该属性类型映射为某个SqlServer数据库类型
        /// 注：调用该方法必须保证被映射的类型和c# 类型是匹配的，该方法也会做必要的判断
        /// 注：只能配置int和decimal类型
        /// 附默认类型映射表：
        /// |   c# 类型       |   SqlServer类型 |
        /// |   int           |     int         |
        /// |   string        |     nvarchar    |
        /// |   DateTime      |     datetime    |
        /// |   byte          |     tinyint     |
        /// |   long          |     bigint      |
        /// |   bool          |     bit         |
        /// |   short         |     smallint    |
        /// |   decimal       |     decimal     |
        /// </summary>
        /// <param name="dbType">SqlServer数据库类型</param>
        /// <returns></returns>
        public PropertyConfiguration MapTo(SqlDbType dbType)
        {
            switch (Property.PropertyTypeCode)
            {
                case TypeCode.Int32:
                    if (new[] { SqlDbType.Int, SqlDbType.TinyInt, SqlDbType.SmallInt }.Contains(dbType))
                    {
                        Property.DbType = dbType;
                    }
                    break;
                case TypeCode.Decimal:
                    if (new[] { SqlDbType.Decimal, SqlDbType.Money, SqlDbType.SmallMoney }.Contains(dbType))
                    {
                        Property.DbType = dbType;
                    }
                    break;
            }
            return this;
        }
        /// <summary>
        /// 如果DbType为decimal，则需要指定其精度
        /// 注：精度请指定为9/19/28/38之一
        /// </summary>
        /// <param name="precision">精度</param>
        /// <param name="scale">小数位数</param>
        /// <returns></returns>
        public PropertyConfiguration HasPrecision(byte precision, byte scale)
        {
            Property.Precision = precision;
            Property.Scale = scale;
            return this;
        }

        internal Property Property { get; private set; }

        private static readonly IDictionary<Type, SqlDbType> mapping = new Dictionary<Type, SqlDbType>(){
            {typeof(int?), SqlDbType.Int},
            {typeof(string), SqlDbType.NVarChar},
            {typeof(DateTime?), SqlDbType.DateTime},
            {typeof(byte?), SqlDbType.TinyInt},
            {typeof(long?), SqlDbType.BigInt},
            {typeof(bool?), SqlDbType.Bit},
            {typeof(short?), SqlDbType.SmallInt},
            {typeof(decimal?), SqlDbType.Decimal},

            {typeof(int), SqlDbType.Int},
            {typeof(DateTime), SqlDbType.DateTime},
            {typeof(byte), SqlDbType.TinyInt},
            {typeof(long), SqlDbType.BigInt},
            {typeof(bool), SqlDbType.Bit},
            {typeof(short), SqlDbType.SmallInt},
            {typeof(decimal), SqlDbType.Decimal},
        };

        private TypeCode GetTypeCode(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Type.GetTypeCode(type.GetGenericArguments()[0]);
            }
            return Type.GetTypeCode(type);
        }
    }
}
