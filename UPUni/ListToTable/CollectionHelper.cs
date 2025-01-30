using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPUni.Attributes;
using static UPUni.ListToTable.CollectionHelper;

namespace UPUni.ListToTable
{
    /// <summary>
    /// Convert object type list in data table. See <see cref="Attributes.TableAttribute"/>.
    /// </summary>
    public class CollectionHelper
    {
        private static List<AttributeData> AttributeDatas { get; set; }

        /// <summary>
        /// Add attribute data
        /// </summary>
        /// <param name="attributeDatas">Attribute data <see cref="AttributeData"/></param>
        public static void AddAttributeData(AttributeData attributeDatas)
        {
            if(AttributeDatas == null)
            {
                AttributeDatas = new List<AttributeData>();
            }
            AttributeDatas.Add(attributeDatas);
        }

        /// <summary>
        /// Convert object list in data table.
        /// </summary>
        /// <typeparam name="T">Type of object list.</typeparam>
        /// <param name="list">List to convetion.</param>
        /// <returns>Data table converted.</returns>
        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.Attributes.Count == 0)
                    {
                        row[prop.Name] = prop.GetValue(item);
                    }
                    else
                    {
                        TableAttribute tableAttribute = getTableAttribute(prop);
                        if (tableAttribute.Visible)
                        {
                            object val = prop.GetValue(item);
                            if (AttributeDatas != null)
                            {
                                foreach (var itemData in AttributeDatas)
                                {
                                    if (prop.Name.Equals(itemData.Key))
                                    {
                                        foreach (var itemItens in itemData.Values)
                                        {
                                            if (itemItens.OriginalValue.Equals(val))
                                            {
                                                val = itemItens.NewValue;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                if (!tableAttribute.Format.Equals(string.Empty))
                                {
                                    val = string.Format(tableAttribute.Format, val);
                                }
                            }

                            row[tableAttribute.Text] = val;
                        }
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.Attributes.Count == 0)
                {
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
                else
                {
                    TableAttribute tableAttribute = getTableAttribute(prop);
                    if (tableAttribute.Visible)
                    {
                        Type type = prop.PropertyType;
                        if (AttributeDatas != null)
                        {
                            foreach (var itemData in AttributeDatas)
                            {
                                if (prop.Name.Equals(itemData.Key))
                                {
                                    type = itemData.Type;
                                    break;
                                }
                            }
                        }

                        if (!tableAttribute.Format.Equals(string.Empty))
                        {
                            type = typeof(string);
                        }

                        table.Columns.Add(tableAttribute.Text, type);
                    }
                }

            }
            return table;
        }

        private static TableAttribute getTableAttribute(PropertyDescriptor prop)
        {
            TableAttribute tableAttribute = null;
            foreach (var item in prop.Attributes)
            {
                if (item.GetType() == typeof(TableAttribute))
                {
                    return (TableAttribute)item;
                }
            }
            return tableAttribute;
        }

        /// <summary>
        /// Class attribute data
        /// </summary>
        public class AttributeData
        {
            /// <summary>
            /// Key attribute
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// Type attribute <see cref="Type"/>
            /// </summary>
            public Type Type { get; set; }
            /// <summary>
            /// List values attributes data itens <see cref="AttributeDataItens"/>
            /// </summary>
            public List<AttributeDataItens> Values { get; set; }

            /// <summary>
            /// Create new attribute data
            /// </summary>
            /// <param name="key">Key attribute</param>
            /// <param name="type">Type attribute <see cref="Type"/></param>
            /// <param name="value">List values attributes data itens <see cref="AttributeDataItens"/></param>
            public AttributeData(string key, Type type, List<AttributeDataItens> value)
            {
                this.Key = key;
                this.Type = type;
                this.Values = value;
            }
        }

        /// <summary>
        /// Class attributes data itens
        /// </summary>
        public class AttributeDataItens
        {
            /// <summary>
            /// Value original
            /// </summary>
            public object OriginalValue { get; set; }
            /// <summary>
            /// New value
            /// </summary>
            public object NewValue { get; set; }

            /// <summary>
            /// Create new object attribute data itens
            /// </summary>
            /// <param name="originalValue">Value original</param>
            /// <param name="newValue">New value</param>
            public AttributeDataItens(object originalValue, object newValue)
            {
                this.OriginalValue = originalValue;
                this.NewValue = newValue;
            }
        }
    }
}
