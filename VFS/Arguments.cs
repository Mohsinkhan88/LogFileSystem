using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFileSystem
{
    class Arguments
	{
		public static readonly int DIR_ARGS= 1;
		public static readonly int CD_ARGS = 2;
		public static readonly int PWD_ARGS = 1;
		public static readonly int COPYFS_ARGS = 3;
		public static readonly int READ_ARGS = 4;
		public static readonly int WRITE_ARGS = 4;
		public static readonly int DELETE_ARGS = 2;
		public static readonly int SWITCH_ARGS = 2;
		public static readonly int SCRIPT_ARGS = 2;
		public static readonly int CHECKPOINT_ARGS = 1;


		public static readonly string READ = "read";
		public static readonly string WRITE = "write";
		public static readonly string DELETE = "delete";
		public static readonly string DIR = "dir";
		public static readonly string CHANGE_DIRECTORY = "cd";
		public static readonly string SWITCH = "switch";
		public static readonly string CHECKPOINT = "checkpoint";
		public static readonly string PWD = "pwd";
		public static readonly string COPYFS = "copyfs";
		public static readonly string SCRIPT= "script";
		public static readonly string EXIT = "exit";


		public static readonly string INIT_ERROR="\n>ERROR : INCOMPLETE ARGUMENT LIST - ENTER vfs <file_name> ";
		public static readonly string READ_ERROR = "\n>ERROR : INCOMPLETE ARGUMENT LIST FOR READ - >ENTER read <file_name> <block_Number> <outputFile> ";
		public static readonly string WRITE_ERROR = "\n>ERROR : INCOMPLETE ARGUMENT LIST FOR WRITE - >ENTER write <file_name> <block_Number> <inputFile> ";
		public static readonly string DELETE_ERROR = "\n>ERROR : INCOMPLETE ARGUMENT LIST FOR DELETE - >ENTER delete <file_name> ";
		public static readonly string SCRIPT_ERROR = "\n>ERROR : INCOMPLETE ARGUMENT LIST FOR SCRIPT - >ENTER script <script_file_name> ";
		public static readonly string CD_ERROR = "\n>ERROR : INCOMPLETE ARGUMENT LIST FOR CD - >ENTER cd <directory_name> ";
		public static readonly string SWITCH_ERROR = "\n>ERROR : INCOMPLETE ARGUMENT LIST FOR SWITCH - >ENTER switch <version> ";
		public static readonly string COPYFS_ERROR = "\n>ERROR : INCOMPLETE ARGUMENT LIST FOR COPYFS - >ENTER copyfs <version> <output_Directory>";
		public static readonly string CHECKPOINT_ERROR = "\n>ERROR : INVALID ARGUMENT - ";
		public static readonly string DIR_ERROR = "\n>ERROR : INCORRECT ARGUMENTS FOR DIR - >ENTER dir";
		public static readonly string PWD_ERROR = "\n>ERROR : INCORRECT ARGUMENTS FOR PWD - >ENTER pwd";
		public static readonly string UNKNOWN_COMMAND_ERROR = "\n>ERROR : UNKNOWN COMMAND ";
		public static readonly string EXIT_MESSAGE= "\n>.....EXITTING VFS.....\n";
	}
}
