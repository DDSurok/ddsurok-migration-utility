using System;
using System.Collections.Generic;

namespace migration

{
    public static class RevisionList
    {
        public static List<RevisionInfo> Run()
        {
            throw new NotImplementedException();
        }
    }
    public struct RevisionInfo
    {
        public DateTime GenerateDateTime { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }
        public string HashCode { get; set; }
    }
    internal struct RevisionSimpleInfo
    {
        public string FileName { get; set; }
        public int Id { get; set; }
    }
}
