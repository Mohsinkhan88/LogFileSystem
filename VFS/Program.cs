using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  LogFileSystem

{
    class Program
    {
        private static  LogFileSystem vfs;
        public static void Main(string[] args)
        {
            try
            {
                //RUN THROUGH CMD
                //CHECK FOR VFS INITIALIZATION
                if (args[0] == null || args.Length != 1)
                {
                    Console.WriteLine(Arguments.INIT_ERROR);
                    Console.ReadKey();
                    return;
                }
                else
                {
                    //INITIALIZE NEW VFS INSTANCE
                    vfs = new LogFileSystem();
                    vfs.PrintCommands();
                    vfs.InitializeLogFileSystem(args[0]);
                    ExecuteCommandLine();

                }

                ////--------------------------------------------------------------------------------------------//
                //////FOR CONSOLE DEBUGGING//
                
                //string[] input;
                //Console.WriteLine("*-----           VFS         -----*");
                //Console.WriteLine("Enter VFS Root Command: vfs <fileName>");
                //input = Console.ReadLine().Split(' ');
                //if (input[0] != "vfs" || input[1] == null || input.Length != 2)
                //{
                //    Console.WriteLine(Arguments.INIT_ERROR);
                //    Console.ReadKey();
                //    return;
                //}
                //else
                //{
                //    //INITIALIZE NEW VFS INSTANCE
                //    vfs = new VFS();
                //    vfs.PrintCommands();
                //    vfs.InitializeVFS(input[1]);
                //    ExecuteCommandLine();

                //}
           
            
            }
            catch(Exception ex)
            {
                Console.WriteLine(">" + ex.Message);
            }
        }
       
        public static void ExecuteCommandLine()
        {
           

            //READ USER COMMANDS
            while (true)
            {
                Console.Write(">");
                string[] arguments = (Console.ReadLine().Split(' '));

                //DIR COMMAND
                if (arguments[0].Equals(Arguments.DIR))
                {
                    if (arguments.Length != Arguments.DIR_ARGS)
                    {
                        Console.WriteLine(Arguments.DIR_ERROR);
                        continue;
                    }

                    //EXECUTE DIR
                    vfs.DIR();

                }

                //PWD COMMAND
                else if (arguments[0].Equals(Arguments.PWD))
                {
                    if (arguments.Length != Arguments.PWD_ARGS)
                    {
                        Console.WriteLine(Arguments.PWD_ERROR);
                        continue;
                    }

                    //EXECUTE PWD
                    vfs.PWD();

                }

                //EXIT COMMAND
                else if (arguments[0].Equals(Arguments.EXIT))
                {

                    //EXECUTE EXIT
                    vfs.Exit(); break;


                }

                //CD COMMAND
                else if (arguments[0].Equals(Arguments.CHANGE_DIRECTORY))
                {
                    if (arguments.Length != Arguments.CD_ARGS)
                    {
                        Console.WriteLine(Arguments.CD_ERROR);
                        continue;
                    }

                    //EXECUTE CD
                    vfs.CD(arguments[1]);

                }

                //SWITCH COMMAND
                else if (arguments[0].Equals(Arguments.SWITCH))
                {
                    if (arguments.Length != Arguments.SWITCH_ARGS)
                    {
                        Console.WriteLine(Arguments.SWITCH_ERROR);
                        continue;
                    }

                    //EXECUTE SWITCH
                    int version = int.Parse(arguments[1]);
                    vfs.Switch(version);

                }

                //COPYFS COMMAND
                else if (arguments[0].Equals(Arguments.COPYFS))
                {
                    if (arguments.Length != Arguments.COPYFS_ARGS)
                    {
                        Console.WriteLine(Arguments.COPYFS_ERROR);
                        continue;
                    }

                    int version = int.Parse(arguments[1]);
                    string outputDir = arguments[2];
                    //EXECUTE COPYFS
                    vfs.CopyFS(version, outputDir);

                }

                //READ COMMAND
                else if (arguments[0].Equals(Arguments.READ))
                {
                    if (arguments.Length != Arguments.READ_ARGS)
                    {
                        Console.WriteLine(Arguments.READ_ERROR);
                        continue;
                    }

                    string filename = arguments[1];
                    int blockNumber = int.Parse(arguments[2]);
                    string outputFile = arguments[3];
                    //EXECUTE READ
                    vfs.Read(filename, blockNumber, outputFile);

                }

                //WRITE COMMAND
                else if (arguments[0].Equals(Arguments.WRITE))
                {
                    if (arguments.Length != Arguments.WRITE_ARGS)
                    {
                        Console.WriteLine(Arguments.WRITE_ERROR);
                        continue;
                    }

                    string filename = arguments[1];
                    int blockNumber = int.Parse(arguments[2]);
                    string outputFile = arguments[3];
                    //EXECUTE READ
                    vfs.Write(filename, blockNumber, outputFile);

                }

                //DELETE COMMAND
                else if (arguments[0].Equals(Arguments.DELETE))
                {
                    if (arguments.Length != Arguments.DELETE_ARGS)
                    {
                        Console.WriteLine(Arguments.DELETE_ERROR);
                        continue;
                    }

                    //EXECUTE DELETE
                    vfs.Delete(arguments[1]);

                }

                //RUN SCRIPT COMMAND
                else if (arguments[0].Equals(Arguments.SCRIPT))
                {
                    if (arguments.Length != Arguments.SCRIPT_ARGS)
                    {
                        Console.WriteLine(Arguments.SCRIPT_ERROR);
                        continue;
                    }

                    //EXECUTE SCRIPT
                    vfs.ScriptFS(arguments[1]);

                }
                //RUN CHECKPOINT COMMAND
                else if (arguments[0].Equals(Arguments.CHECKPOINT))
                {
                    if (arguments.Length != Arguments.CHECKPOINT_ARGS)
                    {
                        Console.WriteLine(Arguments.CHECKPOINT_ERROR);
                        continue;
                    }

                    //EXECUTE SCRIPT
                    vfs.Checkpoint();

                }
                else if (!arguments[0].Equals(""))
                {
                    Console.WriteLine(Arguments.UNKNOWN_COMMAND_ERROR);

                }
            }
          

        }
        public static void ExecuteScript(string args)
        {

                string[] arguments = args.Split(' ');

                //DIR COMMAND
                if (arguments[0].Equals(Arguments.DIR))
                {
                    if (arguments.Length != Arguments.DIR_ARGS)
                    {
                        Console.WriteLine(Arguments.DIR_ERROR);
                       // continue;
                    }

                    //EXECUTE DIR
                    vfs.DIR();

                }

                //PWD COMMAND
                else if (arguments[0].Equals(Arguments.PWD))
                {
                    if (arguments.Length != Arguments.PWD_ARGS)
                    {
                        Console.WriteLine(Arguments.PWD_ERROR);
                        //continue;
                    }

                    //EXECUTE PWD
                    vfs.PWD();

                }

                //EXIT COMMAND
                else if (arguments[0].Equals(Arguments.EXIT))
                {

                    //EXECUTE EXIT
                    vfs.Exit(); //break;


                }

                //CD COMMAND
                else if (arguments[0].Equals(Arguments.CHANGE_DIRECTORY))
                {
                    if (arguments.Length != Arguments.CD_ARGS)
                    {
                        Console.WriteLine(Arguments.CD_ERROR);
                        //continue;
                    }

                    //EXECUTE CD
                    vfs.CD(arguments[1]);

                }

                //SWITCH COMMAND
                else if (arguments[0].Equals(Arguments.SWITCH))
                {
                    if (arguments.Length != Arguments.SWITCH_ARGS)
                    {
                        Console.WriteLine(Arguments.SWITCH_ERROR);
                       // continue;
                    }

                    //EXECUTE SWITCH
                    int version = int.Parse(arguments[1]);
                    vfs.Switch(version);

                }

                //COPYFS COMMAND
                else if (arguments[0].Equals(Arguments.COPYFS))
                {
                    if (arguments.Length != Arguments.COPYFS_ARGS)
                    {
                        Console.WriteLine(Arguments.COPYFS_ERROR);
                        //continue;
                    }

                    int version = int.Parse(arguments[1]);
                    string outputDir = arguments[2];
                    //EXECUTE COPYFS
                    vfs.CopyFS(version, outputDir);

                }

                //READ COMMAND
                else if (arguments[0].Equals(Arguments.READ))
                {
                    if (arguments.Length != Arguments.READ_ARGS)
                    {
                        Console.WriteLine(Arguments.READ_ERROR);
                       // continue;
                    }

                    string filename = arguments[1];
                    int blockNumber = int.Parse(arguments[2]);
                    string outputFile = arguments[3];
                    //EXECUTE READ
                    vfs.Read(filename, blockNumber, outputFile);

                }

                //WRITE COMMAND
                else if (arguments[0].Equals(Arguments.WRITE))
                {
                    if (arguments.Length != Arguments.WRITE_ARGS)
                    {
                        Console.WriteLine(Arguments.WRITE_ERROR);
                        //continue;
                    }

                    string filename = arguments[1];
                    int blockNumber = int.Parse(arguments[2]);
                    string outputFile = arguments[3];
                    //EXECUTE READ
                    vfs.Write(filename, blockNumber, outputFile);

                }

                //DELETE COMMAND
                else if (arguments[0].Equals(Arguments.DELETE))
                {
                    if (arguments.Length != Arguments.DELETE_ARGS)
                    {
                        Console.WriteLine(Arguments.DELETE_ERROR);
                        //continue;
                    }

                    //EXECUTE DELETE
                    vfs.Delete(arguments[1]);

                }

                //RUN SCRIPT COMMAND
                else if (arguments[0].Equals(Arguments.SCRIPT))
                {
                    if (arguments.Length != Arguments.SCRIPT_ARGS)
                    {
                        Console.WriteLine(Arguments.SCRIPT_ERROR);
                        //continue;
                    }

                    //EXECUTE SCRIPT
                    vfs.ScriptFS(arguments[1]);

                }
                //RUN CHECKPOINT COMMAND
                else if (arguments[0].Equals(Arguments.CHECKPOINT))
                {
                    if (arguments.Length != Arguments.CHECKPOINT_ARGS)
                    {
                        Console.WriteLine(Arguments.CHECKPOINT_ERROR);
                        //continue;
                    }

                    //EXECUTE SCRIPT
                    vfs.Checkpoint();

                }
                else if (!arguments[0].Equals(""))
                {
                    Console.WriteLine(Arguments.UNKNOWN_COMMAND_ERROR);

                }
            }

    }
}
