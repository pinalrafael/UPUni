UPUni
==========


[![NuGet](https://img.shields.io/badge/NuGet-Download-success)](https://www.nuget.org/packages/UPUni)

The `UPUni` library aims to unify several functions that can be useful in your developments.
See some examples below and feel free to create Pull requests.

Cryptor
---------------------------

The `UPUni.Cryptor.Text` class has functions for encoding and decoding strings.
See some examples below:

```cs
//Byte Encypt
byte[] DESCryptoENCRYPT = UPUni.Cryptor.Text.DESCrypto(CryptoOperation.ENCRYPT, "12345678", Encoding.Default.GetBytes("test"));
Console.WriteLine(Encoding.Default.GetString(DESCryptoENCRYPT)); //J$.òéÎL¦
//Byte Decrypt
byte[] DESCryptoDECRYPT = UPUni.Cryptor.Text.DESCrypto(CryptoOperation.DECRYPT, "12345678", DESCryptoENCRYPT);
Console.WriteLine(Encoding.Default.GetString(DESCryptoDECRYPT)); //test

//Hard Encrypt - the generated strings will never be the same increasing security
Console.WriteLine(UPUni.Cryptor.Text.Encrypt("test", "1234567890"));//g9VxsBxjIiXqAzHL9JwGb+1LM6vuHOXDy8kURAEOcBLhjgS19NTr6ap7t2GC52lS2/y+RC7469/w1rb4F6La5F6ArLLzB1CJVqEt61fvu3Fgk7IGoCdYBDZa+4bUg2b5
//Hard Decript
Console.WriteLine(UPUni.Cryptor.Text.Decrypt("g9VxsBxjIiXqAzHL9JwGb+1LM6vuHOXDy8kURAEOcBLhjgS19NTr6ap7t2GC52lS2/y+RC7469/w1rb4F6La5F6ArLLzB1CJVqEt61fvu3Fgk7IGoCdYBDZa+4bUg2b5", "1234567890"));//test

//Base64 Encode
Console.WriteLine(UPUni.Cryptor.Text.EncodeToBase64("test"));//dGVzdA==
//Base64 Decode
Console.WriteLine(UPUni.Cryptor.Text.DecodeFrom64("dGVzdA=="));//test

//MD5
Console.WriteLine(UPUni.Cryptor.Text.Md5Hash("test"));//098f6bcd4621d373cade4e832627b4f6
//SHA1
Console.WriteLine(UPUni.Cryptor.Text.SHA1Hash("test"));//A94A8FE5CCB19BA61C4C0873D391E987982FBBD3
```

The `UPUni.Cryptor.File` class has functions for encoding and decoding files.
See some examples below:

```cs
//File Encrypt
UPUni.Cryptor.File.Encrypt("D:\\UPUni\\Sample\\file.txt", "12345678");

//File Encrypt
UPUni.Cryptor.File.Decrypt("D:\\UPUni\\Sample\\file.txt", "12345678");
```

![Example Cryptor](https://github.com/pinalrafael/UPUni/blob/master/Sample/assets/Cryptor.png)

ServerDirectory
---------------------------

The `UPUni.ServerDirectory.Impersonator` class can be used to log into domain or non-domain servers to manipulate the files and folders.
See an example below:

```cs
using (new UPUni.ServerDirectory.Impersonator("username", "domain", "password"))
{
    foreach (var item in Directory.GetDirectories("\\\\192.168.0.1\\UPUni\\Sample"))
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(item);
        Console.WriteLine(directoryInfo.Name);
    }
    foreach (var item in Directory.GetFiles("\\\\192.168.0.1\\UPUni\\Sample"))
    {
        FileInfo fileInfo = new FileInfo(item);
        Console.WriteLine(fileInfo.Name);
    }
}
```

![Example ServerDirectory](https://github.com/pinalrafael/UPUni/blob/master/Sample/assets/ServerDirectory.png)

ListToTable
---------------------------

The `UPUni.ListToTable.CollectionHelper` class was developed for the purpose of customizing DataGridView.
See an example below:

Class `Table`, when declaring `[UPUni.Attributes.Table(true, "PRICE", "$ {0:0,0.00}")]` I am informing that the column must be displayed, the column title and formatting for the values.
```cs
public class Table
{
    [UPUni.Attributes.Table(true, "ID")]
    public int Id { get; set; }
    [UPUni.Attributes.Table(true, "NAME")]
    public string Name { get; set; }
    [UPUni.Attributes.Table(true, "DESCRIPTION")]
    public string Description { get; set; }
    [UPUni.Attributes.Table(true, "PRICE", "$ {0:0,0.00}")]
    public double Price { get; set; }
    [UPUni.Attributes.Table(true, "STATE")]
    public int State { get; set; }
    [UPUni.Attributes.Table(false, "TYPE")]
    public string Type { get; set; }
}
```

```cs
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
```

![Example ListToTable](https://github.com/pinalrafael/UPUni/blob/master/Sample/assets/ListToTable.png)

TreeDirectory
---------------------------

The `UPUni.TreeDirectory.SimpleTree` class can be used to automate the listing of files and folders in a specific directory and facilitate searching.
See an example below:

```cs
UPUni.TreeDirectory.SimpleTree.OpenDirectory(treeList.Nodes,
"D:\\UPUni\\Sample",
new UPUni.TreeDirectory.SimpleTree.ConfigSimpleTree()
{
    Search = "value to search",
    TypesConfigSearchTree = UPUni.Enums.TypesConfigSearchTree.FILES_DIRECTORYS,
    IsCaseSensitive = true,
    IsFile = true,
    IsEquals = true
});
```

![Example TreeDirectory](https://github.com/pinalrafael/UPUni/blob/master/Sample/assets/TreeDirectory.png)

FTPServerCommunication
---------------------------

The `UPUni.FTPServerCommunication.FTPManager` class aims to facilitate communication with FTP or SFTP, making changes faster.
See an example below:

```cs
UPUni.FTPServerCommunication.FtpConfig config = new UPUni.FTPServerCommunication.FtpConfig();
config.Host = "host.ftp";
config.Port = 21;
config.UserName = "user.ftp";
config.Password = "pass.ftp";

UPUni.FTPServerCommunication.FTPManager fTPManager = new UPUni.FTPServerCommunication.FTPManager(UPUni.Enums.FTPType.FTP, config);
fTPManager.ServerLog += FTPManager_ServerLog;//Log return event.
try
{
    if (fTPManager.Connect())
    {
        List<DirectoryInfo> directoryInfos = fTPManager.ListAllDirectorys("patch/to/public_html");
        List<FileInfo> fileInfos = fTPManager.ListAllFiles("patch/to/public_html");

        foreach (var item in directoryInfos)
        {
            Console.WriteLine(item.Name);
        }

        foreach (var item in fileInfos)
        {
            Console.WriteLine(item.Name);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    fTPManager.Disconnect();
}
```

Event `UPUni.FTPServerCommunication.FTPManager.ServerLog`.
```cs
private void FTPManager_ServerLog(object sender, UPUni.Events.ServerLogEventArgs e)
{
    string msg = e.Message;
    if (e.TypeLog == UPUni.Enums.TypeLog.EXCEPTION)
    {
        msg = e.Message + ":" + e.Exception.ToString();
    }
    Console.WriteLine(e.DateTimeLog.ToString("dd/MM/yyyy HH:mm:ss") + " [" + e.TypeLog.ToString() + "] " + msg);
}
```

![Example FTPServerCommunication](https://github.com/pinalrafael/UPUni/blob/master/Sample/assets/FTPServerCommunication.png)

WifiManager
---------------------------

The `UPUni.WifiManager.ManagerWifi` class was developed to manipulate the network card by dynamically connecting, displaying information and disconnecting from Wi-Fi networks.
See an example below:

```cs
ManagerWifi managerWifi = new ManagerWifi();
managerWifi.ConnectedComplete += ManagerWifi_ConnectedComplete;//Event called when the connection is completed with or without failure.
managerWifi.ConnectionStatusChanged += ManagerWifi_ConnectionStatusChanged;//Event called when there is a change in the Wi-Fi connection status.

managerWifi.ConnectInWifiAsync("SSID_WIFI", "password", "domain", true);

//Disconnects from all Wi-Fi networks.
managerWifi.Disconnect();
```

Event `UPUni.WifiManager.ManagerWifi.ConnectedComplete`.
```cs
private void ManagerWifi_ConnectedComplete(object sender, UPUni.Events.WifiConnectedCompleteEventArgs e)
{
    Console.WriteLine(string.Format("OnConnectedComplete, success: {0}", e.IsConnected.ToString()));
}
```

Event `UPUni.WifiManager.ManagerWifi.ConnectionStatusChanged`.
```cs
private void ManagerWifi_ConnectionStatusChanged(object sender, WifiConnection.WifiStatusEventArgs e)
{
    Console.WriteLine(string.Format("New status: {0}", e.NewStatus.ToString()));
}
```

![Example WifiManager](https://github.com/pinalrafael/UPUni/blob/master/Sample/assets/WifiManager.png)

StringsCombinations
---------------------------

The `UPUni.StringsCombinations.Combinations` class has the function of generating all possible combinations within the given parameters.
**ATTENTION: the `UPUni.StringsCombinations.Combinations` function may generate excessive processing and storage usage. Use sparingly!**   
See an example below:

```cs
Defaults defaults = new Defaults();
defaults.Minimum = 8;
defaults.Maximum = 100;
defaults.Others = "";
defaults.Remove = "";
defaults.Lower = true;
defaults.Upper = true;
defaults.Number = true;
defaults.Special = true;
defaults.AccentsLower = true;
defaults.AccentsUpper = true;

Combinations combinations = new Combinations(defaults, 0);
combinations.GenerateCombinationsEvent += Combinations_GenerateCombinationsEvent;//Event to handle the generated combinations.

combinations.GenerateCombinations();
```

Event `UPUni.StringsCombinations.Combinations.GenerateCombinationsEvent`.
```cs
private void Combinations_GenerateCombinationsEvent(object sender, UPUni.Events.GenerateCombinationsEventArgs e)
{
    Console.WriteLine(string.Format("[" + e.CombinationCount + "] " + e.CurrentCombinationStr);
}
```

![Example StringsCombinations](https://github.com/pinalrafael/UPUni/blob/master/Sample/assets/StringsCombinations.png)


UPUni.Components
==========


[![NuGet](https://img.shields.io/badge/NuGet-Download-success)](https://www.nuget.org/packages/UPUni.Components)

The `UPUni.Components` library aims to unify several custom components for Windows Forms, to optimize their development.

CustomList
---------------------------

The `UPUni.Components.CustomList.ListView` class creates a fully customizable, accessible and responsive list in the `FlowLayoutPanel`.
The component was inspired by Android's Custom Adapters, bringing this facility to Windows Forms, making use of `UserControl`.
See an example below:

```cs
/*
 * listCustom.Responsive = false;
 * listCustom.CountItensLine_xxl = 2; or CountItensLine_xl or CountItensLine_lg or CountItensLine_md or CountItensLine_xs > 0
 */
UPUni.Components.CustomList.ListView listCustom = new UPUni.Components.CustomList.ListView();
//Add the component to the form

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

listCustom.SelectedItem += ListCustom_SelectedItem;//Event called when a list item is selected.
```

Event `UPUni.Components.CustomList.ListView.SelectedItem`.
```cs
private void ListCustom_SelectedItem(object sender, UPUni.Components.Events.SelectedItemEventArgs e)
{
    Console.WriteLine("Index [" + e.isSelected + "]: " + e.Index.ToString());
}
```

![Example CustomList](https://github.com/pinalrafael/UPUni/blob/master/Sample/assets/CustomList.png)

PassConnection
==========

Console software to demonstrate the use of the `UPUni.WifiManager.ManagerWifi` and `UPUni.StringsCombinations.Combinations` classes.

```shell
PassConnection.exe -TPWPA -LG1 -SV0 -MI8 -MX8 -RC´`~^/\\|(){}[]<>%-_=+?;:,. -AL0 -AU0 -HK1
```

![Example PassConnection](https://github.com/pinalrafael/UPUni/blob/master/Sample/assets/PassConnection.png)

Follow me
==========
[**Follow me**](https://github.com/pinalrafael?tab=followers) for my next creations!