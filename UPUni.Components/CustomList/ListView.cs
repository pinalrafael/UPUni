using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using UPUni.Components.Events;

namespace UPUni.Components.CustomList
{
    /// <summary>
    /// This component developed to create custom list view itens using user control ou windows forms controls.
    /// Delevoped by: Rafael Pinal.
    /// </summary>
    public class ListView : FlowLayoutPanel
    {
        #region Public Propertys
        /// <summary>
        /// Get and set count draw itens in line Extra extra large.
        /// </summary>
        [Category("Custom List View")]
        public int CountItensLine_xxl { get; set; }
        /// <summary>
        /// Get and set count draw itens in line Extra large.
        /// </summary>
        [Category("Custom List View")]
        public int CountItensLine_xl { get; set; }
        /// <summary>
        /// Get and set count draw itens in line Large.
        /// </summary>
        [Category("Custom List View")]
        public int CountItensLine_lg { get; set; }
        /// <summary>
        /// Get and set count draw itens in line Medium.
        /// </summary>
        [Category("Custom List View")]
        public int CountItensLine_md { get; set; }
        /// <summary>
        /// Get and set count draw itens in line Small.
        /// </summary>
        [Category("Custom List View")]
        public int CountItensLine_sm { get; set; }
        /// <summary>
        /// Get and set count draw itens in line Extra small.
        /// </summary>
        [Category("Custom List View")]
        public int CountItensLine_xs { get; set; }
        /// <summary>
        /// Get and set max width Extra extra large.
        /// </summary>
        [Category("Custom List View")]
        public int MaxWidth_xxl { get; set; }
        /// <summary>
        /// Get and set max width Extra large.
        /// </summary>
        [Category("Custom List View")]
        public int MaxWidth_xl { get; set; }
        /// <summary>
        /// Get and set max width Large.
        /// </summary>
        [Category("Custom List View")]
        public int MaxWidth_lg { get; set; }
        /// <summary>
        /// Get and set max width Medium width.
        /// </summary>
        [Category("Custom List View")]
        public int MaxWidth_md { get; set; }
        /// <summary>
        /// Get and set max width Small width.
        /// </summary>
        [Category("Custom List View")]
        public int MaxWidth_sm { get; set; }
        /// <summary>
        /// Get and set draw effects item mouse enter.
        /// </summary>
        [Category("Custom List View")]
        public bool DrawMouseEnter { get; set; }
        /// <summary>
        /// Get and set if itens is multiple select.
        /// </summary>
        [Category("Custom List View")]
        public bool MultipleSelect { get; set; }
        /// <summary>
        /// Get and set draw efftcts item selection.
        /// </summary>
        [Category("Custom List View")]
        public bool DrawSelection { get; set; }
        /// <summary>
        /// Get and set if draw itens is responsive from width.
        /// </summary>
        [Category("Custom List View")]
        public bool Responsive { get; set; }
        /// <summary>
        /// Get and set color effect with mouse enter.
        /// </summary>
        [Category("Custom List View")]
        public Color MouseEnterColor { get; set; }
        /// <summary>
        /// Get and set color effect with selection.
        /// </summary>
        [Category("Custom List View")]
        public Color SelectColor { get; set; }
        /// <summary>
        /// Get and set the mouse enter background image.
        /// </summary>
        [Category("Custom List View")]
        public Image MouseEnterBackgroundImage { get; set; }
        /// <summary>
        /// Get and set the select background image.
        /// </summary>
        [Category("Custom List View")]
        public Image SelectBackgroundImage { get; set; }
        /// <summary>
        /// Get and set the mouse enter layout background image.
        /// </summary>
        [Category("Custom List View")]
        public ImageLayout MouseEnterImageLayout { get; set; }
        /// <summary>
        /// Get and set the select layout background image.
        /// </summary>
        [Category("Custom List View")]
        public ImageLayout SelectImageLayout { get; set; }
        #endregion

        #region Private Propertys
        private int CountItensLine { get; set; }
        #endregion

