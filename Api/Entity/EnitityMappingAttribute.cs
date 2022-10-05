using System;

namespace Api.Entity
{
    /// <summary>
    /// 自定义特性 属性或者类可用  支持继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class EnitityMappingAttribute : Attribute
    {
        private string tableName;
        /// <summary>
        /// 实体实际对应的表名
        /// </summary>
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        private string columnName;
        /// <summary>
        /// 中文列名
        /// </summary>
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }
    }
}
