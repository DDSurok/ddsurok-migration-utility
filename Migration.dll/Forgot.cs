using System;

namespace migration
{
    public static class Migration
    {
        public static void Run()
        {
            DatabaseAdapter.RemoveServiceDataInDatabase();
        }
    }
}
