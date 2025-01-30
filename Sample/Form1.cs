using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UPUni;
using UPUni.Components;
using UPUni.Enums;
using UPUni.StringsCombinations;
using UPUni.WifiManager;

namespace Sample
{
    public partial class Form1 : Form
    {
        private byte[] data { get; set; }
        private string Ext { get; set; }
        private ManagerWifi managerWifi { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Text = "Sample - UPUni.dll v." + UPUniMaster.GetVersion().ToString() + " & UPUni.Components.dll v." + UPUniComponentsMaster.GetVersion().ToString() + " & WifiConnection";

                //ListToTable
                List<Table> tables = new List<Table>();
                tables.Add(new Table() { Id = 1, Name = "Name 1", Description = "The Description 1", Type = "Type 1", State = 1, Price = 1000 });
                tables.Add(new Table() { Id = 2, Name = "Name 2", Description = "The Description 2", Type = "Type 2", State = 2, Price = 12345678.901 });
                tables.Add(new Table() { Id = 3, Name = "Name 3", Description = "The Description 3", Type = "Type 3", State = 3, Price = 123.45 });

                List<UPUni.ListToTable.CollectionHelper.AttributeDataItens> attributeDataItens = new List<UPUni.ListToTable.CollectionHelper.AttributeDataItens>();
                attributeDataItens.Add(new UPUni.ListToTable.CollectionHelper.AttributeDataItens(1, "State 1"));
                attributeDataItens.Add(new UPUni.ListToTable.CollectionHelper.AttributeDataItens(2, "State 2"));
                attributeDataItens.Add(new UPUni.ListToTable.CollectionHelper.AttributeDataItens(3, "State 3"));
                UPUni.ListToTable.CollectionHelper.AttributeData AttrState = new UPUni.ListToTable.CollectionHelper.AttributeData("State", typeof(string), attributeDataItens);

                UPUni.ListToTable.CollectionHelper.AddAttributeData(AttrState);

                dgvLista.DataSource = UPUni.ListToTable.CollectionHelper.ConvertTo<Table>(tables);
                dgvLista.ClearSelection();

                //CustomList
                /*
                 * listCustom.Responsive = false;
                 * listCustom.CountItensLine_xxl = 2; or CountItensLine_xl or CountItensLine_lg or CountItensLine_md or CountItensLine_xs > 0
                 */
                listCustom.Responsive = true;

                listCustom.CountItensLine_xxl = 4;
                listCustom.CountItensLine_xl = 4;
                listCustom.CountItensLine_lg = 2;
                listCustom.CountItensLine_md = 2;
                listCustom.CountItensLine_xs = 1;

                listCustom.AddItem(new ucListControl("test0"));
                listCustom.AddItem(new ucListControl("test1"));
                listCustom.AddItem(new ucListControl("test2"));
                listCustom.AddItem(new ucListControl("Not Selectable"), false);

                listCustom.SelectedItem += ListCustom_SelectedItem;

                //TreeDirectory
                chbTypesTree.Items.AddRange(new object[] {
                    UPUni.Enums.TypesConfigSearchTree.FILES_DIRECTORYS,
                    UPUni.Enums.TypesConfigSearchTree.FILES,
                    UPUni.Enums.TypesConfigSearchTree.DIRECTORYS,
                    UPUni.Enums.TypesConfigSearchTree.FILES_EXT });

                chbTypesTree.SelectedIndex = 0;

                //FTPServerCommunication
                chbFtpType.SelectedIndex = 0;

                txtFtpDir.Text = "/public_html";
                txtFtpPort.Value = 21;
                chbFtpType.SelectedIndex = 0;

                //WifiManager
                try
                {
                    this.managerWifi = new ManagerWifi();
                    this.managerWifi.ConnectedComplete += ManagerWifi_ConnectedComplete;
                    this.managerWifi.ConnectionStatusChanged += ManagerWifi_ConnectionStatusChanged;
                    btnRefreshWifi_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //Cryptor
        private void btnFileEnc_Click(object sender, EventArgs e)
        {
            try
            {
                this.openFileDialog1.Title = "Selecionar Arquivo";
                openFileDialog1.CheckFileExists = true;
                openFileDialog1.CheckPathExists = true;
                openFileDialog1.FilterIndex = 0;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.ReadOnlyChecked = true;
                openFileDialog1.ShowReadOnly = true;
                openFileDialog1.Multiselect = false;
                openFileDialog1.FileName = "";

                DialogResult dr = this.openFileDialog1.ShowDialog();

                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(this.openFileDialog1.FileName);
                    this.Ext = fileInfo.Extension;
                    string file = fileInfo.Directory + "\\" + fileInfo.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + txtExt.Text;
                    System.IO.File.Copy(fileInfo.FullName, file, true);
                    UPUni.Cryptor.File.Encrypt(file, txtKey.Text);
                    Process.Start(file);
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnFileDecr_Click(object sender, EventArgs e)
        {
            try
            {
                this.openFileDialog1.Title = "Selecionar Arquivo";
                openFileDialog1.Filter = "Arquivo " + txtExt.Text + " (*." + txtExt.Text + ")|*." + txtExt.Text;
                openFileDialog1.CheckFileExists = true;
                openFileDialog1.CheckPathExists = true;
                openFileDialog1.FilterIndex = 0;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.ReadOnlyChecked = true;
                openFileDialog1.ShowReadOnly = true;
                openFileDialog1.Multiselect = false;
                openFileDialog1.FileName = "";

                DialogResult dr = this.openFileDialog1.ShowDialog();

                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(this.openFileDialog1.FileName);
                    string file = fileInfo.Directory + "\\" + fileInfo.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + this.Ext;
                    System.IO.File.Copy(fileInfo.FullName, file, true);
                    UPUni.Cryptor.File.Decrypt(file, txtKey.Text);
                    Process.Start(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnByteEnc_Click(object sender, EventArgs e)
        {
            try
            {
                this.data = UPUni.Cryptor.Text.DESCrypto(UPUni.Enums.CryptoOperation.ENCRYPT, txtKey.Text, Encoding.Default.GetBytes(txtText.Text));
                txtTextEncr.Text = Encoding.Default.GetString(this.data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnByteDecr_Click(object sender, EventArgs e)
        {
            try
            {
                this.data = UPUni.Cryptor.Text.DESCrypto(UPUni.Enums.CryptoOperation.DECRYPT, txtKey.Text, this.data);
                txtTextDecr.Text = Encoding.Default.GetString(this.data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnBaseEnc_Click(object sender, EventArgs e)
        {
            try
            {
                txtTextEncr.Text = UPUni.Cryptor.Text.EncodeToBase64(txtText.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnBaseDec_Click(object sender, EventArgs e)
        {
            try
            {
                txtTextDecr.Text = UPUni.Cryptor.Text.DecodeFrom64(txtTextEncr.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnMd5_Click(object sender, EventArgs e)
        {
            try
            {
                txtEncode.Text = UPUni.Cryptor.Text.Md5Hash(txtText.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSha1_Click(object sender, EventArgs e)
        {
            try
            {
                txtEncode.Text = UPUni.Cryptor.Text.SHA1Hash(txtText.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnHardEnc_Click(object sender, EventArgs e)
        {
            try
            {
                txtTextEncr.Text = UPUni.Cryptor.Text.Encrypt(txtText.Text, txtKey.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnHardDecr_Click(object sender, EventArgs e)
        {
            try
            {
                txtTextDecr.Text = UPUni.Cryptor.Text.Decrypt(txtTextEncr.Text, txtKey.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //ServerDirectory
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                listFiles.Items.Clear();
                using (new UPUni.ServerDirectory.Impersonator(txtUser.Text, txtDomain.Text, txtPass.Text))
                {
                    foreach (var item in Directory.GetDirectories(txtPath.Text))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(item);
                        listFiles.Items.Add(directoryInfo.Name);
                    }
                    foreach (var item in Directory.GetFiles(txtPath.Text))
                    {
                        FileInfo fileInfo = new FileInfo(item);
                        listFiles.Items.Add(fileInfo.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //CustomList
        private void ListCustom_SelectedItem(object sender, UPUni.Components.Events.SelectedItemEventArgs e)
        {
            MessageBox.Show("Index [" + e.isSelected + "]: " + e.Index.ToString());
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                listCustom.AddItem(new ucListControl("test" + listCustom.Itens.Count));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAddItemAt_Click(object sender, EventArgs e)
        {
            try
            {
                Random random = new Random();
                listCustom.AddItemAt(new ucListControl("test" + listCustom.Itens.Count), random.Next(0, listCustom.Itens.Count - 1));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(listCustom.SelectedIndexes.Count > 0)
                {
                    listCustom.RemoveItemAt(listCustom.SelectedIndexes[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnUpdateItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listCustom.SelectedItens.Count > 0)
                {
                    foreach (var item in listCustom.SelectedItens)
                    {
                        listCustom.UpdateItem(item, new ucListControl("update" + (100 + listCustom.Itens.Count)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnNove_Click(object sender, EventArgs e)
        {
            try
            {
                if (listCustom.SelectedItens.Count > 0)
                {
                    foreach (var item in listCustom.SelectedItens)
                    {
                        Random random = new Random();
                        listCustom.MoveIntem(item, random.Next(0, listCustom.Itens.Count - 1));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in listCustom.Itens)
                {
                    listCustom.SelectItem(item, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnlCleanSelections_Click(object sender, EventArgs e)
        {
            try
            {
                listCustom.CleanSelections();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnRefreshItens_Click(object sender, EventArgs e)
        {
            try
            {
                listCustom.RefreshItens();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnInvokeItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listCustom.SelectedItens.Count > 0)
                {
                    foreach (var item in listCustom.SelectedItens)
                    {
                        listCustom.InvokeFunction(item, "UpdateText", new object[] { DateTime.Now.ToString("HH:mm:ss") });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnInvokePropItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listCustom.SelectedItens.Count > 0)
                {
                    foreach (var item in listCustom.SelectedItens)
                    {
                        listCustom.InvokeProperty(item, "Aux", DateTime.Now.ToString("HH:mm:ss"));
                        MessageBox.Show(listCustom.InvokeProperty(item, "Aux").ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnItensClear_Click(object sender, EventArgs e)
        {
            try
            {
                listCustom.ItensClear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //TreeDirectory
        private void btnSearchTree_Click(object sender, EventArgs e)
        {
            try
            {
                treeList.Nodes.Clear();
                UPUni.TreeDirectory.SimpleTree.OpenDirectory(treeList.Nodes, 
                    txtPathTree.Text, 
                    new UPUni.TreeDirectory.SimpleTree.ConfigSimpleTree() { 
                        Search = txtSearchTree.Text,
                        TypesConfigSearchTree = (UPUni.Enums.TypesConfigSearchTree)chbTypesTree.SelectedItem,
                        IsCaseSensitive = chbIsCaseSensitiveTree.Checked,
                        IsFile = chbIsFileTree.Checked,
                        IsEquals = chbIsEqualsTree.Checked
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //FTPServerCommunication
        private void btnFtpConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = txtFtpDir.Text;

                UPUni.FTPServerCommunication.FtpConfig config = new UPUni.FTPServerCommunication.FtpConfig();
                config.Host = txtFtpHost.Text;
                config.Port = (int)txtFtpPort.Value;
                config.UserName = txtFtpUser.Text;
                config.Password = txtFtpPass.Text;

                UPUni.Enums.FTPType fTPType = UPUni.Enums.FTPType.FTP;
                switch (chbFtpType.SelectedIndex)
                {
                    case 0:
                        fTPType = UPUni.Enums.FTPType.FTP;
                        break;
                    case 1:
                        fTPType = UPUni.Enums.FTPType.SFTP;
                        break;
                }

                UPUni.FTPServerCommunication.FTPManager fTPManager = new UPUni.FTPServerCommunication.FTPManager(fTPType, config);
                fTPManager.ServerLog += FTPManager_ServerLog;
                try
                {
                    ListFtpFiles.Items.Clear();
                    if (fTPManager.Connect())
                    {
                        List<DirectoryInfo> directoryInfos = fTPManager.ListAllDirectorys(dir);
                        List<FileInfo> fileInfos = fTPManager.ListAllFiles(dir);

                        foreach (var item in directoryInfos)
                        {
                            ListFtpFiles.Items.Add(item.Name);
                        }

                        foreach (var item in fileInfos)
                        {
                            ListFtpFiles.Items.Add(item.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    fTPManager.Disconnect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FTPManager_ServerLog(object sender, UPUni.Events.ServerLogEventArgs e)
        {
            try
            {
                string msg = e.Message;
                if (e.TypeLog == UPUni.Enums.TypeLog.EXCEPTION)
                {
                    msg = e.Message + ":" + e.Exception.ToString();
                }
                this.listLogFtp.Items.Add(e.DateTimeLog.ToString("dd/MM/yyyy HH:mm:ss") + " [" + e.TypeLog.ToString() + "] " + msg);
                int nItems = (int)(this.listLogFtp.Height / this.listLogFtp.ItemHeight);
                this.listLogFtp.TopIndex = this.listLogFtp.Items.Count - nItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void listLogFtp_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(listLogFtp.SelectedItems.Count != 0)
                {
                    Clipboard.SetText(listLogFtp.SelectedItems[0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //WifiManager
        private void btnRefreshWifi_Click(object sender, EventArgs e)
        {
            try
            {
                listWifi.Items.Clear();

                foreach (var item in this.managerWifi.ListWifi())
                {
                    listWifi.Items.Add(item.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnWifiStatus_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(this.managerWifi.ShowInfoWifi(listWifi.SelectedItem.ToString()).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnWifiDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.managerWifi.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnWifiConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.managerWifi.ConnectInWifiAsync(listWifi.SelectedItem.ToString(), txtWifiPass.Text, txtWifiDom.Text, chbWifiOverProf.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ManagerWifi_ConnectionStatusChanged(object sender, WifiConnection.WifiStatusEventArgs e)
        {
            try
            {
                try
                {
                    lblWifiInfo.Text = string.Format("New status: {0}", e.NewStatus.ToString());
                }
                catch
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        lblWifiInfo.Text = string.Format("New status: {0}", e.NewStatus.ToString());
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ManagerWifi_ConnectedComplete(object sender, UPUni.Events.WifiConnectedCompleteEventArgs e)
        {
            try
            {
                try
                {
                    lblWifiInfo.Text = string.Format("OnConnectedComplete, success: {0}", e.IsConnected.ToString());
                }
                catch
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        lblWifiInfo.Text = string.Format("OnConnectedComplete, success: {0}", e.IsConnected.ToString());
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //StringsCombinations
        private void btnGenerateCombinations_Click(object sender, EventArgs e)
        {
            try
            {
                listPassComb.Items.Clear();

                Defaults defaults = new Defaults();
                defaults.Maximum = Convert.ToInt32(txtMaximum.Value);
                defaults.Minimum = Convert.ToInt32(txtMinimum.Value);
                defaults.Others = txtOthers.Text;
                defaults.Remove = txtRemove.Text;
                defaults.Lower = chbLower.Checked;
                defaults.Upper = chbUpper.Checked;
                defaults.Number = chbNumber.Checked;
                defaults.Special = chbSpecial.Checked;
                defaults.AccentsLower = chbAccentsLower.Checked;
                defaults.AccentsUpper = chbAccentsUpper.Checked;

                Combinations combinations = new Combinations(defaults, Convert.ToInt32(txtLimit.Value));
                combinations.GenerateCombinationsEvent += Combinations_GenerateCombinationsEvent;

                combinations.GenerateCombinations();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Combinations_GenerateCombinationsEvent(object sender, UPUni.Events.GenerateCombinationsEventArgs e)
        {
            try
            {
                try
                {
                    listPassComb.Items.Add("[" + e.CombinationCount + "] " + e.CurrentCombinationStr);
                }
                catch
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        listPassComb.Items.Add("[" + e.CombinationCount + "] " + e.CurrentCombinationStr);
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
