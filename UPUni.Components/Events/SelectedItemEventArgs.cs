using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UPUni.Components.CustomList.ListView;

namespace UPUni.Components.Events
{
    /// <summary>
    /// Event args selected item. See <see cref="CustomList.ListView"/>.
    /// </summary>
    public class SelectedItemEventArgs : EventArgs
    {
        /// <summary>
        /// Index item selected.
        /// </summary>
        public int Index { get; private set; }
        /// <summary>
        /// Get state select or deselect.
        /// </summary>
        public bool isSelected { get; private set; }
        /// <summary>
        /// Item selected.
        /// </summary>
        public ItemControl Item { get; private set; }

        /// <summary>
        /// Create new selection args.
        /// </summary>
        /// <param name="index">Index item.</param>
        /// <param name="isSelected">Select state.</param>
        /// <param name="item">Object item.</param>
        public SelectedItemEventArgs(int index, bool isSelected, ItemControl item)
        {
            this.Index = index;
            this.Item = item;
            this.isSelected = isSelected;
        }
    }
}
