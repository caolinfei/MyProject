using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace ZSZ.Core.Data
{
    /// <summary>
    /// 属性配置
    /// </summary>
    public class Property
    {
        /// <summary>
        /// 属性PropertyInfo
        /// </summary>
        public PropertyInfo PropertyInfo { get;set; }
        /// <summary>
        /// Column名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 列长度
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// 列数据库类型
        /// </summary>
        public SqlDbType DbType { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; set; }
        /// <summary>
        /// 属性类型码
        /// </summary>
        public TypeCode PropertyTypeCode { get; set; }
        /// <summary>
        /// 数据库备注
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// dicimal长度
        /// </summary>
        public byte Precision { get; set; }
        /// <summary>
        /// decimal精度
        /// </summary>
        public byte Scale { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// 是否键值
        /// </summary>
        public bool IsKey { get; set; }
        /// <summary>
        /// 键生成的配置
        /// </summary>
        public DatabaseGeneratedOption Option { get; set; }
    }
}
