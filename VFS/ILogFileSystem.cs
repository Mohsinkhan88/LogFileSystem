using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LogFileSystem
{
    interface ILogFileSystem
    {
        void Read(string fileName, int blockNumber,string outputFile);
        void Write(string fileName, int blockNumber, string inputFile);
        void Delete(string fileName);
        void Checkpoint();
        void Switch(int version);
        void DIR();
        void PWD();
        void CD(string directoryName);
        void CopyFS(int version, string outputDirectory);
        void ScriptFS(string scriptFile);
        void Exit();

    }
}
