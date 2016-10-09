using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFileSystem
{
    public class Command
    {
        public static readonly string FILE_ERROR = ">Error: File does not exist ";
        public static readonly string VERSION_ERROR = ">Error: Version does not exist!";
        public static readonly string DELETE_ERROR = ">Error: File  does not exist!";
        public static readonly string BLOCK_NUMBER_ERROR = ">Error: Block number is Out of bounds - ";
        public static readonly string CHECKPOINT_MESSAGE = ">Checkpoint Sucessful - New Version Number : ";
        public static readonly string COPYFS_MESSAGE = ">Copy FS Sucessful -";
        public static readonly string SWITCH_MESSAGE = ">Switch Sucessful - switched to Version Number : ";
        public static readonly string DELETE_MESSAGE = ">File Deleted - ";
        // **************** - 16 
        public static readonly string EMPTY_INODE = "*******************************************************************************************************************************"; //127 
        public static readonly string EMPTY_INODE_CHR = "*";
        public static readonly char INODE_SEPARATOR_CHR = '-';
        public static readonly char INODE_ATTRIBUTE_SEPARATOR_CHR =';' ;
        public static readonly string INODE_ATTRIBUTE_SEPARATOR_STRING = "--";
        public static readonly int ADDRESS_OFFSET = 1;
      //  public static readonly int INODE_SEPARATOR_LENGTH = INODE_SEPARATOR.Length;




        //Console.WriteLine("VFS SYSTEM:\n");
        //Console.WriteLine("Enter Command:\n");
        //string[] arg = Console.ReadLine().Split(' ');
        //if (arg.Length != 2 || arg[1] == null)
        //{
        //    Console.WriteLine(Arguments.INIT_ERROR);
        //    Console.ReadKey();
        //    return;
        //}
    }
}
