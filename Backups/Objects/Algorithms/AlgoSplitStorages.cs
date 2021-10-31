using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Backups.Services;
using Backups.Tools;

namespace Backups.Objects
{
    public class AlgoSplitStorages : IAlgorithmicBackup
    {
        private readonly string _pathToBackupTmpFolderForSingleAlgo;

        public AlgoSplitStorages(string path)
        {
            _pathToBackupTmpFolderForSingleAlgo = path;
        }

        public IEnumerable<Storage> DoAlgorithmic(List<JobObject> repo, int launchNumber)
        {
            var tmpListStorage = new List<Storage>();
            foreach (JobObject imgFile in repo)
            {
                string pathForAuxDirectory =
                    $"{_pathToBackupTmpFolderForSingleAlgo}\\{imgFile.Name.Split(char.Parse("."))[0]}_{launchNumber}";
                Directory.CreateDirectory(pathForAuxDirectory);
                string path =
                    $"{pathForAuxDirectory}\\{imgFile.Name.Split(char.Parse("."))[0]}_{launchNumber}.{imgFile.Name.Split(char.Parse("."))[1]}";
                var fileInf = new FileInfo(imgFile.GetPath());
                if (fileInf.Exists)
                {
                    fileInf.CopyTo(path);
                }
                else
                {
                    Directory.Delete(pathForAuxDirectory, true);
                    throw new BackupException("The file does not exist");
                }

                tmpListStorage.Add(new Storage(path));
                var zipInf = new FileInfo(pathForAuxDirectory + ".zip");
                if (!zipInf.Exists)
                {
                    ZipFile.CreateFromDirectory(pathForAuxDirectory, pathForAuxDirectory + ".zip");
                }

                Directory.Delete(pathForAuxDirectory, true);
            }

            return tmpListStorage;
        }
    }
}