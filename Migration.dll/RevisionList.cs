using System;
using System.Collections.Generic;

namespace migration

{
    public static class RevisionList
    {
        public static List<VersionInfo> Run()
        {
            throw new NotImplementedException();
        }
    }
    public struct VersionInfo
    {
        public DateTime GenerateDateTime { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }
    }
    internal struct VersionSimpleInfo
    {
        public string FileName { get; set; }
        public int Id { get; set; }
    }
}