        #region Attributes
        /// <summary>
        /// Get the list intens.
        /// Do not use list functions Add, Remove...
        /// </summary>
        public List<ItemControl> Itens { get; private set; }
        /// <summary>
        /// Get selected itens.
        /// </summary>
        public List<ItemControl> SelectedItens {
            get {
                return this.GetSelectedItens();
            }
        }
        /// <summary>
        /// Get selected indexs
        /// </summary>
        public List<int> SelectedIndexes
        {
            get
            {
                return this.GetSelectedIndexs();
            }
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Event selected item handler delegate
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event args <see cref="SelectedItemEventArgs"/></param>
        public delegate void SelectedItemEventHandler(object sender, SelectedItemEventArgs e);
        #endregion

        #region Events
        /// <summary>
        /// Event call when item select <see cref="SelectedItemEventHandler"/>.
        /// </summary>
        public event SelectedItemEventHandler SelectedItem;
        #endregion

        #region Constrcts
        /// <summary>
        /// Create new custom list view.
        /// </summary>
        public ListView()
        {
            this.Resize += ListView_Resize;
            this.Itens = new List<ItemControl>();
            this.CountItensLine = 0;
            this.CountItensLine_xxl = 1;
            this.CountItensLine_xl = 1;
            this.CountItensLine_lg = 1;
            this.CountItensLine_md = 1;
            this.CountItensLine_sm = 1;
            this.CountItensLine_xs = 1;
            this.MaxWidth_xxl = 1400;
            this.MaxWidth_xl = 1200;
            this.MaxWidth_lg = 992;
            this.MaxWidth_md = 768;
            this.MaxWidth_sm = 576;
            this.DrawMouseEnter = true;
            this.DrawSelection = true;
            this.MultipleSelect = true;
            this.AutoScroll = true;
            this.Responsive = true;
            this.MouseEnterColor = Color.Gray;
            this.SelectColor = Color.Blue;
            this.MouseEnterImageLayout = ImageLayout.Tile;
            this.SelectImageLayout = ImageLayout.Tile;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Add new item user control or windows forms controls in custom list view.
        /// </summary>
        /// <param name="control">User control or windows forms controls <see cref="Control"/>.</param>
        /// <param name="auxiliary">Object auxiliary.</param>
        /// <param name="isSelectable">Define if item is selectable.</param>
        public void AddItem(Control control, object auxiliary, bool isSelectable)
        {
            this.UpdateCountItensLine();

            ItemControl item = this.CreateItemControl(control);
            item.IsSelectable = isSelectable;
            item.Auxiliary = auxiliary;
            this.Controls.Add(item.Control);
            this.Itens.Add(item);
            this.SizeItem(item);
        }

        /// <summary>
        /// Add new item user control or windows forms controls in custom list view.
        /// </summary>
        /// <param name="control">User control or windows forms controls <see cref="Control"/>.</param>
        /// <param name="isSelectable">Define if item is selectable.</param>
        public void AddItem(Control control, bool isSelectable)
        {
            this.AddItem(control, null, isSelectable);
        }

        /// <summary>
        /// Add new item user control or windows forms controls in custom list view.
        /// </summary>
        /// <param name="control">User control or windows forms controls <see cref="Control"/>.</param>
        /// <param name="auxiliary">Object auxiliary.</param>
        public void AddItem(Control control, object auxiliary)
        {
            this.AddItem(control, auxiliary, true);
        }

        /// <summary>
        /// Add new item user control or windows forms controls in custom list view.
        /// </summary>
        /// <param name="control">User control or windows forms controls <see cref="Control"/>.</param>
        public void AddItem(Control control)
        {
            this.AddItem(control, null, true);
        }

        /// <summary>
        /// Add new item user control or windows forms controls in custom list view at index.
        /// </summary>
        /// <param name="control">User control or windows forms controls.</param>
        /// <param name="index">Define if item is selectable.</param>
        /// <param name="auxiliary">Object auxiliary.</param>
        /// <param name="isSelectable">Index to add item <see cref="Control"/>.</param>
        public void AddItemAt(Control control, int index, object auxiliary, bool isSelectable)
        {
            this.AddItem(control, auxiliary, isSelectable);

            this.MoveItemAt(this.Itens.Count - 1, index);
        }

        /// <summary>
        /// Add new item user control or windows forms controls in custom list view at index.
        /// </summary>
        /// <param name="control">User control or windows forms controls <see cref="Control"/>.</param>
        /// <param name="index">Define if item is selectable.</param>
        /// <param name="isSelectable">Index to add item.</param>
        public void AddItemAt(Control control, int index, bool isSelectable)
        {
            this.AddItemAt(control, index, null, isSelectable);
        }

        /// <summary>
        /// Add new item user control or windows forms controls in custom list view at index.
        /// </summary>
        /// <param name="control">User control or windows forms controls <see cref="Control"/>.</param>
        /// <param name="index">Define if item is selectable.</param>
        /// <param name="auxiliary">Object auxiliary.</param>
        public void AddItemAt(Control control, int index, object auxiliary)
        {
            this.AddItemAt(control, index, auxiliary, true);
        }

        /// <summary>
        /// Add new item user control or windows forms controls in custom list view at index.
        /// </summary>
        /// <param name="control">User control or windows forms controls <see cref="Control"/>.</param>
        /// <param name="index">Define if item is selectable.</param>
        public void AddItemAt(Control control, int index)
        {
            this.AddItemAt(control, index, null, true);
        }

        /// <summary>
        /// Remove item at index in custom list view.
        /// </summary>
        /// <param name="index">Index to removing.</param>
        public void RemoveItemAt(int index)
        {
            this.Controls.RemoveAt(index);
            this.Itens.RemoveAt(index);
            this.CleanSelections();
        }

        /// <summary>
        /// Remove item at <see cref="ItemControl"/> in custom list view.
        /// </summary>
        /// <param name="itemControl">Item to removing.</param>
        public void RemoveItem(ItemControl itemControl)
        {
            int index = this.GetIndex(itemControl.Guid);
            this.RemoveItemAt(index);
        }

        /// <summary>
        /// Update item index in custom list view.
        /// </summary>
        /// <param name="index">Index to update <see cref="Control"/>.</param>
        /// <param name="control">User control or windows forms controls to update.</param>
        public void UpdateItemAt(int index, Control control)
        {
            this.Itens[index] = this.CreateItemControl(control);
            this.SizeItem(this.Itens[index]);
            this.RefreshItens();
        }

        /// <summary>
        /// Update item at <see cref="ItemControl"/> in custom list view.
        /// </summary>
        /// <param name="itemControl">Item to update <see cref="ItemControl"/>.</param>
        /// <param name="control">User control or windows forms controls to update <see cref="Control"/>.</param>
        public void UpdateItem(ItemControl itemControl, Control control)
        {
            int index = this.GetIndex(itemControl.Guid);
            this.UpdateItemAt(index, control);
        }

        /// <summary>
        /// Move item index in custom list view.
        /// </summary>
        /// <param name="indexFrom">Index from move.</param>
        /// <param name="indexTo">Index to move.</param>
        public void MoveItemAt(int indexFrom, int indexTo)
        {
            this.MoveItem(indexFrom, indexTo);
            this.RefreshItens();
        }

        /// <summary>
        /// Move item at <see cref="ItemControl"/> in custom list view.
        /// </summary>
        /// <param name="itemControl">Item from move <see cref="ItemControl"/>.</param>
        /// <param name="indexTo">Index to move.</param>
        public void MoveIntem(ItemControl itemControl, int indexTo)
        {
            int index = this.GetIndex(itemControl.Guid);
            this.MoveItemAt(index, indexTo);
        }

        /// <summary>
        /// Select or unselect item in custom list view.
        /// </summary>
        /// <param name="index">Index item to select or unselect.</param>
        /// <param name="selected">State item select.</param>
        public void SelectItemAt(int index, bool selected)
        {
            ItemControl item = this.Itens[index];
            if (item.IsSelectable)
            {
                item.Selected = selected;
                if (item.Selected)
                {
                    item.Control.BackColor = this.SelectColor;
                    if (this.SelectBackgroundImage != null)
                    {
                        item.Control.BackgroundImage = this.SelectBackgroundImage;
                        item.Control.BackgroundImageLayout = this.SelectImageLayout;
                    }
                }
                else
                {
                    item.Control.BackColor = item.BackColorItem;
                    if (item.BackgroundImageItem != null)
                    {
                        item.Control.BackgroundImage = item.BackgroundImageItem;
                        item.Control.BackgroundImageLayout = item.ItemImageLayout;
                    }
                }
            }
        }

        /// <summary>
        /// Select or unselect item at <see cref="ItemControl"/> in custom list view.
        /// </summary>
        /// <param name="itemControl">Item item to select or unselect <see cref="ItemControl"/>.</param>
        /// <param name="selected">State item select.</param>
        public void SelectItem(ItemControl itemControl, bool selected)
        {
            int index = this.GetIndex(itemControl.Guid);
            this.SelectItemAt(index, selected);
        }

        /// <summary>
        /// Clean all itens selected.
        /// </summary>
        public void CleanSelections()
        {
            foreach (var item in this.Itens)
            {
                item.Selected = false;
                item.Control.BackColor = item.BackColorItem;
                if (item.BackgroundImageItem != null)
                {
                    item.Control.BackgroundImage = item.BackgroundImageItem;
                    item.Control.BackgroundImageLayout = item.ItemImageLayout;
                }
                else
                {
                    item.Control.BackgroundImage = null;
                }
            }
        }

        /// <summary>
        /// Refresh all itens.
        /// </summary>
        public void RefreshItens()
        {
            this.Controls.Clear();
            foreach (var item in this.Itens)
            {
                this.Controls.Add(item.Control);
            }
        }

        /// <summary>
        /// Clear all itens and controls.
        /// </summary>
        public void ItensClear()
        {
            this.Controls.Clear();
            this.Itens.Clear();
        }

        /// <summary>
        /// Invoke function in user control or windows forms controls at index.
        /// </summary>
        /// <param name="index">Index to invoke.</param>
        /// <param name="functionName">Function name.</param>
        /// <param name="functionParms">Paramters sent to function.</param>
        /// <returns>Object return function.</returns>
        public object InvokeFunctionAt(int index, string functionName, object[] functionParms = null)
        {
            ItemControl item = this.Itens[index];
            return item.Control.GetType().GetMethod(functionName).Invoke(item.Control, functionParms);
        }

        /// <summary>
        /// Invoke function in user control or windows forms controls at <see cref="ItemControl"/>.
        /// </summary>
        /// <param name="itemControl">Item to invoke <see cref="ItemControl"/>.</param>
        /// <param name="functionName">Function name.</param>
        /// <param name="functionParms">Paramters sent to function.</param>
        /// <returns>Object return function.</returns>
        public object InvokeFunction(ItemControl itemControl, string functionName, object[] functionParms = null)
        {
            int index = this.GetIndex(itemControl.Guid);
            return this.InvokeFunctionAt(index, functionName, functionParms);
        }

        /// <summary>
        /// Invoke property in user control or windows forms controls at index.
        /// </summary>
        /// <param name="index">Index to invoke.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyValue">Value sent to property.</param>
        /// <returns>Object value property.</returns>
        public object InvokePropertyAt(int index, string propertyName, object propertyValue = null)
        {
            ItemControl item = this.Itens[index];
            if(propertyValue != null)
            {
                item.Control.GetType().GetProperty(propertyName).SetValue(item.Control, propertyValue);
            }
            return item.Control.GetType().GetProperty(propertyName).GetValue(item.Control);
        }

        /// <summary>
        /// Invoke property in user control or windows forms controls at <see cref="ItemControl"/>.
        /// </summary>
        /// <param name="itemControl">Item to invoke <see cref="ItemControl"/>.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyValue">Value sent to property.</param>
        /// <returns>Object value property.</returns>
        public object InvokeProperty(ItemControl itemControl, string propertyName, object propertyValue = null)
        {
            int index = this.GetIndex(itemControl.Guid);
            return this.InvokePropertyAt(index, propertyName, propertyValue);
        }
        #endregion

        #region Private Functions
        private ItemControl CreateItemControl(Control control)
        {
            ItemControl item = new ItemControl(control);
            this.SetEventsControls(item, item.Control);
            item.Control.Click += (sender, e) =>
            {
                this.SendClick(item);
            };
            item.Control.MouseEnter += (sender, e) =>
            {
                this.SendEnter(item, sender);
            };
            item.Control.MouseLeave += (sender, e) =>
            {
                this.SendLeave(item, sender);
            };
            item.Control.Resize += (sender, e) =>
            {
                this.UpdateControl(item, sender);
            };
            return item;
        }

        private void SetEventsControls(ItemControl item , Control control)
        {
            foreach (Control item1 in control.Controls)
            {
                item1.Click += (sender1, e1) =>
                {
                    this.SendClick(item);
                };
                item1.MouseEnter += (sender1, e1) =>
                {
                    this.SendEnter(item, item.Control);
                };
                item1.MouseLeave += (sender1, e1) =>
                {
                    this.SendLeave(item, item.Control);
                };
                this.SetEventsControls(item, item1);
            }
        }

        private void MoveItem(int oldIndex, int newIndex)
        {
            var item = this.Itens[oldIndex];

            this.Itens.RemoveAt(oldIndex);

            //if (newIndex > oldIndex) newIndex--;
            // the actual index could have shifted due to the removal

            this.Itens.Insert(newIndex, item);
        }

        private void SizeItem(ItemControl item)
        {
            int scroll = 0;
            if (this.VScroll)
            {
                scroll = 20;
            }

            if (this.CountItensLine > 0)
            {
                item.Control.Width = ((this.Width - scroll) - (6 * this.CountItensLine)) / this.CountItensLine;
            }
        }

        private int GetIndex(Guid guid)
        {
            int index = 0;
            foreach (var item in this.Itens)
            {
                if (item.Guid.Equals(guid))
                {
                    break;
                }
                index++;
            }
            return index;
        }

        private List<ItemControl> GetSelectedItens()
        {
            List<ItemControl> itemControls = new List<ItemControl>();
            foreach (var item in this.Itens)
            {
                if (item.Selected)
                {
                    itemControls.Add(item);
                }
            }
            return itemControls;
        }

        private List<int> GetSelectedIndexs()
        {
            List<int> itemControls = new List<int>();
            foreach (var item in this.Itens)
            {
                if (item.Selected)
                {
                    itemControls.Add(this.GetIndex(item.Guid));
                }
            }
            return itemControls;
        }

        private void SendClick(ItemControl item)
        {
            int index = this.GetIndex(item.Guid);
            if (item.IsSelectable)
            {
                if (this.DrawSelection)
                {
                    if (!this.MultipleSelect)
                    {
                        this.CleanSelections();
                    }
                    this.SelectItemAt(index, !item.Selected);
                }
                this.OnSelectedIndex(new SelectedItemEventArgs(index, item.Selected, item));
            }
        }

        private void SendEnter(ItemControl item, object sender)
        {
            if (item.IsSelectable)
            {
                if (this.DrawMouseEnter)
                {
                    if (!item.Selected)
                    {
                        Control c = (Control)sender;
                        c.Cursor = Cursors.Hand;
                        c.BackColor = this.MouseEnterColor;
                        if (this.MouseEnterBackgroundImage != null)
                        {
                            c.BackgroundImage = this.MouseEnterBackgroundImage;
                            c.BackgroundImageLayout = this.MouseEnterImageLayout;
                        }
                        else
                        {
                            c.BackgroundImage = null;
                        }
                    }
                }
            }
        }

        private void SendLeave(ItemControl item, object sender)
        {
            if (item.IsSelectable)
            {
                if (this.DrawMouseEnter)
                {
                    if (!item.Selected)
                    {
                        Control c = (Control)sender;
                        c.BackColor = item.BackColorItem;

                        if (item.BackgroundImageItem != null)
                        {
                            c.BackgroundImage = item.BackgroundImageItem;
                            c.BackgroundImageLayout = item.ItemImageLayout;
                        }
                        else
                        {
                            c.BackgroundImage = null;
                        }
                    }
                }
            }
        }

        private void UpdateControl(ItemControl item, object sender)
        {
            int index = this.GetIndex(item.Guid);
            Control c = (Control)sender;
            this.Itens[index].UpdateControl(c);

            foreach (var item1 in this.SelectedItens)
            {
                if(item.Guid == item1.Guid)
                {
                    item1.UpdateControl(c);
                    break;
                }
            }
        }

        private void UpdateCountItensLine()
        {
            if (this.Responsive)
            {
                if (this.Width >= this.MaxWidth_xxl)
                {
                    this.CountItensLine = this.CountItensLine_xxl;
                }
                else if (this.Width >= this.MaxWidth_xl)
                {
                    this.CountItensLine = this.CountItensLine_xl;
                }
                else if (this.Width >= this.MaxWidth_lg)
                {
                    this.CountItensLine = this.CountItensLine_lg;
                }
                else if (this.Width >= this.MaxWidth_md)
                {
                    this.CountItensLine = this.CountItensLine_md;
                }
                else if (this.Width >= this.MaxWidth_sm)
                {
                    this.CountItensLine_sm = this.CountItensLine_sm;
                }
                else if (this.Width < this.MaxWidth_sm)
                {
                    this.CountItensLine = this.CountItensLine_xs;
                }
            }
            else
            {
                if (this.CountItensLine_xxl > 0)
                {
                    this.CountItensLine = this.CountItensLine_xxl;
                }
                else if (this.CountItensLine_xl > 0)
                {
                    this.CountItensLine = this.CountItensLine_xl;
                }
                else if (this.CountItensLine_lg > 0)
                {
                    this.CountItensLine = this.CountItensLine_lg;
                }
                else if (this.CountItensLine_md > 0)
                {
                    this.CountItensLine = this.CountItensLine_md;
                }
                else if (this.CountItensLine_sm > 0)
                {
                    this.CountItensLine = this.CountItensLine_sm;
                }
                else if (this.CountItensLine_xs > 0)
                {
                    this.CountItensLine = this.CountItensLine_xs;
                }
            }
        }
        #endregion

        #region Events Functions
        protected virtual void OnSelectedIndex(SelectedItemEventArgs e)
        {
            SelectedItemEventHandler handler = this.SelectedItem;
            handler?.Invoke(this, e);
        }
        #endregion

        #region Events Object
        private void ListView_Resize(object sender, EventArgs e)
        {
            this.UpdateCountItensLine();

            foreach (var item in this.Itens)
            {
                this.SizeItem(item);
            }
        }
        #endregion

        #region Class
        /// <summary>
        /// Class custom object itens list.
        /// </summary>
        public class ItemControl : Control
        {
            /// <summary>
            /// Get and set item selection.
            /// </summary>
            public bool Selected { get; set; }
            /// <summary>
            /// Get and set if item is selectable.
            /// </summary>
            public bool IsSelectable { get; set; }
            /// <summary>
            /// Get and set object item auxiliary.
            /// </summary>
            public object Auxiliary { get; set; }
            /// <summary>
            /// Get ID item.
            /// </summary>
            public Guid Guid { get; private set; }
            /// <summary>
            /// Get the user control or windows forms control item.
            /// </summary>
            public Control Control { get; private set; }
            /// <summary>
            /// Get default back color item.
            /// </summary>
            public Color BackColorItem { get; private set; }
            /// <summary>
            /// Get default background image.
            /// </summary>
            public Image BackgroundImageItem { get; private set; }
            /// <summary>
            /// Get default image layout.
            /// </summary>
            public ImageLayout ItemImageLayout { get; private set; }

            /// <summary>
            /// Create new item control list.
            /// </summary>
            /// <param name="control">User control or windows forms control <see cref="Control"/>.</param>
            public ItemControl(Control control)
            {
                this.Guid = Guid.NewGuid();
                this.Control = control;
                this.Selected = false;
                this.IsSelectable = true;
                this.Auxiliary = null;
                this.BackColorItem = this.Control.BackColor;
                this.BackgroundImageItem = this.Control.BackgroundImage;
                this.ItemImageLayout = this.Control.BackgroundImageLayout;
            }

            /// <summary>
            /// Update object control.
            /// </summary>
            /// <param name="control">Object control to update <see cref="Control"/></param>
            public void UpdateControl(Control control)
            {
                this.Control = control;
            }
        }
        #endregion
    }
}
