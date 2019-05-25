using InQuant.Framework.Data.Core;
using InQuant.Framework.Data.Core.Attributes;
using System;

namespace InQuant.BaseData.Models.Entities
{
    [TableName("t_file")]
    public class FileContent : IEntity
    {
        public int Id { get; set; }

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

        public DateTime LastModifiedTime { get; set; }
    }
}
