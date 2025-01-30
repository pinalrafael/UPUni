using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static UPUni.ListToTable.CollectionHelper;

namespace UPUni.Attributes
{
    /// <summary>
    /// Attribute configuratrion object list property. See <see cref="ListToTable.CollectionHelper"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Get the column state visible.
        /// </summary>
        public bool Visible { get; private set; }
        /// <summary>
        /// Get custom text column.
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// Get Custom format your data values.
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// Create attribite configuration.
        /// </summary>
        /// <param name="visible">Set state column visible.</param>
        /// <param name="text">Set custom text column.</param>
        public TableAttribute(bool visible, string text)
        {
            this.Visible = visible;
            this.Text = text;
            this.Format = "";
        }

        /// <summary>
        /// Create attribite configuration with format.
        /// </summary>
        /// <param name="visible">Set state column visible.</param>
        /// <param name="text">Set custom text column.</param>
        /// <param name="format">Set custom format of values.</param>
        public TableAttribute(bool visible, string text, string format)
        {
            this.Visible = visible;
            this.Text = text;
            this.Format = format;
        }
    }
}
