using InQuant.BaseData.Models.Entities;
using InQuant.BaseData.Models.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace InQuant.BaseData.Services
{
    public interface IFileService
    {
        Task<string> GetPublicUrl(string relativePath);
        
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream"></param>
        /// <returns>文件完整的相对路径</returns>
        Task<FileContent> Save(string path, string fileName, Stream fileStream);

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        Task<Stream> Open(string relativePath);

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<Stream> Open(string path, string fileName);

        /// <summary>
        /// get file by id
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        Task<FileModel> GetById(int fileId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<FileModel>> Gets(params int[] ids);
    } 
}
