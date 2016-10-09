using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LogFileSystem
{
    class LogFileSystem : ILogFileSystem
    {
        //FIXED VARIABLES-----------------------------------------------------------//
        public static readonly int BLOCK_SIZE = 1000;
        public static readonly int MAX_FILES = 1000;
        public static readonly int MAX_FILE_PATH = 127;
        public static readonly string EMPTY_SPACE = "*";
       // public static readonly string DIRECTORY = "isDirectory";
        public static readonly string DIRECTORY_COUNT_STRING = "dir";
        public static readonly string EXTERNAL_FILE_SYSTEM = "LogFileSystem - version- ";
        public static readonly string FS_FILE_TYPE = ".txt";
        public static readonly string NEXT_BLOCK = System.Environment.NewLine;
        public static readonly int NEXT_LINE_SIZE = 2;
        public static readonly int INODEMAP_SIZE = (MAX_FILE_PATH + Command.ADDRESS_OFFSET) * MAX_FILES + NEXT_LINE_SIZE;
        public static readonly int DIR_BLOCK_NUM = 0;
        private static bool isLogFileSystemAvailable = false;
        private static FileStream LogFileSystemFile = null;
        private static Dictionary<string, INode> INodeMap;
        private static Dictionary<string, string> DirectoryMap;
        private static long startBlockAddress;
        private static long nextBlockAddress;
        private static int nextINodeNumber;
        private static int directoryCount;
        private static string currentDirectory;
        private static string LogFileSystemRootFile = "";
        private Dictionary<int, long> Version;
        private static int versionNumber ;
        private static int currentVersion = 0;

        //CONSTRUCTOR----------------------------------------------------------------//
        public LogFileSystem()
        {
            INodeMap = null;
            startBlockAddress = 0;
            nextBlockAddress = 0;
            nextINodeNumber = 0;
            currentDirectory = "LogFileSystem";
            versionNumber=0;
            directoryCount = 1;
            Version = new Dictionary<int, long>();
            DirectoryMap = new Dictionary<string, string>();
        }
        
        //PARSING METHODS
        public static void ParseFilePath(ref string filePath,ref string directoryPath)
        {
            string directory, fileName;
            int pathLength=filePath.Length;
            int index=0;
            try
            {
                if (pathLength < MAX_FILE_PATH)
                {
                    if (filePath.Trim().Contains("/"))
                    {

                        index = filePath.Trim().LastIndexOf('/');
                        fileName = filePath.Trim().Substring(index + 1);
                        directory = filePath.Trim().Substring(0, pathLength - (fileName.Length + 1));
                        filePath = fileName;
                        directoryPath= directory;
                        
                    }
                   
                }
               
                
            }
            catch
            {
                throw new ArgumentOutOfRangeException(">File Path Length exceeds limit - should be within 0 - "+MAX_FILE_PATH);
            }
            
            
        }

        //INITIALIZATION METHODS-----------------------------------------------------//
        public FileStream CreateFileStream(string fileName, string mode)
        {
            if (mode.Equals("r"))
            {
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            else if (mode.Equals("w"))
            {
                return new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            }
            else if (mode.Equals("rw"))
            {
                return new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            else if (mode.Equals("n"))
            {
                return new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            else if (mode.Equals("o"))
            {
                return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            }

            else
            {
                return null;
            }

        }
        public void InitializeLogFileSystem(string fileName)                               //DONE
        {
            string file=fileName;
            string directory=currentDirectory;
            ParseFilePath(ref file, ref directory);

            LogFileSystemRootFile = file + FS_FILE_TYPE;
            if (File.Exists(file + FS_FILE_TYPE))
            {
                isLogFileSystemAvailable = true;
                LogFileSystemFile = CreateFileStream(LogFileSystemRootFile, "rw");
                nextBlockAddress = LogFileSystemFile.Length;
                
            }
            else
            {
                isLogFileSystemAvailable = false;
                LogFileSystemFile = CreateFileStream(LogFileSystemRootFile, "n");
                
            }
            //SET INITIAL DIRECTORY
            currentDirectory = directory;
            //INITIALIZE INODE MAP
            InitializeINodeMap();
            AddBaseDirectoryNode(directory);
            return;
        }

        //INODE MAP RELATED METHODS--------------------------------------------------//
        public void BuildINodeMap(string INodeMapString, Dictionary<string, INode> INodeMap)
        {
            if (!INodeMapString.Trim().Equals(""))
            {
        
                string[] INodes = INodeMapString.Split(Command.INODE_SEPARATOR_CHR);

                for (int i = 0; i < MAX_FILES; i++)
                {
                    if (!INodes[i].Equals(Command.EMPTY_INODE) && INodes[i].Length==MAX_FILE_PATH)
                    {
                        INode newINode = INode.parseINodeString(INodes[i]);
                        if (newINode != null)
                        {
                            INodeMap.Add(newINode.Filename, newINode);

                        }
                    }
                    else
                    {
                        continue;
                    }
                }

            }


        }
        public List<INode> BuildINodeList(string INodeMapString)
        {
            List<INode> INodesList = new List<INode>();
            if (!INodeMapString.Trim().Equals(""))
            {

                string[] INodes = INodeMapString.Split(Command.INODE_SEPARATOR_CHR);
                foreach (string inode in INodes)
                {
                    if (!inode.Equals(Command.EMPTY_INODE) && inode.Length==MAX_FILE_PATH)
                    {
                        INode newINode = INode.parseINodeString(inode);
                        if (newINode != null)
                        {
                            INodesList.Add(newINode);

                        }
                    }
                    else
                    {
                        continue;
                    }
                }

            }

            return INodesList;
        }
        public  void AddBaseDirectoryNode(string directoryName)
        {
            INode dirNode = new INode(directoryName, DIRECTORY_COUNT_STRING + (directoryCount), DIR_BLOCK_NUM, ++nextINodeNumber, 0);
            INodeMap.Add(DIRECTORY_COUNT_STRING + (directoryCount), dirNode);
            UpdateINodeMap(dirNode);
            DirectoryMap.Add(directoryName, DIRECTORY_COUNT_STRING + (directoryCount));


        }
        public void InitializeINodeMap()                                         //DONE
        {


            string mapData;
            int mapDataSize = (MAX_FILE_PATH + Command.ADDRESS_OFFSET) * MAX_FILES; //(MAX_FILE_PATH + Command.INODE_SEPARATOR.Length) * MAX_FILES;

            INodeMap = new Dictionary<string, INode>(MAX_FILES);
            try
            {
                if (isLogFileSystemAvailable)
                {
                    using (StreamReader sReader = new StreamReader(CreateFileStream(LogFileSystemRootFile, "r")))
                    {
                        sReader.BaseStream.Seek(0, SeekOrigin.Begin);
                        mapData = sReader.ReadLine();
                    }

                    BuildINodeMap(mapData, INodeMap);
                }
                else
                {
                    StringBuilder iNodeMapBuilder = new StringBuilder();
                    //BUILDING EMPTY INODE MAP CONTENT
                    for (int i = 0; i < MAX_FILES; i++)
                    {
                        iNodeMapBuilder.Append(Command.EMPTY_INODE + Command.INODE_SEPARATOR_CHR); // 127 * 1000
                    }


                    //WRITE INODEMAP TO FILE SYSTEM - 
                    using (StreamWriter writer = new StreamWriter(CreateFileStream(LogFileSystemRootFile, "w")))
                    {

                        writer.BaseStream.Seek(0, SeekOrigin.Begin);
                        iNodeMapBuilder.Append(NEXT_BLOCK);
                        writer.WriteLine(iNodeMapBuilder.ToString());


                    }

                    startBlockAddress += iNodeMapBuilder.Length; //SET START ADDRESS FOR BLOCKS
                    nextBlockAddress = startBlockAddress;
                    Version.Add(versionNumber, 0);
                    currentVersion = versionNumber;
                }

            }
            catch
            {
                throw new IOException(">INODEMAP could not be initialized!");
            }


        }
        public  void UpdateINodeMap(INode inode)                                 //DONE
        {
       
            try
            {
                if (inode.INodeNumber <= MAX_FILES)
                {
                    long begin = (inode.INodeNumber - 1) * (MAX_FILE_PATH + Command.ADDRESS_OFFSET);
                    using (StreamWriter writer = new StreamWriter(CreateFileStream(LogFileSystemRootFile,"rw")))
                    {
                        writer.BaseStream.Seek(begin, SeekOrigin.Begin);
                        writer.Write(Command.EMPTY_INODE);

                       
                    }
                    using (StreamWriter Inodewriter = new StreamWriter(CreateFileStream(LogFileSystemRootFile, "rw")))
                    {
                        Inodewriter.BaseStream.Seek(begin, SeekOrigin.Begin);
                        Inodewriter.Write(inode.createINodeString());
                        // Console.WriteLine("Inode Map Updated");

                    }


                }
            }
            catch(IOException ex)
            {
                Console.WriteLine(">" + ex.Message);

            }
        }

        //DATA MANIPULATION METHODS--------------------------------------------------//
        public void Read(string fileName, int blockNumber, string outputFile)   // DONE
        {
            //Read the block block_no of file filename in your file system and write it to the external file (notstored in your file system) outputfile.

            char[] readData = new char[BLOCK_SIZE];
            try
            {
                //CHECK IF FILE OR DIRECTORY EXISTS OR NOT
                if (!INodeMap.ContainsKey(fileName))
                {
                    Console.WriteLine(Command.FILE_ERROR+"- "+fileName);
                    return;
                }

                //GET INODE FROM INODE MAP
                INode inode = INodeMap[fileName];

                //CHECK IF BLOCK NUMBER IS VALID OR NOT
                if (inode.FileSize / BLOCK_SIZE < blockNumber)
                {
                    string message = ("Enter Block Number between 1 - " + inode.FileSize / BLOCK_SIZE);
                    Console.WriteLine(Command.BLOCK_NUMBER_ERROR + message);
                    return;
                }
 
                //READ FILE
                using (StreamReader reader = new StreamReader(CreateFileStream(LogFileSystemRootFile, "r")))
                {
                    //readData = new char[BLOCK_SIZE];
                    //SET READ START POSITION
                    long begin =((inode.BlockNumber) + (blockNumber - 1) * BLOCK_SIZE);//= startBlockPosition + 
                     
                    //READING BLOCK
                    reader.BaseStream.Seek(begin, SeekOrigin.Begin);

                    //STORE READ DATA IN CHAR ARRAY
                    reader.Read(readData, 0, BLOCK_SIZE);
                }

                    //CREATE OUTPUT FILE IF DOES NOT EXIST , ELSE OPEN IT
                    //INITIALIZING WRITER
                    using (StreamWriter writer = new StreamWriter(CreateFileStream(outputFile + FS_FILE_TYPE,"o")))
                    {
                        //WRITE DATA TO OUTPUT FILE
                        writer.BaseStream.Seek(0, SeekOrigin.Begin);
                        writer.WriteLine(new string(readData));
                    }

                
            }
            catch (FileLoadException ex)
            {

                Console.WriteLine(">"+ex.Message);
            }


        }
        public void Write(string fileName, int blockNumber, string inputFile)   //DONE
        {
            //Write 1K bytes at block block_no of filename read from the start of external file infile. 
            //The command should create any directories necessary. No meta-deta (checkpoint blocks etc.) should be
            //written to disk by this command. The in-memory meta-deta changes will go to disk when a checkpoint
            //command is given.

            string readData;
            char[] data = new char[BLOCK_SIZE];
            bool cowFlag = false;
            StringBuilder copyData = new StringBuilder();


            try
            {
                if (!File.Exists(inputFile.Trim() + FS_FILE_TYPE))
                {
                    {
                        Console.WriteLine(Command.FILE_ERROR);
                    }
                }
                //READ INPUT FILE & EXTRACT DATA
                using (StreamReader reader = new StreamReader(CreateFileStream(inputFile.Trim() + FS_FILE_TYPE, "r")))
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    reader.Read(data, 0, BLOCK_SIZE);
                    readData = new string(data);
                    //  Console.WriteLine(readData);
                }

                string directory = currentDirectory;
                //GET DIRECTORY INFO
                ParseFilePath(ref fileName, ref directory);

                if (!directory.Trim().Equals(currentDirectory))
                {
                    directory = currentDirectory + "/" + directory;
                }

                //CHECK IF FILE EXISTS OR NOT
                //IF NO BLOCKS WRITTEN BEFORE - CREATE NEW FILE,INODE
                if (!INodeMap.ContainsKey(fileName.Trim()))
                {

                    if (blockNumber == 1 )
                    {

                        using (StreamWriter writer = new StreamWriter(CreateFileStream(LogFileSystemRootFile, "w")))
                        {

                            //BEGIN AT NEXT BLOCK NUMBER
                            long beginPostion = nextBlockAddress;
                            writer.BaseStream.Seek(beginPostion, SeekOrigin.Begin);

                            //WRITE DATA
                            writer.WriteLine(readData);
                            
                            
                            //INITIALIZE NEW INODE
                            INode newInode = new INode(directory, fileName, nextBlockAddress, ++nextINodeNumber, BLOCK_SIZE);

                            //
                            //ADD INODE TO INODE MAP and UPDATE INODE MAP IN LOG
                            if (!DirectoryMap.ContainsKey(directory))
                            {   //INITIALIZE DIRECTORY INODE
                                INode dirInode = new INode(directory, DIRECTORY_COUNT_STRING + (++directoryCount), DIR_BLOCK_NUM, ++nextINodeNumber, 0);
                                INodeMap.Add(DIRECTORY_COUNT_STRING + (directoryCount), dirInode);
                                DirectoryMap.Add(directory,DIRECTORY_COUNT_STRING + (directoryCount));
                                UpdateINodeMap(dirInode);
                            }

                            //FileMap.Add(directory + "/" + fileName);
                            INodeMap.Add(fileName, newInode);
                            UpdateINodeMap(newInode);
                            

                            //SET NEXT BLOCK POSITION
                            nextBlockAddress += BLOCK_SIZE;

                        }

                    }
                    else
                    {

                        Console.WriteLine(Command.FILE_ERROR);
                        return;
                    }

                }
                else
                {
                    INode inode = INodeMap[fileName];
                    //FILE ALREADY EXISTS IN SYSTEM
                    //CHECK IF BLOCK NUMBER IS VALID OR NOT



                    if ((blockNumber > 0) && (blockNumber <= (inode.FileSize / BLOCK_SIZE) + 1)&& (inode.Directory==directory))
                    {
                        if (inode.INodeNumber < INodeMap.Count-directoryCount)
                        {
                            //COPY ON WRITE MECHANISM---------------------
                            using (StreamReader copyReader = new StreamReader((CreateFileStream(LogFileSystemRootFile, "r"))))
                            {
                                char[] buffer = new char[inode.FileSize];
                                long copyPositionBegin = inode.BlockNumber;
                                copyReader.BaseStream.Seek(copyPositionBegin, SeekOrigin.Begin);
                               
                                {
                                    copyReader.Read(buffer, 0, inode.FileSize);
                                    copyData.Append(new string(buffer));
                                }
                            }
                            using (StreamWriter copyWriter = new StreamWriter((CreateFileStream(LogFileSystemRootFile, "w"))))
                            {
                                long copyToPosition = nextBlockAddress;
                                copyWriter.BaseStream.Seek(copyToPosition, SeekOrigin.Begin);
                                copyWriter.Write(copyData.ToString());

                                //SETTING INODE BLOCK NUMBER TO THE NEW POSITION AFTER COPYING
                                inode.BlockNumber = nextBlockAddress;
                                nextBlockAddress += inode.FileSize;
                            }
                            cowFlag = true;
                            //COPY ON WRITE MECHANISM----------------------
                        }

                        //WRITING DATA READ FROM INPUT FILE
                        using (StreamWriter newWriter = new StreamWriter(CreateFileStream(LogFileSystemRootFile, "w")))
                        {
                            //SET BLOCK BEGIN POSITION

                            long beginPosition = (cowFlag) ? inode.BlockNumber + ((blockNumber - 1) * BLOCK_SIZE) : nextBlockAddress;
                            if (beginPosition < newWriter.BaseStream.Length)
                            {
                                beginPosition = newWriter.BaseStream.Length;
                                nextBlockAddress = beginPosition;
                            }
                            newWriter.BaseStream.Seek(beginPosition, SeekOrigin.Begin);
                            //WRITE DATA
                            newWriter.Write(readData);


                        }
                       

                        //UPDATING INODE PARAMETERS & INODEMAP ENTRY
                        
                        if ((blockNumber == (inode.FileSize / BLOCK_SIZE) + 1))
                        {
                            inode.FileSize += BLOCK_SIZE;
                            //modify next block address
                            nextBlockAddress += BLOCK_SIZE;
                        }
                        INodeMap[fileName] = inode;
                        UpdateINodeMap(inode);
                    }

                    else
                    {

                        Console.WriteLine(Command.BLOCK_NUMBER_ERROR);
                        Console.WriteLine(">Enter Block Number in the range - 1 to " + inode.FileSize / BLOCK_SIZE + 1);

                        return;
                    }

                }
            }

            catch(FileNotFoundException ex)
            {
                Console.WriteLine(">"+ex.Message);

            }


        }
        public void Delete(string fileName)                                     //DONE
        {
            try
            {


                if (INodeMap.ContainsKey(fileName))
                {
                    INode inode = INodeMap[fileName];
                    long INodeMapPosition = Version[currentVersion];
                    long inodePosition = (inode.INodeNumber - 1) * (MAX_FILE_PATH + Command.ADDRESS_OFFSET); //INodeMapPosition +

                    using (StreamWriter INodeMapWriter = new StreamWriter(CreateFileStream(LogFileSystemRootFile, "w")))
                    {
                        INodeMapWriter.BaseStream.Seek(inodePosition, SeekOrigin.Begin);
                        INodeMapWriter.Write(Command.EMPTY_INODE);

                    }

                    //UpdateINodeMap(inode);
                    INodeMap.Remove(fileName);
                    Console.WriteLine(Command.DELETE_MESSAGE+fileName);
                }
                else
                {
                    Console.WriteLine(Command.DELETE_ERROR);
                }
            }

            catch(KeyNotFoundException ex)
            {
                Console.WriteLine(">"+ex.Message);
            }

        }

        //CHECKPOINT & SWITCH--------------------------------------------------------//
        public void Checkpoint()                                                //DONE
        {
            //Checkpoint by writing the updated in-memory meta-deta including the checkpoint block to the log
            //and output the block number of checkpoint block to the user. We can call this number the checkpoint
            //block number, the snapshot number, or the version number. Only meta-data blocks should be written
            //by this command and no data blocks should be written.

            char[] checkpointBlock = new char[INODEMAP_SIZE];
            try
            {
                using (StreamReader checkpointReader = new StreamReader(CreateFileStream(LogFileSystemRootFile, "r")))
                {
                    checkpointReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    checkpointReader.Read(checkpointBlock, 0, INODEMAP_SIZE);

                }

                using (StreamWriter checkpointWriter = new StreamWriter(CreateFileStream(LogFileSystemRootFile, "w")))
                {
                    checkpointWriter.BaseStream.Seek(nextBlockAddress, SeekOrigin.Begin);
                    checkpointWriter.Write(checkpointBlock);

                    Version.Add(++versionNumber, nextBlockAddress);
                    nextBlockAddress += INODEMAP_SIZE;
                }

                Console.WriteLine(Command.CHECKPOINT_MESSAGE + versionNumber);
            }
        
        catch(IOException ex)
            {
                Console.WriteLine(">"+ex.Message);
             }
        }
        public void Switch(int version)
        {
            //Switches to an old checkpoint. Assume version is an old checkpoint block and trust the user. You
            //do not have to validate that it is indeed a correct checkpoint block i.e. I will not test your program
            //by giving it an incorrect checkpoint block. After switching, read commands should work correctly.

            char[] checkpointBlock = new char[INODEMAP_SIZE ];
            char[] newCheckpointBlock = new char[INODEMAP_SIZE];

            try
            {

                if (!Version.ContainsKey(version))
                {
                    throw new KeyNotFoundException(Command.VERSION_ERROR + " Enter versions between 1 - " + Version.Count);

                }
                //READ CHECKPOINT BLOCK
                using (StreamReader checkpointReader = new StreamReader(CreateFileStream(LogFileSystemRootFile, "r")))
                {
                    long checkpointPosition = Version[version];
                    checkpointReader.BaseStream.Seek(checkpointPosition, SeekOrigin.Begin);
                    checkpointReader.Read(checkpointBlock, 0, INODEMAP_SIZE );

                }

                //WRITE TO INITIAL INODEMAP
                using (StreamWriter checkpointWriter = new StreamWriter(CreateFileStream(LogFileSystemRootFile, "w")))
                {
                    checkpointWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                    checkpointWriter.Write(checkpointBlock);
                    //Version[1]=
                   // Version.Add(++versionNumber, 0);
                }

                //using (StreamReader checkpointReader = new StreamReader(CreateFileStream(LogFileSystemRootFile, "r")))
                //{
                //    //READ NEW INODE MAP STRING
                //    checkpointReader.BaseStream.Seek(0, SeekOrigin.Begin);
                //    checkpointReader.Read(newCheckpointBlock, 0, INODEMAP_SIZE );

                    //CLEAR EXISTING INODE MAP ENTRIES
                    INodeMap.Clear();
                    
                    //BUILD NEW INODE MAP
                    //BuildINodeMap(new string(newCheckpointBlock).Trim(), INodeMap);
                    BuildINodeMap(new string(checkpointBlock).Trim(), INodeMap);
                    currentVersion = version;
                    Console.WriteLine(Command.SWITCH_MESSAGE + version);
                //}
               
            }
            catch(Exception ex)
            {
               Console.WriteLine(">"+ex.Message+" : Switch Not Successful");
            }
        }                                      //DONE

        //DIRECTORY RELATED METHODS--------------------------------------------------//
        public void DIR()
        {
            Dictionary<string, INode>.KeyCollection filenames = new Dictionary<string, INode>.KeyCollection(INodeMap);
            try
            {
                Console.WriteLine(">Directory : " + currentDirectory);
                foreach (string filename in filenames)
                {
                    INode inode = INodeMap[filename];

                    if (inode.Directory == currentDirectory)// && inode.Filename == DirectoryMap[currentDirectory])
                    {
                        if (filename.Contains("dir"))
                        {
                            //Console.WriteLine(">" + "Directory UID - " + filename);
                        }
                        else
                        {

                            Console.WriteLine(">" + filename);
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(">" + ex.TargetSite);
            }


        }
        public void PWD()                                                       //DONE
        {
            try
            {
                Console.WriteLine(">Working Directory: " + currentDirectory);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(">" + ex.StackTrace);
            }
        }
        public void CD(string directoryName)
        {

            try
            {
                if (!directoryName.Trim().Equals(""))
                {
                    if (DirectoryMap.ContainsKey(directoryName))
                    {
                        currentDirectory = directoryName;
                        Console.WriteLine(">Directory changed to - " + currentDirectory);

                    }


                }
                return;
            }
            catch(KeyNotFoundException ex)
            {
                Console.WriteLine(">"+ex.Message);
            }
        }

        //COPY FILE SYSTEM-----------------------------------------------------------//
        public void CopyFS(int version, string outputDirectory)
        {
            //Copies the given version of the whole file system to outputdir on an external file system.

            List<INode> Inodes = new List<INode>();
            char[] iNodeMapBlock = new char[INODEMAP_SIZE];

            //CREATE OUTPUT DIRECTORY IF IT DOES NOT EXIST - ASSUMING OUTPUT DIRECTORY STRING IS IN CORRECT FORMAT
            if (!Directory.Exists(outputDirectory))
            {

                Directory.CreateDirectory(@outputDirectory);

            }

            //CHECK IF VERSION EXISTS
            if (!Version.ContainsKey(version))
            {
                Console.WriteLine(Command.VERSION_ERROR+" ENTER VERSION BETWEEN 1 - "+Version.Count);
            }


            try
            {
                string LogFileSystemMetaData = @outputDirectory +"/" + EXTERNAL_FILE_SYSTEM + version.ToString() + FS_FILE_TYPE;
               
                //READ INODEMAP STRING
                using (StreamReader iNodeMapReader = new StreamReader(CreateFileStream(LogFileSystemRootFile,"r")))
                {
                    //GET INODEMAP ADDRESS FOR THE GIVEN VERSION
                    long inodeMapPosition = Version[version];
                    
                    //READ INODEMAP OF THE GIVEN VERSION
                    iNodeMapReader.BaseStream.Seek(inodeMapPosition, SeekOrigin.Begin);
                    iNodeMapReader.Read(iNodeMapBlock, 0, INODEMAP_SIZE);

                    //WRITING INODE MAP TO NEW FILE CREATED AT OUTPUT DIRECTORY
                    using (StreamWriter writer = new StreamWriter(CreateFileStream(LogFileSystemMetaData, "o")))
                    {
                        writer.BaseStream.Seek(0, SeekOrigin.Begin);
                        writer.Write(iNodeMapBlock);
                    }
                    Inodes = BuildINodeList(new string(iNodeMapBlock).Trim());
                }


                //CREATE DIRECTORY STRUCTURE AND FILES IN INODEMAP
                //WRITE FILE  BLOCKS TO NEW FILE SYSTEM
                foreach (INode inode in Inodes)
                {
                    //CREATE ALL DIRECTORIES AND SUB DIRECTORIES FOR EACH INODE
                    string dirPath = @outputDirectory + "/" + inode.Directory;
                    Directory.CreateDirectory(dirPath);
                    char[] iNodeBlock = new char[inode.FileSize];
                    using (StreamReader iNodeReader = new StreamReader(CreateFileStream(LogFileSystemRootFile,"r")))
                    {
                        if (inode.BlockNumber != DIR_BLOCK_NUM)
                        {
                            //READ FILE DATA FROM LogFileSystem
                            long readPosition = inode.BlockNumber;
                            iNodeReader.BaseStream.Seek(readPosition, SeekOrigin.Begin);
                            iNodeReader.Read(iNodeBlock, 0, inode.FileSize);


                            //WRITE TO FILE IN EXTERNAL FILE SYSTEM
                            using (StreamWriter iNodeWriter = new StreamWriter(CreateFileStream(dirPath + "/" + inode.Filename + FS_FILE_TYPE, "o")))
                            {

                                iNodeWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                                iNodeWriter.Write(iNodeBlock);
                            }
                            
                        }

                    }

                }


                Console.WriteLine(Command.COPYFS_MESSAGE);
            }
            catch(IOException ex)
            {
                Console.WriteLine(">"+ex.Message);
            
            }



        }               //DONE
        public void ScriptFS(string scriptFile)
        {
            string[] commands = new string[128];
            int index = 0;
            try
            {
                using (StreamReader reader = new StreamReader(CreateFileStream(scriptFile + FS_FILE_TYPE, "r")))
                {
                    while (!reader.EndOfStream)
                    {
                        commands[index++] = reader.ReadLine();

                    }
                }
                foreach (string command in commands)
                {
                    
                    if (command != null &&  command.Split(' ').Length < 5)
                    {

                        Program.ExecuteScript(command);

                    }
                }
                Console.WriteLine(">Script File Commands Executed.");
                //Console.ReadKey();

            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine(">"+ex.Message);
            }
        }                               //DONE

        //MISC METHODS-------------------------------------------------------------------//
        public void PrintCommands()
        {
            Console.WriteLine("*-----       LogFileSystem COMMANDS        -----*");
            Console.WriteLine("READ FILE :ENTER read <file_name> <block_Number> <outputFile>");
            Console.WriteLine("WRITE FILE :ENTER write <file_name> <block_Number> <inputFile>");
            Console.WriteLine("DELETE FILE :ENTER delete <file_name> ");
            Console.WriteLine("CHECKPOINT :ENTER checkpoint");
            Console.WriteLine("SWITCH :ENTER switch <version>");
            Console.WriteLine("COPY FILE SYSTEM :ENTER copyfs <version> <output_Directory>");
            Console.WriteLine("RUN SCRIPT :ENTER script <script_file_name>");
            Console.WriteLine("PRINT DIRECTORY CONTENTS :ENTER dir");
            Console.WriteLine("CHANGE DIRECTORY :ENTER cd <directory_name>");
            Console.WriteLine("PRINT WORKING DIRECTORY :ENTER pwd\n\n");

        }
        public void Exit()
        {
            Console.WriteLine(Arguments.EXIT_MESSAGE);
            Console.ReadKey();
        }
    }

}
