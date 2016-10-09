using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace LogFileSystem
{
    class INode
    {
        public static readonly int INODE_PARAMETERS = 5;
        //PROPERTIES OF INODE
        public string Directory
        {
            get;
            set;
        }
        public string Filename
        {   get;
            set;
        }
        public long BlockNumber
        {
            get;
            set;
        }
        public int INodeNumber
        {
            get;
            set;
        }
        public int FileSize
        {
            get;
            set;
        }
       
        
        //INODE CONSTRUCTORS
        public INode()
        {
            // TODO: Complete member initialization
        }

        public INode(string directory, string filename, long blocknumber, int inodeNumber, int inodeSize)
        {
            Directory = directory;
            Filename = filename;
            BlockNumber = blocknumber;
            INodeNumber = inodeNumber;
            FileSize = inodeSize;

        }
        public static INode parseINodeString(string inodeString)
        {
            string[] fileAttributes = inodeString.Split(Command.INODE_ATTRIBUTE_SEPARATOR_CHR);
            INode inode = new INode();

            if (fileAttributes.Length == INODE_PARAMETERS+1)
            {
                // INODE MAP : INODE ATTRIBUTE ORDER
                // INODENUMBER ; FILENAME ; DIRECTORY ; FILE_SIZE ; BLOCK_NUMBER - NEXT INODE 

                inode.INodeNumber = int.Parse(fileAttributes[0]);
                inode.Filename = fileAttributes[1];
                inode.Directory = fileAttributes[2]; 
                inode.FileSize = int.Parse(fileAttributes[3]);
                inode.BlockNumber = int.Parse(fileAttributes[4]);

                return inode;
            }
            return null;
        }
        public  string createINodeString()
        {
            string inodeAttributes = 
                INodeNumber.ToString().Trim() + Command.INODE_ATTRIBUTE_SEPARATOR_CHR
                + Filename + Command.INODE_ATTRIBUTE_SEPARATOR_CHR
                + Directory + Command.INODE_ATTRIBUTE_SEPARATOR_CHR 
                + FileSize.ToString() + Command.INODE_ATTRIBUTE_SEPARATOR_CHR 
                + BlockNumber.ToString()+ Command.INODE_ATTRIBUTE_SEPARATOR_CHR;

            return inodeAttributes;

        }

    }
}
