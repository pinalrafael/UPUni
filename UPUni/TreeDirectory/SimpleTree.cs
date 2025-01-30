using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UPUni.Enums;

namespace UPUni.TreeDirectory
{
    /// <summary>
    /// Class create simple tree
    /// </summary>
    public class SimpleTree
    {
        private static TreeNodeCollection treeNodeCollection { get; set; }
        private static ConfigSimpleTree configSimpleTree { get; set; }

        /// <summary>
        /// Create node tree from collection
        /// </summary>
        /// <param name="nodeCollection">Node tree collection <see cref="TreeNodeCollection"/></param>
        /// <param name="path">String folder patch</param>
        /// <param name="configTree">Configs tree <see cref="ConfigSimpleTree"/></param>
        /// <returns>tree node <see cref="NodeTree"/></returns>
        public static NodeTree OpenDirectory(TreeNodeCollection nodeCollection, string path, ConfigSimpleTree configTree = null)
        {
            treeNodeCollection = nodeCollection;
            if(configTree == null)
            {
                configTree = new ConfigSimpleTree();
            }
            configSimpleTree = configTree;

            NodeTree nodeTree = new NodeTree(new DirectoryInfo(path));

            CreateNodes(null, nodeTree, path);

            return nodeTree;
        }

        /// <summary>
        /// Create node tree
        /// </summary>
        /// <param name="path">String folder patch</param>
        /// <param name="configTree">Configs simple tree <see cref="ConfigSimpleTree"/></param>
        /// <returns>tree node <see cref="NodeTree"/></returns>
        public static NodeTree OpenDirectory(string path, ConfigSimpleTree configTree = null)
        {
            if (configTree == null)
            {
                configTree = new ConfigSimpleTree();
            }
            configSimpleTree = configTree;

            NodeTree nodeTree = new NodeTree(new DirectoryInfo(path));

            CreateNodes(null, nodeTree, path);

            return nodeTree;
        }

        private static void CreateNodes(TreeNode TreeNodes, NodeTree node, string dir)
        {
            int x = 0;
            foreach (var item in Directory.GetDirectories(dir))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(item);

                if (!configSimpleTree.Search.Equals(string.Empty))
                {
                    if (configSimpleTree.TypesConfigSearchTree == TypesConfigSearchTree.DIRECTORYS ||
                       configSimpleTree.TypesConfigSearchTree == TypesConfigSearchTree.FILES_DIRECTORYS)
                    {
                        string search = configSimpleTree.Search;
                        string str = directoryInfo.Name;

                        if (!configSimpleTree.IsCaseSensitive)
                        {
                            search = search.ToLower();
                            str = str.ToLower();
                        }

                        if (!search.Equals(string.Empty))
                        {
                            if (configSimpleTree.IsEquals)
                            {
                                if (!str.Equals(search))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (!str.Contains(search))
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }

                node.Directoryes.Add(directoryInfo);

                NodeTree nodeTree = new NodeTree(directoryInfo);
                node.NodeTrees.Add(nodeTree);

                if (treeNodeCollection != null)
                {
                    TreeNode treeNode = new TreeNode(directoryInfo.Name);
                    if (TreeNodes == null)
                    {
                        treeNodeCollection.Add(treeNode);
                        CreateNodes(treeNode, nodeTree, directoryInfo.FullName);
                    }
                    else
                    {
                        TreeNodes.Nodes.Add(treeNode);
                        CreateNodes(TreeNodes.Nodes[x], nodeTree, directoryInfo.FullName);
                    }
                }

                x++;
            }

            if (configSimpleTree.IsFile)
            {
                foreach (var itemFile in Directory.GetFiles(dir))
                {
                    FileInfo fileInfo = new FileInfo(itemFile);

                    if (configSimpleTree.TypesConfigSearchTree == TypesConfigSearchTree.FILES ||
                        configSimpleTree.TypesConfigSearchTree == TypesConfigSearchTree.FILES_DIRECTORYS ||
                        configSimpleTree.TypesConfigSearchTree == TypesConfigSearchTree.FILES_EXT)
                    {
                        string search = configSimpleTree.Search;
                        string str = fileInfo.Name;

                        if(configSimpleTree.TypesConfigSearchTree == TypesConfigSearchTree.FILES_EXT)
                        {
                            str = fileInfo.Extension;
                        }

                        if (!configSimpleTree.IsCaseSensitive)
                        {
                            search = search.ToLower();
                            str = str.ToLower();
                        }

                        if (!search.Equals(string.Empty))
                        {
                            if (configSimpleTree.IsEquals)
                            {
                                if (!str.Equals(search))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (!str.Contains(search))
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    node.Files.Add(fileInfo);

                    if (treeNodeCollection != null)
                    {
                        TreeNode treeNode = new TreeNode(fileInfo.Name);
                        if (TreeNodes == null)
                        {
                            treeNodeCollection.Add(treeNode);
                        }
                        else
                        {
                            TreeNodes.Nodes.Add(treeNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Class node tree
        /// </summary>
        public class NodeTree
        {
            /// <summary>
            /// Directory owner <see cref="DirectoryInfo"/>
            /// </summary>
            public DirectoryInfo Owner { get; set; }
            /// <summary>
            /// List of directories <see cref="DirectoryInfo"/>
            /// </summary>
            public List<DirectoryInfo> Directoryes { get; set; }
            /// <summary>
            /// List of files <see cref="FileInfo"/>
            /// </summary>
            public List<FileInfo> Files { get; set; }
            /// <summary>
            /// List of nodes parent <see cref="NodeTree"/>
            /// </summary>
            public List<NodeTree> NodeTrees { get;set; }

            /// <summary>
            /// Create new node tree
            /// </summary>
            /// <param name="owner">Directory owner <see cref="DirectoryInfo"/></param>
            public NodeTree(DirectoryInfo owner) 
            {
                this.Owner = owner;
                this.Directoryes = new List<DirectoryInfo>();
                this.Files = new List<FileInfo>();
                this.NodeTrees = new List<NodeTree>();
            }
        }

        /// <summary>
        /// Class configs simple tree
        /// </summary>
        public class ConfigSimpleTree
        {
            /// <summary>
            /// Get or set if show files
            /// </summary>
            public bool IsFile { get; set; }
            /// <summary>
            /// Get or set search case sensitive
            /// </summary>
            public bool IsCaseSensitive { get; set; }
            /// <summary>
            /// Get or set if equals
            /// </summary>
            public bool IsEquals { get; set; }
            /// <summary>
            /// Get or set string search
            /// </summary>
            public string Search { get; set; }
            /// <summary>
            /// Type config search tree <see cref="TypesConfigSearchTree"/>
            /// </summary>
            public TypesConfigSearchTree TypesConfigSearchTree { get; set; }

            /// <summary>
            /// Create new config simple tree
            /// </summary>
            public ConfigSimpleTree()
            {
                this.IsFile = true;
                this.IsCaseSensitive = false;
                this.IsEquals = false;
                this.Search = string.Empty;
                this.TypesConfigSearchTree = TypesConfigSearchTree.FILES_DIRECTORYS;
            }
        }
    }
}
