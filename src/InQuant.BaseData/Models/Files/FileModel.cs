using InQuant.BaseData.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InQuant.BaseData.Models.Files
{
    public class FileModel
    {
        public FileModel() { }

        public long Id { get; set; }

        /// <summary>
        /// 文件夹
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        public string ReletivePath { get; set; }

        /// <summary>
        /// public url
        /// </summary>
        public string PublicUrl { get; set; }
    }
}
